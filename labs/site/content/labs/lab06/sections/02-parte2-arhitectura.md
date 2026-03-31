# Parte 2 — Repository, Unit of Work, Service Layer

---

## De ce refactorizăm?

La finalul Părții 1, controllerul arăta așa:

```csharp
public class ArticlesController : Controller
{
    private readonly AppDbContext _context;
    // ...
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var articles = await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.User)
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync(cancellationToken);
        // ...
    }
}
```

Problemele acestei abordări pe termen lung:

- **Duplicare** — aceeași interogare cu `Include` apare în mai multe acțiuni
- **Testabilitate** — pentru a testa controllerul ai nevoie de o bază de date reală
- **Reutilizare** — în Lab 7 adăugăm Web API; aceeași logică ar trebui rescrisă
- **Responsabilități amestecate** — controllerul știe de EF Core, de SQL, de structura bazei de date

Soluția: separăm pe **3 niveluri**:

```
Controller  →  Service  →  Repository  →  DbContext  →  SQL Server
                              ↑
                         UnitOfWork (SaveChangesAsync)
```

- **Repository** — abstractizează `DbSet<T>` ca o colecție; nu salvează, nu știe de tranzacții
- **Unit of Work** — deține `AppDbContext`, expune repository-urile, coordonează `SaveChangesAsync`
- **Service** — conține logica de business; apelează repository-uri prin Unit of Work

---

## Structura finală de foldere

```
Lab06/
├── Controllers/
│   └── ArticlesController.cs       ← actualizat (folosește IArticleService)
├── Data/
│   ├── AppDbContext.cs
│   └── SeedData.cs
├── Models/
│   ├── BaseEntity.cs
│   ├── Article.cs
│   ├── Category.cs
│   └── User.cs
├── Repositories/
│   ├── IRepository.cs              ← interfață generică
│   ├── Repository.cs               ← implementare generică (primește AppDbContext)
│   ├── IArticleRepository.cs       ← metode specifice articolelor
│   ├── ArticleRepository.cs        ← implementare cu Include/OrderBy
│   ├── IUnitOfWork.cs              ← expune repo-urile + SaveChangesAsync
│   └── UnitOfWork.cs               ← deține AppDbContext, creează repo-uri
├── Services/
│   ├── IArticleService.cs
│   ├── ArticleService.cs
│   ├── ICategoryService.cs
│   └── CategoryService.cs
├── ViewModels/
│   └── ...                         ← neschimbate
├── Views/
│   └── ...                         ← neschimbate
└── Program.cs                      ← actualizat
```

---

## Repository Pattern

### `Repositories/IRepository.cs`

Interfața generică — fără EF Core, fără `SaveChanges`.

```csharp
using Lab06.Models;

namespace Lab06.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
}
```

Constrângerea `where T : BaseEntity` garantează că orice entitate are `Id` — de aceea am extras-o în `BaseEntity`. Fără ea, `FirstOrDefaultAsync(e => e.Id == id)` nu ar compila pentru un `T` generic.

`Update` și `Delete` sunt sincrone — EF Core doar marchează entitatea în memorie, nu face I/O.

### `Repositories/Repository.cs`

Implementarea generică primește `AppDbContext`, stochează atât `_context` cât și `_dbSet` — derivat prin `context.Set<T>()`. Metodele folosesc `_dbSet` direct. Nu poate salva — nu are `SaveChanges`.

```csharp
using Lab06.Data;
using Lab06.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab06.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.ToListAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public void Update(T entity)
        => _dbSet.Update(entity);

    public void Delete(T entity)
        => _dbSet.Remove(entity);
}
```

`_dbSet` este inițializat o singură dată în constructor prin `context.Set<T>()` — echivalentul lui `_context.Articles` pentru un `T` generic. Clasele derivate au acces la ambele: `_dbSet` pentru operații simple și `_context` pentru query-uri cu `Include()` sau acces la alte entități.

### `Repositories/IArticleRepository.cs`

Repository-ul generic nu știe de `Include()` sau `OrderBy()`. Cream o interfață specifică pentru articole:

```csharp
using Lab06.Models;

namespace Lab06.Repositories;

public interface IArticleRepository : IRepository<Article>
{
    Task<List<Article>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<Article?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Article>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
}
```

### `Repositories/ArticleRepository.cs`

Primește `AppDbContext` și folosește `_context.Articles` direct — `T` este cunoscut ca `Article`.

```csharp
using Lab06.Data;
using Lab06.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab06.Repositories;

public class ArticleRepository : Repository<Article>, IArticleRepository
{
    public ArticleRepository(AppDbContext context) : base(context) { }

    public async Task<List<Article>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.User)
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Article?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Article>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Articles
            .Where(a => a.CategoryId == categoryId)
            .Include(a => a.Category)
            .Include(a => a.User)
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync(cancellationToken);
    }
}
```

[`Include()`](https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager) face eager loading — fără el, `article.Category` ar fi `null`.


---

## Unit of Work

### De ce Unit of Work?

Repository-ul este o **abstracție de colecție** — adaugă, caută, șterge entități. Dar salvarea în baza de date nu este responsabilitatea unei colecții.

Dacă am pune `SaveChangesAsync` în `IRepository<T>`, am crea o problemă când o operație implică mai multe repository-uri:

```csharp
// FĂRĂ Unit of Work — risc de inconsistență
await _articleRepository.AddAsync(article);
await _articleRepository.SaveChangesAsync();     // salvează articolul

await _tagRepository.AddAsync(tag);
await _tagRepository.SaveChangesAsync();         // dacă asta eșuează,
                                                 // articolul e salvat dar tag-ul nu
```

