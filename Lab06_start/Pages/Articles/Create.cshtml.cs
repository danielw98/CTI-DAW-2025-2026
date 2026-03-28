using Lab06.Data;
using Lab06.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab06.Pages.Articles;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public CreateModel(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [BindProperty]
    public Article Article { get; set; } = default!;

    [BindProperty]
    public IFormFile? Upload { get; set; }

    public List<SelectListItem> Categories { get; set; } = [];

    private void LoadCategories()
    {
        Categories = _context.Categories
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToList();
    }

    public IActionResult OnGet()
    {
        LoadCategories();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            LoadCategories();
            return Page();
        }

        if (Upload != null)
        {
            var fileName = Path.GetFileName(Upload.FileName);
            var savePath = Path.Combine(_env.WebRootPath, "images", fileName);
            using var stream = System.IO.File.Create(savePath);
            await Upload.CopyToAsync(stream);
            Article.ImagePath = $"/images/{fileName}";
        }

        Article.PublishedAt = DateTime.Now;
        _context.Articles.Add(Article);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Index");
    }
}