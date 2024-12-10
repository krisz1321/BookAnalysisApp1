using System.ComponentModel.DataAnnotations.Schema;

namespace BookAnalysisApp.Entities
{
    public class Book
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        // WordFrequency should not be part of the input model
        [NotMapped] // This ensures it doesn't show up in Swagger or be part of the API input
        public Dictionary<string, int> WordFrequency { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       
        

    }
}
