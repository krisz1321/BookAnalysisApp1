using BookAnalysisApp.Entities;
using System.Text.RegularExpressions;

namespace BookAnalysisApp.Data
{
    public class BookEditor
    {
        public Book EditBook(Book book, bool removeNonAlphabetic, bool toLowerCase)
        {
            if (removeNonAlphabetic)
            {
                book.Content = RemoveNonAlphabeticCharacters(book.Content);
            }

            if (toLowerCase)
            {
                book.Content = book.Content.ToLower();
            }

            return book;
        }

        private string RemoveNonAlphabeticCharacters(string content)
        {
            return Regex.Replace(content, "[^a-zA-Z ]", string.Empty);
        }
    }
}
