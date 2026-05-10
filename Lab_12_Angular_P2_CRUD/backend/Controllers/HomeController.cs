using Lab12.Mappings;
using Lab12.Services;
using Lab12.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Lab12.Controllers;

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
            RecentArticles = articles.ToViewModelList(),
            TotalArticles = totalArticles,
            TotalCategories = categories.Count
        };

        return View(viewModel);
    }
}
