using Microsoft.AspNetCore.Mvc;

namespace BookAnalysisApp.Endpoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        [HttpPost("upload")]
        public IActionResult UploadBook([FromBody] string bookContent)
        {
            if (string.IsNullOrWhiteSpace(bookContent))
            {
                return BadRequest("Book content cannot be empty.");
            }

            // For now, just return a success message.
            return Ok("Book uploaded successfully.");
        }

        [HttpGet("analyze")]
        public IActionResult AnalyzeBook()
        {
            // Placeholder for future analysis logic.
            return Ok("Book analysis feature is under development.");
        }
    }
}
