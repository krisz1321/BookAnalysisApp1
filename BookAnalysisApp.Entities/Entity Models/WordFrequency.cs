


namespace BookAnalysisApp.Entities
{
    public class WordFrequency
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Word { get; set; }
        public int Frequency { get; set; }
    }
}