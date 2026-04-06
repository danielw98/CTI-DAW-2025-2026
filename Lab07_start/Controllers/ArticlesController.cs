using Lab07.Models;
using Lab07.Services;
using Lab07.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab07.Controllers;

public class ArticlesController : Controller
{
    private readonly IArticleService _articleService;
    private readonly ICategoryService _categoryService;
    private readonly IUserService _userService;
    private readonly IWebHostEnvironment _env;

    private const int PageSize = 5;

    public ArticlesController(
        IArticleService articleService,
        ICategoryService categoryService,
        IUserService userService,
        IWebHostEnvironment env)
    {
        _articleService = articleService;
        _categoryService = categoryService;
        _userService = userService;
        _env = env;
    }

    // GET: /Articles?page=1
    public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
    {
        var totalArticles = await _articleService.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalArticles / (double)PageSize);

        if (page < 1) page = 1;
        if (page > totalPages && totalPages > 0) page = totalPages;

        var articles = await _articleService.GetPagedAsync(page, PageSize, cancellationToken);

        var viewModels = articles.Select(a => new ArticleViewModel
        {
            Id = a.Id,
            Title = a.Title,
            Content = a.Content,
            PublishedAt = a.PublishedAt,
            CategoryName = a.Category?.Name ?? "N/A",
            AuthorName = a.User?.Name ?? "N/A",
            ImagePath = a.ImagePath
        }).ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(viewModels);
    }

    // GET: /Articles/Details/5
    public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken)
    {
        if (id == null)
            return NotFound();

        var article = await _articleService.GetByIdAsync(id.Value, cancellationToken);
        if (article == null)
            return NotFound();

        var viewModel = new ArticleViewModel
        {
            Id = article.Id,
            Title = article.Title,
            Content = article.Content,
            PublishedAt = article.PublishedAt,
            CategoryName = article.Category?.Name ?? "N/A",
            AuthorName = article.User?.Name ?? "N/A",
            ImagePath = article.ImagePath
        };

        return View(viewModel);
    }

    // GET: /Articles/Create
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var viewModel = new CreateArticleViewModel();
        await LoadDropdownsAsync(viewModel, cancellationToken);
        return View(viewModel);
    }

    // POST: /Articles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateArticleViewModel viewModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync(viewModel, cancellationToken);
            return View(viewModel);
        }

        var article = new Article
        {
            Title = viewModel.Title,
            Content = viewModel.Content,
            CategoryId = viewModel.CategoryId,
            UserId = viewModel.UserId
        };

        if (viewModel.Upload != null)
        {
            var fileName = Path.GetFileName(viewModel.Upload.FileName);
            var savePath = Path.Combine(_env.WebRootPath, "images", fileName);
            using var stream = System.IO.File.Create(savePath);
            await viewModel.Upload.CopyToAsync(stream, cancellationToken);
            article.ImagePath = $"/images/{fileName}";
        }

        await _articleService.AddAsync(article, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    // GET: /Articles/Edit/5
    public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
    {
        if (id == null)
            return NotFound();

        var article = await _articleService.GetByIdAsync(id.Value, cancellationToken);
        if (article == null)
            return NotFound();

        var viewModel = new EditArticleViewModel
        {
            Id = article.Id,
            Title = article.Title,
            Content = article.Content,
            CategoryId = article.CategoryId,
            UserId = article.UserId,
            ExistingImagePath = article.ImagePath
        };

        await LoadDropdownsAsync(viewModel, cancellationToken);
        return View(viewModel);
    }

    // POST: /Articles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditArticleViewModel viewModel, CancellationToken cancellationToken)
    {
        if (id != viewModel.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync(viewModel, cancellationToken);
            return View(viewModel);
        }

        var article = await _articleService.GetByIdAsync(id, cancellationToken);
        if (article == null)
            return NotFound();

        article.Title = viewModel.Title;
        article.Content = viewModel.Content;
        article.CategoryId = viewModel.CategoryId;
        article.UserId = viewModel.UserId;

        if (viewModel.Upload != null)
        {
            var fileName = Path.GetFileName(viewModel.Upload.FileName);
            var savePath = Path.Combine(_env.WebRootPath, "images", fileName);
            using var stream = System.IO.File.Create(savePath);
            await viewModel.Upload.CopyToAsync(stream, cancellationToken);
            article.ImagePath = $"/images/{fileName}";
        }
        else if (viewModel.ExistingImagePath != null)
        {
            article.ImagePath = viewModel.ExistingImagePath;
        }

        await _articleService.UpdateAsync(article, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    // GET: /Articles/Delete/5
    public async Task<IActionResult> Delete(int? id, CancellationToken cancellationToken)
    {
        if (id == null)
            return NotFound();

        var article = await _articleService.GetByIdAsync(id.Value, cancellationToken);
        if (article == null)
            return NotFound();

        var viewModel = new ArticleViewModel
        {
            Id = article.Id,
            Title = article.Title,
            Content = article.Content,
            PublishedAt = article.PublishedAt,
            CategoryName = article.Category?.Name ?? "N/A",
            AuthorName = article.User?.Name ?? "N/A"
        };

        return View(viewModel);
    }

    // POST: /Articles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await _articleService.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdownsAsync(CreateArticleViewModel viewModel, CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        viewModel.Categories = categories
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
            .ToList();

        var users = await _userService.GetAllAsync(cancellationToken);
        viewModel.Users = users
            .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name })
            .ToList();
    }
}
