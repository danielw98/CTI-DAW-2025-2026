namespace Lab10.Data;

using Lab10.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Article> Articles { get; set; }
    public DbSet<Category> Categories { get; set; }

    // TODO Lab 10 (Ex. 1): Adaugati DbSet<Tag> pentru entitatea Tag

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Article>()
            .HasOne(a => a.Author)
            .WithMany(u => u.Articles)
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.SetNull);

        // TODO Lab 10 (Ex. 1): Configurati Many-to-Many Article <-> Tag cu junction explicit
        // - HasMany(a => a.Tags).WithMany(t => t.Articles)
        // - UsingEntity<ArticleTag>(...) cu PK compus (ArticleId, TagId)
        // - vezi materialul, sectiunea "AppDbContext / OnModelCreating"
    }
}
