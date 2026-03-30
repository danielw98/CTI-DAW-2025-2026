# Parte 1 — Tranziție la MVC + Async/Await

---

## Punct de plecare

Aveți la dispoziție proiectul **Lab06_start** — un proiect Razor Pages funcțional cu modele, baza de date, migrări și imagini.

Scopul acestei părți este să creați un **proiect MVC nou** și să migrați în el tot ce aveți deja.

---

## Proiect nou MVC

Deschideți o **nouă instanță Visual Studio** (separat de Lab06_start) și creați un proiect nou de la zero:

```
File → New Project → ASP.NET Core Web App (Model-View-Controller)
Nume proiect: Lab06
```

Instalați pachetele NuGet necesare:

```bash
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.25
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.25
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.25
```

---

## Ce copiați din Lab06_start

Din proiectul Lab06_start, copiați în noul proiect MVC:

| De copiat | Observație |
|-----------|-----------|
| `Models/*.cs` | Vom modifica modelele (adăugăm `BaseEntity`) |
| `Data/AppDbContext.cs` | Se copiază ca atare |
| `Data/SeedData.cs` | Se copiază ca atare |
| `wwwroot/images/` | Imaginile articolelor |
| `Views/` | Views-urile MVC pregătite |

---

## Structura finală a proiectului

La final, proiectul trebuie să arate astfel:

```
Lab06/
├── Controllers/
│   └── ArticlesController.cs
├── Data/
│   ├── AppDbContext.cs
│   └── SeedData.cs
├── Models/
│   ├── BaseEntity.cs
│   ├── Article.cs
│   ├── Category.cs
│   └── User.cs
├── ViewModels/
│   ├── ArticleViewModel.cs
│   ├── CreateArticleViewModel.cs
│   └── EditArticleViewModel.cs
├── Views/
│   ├── Articles/
│   │   ├── Index.cshtml
│   │   ├── Details.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   └── Delete.cshtml
│   └── Shared/
│       └── _Layout.cshtml
├── wwwroot/
│   └── images/               ← copiat din Lab06_start
├── appsettings.json
└── Program.cs
```

---

## Models

Modelele copiate din Lab06_start au `Id` declarat direct în fiecare clasă. Le modificăm să moștenească `BaseEntity` — vom vedea de ce e util în Partea 2, la Repository.

### `Models/BaseEntity.cs`

Creați fișier nou:

```csharp
namespace Lab06.Models;

public class BaseEntity
{
    public int Id { get; set; }
}
```

### `Models/Article.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace Lab06.Models;

public class Article : BaseEntity
{
    [Required]
    [MinLength(5)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MinLength(20)]
    public string Content { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime PublishedAt { get; set; } = DateTime.Now;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }

    public string? ImagePath { get; set; }
}
```

### `Models/Category.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace Lab06.Models;

public class Category : BaseEntity
{
    [Required]
    [MinLength(2)]
    public string Name { get; set; } = string.Empty;

    public List<Article> Articles { get; set; } = [];
}
```

### `Models/User.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace Lab06.Models;

public class User : BaseEntity
{
    [Required]
    [MinLength(3)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public List<Article> Articles { get; set; } = [];
}
```

Observați: modelele **nu declară `Id`** — îl moștenesc din `BaseEntity`.

---

## DbContext

### `Data/AppDbContext.cs`

```csharp
using Lab06.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab06.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Article> Articles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
}
```

---

## Connection string

În `appsettings.json`, adăugați în secțiunea `ConnectionStrings`:

```json
"Lab06": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Lab06_final;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"
```

---

## ViewModels

ViewModels sunt clase care conțin exact datele de care are nevoie o pagină — nu expunem modelele de domeniu direct în Views.

### `ViewModels/ArticleViewModel.cs`

```csharp
namespace Lab06.ViewModels;

public class ArticleViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
}
```

### `ViewModels/CreateArticleViewModel.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab06.ViewModels;

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

### `ViewModels/EditArticleViewModel.cs`

```csharp
namespace Lab06.ViewModels;

public class EditArticleViewModel : CreateArticleViewModel
{
    public int Id { get; set; }
    public string? ExistingImagePath { get; set; }
}
```

`ExistingImagePath` păstrează imaginea curentă — dacă utilizatorul nu uploadează una nouă la editare, imaginea existentă nu se pierde.

