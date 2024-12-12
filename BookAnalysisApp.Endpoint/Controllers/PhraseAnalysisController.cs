using Microsoft.AspNetCore.Mvc;
using BookAnalysisApp.Data;
using BookAnalysisApp.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;
using System.Collections.Concurrent;

namespace BookAnalysisApp.Endpoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhraseAnalysisController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PhraseAnalysisController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("analyze/{bookId}")]
        public async Task<IActionResult> AnalyzeBookPhrases(Guid bookId)
        {
            // Indítsuk el az időzítőt
            var stopwatch = Stopwatch.StartNew();

            // Find the book by its ID
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return NotFound("Book not found.");
            }

            // Retrieve all English phrases from the database
            var phrases = await _context.EnglishHungarianPhrases
                                        .Select(ehp => ehp.EnglishPhrase)
                                        .ToListAsync();

            // Calculate the word frequency for the English phrases
            var wordFrequencies = CalculateWordFrequency(book.Content, phrases);

            // Save the word frequency data to the database
            foreach (var wordGroup in wordFrequencies)
            {
                var wordRecord = new WordFrequency
                {
                    Word = wordGroup.Key,
                    Frequency = wordGroup.Value
                };

                // Add the word frequency record to the database
                _context.WordFrequencies.Add(wordRecord);
            }

            await _context.SaveChangesAsync();

            // Retrieve word frequencies from the database and order by frequency (descending)
            var topWords = await _context.WordFrequencies
                                          .OrderByDescending(wf => wf.Frequency)
                                          .Take(10)  // You can adjust this limit
                                          .ToListAsync();

            // Return the top 10 most frequent words (ranked)
            var rankedWords = topWords.Select((wf, index) => new
            {
                Rank = index + 1,
                wf.Word,
                wf.Frequency
            }).ToList();

            // Állítsuk le az időzítőt
            stopwatch.Stop();

            // Az időtartam kinyerése
            var elapsedTime = stopwatch.Elapsed;

            // Válaszban visszaadjuk az elemzési eredményeket és az időtartamot
            return Ok(new
            {
                ElapsedTime = elapsedTime.ToString(),
                AnalysisResult = rankedWords
            });
        }

        // Helper function to calculate word frequency from book content and phrases
        private Dictionary<string, int> CalculateWordFrequency(string content, List<string> phrases)
        {
            var wordFrequency = new ConcurrentDictionary<string, int>();

            // Sort phrases by length in descending order
            var sortedPhrases = phrases.OrderByDescending(p => p.Length).ToList();

            // Use StringBuilder for efficient string manipulation
            var contentBuilder = new StringBuilder(content.ToLower());

            // Use a HashSet for fast phrase lookup
            var phraseSet = new HashSet<string>(sortedPhrases.Select(p => p.ToLower()));

            Parallel.ForEach(phraseSet, lowerPhrase =>
            {
                int count = 0;

                // Count occurrences of the phrase in the content
                int index = contentBuilder.ToString().IndexOf(lowerPhrase);
                while (index != -1)
                {
                    count++;
                    if (index >= 0 && index + lowerPhrase.Length <= contentBuilder.Length)
                    {
                        lock (contentBuilder)
                        {
                            if (index >= 0 && index + lowerPhrase.Length <= contentBuilder.Length)
                            {
                                contentBuilder.Remove(index, lowerPhrase.Length);
                            }
                        }
                    }
                    index = contentBuilder.ToString().IndexOf(lowerPhrase);
                }

                if (count > 0)
                {
                    wordFrequency.AddOrUpdate(lowerPhrase, count, (key, oldValue) => oldValue + count);
                }
            });

            return new Dictionary<string, int>(wordFrequency);
        }

        // Optionally: Add an endpoint to retrieve the word frequencies
        [HttpGet("list")]
        public async Task<IActionResult> GetWordFrequencies()
        {
            var wordFrequencies = await _context.WordFrequencies
                                                 .OrderByDescending(wf => wf.Frequency)
                                                 .ToListAsync();

            return Ok(wordFrequencies);
        }
    }
}
