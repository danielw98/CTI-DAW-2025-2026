namespace Lab12.Data;

using Lab12.Models;
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
    public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Article>()
            .HasOne(a => a.Author)
            .WithMany(u => u.Articles)
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.SetNull);

        // Many-to-Many: Article <-> Tag, prin junction explicit ArticleTag
        modelBuilder.Entity<Article>()
            .HasMany(a => a.Tags)
            .WithMany(t => t.Articles)
            .UsingEntity<ArticleTag>(
                j => j.HasOne(at => at.Tag).WithMany().HasForeignKey(at => at.TagId),
                j => j.HasOne(at => at.Article).WithMany().HasForeignKey(at => at.ArticleId),
                j => j.HasKey(at => new { at.ArticleId, at.TagId }));
    }
}
