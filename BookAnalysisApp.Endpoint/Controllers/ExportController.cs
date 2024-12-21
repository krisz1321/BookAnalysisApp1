using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace BookAnalysisApp.Endpoint.Controllers
{
    public class ExportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("export")] // Method to export phrases to Excel
        public async Task<IActionResult> ExportBookPhrases(Guid bookId, string sortBy = "frequency", string order = "desc")
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

            // var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"{book.Title}_Phrases.xlsx");
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"{book.Title}_{timestamp}_Phrases.xlsx");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Phrases");

                // Add headers
                worksheet.Cells[1, 1].Value = "English Phrase";
                worksheet.Cells[1, 2].Value = "Hungarian Meaning";

                // Add data rows
                int row = 2;
                foreach (var bp in phrases)
                {
                    worksheet.Cells[row, 1].Value = bp.EnglishPhrase.Phrase;
                    worksheet.Cells[row, 2].Value = bp.HungarianMeaning;
                    row++;
                }

                // Save the Excel file
                package.SaveAs(new FileInfo(filePath));
            }

            return Ok(new
            {
                Message = "Data exported successfully.",
                FilePath = filePath
            });
        }
    }
}


