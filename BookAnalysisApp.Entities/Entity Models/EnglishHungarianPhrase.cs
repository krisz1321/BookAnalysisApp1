using System.Collections.Generic;

namespace BookAnalysisApp.Entities
{
    public class EnglishHungarianPhrase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EnglishPhrase { get; set; }
        public string HungarianMeanings { get; set; }
    }
}