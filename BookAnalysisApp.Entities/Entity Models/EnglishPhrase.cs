using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookAnalysisApp.Entities
{
    public class EnglishPhrase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Phrase { get; set; }

        // Many-to-Many relationship
        public ICollection<BookPhrase> BookPhrases { get; set; } = new List<BookPhrase>();
    }
}

