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

            using (var reader = new StreamReader(filePath))
            {
                string line;
                int lineCount = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(new string[] { " - " }, StringSplitOptions.None); // " - " karakterlánc alapján darabolás
                    var englishPhrase = parts[0].Trim();
                    var hungarianMeanings = parts[1].Split(',').Select(s => s.Trim()).ToList();

                    var phrase = new EnglishHungarianPhrase
                    {
                        EnglishPhrase = englishPhrase,
                        HungarianMeanings = hungarianMeanings
                    };

                    _context.EnglishHungarianPhrases.Add(phrase);
                    lineCount++;
                }

                _context.SaveChanges();
            }

        }
    }
}
