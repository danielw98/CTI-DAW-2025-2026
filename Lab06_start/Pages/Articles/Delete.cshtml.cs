namespace Lab06.Pages.Articles;

using Lab06.Data;
using Lab06.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _context;

    public DeleteModel(AppDbContext context)
    {
        _context = context;
    }

    public Article Article { get; set; } = default!;
    public string ErrorMessage { get; set; } = string.Empty;

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

    public IActionResult OnPost(int id, string confirmTitle)
    {
        var article = _context.Articles
            .Include(a => a.Category)
            .FirstOrDefault(a => a.Id == id);

        if (article == null)
        {
            return NotFound();
        }

        if (confirmTitle != article.Title)
        {
            Article = article;
            ErrorMessage = "Titlul introdus nu coincide. Ștergerea a fost anulată.";
            return Page();
        }

        _context.Articles.Remove(article);
        _context.SaveChanges();

        return RedirectToPage("/Index");
    }
}
