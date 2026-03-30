# Parte 3 — Controller și Views
---

# ViewModels

ViewModels sunt clase care conțin doar datele necesare pentru o pagină. Nu expunem direct domain models în Views.

## `ViewModels/ArticleViewModel.cs`

```csharp
public class ArticleViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; } = DateTime.Now;
    public string CategoryName { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
}
```

## `ViewModels/CreateArticleViewModel.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

public class CreateArticleViewModel
{
    [Required(ErrorMessage = "Titlul este obligatoriu")]
    [MinLength(5, ErrorMessage = "Titlul trebuie să aibă minim 5 caractere")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Conținutul este obligatoriu")]
    [MinLength(20, ErrorMessage = "Conținutul trebuie să aibă minim 20 caractere")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "Categoria este obligatorie")]
    [Display(Name = "Categorie")]
    public int CategoryId { get; set; }

    [Display(Name = "Autor")]
    public int? UserId { get; set; }

    public IFormFile? Upload { get; set; }

    public List<SelectListItem> Categories { get; set; } = new();
    public List<SelectListItem> Users { get; set; } = new();
}
```

## `ViewModels/EditArticleViewModel.cs`

Identic cu `CreateArticleViewModel`, plus `Id` și `ExistingImagePath`:

```csharp
public class EditArticleViewModel : CreateArticleViewModel
{
    public int Id { get; set; }
    public string? ExistingImagePath { get; set; }
}
```

`ExistingImagePath` păstrează calea imaginii curente — dacă utilizatorul nu uploadează o imagine nouă la editare, imaginea existentă nu se pierde.

# ArticlesController

## `Controllers/ArticlesController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ArticlesController : Controller
{
    private readonly IArticleService _articleService;
    private readonly ICategoryService _categoryService;
    private readonly IWebHostEnvironment _env;

    public ArticlesController(
        IArticleService articleService,
        ICategoryService categoryService,
        IWebHostEnvironment env)
    {
        _articleService = articleService;
        _categoryService = categoryService;
        _env = env;
    }

    // GET: /Articles
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var articles = await _articleService.GetAllAsync(cancellationToken);

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

        // Upload imagine (opțional)
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

        // Upload imagine nouă sau păstrare imagine existentă
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

        // TODO: Înlocuiți cu IUserService când va fi creat
        viewModel.Users = new List<SelectListItem>();
    }
}
```

Observați:
- Controller-ul **nu injectează** `AppDbContext` — totul trece prin servicii
- Fiecare acțiune async primește `CancellationToken cancellationToken` — ASP.NET Core îl injectează automat din `HttpContext.RequestAborted`
- `CancellationToken` se propagă la servicii, apoi la repository-uri, apoi la EF Core — **CancellationToken all the way**
- `CopyToAsync(stream, cancellationToken)` — și upload-ul de fișiere respectă anularea
- `LoadDropdownsAsync` este o metodă privată pentru a evita duplicarea în Create/Edit
- `[ValidateAntiForgeryToken]` protejează contra atacuri CSRF
- Upload-ul de fișiere rămâne în Controller (are acces la `IWebHostEnvironment.WebRootPath`). Service layer-ul primește doar string-ul `ImagePath` ca proprietate pe entitatea `Article`

# Views

Fiecare view este un fișier `.cshtml` fără code-behind (spre deosebire de Razor Pages care au `.cshtml.cs`).

> 🤔 **De ce MVC Views nu au code-behind?** În MVC, logica se află în Controller. View-ul este responsabil **exclusiv** de afișare — primește date prin ViewModel și le randează. Separarea e mai clară: Controller = logică, View = HTML.

## Tranziție de la Razor Pages la MVC Views

Dacă aveți deja view-urile din Lab 5, modificările necesare sunt minime:

| Aspect | Razor Pages (Lab 5) | MVC Views (Lab 6) |
|--------|---------------------|-------------------|
| Directiva `@page` | Prezentă (`@page` sau `@page "{id:int}"`) | **Eliminată** |
| `@model` | `@model IndexModel` (PageModel) | `@model List<ArticleViewModel>` (ViewModel) |
| Link-uri | `asp-page="/Articles/Details"` | `asp-action="Details"` |
| Form action | `method="post"` (postează la aceeași pagină) | `asp-action="Create" method="post"` |
| Model binding | `asp-for="Article.Title"` | `asp-for="Title"` (direct pe ViewModel) |
| Route params | `asp-route-id="@article.Id"` | `asp-route-id="@article.Id"` (identic) |

## `Views/Shared/_Layout.cshtml`

Layout-ul este template-ul comun pentru toate paginile:

```html
<!DOCTYPE html>
<html lang="ro">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - News Portal</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css"
          rel="stylesheet" />
</head>
<body>
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
        <div class="container">
            <a class="navbar-brand" asp-controller="Articles" asp-action="Index">
                News Portal
            </a>
            <div class="navbar-nav">
                <a class="nav-link" asp-controller="Articles" asp-action="Index">Articole</a>
                <a class="nav-link" asp-controller="Articles" asp-action="Create">Articol nou</a>
            </div>
        </div>
    </nav>

    <main class="container mt-4">
        @RenderBody()
    </main>

    <footer class="container mt-5 mb-3 text-muted text-center">
        <hr />
        <p>&copy; @DateTime.Now.Year - News Portal</p>
    </footer>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

## `Views/_ViewStart.cshtml`

```html
@{
    Layout = "_Layout";
}
```

