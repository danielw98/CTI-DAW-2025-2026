using Lab12.Models;
using Lab12.ViewModels;

namespace Lab12.Mappings;

public static class ArticleViewModelMappings
{
    public static ArticleViewModel ToViewModel(this Article article) => new()
    {
        Id = article.Id,
        Title = article.Title,
        Content = article.Content,
        PublishedAt = article.PublishedAt,
        CategoryName = article.Category?.Name ?? "N/A",
        AuthorName = article.Author?.FullName ?? "N/A",
        ImagePath = article.ImagePath,
        Tags = article.Tags?.Select(t => t.Name).ToList() ?? new()
    };

    public static List<ArticleViewModel> ToViewModelList(this IEnumerable<Article> articles)
        => articles.Select(a => a.ToViewModel()).ToList();
}
