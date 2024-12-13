using BookAnalysisApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookAnalysisApp.Data
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;

        public DatabaseSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public void SeedDatabase()
        {
            // Olvasd be a dictionary.txt fájlt és töltsd fel az adatbázist
            //var filePath = Path.Combine("Resources", "dictionary.txt");
            //var filePath = "dictionary.txt";
            // var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dictionary.txt");
            //var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "BookAnalysisApp.Data", "BookAnalysisApp.Data", "bin", "Debug", "net8.0", "dictionary.txt");

            var relativePath = Path.Combine("..", "..", "..", "..","BookAnalysisApp.Data", "bin", "Debug", "net8.0", "Resources", "dictionary.dat");
            var filePath = Path.GetFullPath(relativePath, AppDomain.CurrentDomain.BaseDirectory);


            if (!File.Exists(filePath))
            {
                Console.WriteLine("dictionary.txt file not found.");
                return;
            }

            var phrases = new List<EnglishHungarianPhrase>();

            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(new string[] { " - " }, StringSplitOptions.None); // " - " karakterlánc alapján darabolás
                    var englishPhrase = parts[0].Trim().ToLower();
                    //var hungarianMeanings = parts[1].Split(',').Select(s => s.Trim()).ToList();
                    var hungarianMeanings = parts[1].Trim();

                    if (englishPhrase.Length >= 3)
                    {
                        var phrase = new EnglishHungarianPhrase
                        {
                            EnglishPhrase = englishPhrase,
                            HungarianMeanings = hungarianMeanings
                        };

                        phrases.Add(phrase);
                    }
                }
            }

            // Rendezze az EnglishPhrase kifejezéseket a hosszuk szerint, leghosszabbtól a legrövidebbig
            var sortedPhrases = phrases.OrderByDescending(p => p.EnglishPhrase.Length).ToList();

            // Adja hozzá a rendezett kifejezéseket az adatbázishoz
            _context.EnglishHungarianPhrases.AddRange(sortedPhrases);
            _context.SaveChanges();
        }
    }
}
