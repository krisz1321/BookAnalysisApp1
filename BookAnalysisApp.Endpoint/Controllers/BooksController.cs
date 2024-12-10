using Microsoft.AspNetCore.Mvc;
using BookAnalysisApp.Data;
using BookAnalysisApp.Entities;

namespace BookAnalysisApp.Endpoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadBook([FromBody] Book book)
        {
            if (string.IsNullOrWhiteSpace(book.Content))
            {
                return BadRequest("Book content cannot be empty.");
            }

            book.Id = Guid.NewGuid();
            book.CreatedAt = DateTime.UtcNow;

            // Automatically calculate the word frequency
            book.CalculateWordFrequency();

            // Save the book to the database
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Return the book without wordFrequency as it's calculated on the server side
            return Ok(new
            {
                book.Id,
                book.Title,
                book.Content,
                book.CreatedAt
            });
        }


        [HttpGet("analyze")]
        public IActionResult AnalyzeBook()
        {
            // Placeholder for future analysis logic
            return Ok("Book analysis feature is under development.");
        }
    }
}
