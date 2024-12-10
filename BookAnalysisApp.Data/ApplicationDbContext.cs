using Microsoft.EntityFrameworkCore;
using BookAnalysisApp.Entities;
using System.Collections.Generic;

namespace BookAnalysisApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

  
        public DbSet<Book> Books { get; set; }

        public DbSet<WordFrequency> WordFrequencies { get; set; }   
    }
}