---

## SeedData

Copiați `Data/SeedData.cs` din **Lab06_start**. Conține date inițiale (categorii + articole) care se inserează automat la prima pornire.

---

## Views

Creați folderul `Views/Articles/` în noul proiect MVC.

Proiectul MVC generat de Visual Studio vine deja cu `_ViewImports.cshtml`, `_ViewStart.cshtml` și `Views/Shared/_Layout.cshtml` — nu trebuie să le copiați din Lab06_start.

Copiați fișierele `.cshtml` (nu și `.cshtml.cs`) din Lab06_start în `Views/Articles/`:
- `Pages/Articles/Create.cshtml`, `Edit.cshtml`, `Details.cshtml`, `Delete.cshtml`
- `Pages/Index.cshtml` → `Views/Articles/Index.cshtml`

Aplicați modificările de mai jos în fiecare fișier copiat.

Copiați și `wwwroot/images/` din Lab06_start — conține imaginile din seed data.

### Modificări comune (toate fișierele)

**1. Ștergeți prima linie** — `@page` sau `@page "{id:int}"`

În Razor Pages, `@page` transformă fișierul în endpoint HTTP de sine stătător — gestionează el însuși request-ul. În MVC, view-ul este randat de controller, deci `@page` nu există.

**2. Ștergeți `@using Lab06.Pages.Articles`**

**3. Schimbați `@model`** conform tabelului:

| Fișier | `@model` |
|--------|---------|
| `Index.cshtml` | `@model List<ArticleViewModel>` |
| `Create.cshtml` | `@model CreateArticleViewModel` |
| `Edit.cshtml` | `@model EditArticleViewModel` |
| `Details.cshtml` | `@model ArticleViewModel` |
| `Delete.cshtml` | `@model ArticleViewModel` |

**4. Schimbați accesul la proprietăți**

În Razor Pages, PageModel-ul conținea un câmp `Article`, deci accesul era `Model.Article.Title`. ViewModel-ul este flat:

| Razor Pages | MVC |
|-------------|-----|
| `Model.Article.Title` | `Model.Title` |
| `Model.Article.CategoryId` | `Model.CategoryId` |
| `asp-for="Article!.Title"` | `asp-for="Title"` |
| `asp-for="Article!.CategoryId"` | `asp-for="CategoryId"` |

**5. Rutare**

| Razor Pages | MVC |
|-------------|-----|
| `asp-page="/Index"` | `asp-action="Index"` |
| `<form method="post">` | `<form asp-action="Create" method="post">` |

Dacă `asp-controller` lipsește, MVC folosește implicit controller-ul curent. Deci din `Views/Articles/`, `asp-action="Index"` ajunge la `ArticlesController.Index` fără a specifica controller-ul explicit.

**6. Ștergeți `@section Scripts { ... }`** dacă există — validarea e deja inclusă în `_Layout.cshtml`.

### `Index.cshtml`

Corespondentul este `Pages/Index.cshtml` (din rădăcina Pages, nu din Pages/Articles/). Copiați-l în `Views/Articles/Index.cshtml` și aplicați aceleași modificări: scoateți `@page`, `@using Lab06.Pages`, schimbați `@model IndexModel` cu `@model List<ArticleViewModel>`, și înlocuiți `Model.Articles` cu `Model`, `Model.Categories` cu lista respectivă etc. Paginarea și filtrarea din view-ul Razor Pages pot fi simplificate sau omise în această etapă — controller-ul din Partea 1 returnează toate articolele fără paginare.

---

## Async/Await

### De ce async?

ASP.NET Core servește cereri HTTP cu un **thread pool** — câteva zeci de thread-uri, nu mii. Când un thread execută o operație sincronă ca `_context.Articles.ToList()`, el trimite query-ul la SQL Server și **stă blocat** câteva zeci sau sute de milisecunde așteptând răspunsul. Cu 50 de utilizatori simultani, pool-ul se epuizează rapid.

Cu `await _context.Articles.ToListAsync()`, thread-ul trimite query-ul și se **eliberează înapoi în pool** — poate servi alte cereri. Când SQL Server răspunde, framework-ul preia un thread disponibil și continuă.

```
Sync:   Thread ████████████████████ (blocat 200ms, nu poate face altceva)
Async:  Thread ░─────────────────── (liber imediat, preia alte cereri)
```

