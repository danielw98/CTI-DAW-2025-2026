using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab04.Pages;

using Lab04.Data;
using Lab04.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    public IndexModel(AppDbContext context)
    {
        _context = context;
    }
    public List<Article> Articles { get; set; } = [];
    public async Task OnGetAsync()
    {
        Articles = await _context.Articles
        .Include(a => a.Category)
        .OrderByDescending(a => a.PublishedAt)
        .ToListAsync();
        ViewData["Count"] = Articles.Count;
    }
}