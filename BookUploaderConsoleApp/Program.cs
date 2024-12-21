using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BookUploaderConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            Console.WriteLine("Book Uploader Console App");
            var baseUrl = "https://localhost:7223"; // Adjust API base URL
            var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };

            while (true)
            {
                Console.WriteLine("\nMűveletek:\n1. Könyvek feltöltése\n2. Könyvcímek listázása\n3. Könyv kiválasztása elemzéshez\n4. Kilépés");
                Console.Write("Válassz egy opciót: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        await UploadBooksAsync(httpClient);
                        break;
                    case "2":
                        await ListBookTitlesAsync(httpClient);
                        break;
                    case "3":
                        await AnalyzeSelectedBookAsync(httpClient);
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Érvénytelen opció. Próbáld újra.");
                        break;
                }
            }
        }

        private static async Task UploadBooksAsync(HttpClient httpClient)
        {
            string directoryPath = "Books Files";

            if (!System.IO.Directory.Exists(directoryPath))
            {
                Console.WriteLine($"Hiba: A megadott mappa nem található: {directoryPath}");
                return;
            }

            var txtFiles = System.IO.Directory.GetFiles(directoryPath, "*.txt");
            if (txtFiles.Length == 0)
            {
                Console.WriteLine("Nincsenek .txt fájlok a megadott mappában.");
                return;
            }

            foreach (var filePath in txtFiles)
            {
                var bookTitle = System.IO.Path.GetFileNameWithoutExtension(filePath);
                var bookContent = await System.IO.File.ReadAllTextAsync(filePath);

                if (string.IsNullOrWhiteSpace(bookContent))
                {
                    Console.WriteLine($"Hiba: Az alábbi fájl üres: {filePath}");
                    continue;
                }

                var book = new BookDto { Title = bookTitle, Content = bookContent };
                var response = await httpClient.PostAsJsonAsync("/api/Books/uploadAndEdit?removeNonAlphabetic=true&toLowerCase=true", book);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Sikeres feltöltés: {bookTitle}");
                }
                else
                {
                    Console.WriteLine($"Sikertelen feltöltés: {bookTitle}");
                    Console.WriteLine($"Hiba: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }

        private static async Task ListBookTitlesAsync(HttpClient httpClient)
        {
            Console.WriteLine("\nFeltöltött könyvek:");
            var response = await httpClient.GetAsync("/api/Books/GetBookTitles");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Nem sikerült lekérni a könyveket.");
                return;
            }

            var bookTitles = await response.Content.ReadFromJsonAsync<List<BookTitleDto>>();
            if (bookTitles == null || bookTitles.Count == 0)
            {
                Console.WriteLine("Nincsenek feltöltött könyvek.");
                return;
            }

            foreach (var book in bookTitles)
            {
                Console.WriteLine($"ID: {book.Id}, Cím: {book.Title}");
            }
        }

        private static async Task AnalyzeSelectedBookAsync(HttpClient httpClient)
        {
            Console.Write("Adja meg a könyv számát a listában: ");
            string bookIdInput = Console.ReadLine();

            if (!Guid.TryParse(bookIdInput, out var bookId))
            {
                Console.WriteLine("Érvénytelen ID formátum.");
                return;
            }

            Console.WriteLine("Alapértelmezett rendezési mód: frekvencia (desc).");
            Console.WriteLine("Elemzés indul...");

            await StoreAndListPhrases(httpClient, bookId);
        }

        private static async Task StoreAndListPhrases(HttpClient httpClient, Guid bookId)
        {
            var storeResponse = await httpClient.PostAsync($"/api/PhraseStorage/store?bookId={bookId}", null);

            if (!storeResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Nem sikerült menteni az elemzést.");
                return;
            }

            Console.WriteLine("Elemzés mentése sikeres.");

            var listResponse = await httpClient.GetAsync($"/api/PhraseStorage/list?bookId={bookId}&sortBy=frequency&order=desc");
            var phrasesResponse = await listResponse.Content.ReadFromJsonAsync<PhrasesResponseDto>();

            var jsonResponse = await listResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response: {jsonResponse}"); // Debug log az API válaszhoz

            if (phrasesResponse == null || phrasesResponse.Phrases == null || !phrasesResponse.Phrases.Any())
            {
                Console.WriteLine("Nincsenek mentett kifejezések.");
                return;
            }

            Console.WriteLine($"\nKönyv címe: {phrasesResponse.BookTitle}");
            Console.WriteLine("Mentett kifejezések:");

            foreach (var phrase in phrasesResponse.Phrases)
            {
                Console.WriteLine($"Angol: {phrase.Phrase}, Magyar: {phrase.HungarianMeaning}, Gyakoriság: {phrase.Frequency}");
            }

            // Excel mentés
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string filePath = $"{phrasesResponse.BookTitle.Replace(' ', '_')}_Phrases_{timestamp}.xlsx";

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Phrases");

                // Fejléc hozzáadása
                worksheet.Cells[1, 1].Value = "Angol kifejezés";
                worksheet.Cells[1, 2].Value = "Magyar jelentés";
                worksheet.Cells[1, 3].Value = "Gyakoriság";

                // Adatok hozzáadása
                for (int i = 0; i < phrasesResponse.Phrases.Count; i++)
                {
                    var phrase = phrasesResponse.Phrases[i];
                    worksheet.Cells[i + 2, 1].Value = phrase.Phrase; // Javítás: angol kifejezés mentése
                    worksheet.Cells[i + 2, 2].Value = phrase.HungarianMeaning;
                    worksheet.Cells[i + 2, 3].Value = phrase.Frequency;
                }

                // Fájl mentése
                package.SaveAs(new System.IO.FileInfo(filePath));
            }

            Console.WriteLine($"Az elemzés mentve lett az alábbi fájlba: {filePath}");
        }
    }

    public class PhrasesResponseDto
    {
        public string BookTitle { get; set; }
        public List<PhraseDto> Phrases { get; set; }
    }

    public class BookDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class BookTitleDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    public class PhraseDto
    {
        public string Phrase { get; set; }
        public string HungarianMeaning { get; set; }
        public int Frequency { get; set; }
    }
}
