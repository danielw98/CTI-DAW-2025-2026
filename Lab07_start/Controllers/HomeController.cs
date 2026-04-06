using Lab07.Services;
using Lab07.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Lab07.Controllers;

public class HomeController : Controller
{
    private readonly IArticleService _articleService;
    private readonly ICategoryService _categoryService;

    public HomeController(IArticleService articleService, ICategoryService categoryService)
    {
        _articleService = articleService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var articles = await _articleService.GetPagedAsync(1, 3, cancellationToken);
        var totalArticles = await _articleService.CountAsync(cancellationToken);
        var categories = await _categoryService.GetAllAsync(cancellationToken);

        var viewModel = new HomeViewModel
        {
            RecentArticles = articles.Select(a => new ArticleViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                PublishedAt = a.PublishedAt,
                CategoryName = a.Category?.Name ?? "N/A",
                AuthorName = a.User?.Name ?? "N/A",
                ImagePath = a.ImagePath
            }).ToList(),
            TotalArticles = totalArticles,
            TotalCategories = categories.Count
        };

        return View(viewModel);
    }
}