**Unit of Work** coordonează salvarea pentru toate repository-urile printr-un singur apel:

```csharp
// CU Unit of Work — atomic
await _unitOfWork.ArticleRepository.AddAsync(article);
await _unitOfWork.TagRepository.AddAsync(tag);
await _unitOfWork.SaveChangesAsync();            // salvează TOT sau NIMIC
```

Funcționează deoarece `UnitOfWork` și repository-urile împart **același `AppDbContext`** — toate modificările se acumulează în memorie și se trimit bazei de date printr-un singur `SaveChanges`.

### `Repositories/IUnitOfWork.cs`

```csharp
using Lab06.Models;

namespace Lab06.Repositories;

public interface IUnitOfWork
{
    IArticleRepository ArticleRepository { get; }
    IRepository<Category> CategoryRepository { get; }
    IRepository<User> UserRepository { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

`IUnitOfWork` expune repository-urile ca proprietăți — Service-ul accesează datele prin `_unitOfWork.ArticleRepository`, nu prin injectare separată.

### `Repositories/UnitOfWork.cs`

```csharp
using Lab06.Data;
using Lab06.Models;

namespace Lab06.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private IArticleRepository? _articleRepository;
    private IRepository<Category>? _categoryRepository;
    private IRepository<User>? _userRepository;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IArticleRepository ArticleRepository
        => _articleRepository ??= new ArticleRepository(_context);

    public IRepository<Category> CategoryRepository
        => _categoryRepository ??= new Repository<Category>(_context);

    public IRepository<User> UserRepository
        => _userRepository ??= new Repository<User>(_context);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
```

`UnitOfWork` primește `AppDbContext` și îl pasează direct fiecărui repository. Lazy initialization (`??=`) garantează că aceeași instanță de repository este reutilizată în cadrul aceluiași request. Repository-urile împart același `_context`, deci `SaveChangesAsync` salvează modificările din toate.

---

## Service Layer

Serviciul conține **logica de business** și este singurul layer pe care controllerul îl apelează.

### `Services/IArticleService.cs`

```csharp
using Lab06.Models;

namespace Lab06.Services;

public interface IArticleService
{
    Task<List<Article>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Article?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Article article, CancellationToken cancellationToken = default);
    Task UpdateAsync(Article article, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
```

### `Services/ArticleService.cs`

```csharp
using Lab06.Models;
using Lab06.Repositories;

namespace Lab06.Services;

public class ArticleService : IArticleService
{
    private readonly IUnitOfWork _unitOfWork;

    public ArticleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Article>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.ArticleRepository.GetAllWithDetailsAsync(cancellationToken);
    }

    public async Task<Article?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.ArticleRepository.GetByIdWithDetailsAsync(id, cancellationToken);
    }

    public async Task AddAsync(Article article, CancellationToken cancellationToken = default)
    {
        article.PublishedAt = DateTime.Now;  // logică de business — nu aparține repository-ului
        await _unitOfWork.ArticleRepository.AddAsync(article, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Article article, CancellationToken cancellationToken = default)
    {
        _unitOfWork.ArticleRepository.Update(article);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var article = await _unitOfWork.ArticleRepository.GetByIdAsync(id, cancellationToken);
        if (article != null)
        {
            _unitOfWork.ArticleRepository.Delete(article);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
```

`article.PublishedAt = DateTime.Now` este o regulă de business (nu de stocare) — locul ei este în serviciu, nu în controller și nu în repository.

### `Services/ICategoryService.cs`

```csharp
using Lab06.Models;

namespace Lab06.Services;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default);
}
```

### `Services/CategoryService.cs`

```csharp
using Lab06.Models;
using Lab06.Repositories;

namespace Lab06.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.CategoryRepository.GetAllAsync(cancellationToken);
    }
}
```

`CategoryService` este read-only — nu modifică date, deci nu apelează `SaveChangesAsync`.

---

## Controller actualizat

Înlocuim `AppDbContext` cu `IArticleService` și `ICategoryService`. Views-urile și ViewModels-urile rămân **neschimbate**.

### `Controllers/ArticlesController.cs`

```csharp
using Lab06.Models;
using Lab06.Services;
using Lab06.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab06.Controllers;

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

        viewModel.Users = new List<SelectListItem>();
    }
}
```

Comparând cu controllerul din Partea 1:
- `AppDbContext _context` → `IArticleService _articleService` + `ICategoryService _categoryService`
- `_context.Articles.Include(...)...ToListAsync()` → `_articleService.GetAllAsync()`
- `_context.SaveChangesAsync()` → dispare din controller (e în serviciu)
- `PublishedAt = DateTime.Now` → dispare din controller (e în serviciu)

---

## `Program.cs` — versiunea finală

```csharp
using Lab06.Data;
using Lab06.Repositories;
using Lab06.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

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

Repository-urile **nu se înregistrează direct în DI** — sunt create de `UnitOfWork` la primul acces. Singura înregistrare legată de date este `IUnitOfWork`.

### Lifetime-uri DI

| Lifetime | Durata | Când se folosește |
|----------|--------|-------------------|
| `AddScoped` | Per request HTTP | `DbContext`, `UnitOfWork`, servicii cu stare |
| `AddTransient` | Per injecție | Servicii ușoare, fără stare |
| `AddSingleton` | Viața aplicației | Configurări, cache global |

`AppDbContext` este Scoped — o singură instanță per request. `UnitOfWork` este și el Scoped, deci primește același `AppDbContext`. Modificările făcute în repository-uri se acumulează pe același context și sunt trimise bazei de date la `SaveChangesAsync`.

---
