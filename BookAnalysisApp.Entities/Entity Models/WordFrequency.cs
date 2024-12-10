


namespace BookAnalysisApp.Entities
{
    public class WordFrequency
    {
        public Guid Id { get; set; } // Unique identifier for each word record
        public string Word { get; set; } = string.Empty; // The word itself
        public int Frequency { get; set; } // How many times this word appears in the book
    }
}