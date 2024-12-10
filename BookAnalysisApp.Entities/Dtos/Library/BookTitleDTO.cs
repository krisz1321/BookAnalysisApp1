using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookAnalysisApp.Entities
{
    public class BookTitleDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}
