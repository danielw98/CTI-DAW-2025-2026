using Lab04.Data;
using Lab04.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lab04.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    public IndexModel(AppDbContext context)
    {
        _context = context;
    }
    public List<Article> Articles { get; set; } = [];
    public void OnGet()
    {
        Articles = _context.Articles
        .Include(a => a.Category)
        .OrderByDescending(a => a.PublishedAt)
        .ToList();
    }
}
