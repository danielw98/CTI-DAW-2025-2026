namespace Lab06.Pages;

using Lab06.Data;
using Lab06.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    private const int PageSize = 5;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Article> Articles { get; set; } = [];
    public List<Category> Categories { get; set; } = [];
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int? SelectedCategoryId { get; set; }

    public void OnGet(int? pageNumber, int? categoryId)
    {
        CurrentPage = pageNumber ?? 1;
        SelectedCategoryId = categoryId;

        Categories = _context.Categories.OrderBy(c => c.Name).ToList();

        var query = _context.Articles
            .Include(a => a.Category)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(a => a.CategoryId == categoryId.Value);
        }

        var totalItems = query.Count();
        TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

        Articles = query
            .OrderByDescending(a => a.PublishedAt)
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();
    }
}
