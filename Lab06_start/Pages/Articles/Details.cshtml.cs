using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lab06.Models;
using Lab06.Data;

namespace Lab06.Pages.Articles;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;

    public DetailsModel(AppDbContext context)
    {
        _context = context;
    }

    public Article? Article { get; set; }
    public IActionResult OnGet(int id)
    {
        Article = _context.Articles
            .Include(a => a.Category)
            .FirstOrDefault(a => a.Id == id);
        if (Article == null)
            return NotFound();
        return Page();
    }
}