using Lab09.DTOs;
using Lab09.Models;

namespace Lab09.Mappings;

public static class ArticleMappings
{
    public static ArticleDto ToDto(this Article article) => new(
        Id: article.Id,
        Title: article.Title,
        Content: article.Content,
        PublishedAt: article.PublishedAt,
        CategoryName: article.Category?.Name ?? "N/A",
        AuthorName: article.Author?.FullName ?? "N/A");

    public static List<ArticleDto> ToDtoList(this IEnumerable<Article> articles)
        => articles.Select(a => a.ToDto()).ToList();

    public static Article ToEntity(this CreateArticleDto dto) => new()
    {
        Title = dto.Title,
        Content = dto.Content,
        CategoryId = dto.CategoryId
    };

    public static void ApplyTo(this UpdateArticleDto dto, Article article)
    {
        article.Title = dto.Title;
        article.Content = dto.Content;
        article.CategoryId = dto.CategoryId;
    }
}