### Regulile async

- Metoda primește modificatorul `async`
- Tipul returnat devine `Task<T>` (sau `Task` pentru `void`)
- Apelurile EF Core primesc `await` și sufixul `Async`
- **"async all the way"** — dacă o metodă e async, toate cele care o apelează trebuie să fie async

| Sync | Async |
|------|-------|
| `ToList()` | `ToListAsync(ct)` |
| `FirstOrDefault()` | `FirstOrDefaultAsync(pred, ct)` |
| `SaveChanges()` | `SaveChangesAsync(ct)` |
| `Find(id)` | `FindAsync(new object[] { id }, ct)` |

### CancellationToken

Când un utilizator închide tab-ul sau un request expiră, ASP.NET Core semnalează anularea printr-un `CancellationToken`. Fără el, query-urile SQL continuă să ruleze pe server chiar dacă nimeni nu mai așteaptă răspunsul.

```
Utilizator închide tab-ul
    → ASP.NET Core setează HttpContext.RequestAborted
        → Controller primește CancellationToken (injectat automat)
            → EF Core anulează comanda SQL
```

Reguli:
- `CancellationToken cancellationToken` este **ultimul parametru**, cu valoarea implicită `default`
- ASP.NET Core îl injectează **automat** în acțiunile controller-ului — nu trebuie creat manual
- Se transmite la **fiecare metodă async** din lanț: Controller → EF Core

---

## `Controllers/ArticlesController.cs`

Controller-ul injectează `AppDbContext` direct (în Partea 2 vom înlocui cu un Service Layer). Toate acțiunile sunt async și primesc `CancellationToken`.

```csharp
using Lab06.Data;
using Lab06.Models;
using Lab06.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lab06.Controllers;

public class ArticlesController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ArticlesController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET: /Articles
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var articles = await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.User)
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync(cancellationToken);

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

        var article = await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

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
            UserId = viewModel.UserId,
            PublishedAt = DateTime.Now
        };

        if (viewModel.Upload != null)
        {
            var fileName = Path.GetFileName(viewModel.Upload.FileName);
            var savePath = Path.Combine(_env.WebRootPath, "images", fileName);
            using var stream = System.IO.File.Create(savePath);
            await viewModel.Upload.CopyToAsync(stream, cancellationToken);
            article.ImagePath = $"/images/{fileName}";
        }

        _context.Articles.Add(article);
        await _context.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    // GET: /Articles/Edit/5
    public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
    {
        if (id == null)
            return NotFound();

        var article = await _context.Articles
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

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

        var article = await _context.Articles
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

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

        await _context.SaveChangesAsync(cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    // GET: /Articles/Delete/5
    public async Task<IActionResult> Delete(int? id, CancellationToken cancellationToken)
    {
        if (id == null)
            return NotFound();

        var article = await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

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
        var article = await _context.Articles
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (article != null)
        {
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdownsAsync(CreateArticleViewModel viewModel, CancellationToken cancellationToken)
    {
        var categories = await _context.Categories.ToListAsync(cancellationToken);
        viewModel.Categories = categories
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
            .ToList();

        viewModel.Users = new List<SelectListItem>();
    }
}
```

---

## `Program.cs` — versiunea inițială

```csharp
using Lab06.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Lab06")));

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.Initialize(context);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Articles}/{action=Index}/{id?}");

app.Run();
```

### Rute convenționale

Ruta implicită `{controller=Articles}/{action=Index}/{id?}`:

| URL | Controller | Action | id |
|-----|-----------|--------|----|
| `/` | ArticlesController | Index | — |
| `/Articles/Details/5` | ArticlesController | Details | 5 |
| `/Articles/Create` | ArticlesController | Create | — |
| `/Articles/Edit/3` | ArticlesController | Edit | 3 |

---

## Migrare și pornire

Generați migrarea inițială și creați baza de date `Lab06_final`:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

Aplicația pornește la `https://localhost:XXXX` și afișează lista de articole din seed data.

La finalul Părții 1, aplicația funcționează complet — CRUD cu async/await, `CancellationToken` propagat. Controllerul accesează `AppDbContext` direct. În Partea 2 vom introduce Repository, Unit of Work și Service Layer pentru a elimina această dependență.

---
