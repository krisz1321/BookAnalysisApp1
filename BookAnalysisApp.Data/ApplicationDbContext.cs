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

        // DbSet for books
        public DbSet<Book> Books { get; set; }
    }
}