using Microsoft.AspNetCore.Mvc;
using BookAnalysisApp.Data;
using BookAnalysisApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookAnalysisApp.Endpoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly BookEditor _bookEditor;


        public BooksController(ApplicationDbContext context, BookEditor bookEditor)
        {
            _context = context;
            _bookEditor = bookEditor;
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

        [HttpPost("uploadAndEdit")]
        public async Task<IActionResult> UploadAndEditBook([FromBody] Book book, bool removeNonAlphabetic = false, bool toLowerCase = false)
        {
            if (string.IsNullOrWhiteSpace(book.Content))
            {
                return BadRequest("Book content cannot be empty.");
            }

            // Edit the book content based on the provided parameters
            book = _bookEditor.EditBook(book, removeNonAlphabetic, toLowerCase);

            book.Id = Guid.NewGuid();
            book.CreatedAt = DateTime.UtcNow;

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



        [HttpGet("list")]
        public async Task<IActionResult> GetBooks()
        {
            // Retrieve all books from the database
            var books = await _context.Books.ToListAsync();

            // Return the list of books
            return Ok(books);
        }

        [HttpGet("GetBookTitles")]
        public async Task<IActionResult> GetBookTitles()
        {
            // Retrieve only the Id and Title of books from the database
            var bookTitles = await _context.Books
                                            .Select(book => new BookTitleDTO
                                            {
                                                Id = book.Id,
                                                Title = book.Title
                                            })
                                            .ToListAsync();

            // Return the list of book titles
            return Ok(bookTitles);
        }
    }
}
