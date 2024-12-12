using Microsoft.AspNetCore.Mvc;
using BookAnalysisApp.Data;
using BookAnalysisApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookAnalysisApp.Endpoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DictionaryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DictionaryController(ApplicationDbContext context)
        {
            _context = context;     
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetEnglishHungarianPhrases()
        {
            // Retrieve all English-Hungarian phrases from the database
            var phrases = await _context.EnglishHungarianPhrases
                                         .ToListAsync();

            // Check if any phrases exist
            if (!phrases.Any())
            {
                return NotFound("No English-Hungarian phrases found in the database.");
            }

            return Ok(phrases);
        }


        [HttpGet("listFiveHundred")]
        public async Task<IActionResult> GetFirstFiveHundredEnglishHungarianPhrases()
        {
            // Retrieve the first 500 English-Hungarian phrases from the database
            var phrases = await _context.EnglishHungarianPhrases
                                         .Take(500)
                                         .ToListAsync();

            // Check if any phrases exist
            if (!phrases.Any())
            {
                return NotFound("No English-Hungarian phrases found in the database.");
            }

            return Ok(phrases);
        }

        [HttpGet("listRange")]
        public async Task<IActionResult> GetEnglishHungarianPhrasesInRange([FromQuery] int start = 0, [FromQuery] int end = 499)
        {
            // Retrieve the total count of phrases in the database
            var totalPhrases = await _context.EnglishHungarianPhrases.CountAsync();

            // Adjust the start and end values if they are out of range
            if (start < 0) start = 0;
            if (end >= totalPhrases) end = totalPhrases - 1;
            if (start > end) start = end;

            // Retrieve the phrases in the specified range
            var phrases = await _context.EnglishHungarianPhrases
                                         .Skip(start)
                                         .Take(end - start + 1)
                                         .ToListAsync();

            // Check if any phrases exist
            if (!phrases.Any())
            {
                return NotFound("No English-Hungarian phrases found in the database.");
            }

            return Ok(phrases);
        }







    }
}