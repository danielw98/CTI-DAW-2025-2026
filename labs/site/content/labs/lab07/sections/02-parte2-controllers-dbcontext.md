# Parte 2 — Curățare Controllers, Repository și AppDbContext

---

## Pasul 6 — Actualizare ArticlesController

### 6a. Scoateți IUserService din constructor

```csharp
// Ștergeți din câmpuri:
// private readonly IUserService _userService;

// Ștergeți din constructor parametrul și assignment-ul:
// IUserService userService
// _userService = userService;
```

Constructorul devine:

```csharp
public ArticlesController(
    IArticleService articleService,
    ICategoryService categoryService,
    IWebHostEnvironment env)
{
    _articleService = articleService;
    _categoryService = categoryService;
    _env = env;
}
```

### 6b. Înlocuiți a.User?.Name cu a.Author?.FullName

Apare în 3 locuri — în `Index`, `Details` și `Delete`:

```csharp
// Peste tot înlocuiți:
// AuthorName = a.User?.Name ?? "N/A",
// cu:
AuthorName = a.Author?.FullName ?? "N/A",
```

### 6c. Scoateți UserId din Create POST

În metoda `Create` POST, ștergeți linia:

```csharp
// Ștergeți:
// UserId = viewModel.UserId
```

Articolul se creează fără autor deocamdată — `AuthorId` va fi setat automat în exerciții.

### 6d. Scoateți UserId din Edit GET și POST

În metoda `Edit` GET, ștergeți linia din inițializarea viewModel-ului:

```csharp
// Ștergeți:
// UserId = article.UserId,
```

În metoda `Edit` POST, ștergeți linia de actualizare:

```csharp
// Ștergeți:
// article.UserId = viewModel.UserId;
```

### 6e. Simplificați LoadDropdownsAsync

Scoateți blocul cu users din metodă:

```csharp
private async Task LoadDropdownsAsync(CreateArticleViewModel viewModel, CancellationToken cancellationToken)
{
    var categories = await _categoryService.GetAllAsync(cancellationToken);
    viewModel.Categories = categories
        .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
        .ToList();

    // Ștergeți aceste linii:
    // var users = await _userService.GetAllAsync(cancellationToken);
    // viewModel.Users = users
    //     .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name })
    //     .ToList();
}
```

---

## Pasul 7 — Actualizare HomeController

În `Controllers/HomeController.cs`, în metoda `Index`, înlocuiți referința la `User`:

```csharp
// Înlocuiți:
// AuthorName = a.User?.Name ?? "N/A",
// cu:
AuthorName = a.Author?.FullName ?? "N/A",
```

---

## Pasul 8 — Actualizare ArticleRepository

În `Repositories/ArticleRepository.cs`, înlocuiți `.Include(a => a.User)` cu `.Include(a => a.Author)` în toate metodele:

- `GetAllWithDetailsAsync`
- `GetByIdWithDetailsAsync`
- `GetByCategoryAsync`
- `GetPagedAsync`

```csharp
// Peste tot înlocuiți:
// .Include(a => a.User)
// cu:
.Include(a => a.Author)
```

---

## Pasul 9 — Actualizare AppDbContext

Înlocuiți conținutul `Data/AppDbContext.cs`:

```csharp
namespace Lab07.Data;

using Lab07.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Article> Articles { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Article>()
            .HasOne(a => a.Author)
            .WithMany(u => u.Articles)
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
```

**Ce s-a schimbat:**
- `DbContext` → `IdentityDbContext<ApplicationUser>` — creează automat tabelele Identity (`AspNetUsers`, `AspNetRoles`, etc.)
- `DbSet<User>` eliminat
- `OnModelCreating` cu `base.OnModelCreating(modelBuilder)` — **obligatoriu**, altfel tabelele Identity nu se configurează
- Relație `Article → Author` cu `DeleteBehavior.SetNull`
