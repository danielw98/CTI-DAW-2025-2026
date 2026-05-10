using Lab11.Data;
using Lab11.Models;
using Lab11.Repositories;
using Lab11.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Lab11.Tests.Services;

public class ArticleServiceTests
{
    private static (ArticleService service, AppDbContext context) CreateService(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var context = new AppDbContext(options);
        var unitOfWork = new UnitOfWork(context);
        var service = new ArticleService(unitOfWork, NullLogger<ArticleService>.Instance);
        return (service, context);
    }

    private static void SeedTwoArticles(AppDbContext context)
    {
        var category = new Category { Id = 1, Name = "Tehnologie" };
        context.Categories.Add(category);
        context.Articles.AddRange(
            new Article
            {
                Id = 1,
                Title = "Articol test 1",
                Content = "Continut suficient de lung pentru validare",
                PublishedAt = new DateTime(2026, 4, 20),
                CategoryId = 1,
                AuthorId = "user-1"
            },
            new Article
            {
                Id = 2,
                Title = "Articol test 2",
                Content = "Alt continut suficient de lung pentru test",
                PublishedAt = new DateTime(2026, 4, 19),
                CategoryId = 1,
                AuthorId = "user-2"
            }
        );
        context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllArticles()
    {
        var (service, context) = CreateService(nameof(GetAllAsync_ReturnsAllArticles));
        SeedTwoArticles(context);

        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        var (service, _) = CreateService(nameof(GetAllAsync_EmptyDatabase_ReturnsEmptyList));

        var result = await service.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsArticle()
    {
        var (service, context) = CreateService(nameof(GetByIdAsync_ExistingId_ReturnsArticle));
        SeedTwoArticles(context);

        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Articol test 1", result!.Title);
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ReturnsNull()
    {
        var (service, context) = CreateService(nameof(GetByIdAsync_InvalidId_ReturnsNull));
        SeedTwoArticles(context);

        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_IncreasesCount()
    {
        var (service, context) = CreateService(nameof(AddAsync_IncreasesCount));
        SeedTwoArticles(context);

        var newArticle = new Article
        {
            Title = "Articol nou",
            Content = "Continut suficient de lung pentru validare",
            CategoryId = 1,
            AuthorId = "user-1"
        };
        await service.AddAsync(newArticle);

        var all = await service.GetAllAsync();
        Assert.Equal(3, all.Count);
    }

    [Fact]
    public async Task AddAsync_SetsPublishedAt()
    {
        var (service, context) = CreateService(nameof(AddAsync_SetsPublishedAt));
        SeedTwoArticles(context);

        var newArticle = new Article
        {
            Title = "Articol cu data",
            Content = "Continut suficient de lung pentru validare",
            CategoryId = 1,
            AuthorId = "user-1",
            PublishedAt = DateTime.MinValue
        };
        await service.AddAsync(newArticle);

        Assert.True(newArticle.PublishedAt > DateTime.MinValue);
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_RemovesArticle()
    {
        var (service, context) = CreateService(nameof(DeleteAsync_ExistingId_RemovesArticle));
        SeedTwoArticles(context);

        await service.DeleteAsync(1);

        var all = await service.GetAllAsync();
        Assert.Single(all);
        Assert.Null(await service.GetByIdAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_InvalidId_DoesNotThrow()
    {
        var (service, context) = CreateService(nameof(DeleteAsync_InvalidId_DoesNotThrow));
        SeedTwoArticles(context);

        await service.DeleteAsync(999);

        var all = await service.GetAllAsync();
        Assert.Equal(2, all.Count);
    }

    [Fact]
    public async Task UpdateAsync_ModifiesTitle()
    {
        var (service, context) = CreateService(nameof(UpdateAsync_ModifiesTitle));
        SeedTwoArticles(context);

        var article = await service.GetByIdAsync(1);
        article!.Title = "Titlu modificat";
        await service.UpdateAsync(article);

        var updated = await service.GetByIdAsync(1);
        Assert.Equal("Titlu modificat", updated!.Title);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    [InlineData(999, false)]
    public async Task GetByIdAsync_ReturnsExpected(int id, bool shouldExist)
    {
        var (service, context) = CreateService($"GetByIdAsync_Theory_{id}");
        SeedTwoArticles(context);

        var result = await service.GetByIdAsync(id);

        Assert.Equal(shouldExist, result is not null);
    }
}
