using Microsoft.AspNetCore.Mvc;
using BookAnalysisApp.Data;
using BookAnalysisApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookAnalysisApp.Endpoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("analyze/{bookId}")]
        public async Task<IActionResult> AnalyzeBook(Guid bookId)
        {
            // Find the book by its ID
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return NotFound("Book not found.");
            }

            // Calculate the word frequency for the book
            var wordFrequencies = CalculateWordFrequency(book.Content);

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

            return Ok(rankedWords);
        }

        // Helper function to calculate word frequency from book content
        private Dictionary<string, int> CalculateWordFrequency(string content)
        {
            var words = content.Split(new[] { ' ', '.', ',', '!', '?', ';', ':', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            return words.GroupBy(word => word.ToLower())
                        .ToDictionary(group => group.Key, group => group.Count());
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
