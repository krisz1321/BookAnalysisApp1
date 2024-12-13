using BookAnalysisApp.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;
using System.Collections.Concurrent;

namespace BookAnalysisApp.Endpoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhraseStorageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PhraseStorageController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("store")] // Method to analyze and store phrases
        public async Task<IActionResult> StoreBookPhrases(Guid bookId)
        {
            var stopwatch = Stopwatch.StartNew();

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
                var englishPhrase = await _context.EnglishPhrases
                                                  .FirstOrDefaultAsync(ep => ep.Phrase == wordGroup.Key)
                                                  ?? new EnglishPhrase { Phrase = wordGroup.Key };

                var bookPhrase = new BookPhrase
                {
                    BookId = book.Id,
                    Book = book,
                    EnglishPhrase = englishPhrase,
                    HungarianMeaning = _context.EnglishHungarianPhrases
                                               .Where(ehp => ehp.EnglishPhrase == wordGroup.Key)
                                               .Select(ehp => ehp.HungarianMeanings)
                                               .FirstOrDefault(),
                    Frequency = wordGroup.Value
                };

                _context.BookPhrases.Add(bookPhrase);
            }

            await _context.SaveChangesAsync();

            stopwatch.Stop();
            return Ok(new
            {
                Message = "Data stored successfully.",
                ElapsedTime = stopwatch.Elapsed.ToString(),
                BookTitle = book.Title,
                AnalyzedPhrases = wordFrequencies.Select(wf => new
                {
                    Phrase = wf.Key,
                    Frequency = wf.Value,
                    HungarianMeaning = _context.EnglishHungarianPhrases
                                               .Where(ehp => ehp.EnglishPhrase == wf.Key)
                                               .Select(ehp => ehp.HungarianMeanings)
                                               .ToList()
                })
            });
        }

        [HttpGet("retrieve")] // Method to retrieve stored data
        public async Task<IActionResult> RetrieveBookPhrases(Guid bookId)
        {
            var book = await _context.Books
                .Include(b => b.BookPhrases)
                    .ThenInclude(bp => bp.EnglishPhrase)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
            {
                return NotFound("Book not found.");
            }

            var phrases = book.BookPhrases.Select(bp => new
            {
                Phrase = bp.EnglishPhrase.Phrase,
                Frequency = bp.Frequency,
                HungarianMeaning = bp.HungarianMeaning
            });

            return Ok(new
            {
                BookTitle = book.Title,
                Phrases = phrases
            });
        }

        [HttpGet("list")] // Method to list phrases with sorting options
        public async Task<IActionResult> ListBookPhrases(Guid bookId, string sortBy = "frequency", string order = "desc")
        {
            var book = await _context.Books
                .Include(b => b.BookPhrases)
                    .ThenInclude(bp => bp.EnglishPhrase)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
            {
                return NotFound("Book not found.");
            }

            var phrases = book.BookPhrases.AsQueryable();

            // Apply sorting
            phrases = sortBy.ToLower() switch
            {
                "alphabetical" => order.ToLower() == "asc" ? phrases.OrderBy(bp => bp.EnglishPhrase.Phrase) : phrases.OrderByDescending(bp => bp.EnglishPhrase.Phrase),
                "length" => order.ToLower() == "asc" ? phrases.OrderBy(bp => bp.EnglishPhrase.Phrase.Length) : phrases.OrderByDescending(bp => bp.EnglishPhrase.Phrase.Length),
                _ => order.ToLower() == "asc" ? phrases.OrderBy(bp => bp.Frequency) : phrases.OrderByDescending(bp => bp.Frequency),
            };

            var response = phrases.Select(bp => new
            {
                Phrase = bp.EnglishPhrase.Phrase,
                Frequency = bp.Frequency,
                HungarianMeaning = bp.HungarianMeaning
            });

            return Ok(new
            {
                BookTitle = book.Title,
                Phrases = response
            });
        }

        // Helper function to calculate word frequency from book content and phrases
        private Dictionary<string, int> CalculateWordFrequency(string content, List<string> phrases)
        {
            var wordFrequency = new ConcurrentDictionary<string, int>();

            // Sort phrases by length in descending order
            var sortedPhrases = phrases.OrderByDescending(p => p.Length).ToList();

            // Use a HashSet for fast phrase lookup
            var phraseSet = new HashSet<string>(sortedPhrases.Select(p => p.ToLower()));

            // Split the content into smaller chunks for parallel processing
            var contentChunks = SplitContentIntoChunks(content.ToLower(), Environment.ProcessorCount);

            Parallel.ForEach(contentChunks, chunk =>
            {
                var chunkBuilder = new StringBuilder(chunk);

                foreach (var lowerPhrase in phraseSet)
                {
                    int count = 0;

                    // Count occurrences of the phrase in the chunk
                    int index = chunkBuilder.ToString().IndexOf(lowerPhrase);
                    while (index != -1)
                    {
                        count++;
                        if (index >= 0 && index + lowerPhrase.Length <= chunkBuilder.Length)
                        {
                            chunkBuilder.Remove(index, lowerPhrase.Length);
                        }
                        index = chunkBuilder.ToString().IndexOf(lowerPhrase);
                    }

                    if (count > 0)
                    {
                        wordFrequency.AddOrUpdate(lowerPhrase, count, (key, oldValue) => oldValue + count);
                    }
                }
            });

            return new Dictionary<string, int>(wordFrequency);
        }

        // Helper function to split content into smaller chunks
        private List<string> SplitContentIntoChunks(string content, int chunkCount)
        {
            var chunks = new List<string>();
            int chunkSize = content.Length / chunkCount;
            for (int i = 0; i < chunkCount; i++)
            {
                int start = i * chunkSize;
                int length = (i == chunkCount - 1) ? content.Length - start : chunkSize;
                chunks.Add(content.Substring(start, length));
            }
            return chunks;
        }
    }
}
