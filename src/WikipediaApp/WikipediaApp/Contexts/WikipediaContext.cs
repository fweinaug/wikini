using Microsoft.EntityFrameworkCore;

namespace WikipediaApp
{
  public class WikipediaContext : DbContext
  {
    public DbSet<ReadArticle> History { get; set; }
    public DbSet<FavoriteArticle> Favorites { get; set; }
    public DbSet<FavoriteLanguage> Languages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite("Filename=Wikipedia.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<ReadArticle>()
        .Property<string>("Url").HasField("url");

      modelBuilder.Entity<FavoriteArticle>()
        .Property<string>("Url").HasField("url");
    }
  }
}