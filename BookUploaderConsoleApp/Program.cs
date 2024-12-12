using System.Net.Http.Json;

namespace BookUploaderConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Book Uploader Console App");
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

                    // Send the data to the API with editing options
                    var response = await httpClient.PostAsJsonAsync("/api/Books/uploadAndEdit?removeNonAlphabetic=true&toLowerCase=true", book);

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

            Console.WriteLine("Zárás, nyomj entert a kilépéshez vagy más gombot az összes fájl egyben való feltöltéséhez.");
            var key = Console.ReadKey();

            if (key.Key != ConsoleKey.Enter)
            {
                await UploadAllBooksAsync(txtFiles, httpClient);
            }
        }

        private static async Task UploadAllBooksAsync(string[] txtFiles, HttpClient httpClient)
        {
            try
            {
                // Create a header with the names of all files
                var header = string.Join("\n", txtFiles.Select(Path.GetFileName));

                // Concatenate all file contents into a single text
                var allBooksContent = header + "\n\n" + string.Join("\n\n", txtFiles.Select(filePath => File.ReadAllText(filePath)));

                // Save the concatenated text into a new file
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var outputFilePath = $"AllBooks_{timestamp}.txt";
                await File.WriteAllTextAsync(outputFilePath, allBooksContent);

                Console.WriteLine($"Az összesített fájl elmentve: {outputFilePath}");

                // Prepare the single book object with all contents
                var allBooks = new BookDto
                {
                    Title = "AllBooks",
                    Content = allBooksContent
                };

                // Send the concatenated data to the API
                var response = await httpClient.PostAsJsonAsync("/api/Books/uploadAndEdit?removeNonAlphabetic=true&toLowerCase=true", allBooks);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Sikeres adatfeltöltés: AllBooks");
                }
                else
                {
                    Console.WriteLine($"Sikertelen adatfeltöltés: AllBooks");
                    Console.WriteLine($"Hiba: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba történt az összes fájl feltöltésekor: {ex.Message}");
            }
        }
    }

    public class BookDto
    {
        public string Title { get; set; } // Book title
        public string Content { get; set; } // Book content
    }
}
