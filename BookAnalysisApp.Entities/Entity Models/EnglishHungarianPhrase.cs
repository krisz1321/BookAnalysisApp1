using System.Collections.Generic;

namespace BookAnalysisApp.Entities
{
    public class EnglishHungarianPhrase
    {
        public Guid Id { get; set; } // Unique identifier for each phrase record
        public string EnglishPhrase { get; set; } = string.Empty; // The English phrase
        public List<string> HungarianMeanings { get; set; } = new List<string>(); // A list of Hungarian translations
    }
}