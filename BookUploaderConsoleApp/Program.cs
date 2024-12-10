using System.Net.Http.Json;

namespace BookUploaderConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Book Uploader Console App");
           // Console.ReadLine();
            Console.WriteLine("indul!");

            // Base URL of your API
            var baseUrl = "https://localhost:7223"; // Adjust this to your API's base URL
            var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };

            // Directory containing TXT files
            string directoryPath = "Books";

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"Hiba: A mappa nem található: {directoryPath}");
                return;
            }

            // Get all TXT files in the directory
            var txtFiles = Directory.GetFiles(directoryPath, "*.txt");

            if (txtFiles.Length == 0)
            {
                Console.WriteLine("Nincsenek .txt fájlok a megadott mappában.");
                return;
            }

            try
            {
                foreach (var filePath in txtFiles)
                {
                    // Extract book title from file name
                    var bookTitle = Path.GetFileNameWithoutExtension(filePath);

                    // Read content from the file
                    var bookContent = await File.ReadAllTextAsync(filePath);

                    if (string.IsNullOrWhiteSpace(bookContent))
                    {
                        Console.WriteLine($"Hiba: Az alábbi fájl üres: {filePath}");
                        continue;
                    }

                    // Prepare the book object
                    var book = new BookDto
                    {
                        Title = bookTitle,
                        Content = bookContent
                    };

                    // Send the data to the API
                    var response = await httpClient.PostAsJsonAsync("/api/Books/upload", book);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Sikeres adatfeltöltés: {bookTitle}");
                    }
                    else
                    {
                        Console.WriteLine($"Sikertelen adatfeltöltés: {bookTitle}");
                        Console.WriteLine($"Hiba: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }

                Console.WriteLine("Minden adat feltöltése befejeződött.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba történt: {ex.Message}");
            }

            Console.WriteLine("Zárás, nyomj entert a kilépéshez.");  
            Console.ReadKey();
        }
        

    }

    public class BookDto
    {
        public string Title { get; set; } // Book title
        public string Content { get; set; } // Book content
    }
}
