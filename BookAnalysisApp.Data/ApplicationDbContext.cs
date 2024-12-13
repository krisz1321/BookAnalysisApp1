using BookAnalysisApp.Entities;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<EnglishPhrase> EnglishPhrases { get; set; }
    public DbSet<BookPhrase> BookPhrases { get; set; }
    public DbSet<EnglishHungarianPhrase> EnglishHungarianPhrases { get; set; }
    public DbSet<WordFrequency> WordFrequencies { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuring the Many-to-Many relationship
        modelBuilder.Entity<BookPhrase>()
            .HasKey(bp => new { bp.BookId, bp.EnglishPhraseId });

        modelBuilder.Entity<BookPhrase>()
            .HasOne(bp => bp.Book)
            .WithMany(b => b.BookPhrases)
            .HasForeignKey(bp => bp.BookId);

        modelBuilder.Entity<BookPhrase>()
            .HasOne(bp => bp.EnglishPhrase)
            .WithMany(ep => ep.BookPhrases)
            .HasForeignKey(bp => bp.EnglishPhraseId);

        base.OnModelCreating(modelBuilder);
    }
}