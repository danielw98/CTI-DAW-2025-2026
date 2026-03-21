using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lab04.Data;
using Lab04.Models;

namespace Lab04.Pages.Articles;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;

    public DetailsModel(AppDbContext context)
    {
        _context = context;
    }

    public Article Article { get; set; } = default!;

    public IActionResult OnGet(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var article = _context.Articles
            .Include(art => art.Category)
            .FirstOrDefault(m => m.Id == id);
        if (article == null)
        {
            return NotFound();
        }
        else
        {
            Article = article;
        }
        return Page();
    }
}
