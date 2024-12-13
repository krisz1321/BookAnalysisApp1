using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookAnalysisApp.Entities
{
    public class BookPhrase
    {
        public Guid BookId { get; set; }
        public Book Book { get; set; }

        public Guid EnglishPhraseId { get; set; }
        public EnglishPhrase EnglishPhrase { get; set; }

        public string HungarianMeaning { get; set; } // Meaning of the phrase
        public int Frequency { get; set; } // Frequency of the phrase in the book
    }
}
