using BookAnalysisApp.Entities;
using Microsoft.EntityFrameworkCore;

//public class BookLogic
//{
//    private readonly ApplicationDbContext _context;

//    public BookLogic(ApplicationDbContext context)
//    {
//        _context = context;
//    }

//    public async Task AddBookAsync(Book book)
//    {
//        book.Id = Guid.NewGuid();
//        book.CreatedAt = DateTime.UtcNow;
//        _context.Books.Add(book);
//        await _context.SaveChangesAsync();
//    }

//    public async Task<List<Book>> GetBooksAsync()
//    {
//        return await _context.Books.ToListAsync();
//    }

//    public async Task<Book> GetBookByIdAsync(Guid id)
//    {
//        return await _context.Books.FindAsync(id);
//    }

//    public async Task AnalyzeBookAsync(Guid bookId)
//    {
//        var book = await _context.Books.FindAsync(bookId);
//        if (book == null) throw new ArgumentException("Book not found");

//        var phrases = await _context.EnglishHungarianPhrases.Select(p => p.EnglishPhrase).ToListAsync();

//        var wordFrequencies = CalculateWordFrequency(book.Content, phrases);

//        foreach (var entry in wordFrequencies)
//        {
//            var englishPhrase = await _context.EnglishPhrases.FirstOrDefaultAsync(p => p.Phrase == entry.Key)
//                ?? new EnglishPhrase { Phrase = entry.Key };

//            _context.BookPhrases.Add(new BookPhrase
//            {
//                BookId = book.Id,
//                Book = book,
//                EnglishPhrase = englishPhrase,
//                Frequency = entry.Value,
//                HungarianMeaning = _context.EnglishHungarianPhrases.FirstOrDefault(p => p.EnglishPhrase == entry.Key)?.HungarianMeanings
//            });
//        }

//        await _context.SaveChangesAsync();
//    }

//    private Dictionary<string, int> CalculateWordFrequency(string content, List<string> phrases)
//    {
//        var frequency = new Dictionary<string, int>();
//        foreach (var phrase in phrases)
//        {
//            var count = content.Split(new[] { phrase }, StringSplitOptions.None).Length - 1;
//            if (count > 0) frequency[phrase] = count;
//        }
//        return frequency;
//    }
//}