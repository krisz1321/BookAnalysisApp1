using System;


namespace BookAnalysisApp.Entities
{
    public class Book
    {
        // Unique identifier for the book
        public Guid Id { get; set; }

        // Title of the book
        public string Title { get; set; } = string.Empty;

        // Full content of the book
        public string Content { get; set; } = string.Empty;

        // Optional: Word frequency dictionary
        public Dictionary<string, int> WordFrequency { get; set; } = new();

        // Date when the book was added
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

