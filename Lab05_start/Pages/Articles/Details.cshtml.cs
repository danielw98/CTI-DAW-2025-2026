using Lab05.Data;
using Lab05.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lab05.Pages.Articles;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;

    public DetailsModel(AppDbContext context)
    {
        _context = context;
    }

    public Article Article { get; set; } = default!;

    public IActionResult OnGet(int id)
    {
        var article = _context.Articles
            .Include(a => a.Category)
            .FirstOrDefault(a => a.Id == id);

        if (article == null)
        {
            return NotFound();
        }

        Article = article;
        return Page();
    }
}