Acest fișier setează layout-ul implicit pentru toate view-urile.

## `Views/_ViewImports.cshtml`

```html
@using Lab06
@using Lab06.Models
@using Lab06.ViewModels
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

## `Views/Articles/Create.cshtml`

```html
@model CreateArticleViewModel

@{
    ViewData["Title"] = "Articol nou";
}

<div class="row">
    <div class="col-md-8">
        <h1>Articol nou</h1>
        <form asp-action="Create" method="post" enctype="multipart/form-data">
            <div class="mb-3">
                <label asp-for="Title" class="form-label"></label>
                <input asp-for="Title" class="form-control" />
                <span asp-validation-for="Title" class="text-danger"></span>
            </div>

            <div class="mb-3">
                <label asp-for="Content" class="form-label"></label>
                <textarea asp-for="Content" class="form-control" rows="6"></textarea>
                <span asp-validation-for="Content" class="text-danger"></span>
            </div>

            <div class="mb-3">
                <label asp-for="CategoryId" class="form-label"></label>
                <select asp-for="CategoryId" class="form-select"
                        asp-items="Model.Categories">
                    <option value="">-- Selectați --</option>
                </select>
                <span asp-validation-for="CategoryId" class="text-danger"></span>
            </div>

            <div class="mb-3">
                <label asp-for="UserId" class="form-label"></label>
                <select asp-for="UserId" class="form-select"
                        asp-items="Model.Users">
                    <option value="">-- Selectați --</option>
                </select>
                <span asp-validation-for="UserId" class="text-danger"></span>
            </div>

            <div class="mb-3">
                <label class="form-label">Imagine articol (opțional)</label>
                <input type="file" name="Upload" class="form-control" accept="image/*" />
            </div>

            <button type="submit" class="btn btn-primary">Salvare</button>
            <a asp-action="Index" class="btn btn-secondary">Anulare</a>
        </form>
    </div>
</div>
```

## `Views/Articles/Edit.cshtml`

Similar cu Create, dar cu `Id` hidden, `ExistingImagePath` hidden și posibilitatea de a vedea imaginea curentă:

```html
@model EditArticleViewModel

@{
    ViewData["Title"] = "Editare articol";
}

<div class="row">
    <div class="col-md-8">
        <h1>Editare articol</h1>
        <form asp-action="Edit" method="post" enctype="multipart/form-data">
            <input type="hidden" asp-for="Id" />
            <input type="hidden" asp-for="ExistingImagePath" />

            <div class="mb-3">
                <label asp-for="Title" class="form-label"></label>
                <input asp-for="Title" class="form-control" />
                <span asp-validation-for="Title" class="text-danger"></span>
            </div>

            <div class="mb-3">
                <label asp-for="Content" class="form-label"></label>
                <textarea asp-for="Content" class="form-control" rows="6"></textarea>
                <span asp-validation-for="Content" class="text-danger"></span>
            </div>

            <div class="mb-3">
                <label asp-for="CategoryId" class="form-label"></label>
                <select asp-for="CategoryId" class="form-select"
                        asp-items="Model.Categories">
                    <option value="">-- Selectați --</option>
                </select>
                <span asp-validation-for="CategoryId" class="text-danger"></span>
            </div>

            <div class="mb-3">
                <label asp-for="UserId" class="form-label"></label>
                <select asp-for="UserId" class="form-select"
                        asp-items="Model.Users">
                    <option value="">-- Selectați --</option>
                </select>
                <span asp-validation-for="UserId" class="text-danger"></span>
            </div>

            <div class="mb-3">
                @if (Model.ExistingImagePath != null)
                {
                    <p class="text-muted">Imagine curentă:</p>
                    <img src="@Model.ExistingImagePath" alt="Imagine curentă"
                         style="max-height: 150px; object-fit: cover;" class="mb-2 rounded" />
                }
                <label class="form-label">Imagine nouă (opțional — lasați gol pentru a păstra imaginea curentă)</label>
                <input type="file" name="Upload" class="form-control" accept="image/*" />
            </div>

            <button type="submit" class="btn btn-primary">Salvare</button>
            <a asp-action="Index" class="btn btn-secondary">Anulare</a>
        </form>
    </div>
</div>
```

## `Views/Articles/Index.cshtml`, `Details.cshtml`, `Delete.cshtml`

Aceste view-uri sunt identice structural cu cele din Lab 5. Modificările față de Razor Pages:

```html
@* Eliminați @page de la începutul fișierului *@

@* Schimbați @model din PageModel în ViewModel: *@
@model List<ArticleViewModel>   @* în loc de @model IndexModel *@

@* Schimbați asp-page cu asp-action: *@
<a asp-action="Details" asp-route-id="@article.Id">...</a>
@* în loc de: *@
<a asp-page="/Articles/Details" asp-route-id="@article.Id">...</a>

@* În form-uri, adăugați asp-action: *@
<form asp-action="Delete" method="post">
@* în loc de: *@
<form method="post">
```

Restul HTML-ului (Bootstrap cards, tabele, butoane) rămâne identic.

# Cleanup — Ștergerea Razor Pages

Acum că avem MVC funcțional, ștergem folderul `Pages/Articles/` (inclusiv toate fișierele `.cshtml` și `.cshtml.cs` de acolo). Puteți păstra `Pages/Error.cshtml` dacă doriți o pagină de eroare.

De asemenea, scoateți din `Program.cs`:
- `builder.Services.AddRazorPages()` (dacă există)
- `app.MapRazorPages()` (dacă există)
