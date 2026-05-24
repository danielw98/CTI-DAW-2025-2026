using Lab12.DTOs;
using Lab12.Models;

namespace Lab12.Mappings;

public static class ArticleMappings
{
    public static ArticleDto ToDto(this Article article) => new(
        Id: article.Id,
        Title: article.Title,
        Content: article.Content,
        PublishedAt: article.PublishedAt,
        CategoryName: article.Category?.Name ?? "N/A",
        CategoryId: article.CategoryId,
        AuthorName: article.Author?.FullName ?? "N/A",
        AuthorId: article.AuthorId,
        ImagePath: article.ImagePath,
        Tags: article.Tags?.Select(t => new TagDto(t.Id, t.Name)).ToList() ?? new List<TagDto>());

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
