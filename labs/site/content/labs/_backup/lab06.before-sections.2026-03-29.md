---
title: "Lab 06 - MVC, Async, Service Layer, Repository"
slug: "lab06"
order: 6
excerpt: "Tranzitia de la Razor Pages la MVC, introducerea async/await si structurarea pe Service + Repository."
downloadUrl: "/downloads/lab06-start.zip"
repoUrl: "https://github.com/danielw98/DAW-2025-2026/tree/main/Lab06_start"
---

CTI – Dezvoltarea Aplicațiilor Web – Laborator 6

**MVC — Refactoring, Async/Await, Service Layer, Repository Pattern**

---

# Obiective

Laboratorul continuă proiectul **News Portal** din Lab 5. Până acum, logica de acces la date se află direct în PageModel-uri, iar operațiile sunt sincrone. În acest laborator vom:

- Restructura aplicația de la Razor Pages la **MVC** (Controllers + Views + ViewModels)
- Introduce pattern-ul **async/await** cu **CancellationToken** pentru operații EF Core
- Crea un **Service Layer** (`IArticleService` / `ArticleService`) pentru logica de business
- Crea un **Repository Pattern** (`IRepository<T>` / `IArticleRepository`) pentru accesul la date
- Implementa **Unit of Work** (`IUnitOfWork`) pentru coordonarea persistării datelor
- Configura **Dependency Injection** pentru toate layerele
- Adăuga un **Layout** comun cu navigare

La finalul laboratorului, aplicația va avea aceeași funcționalitate ca înainte, dar cu o arhitectură curată pe 3 niveluri (Controller → Service → Repository + Unit of Work), pregătită pentru Web API (Lab 7) și autentificare (Lab 8).

# Recapitulare Lab 5

Din laboratorul anterior avem:

- Modelele: `Article`, `Category`, `User` cu `DataAnnotations`
- `AppDbContext` cu `DbSet<Article>`, `DbSet<Category>`, `DbSet<User>`
- Connection string + DI pentru DbContext
- Migrations create și aplicate
- Razor Pages CRUD complet: Index (cu paginare), Details, Create, Edit, Delete
- Operații sincrone (`ToList()`, `SaveChanges()`)
- Accesul la date se face direct din PageModel-uri prin `AppDbContext`

---
# Parte 1 — Tranziție la MVC
---

# De ce restructurăm?

Până acum, fiecare PageModel accesează direct `AppDbContext`:

```csharp
// Lab 5 — logica direct în PageModel
public void OnGet()
{
    Articles = _context.Articles
        .Include(a => a.Category)
        .OrderByDescending(a => a.PublishedAt)
        .ToList();
}
```

Problemele acestei abordări:

- **Duplicare** — aceeași interogare apare în mai multe pagini
- **Testabilitate** — nu putem testa logica fără o bază de date reală
- **Reutilizare** — când vom adăuga Web API (Lab 7), ar trebui să rescriem aceleași query-uri
- **Separare** — UI-ul și accesul la date sunt amestecate

Soluția: separăm aplicația în **3 niveluri** (layers):

```
┌─────────────────────────────────────────────────────┐
│                   HTTP Request                      │
│              + CancellationToken                    │
└────────────────────┬────────────────────────────────┘
                     │
              ┌──────▼──────┐
              │  Controller │  ← primește request-uri, returnează Views
              └──────┬──────┘
                     │ apelează
              ┌──────▼──────┐
              │   Service   │  ← logică de business, reguli, validări
              └──┬───────┬──┘
                 │       │ apelează
          ┌──────▼──┐ ┌──▼─────────┐
          │  Repo   │ │ UnitOfWork │  ← SaveChangesAsync()
          └──┬──────┘ └──┬─────────┘
             │           │ folosesc
           ┌─▼───────────▼─┐
           │    DbContext   │  ← ORM, traduce la SQL (instanță partajată)
           └───────┬───────┘
                   │
            ┌──────▼──────┐
            │  SQL Server │
            └─────────────┘
```

**De ce Repository?**

Repository-ul abstractizează sursa de date. Dacă mâine vreți să stocați articolele în MongoDB sau să citiți dintr-un fișier JSON, înlocuiți doar implementarea Repository — Service-ul și Controller-ul rămân intacte.

> 🤔 **De ce nu facem asta direct în Controller?** Testarea unui Controller care apelează direct DbContext necesită o bază de date. Testarea unui Controller care primește un `IArticleService` necesită doar un obiect mock.

**De ce Service Layer?**

Service Layer conține **logica de business** — regulile specifice domeniului aplicației. De exemplu:
- `article.PublishedAt = DateTime.Now` la creare — nu e o regulă de stocare, e o regulă de business
- Verificarea că articolul există înainte de ștergere — business logic, nu SQL

> 🤔 **Ce s-ar întâmpla dacă am migra la MongoDB?** Repository-ul se schimbă. Service-ul și Controller-ul rămân identice — regulile de business sunt independente de sursa de date.

**Mențiune: CQRS**

O extindere a acestui pattern este [CQRS (Command Query Responsibility Segregation)](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs), care separă operațiile de citire de cele de scriere în servicii/modele distincte. Nu vom implementa CQRS în acest laborator, dar este un concept important de cunoscut pentru proiecte complexe.

# Configurare MVC

## Actualizare `Program.cs` — versiune inițială

Înlocuim Razor Pages cu MVC. Deocamdată pornim cu versiunea minimă (fără Repository și Service înregistrate — le vom adăuga în Parte 2):

```csharp
var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Articles}/{action=Index}/{id?}");

app.Run();
```

Observați: **nu mai avem** `builder.Services.AddRazorPages()` și `app.MapRazorPages()`. Trecem complet la MVC.

## Rute convenționale

Ruta implicită `{controller=Articles}/{action=Index}/{id?}` se decodifică astfel:

| Segment | Semnificație | Exemplu |
|---------|-------------|---------|
| `{controller=Articles}` | Numele controller-ului (default: `Articles`) | `Articles` → `ArticlesController` |
| `{action=Index}` | Numele acțiunii (default: `Index`) | `Index` → metoda `Index()` |
| `{id?}` | Parametru opțional (`?` = nu e obligatoriu) | `5` → `id = 5` |

Exemple de URL-uri:

| URL | Controller | Action | id |
|-----|-----------|--------|-----|
| `/` | ArticlesController | Index | - |
| `/Articles` | ArticlesController | Index | - |
| `/Articles/Details/5` | ArticlesController | Details | 5 |
| `/Articles/Create` | ArticlesController | Create | - |
| `/Articles/Edit/3` | ArticlesController | Edit | 3 |

> 🤔 **De ce nu definim rutele manual pentru fiecare URL, ca în Lab 2?** Rutele convenționale eliminând duplicarea — un singur pattern acoperă sute de URL-uri. Definirea manuală devine imposibil de gestionat pe măsură ce aplicația crește.

## Pipeline-ul de middleware

Ordinea liniilor `app.Use*()` contează. Fiecare linie adaugă un "strat" care procesează request-ul în ordine:

```
Request → UseHttpsRedirection → UseStaticFiles → UseRouting → UseAuthorization → Controller
Response ←──────────────────────────────────────────────────────────────────────────────────
```

- `UseStaticFiles` trebuie să fie **înaintea** rutelor — fișierele statice (CSS, JS, imagini) se servesc direct, fără a ajunge la Controller
- `UseAuthorization` trebuie să fie **după** `UseRouting` — nu putem verifica permisiunile înainte de a ști la ce endpoint merge request-ul

## `AddControllersWithViews()` vs alte variante

| Metodă | Activează | Folosit în |
|--------|-----------|-----------|
| `AddControllersWithViews()` | MVC Controllers + Views | Aplicații web tradiționale |
| `AddRazorPages()` | Razor Pages | Lab 4–5 |
| `AddControllers()` | Doar API Controllers (fără Views) | Web API pur (Lab 7) |

# Async / Await

## De ce async?

ASP.NET Core gestionează cereri HTTP cu un **thread pool** — un număr limitat de thread-uri disponibile simultan (câteva zeci, nu mii).

**Problema cu operații sincrone:**

Când un thread execută `_context.Articles.ToList()`, el trimite query-ul la SQL Server și **stă blocat** așteptând răspunsul — poate 10ms, poate 200ms. În acel timp, thread-ul nu poate servi nicio altă cerere. Cu 50 de utilizatori simultani, ați putea epuiza tot thread pool-ul așteptând SQL Server.

**Soluția cu async/await:**

Când un thread execută `await _context.Articles.ToListAsync()`, el trimite query-ul și se **eliberează înapoi în pool**. Când SQL Server răspunde, framework-ul preia un thread disponibil și continuă execuția. Același thread poate servi alte cereri în timpul așteptării.

```
Sync:   Thread1 ████████████████████ (blocat 200ms)
Async:  Thread1 ░─────────────────── (liber după 1ms)
                              Thread2 ████ (preia răspunsul)
```

> 💡 **Fun fact:** O aplicație ASP.NET Core bine scrisă cu async poate gestiona mii de cereri simultane cu câteva zeci de thread-uri. Fără async, același număr de thread-uri acoperă cu greu câteva zeci de utilizatori.

> 🤔 **De ce async nu ajută pentru calcule matematice complexe?** Calculele sunt **CPU-bound** — thread-ul nu "așteaptă" nimic, e activ tot timpul. Async/await ajută doar operațiile **I/O-bound** (bază de date, fișiere, rețea) unde thread-ul altfel ar sta inactiv.

## Pattern-ul async/await

```csharp
// SYNC (Lab 5)
public List<Article> GetAll()
{
    return _context.Articles.ToList();
}

// ASYNC (Lab 6+)
public async Task<List<Article>> GetAllAsync()
{
    return await _context.Articles.ToListAsync();
}
```

Reguli:
- Metoda este marcată cu `async`
- Tipul returnat devine `Task<T>` (sau `Task` dacă era `void`)
- Apelul EF Core este precedat de `await`
- Prin convenție, numele metodei primește sufixul `Async`

**Regula "async all the way":** dacă Repository-ul este async, Service-ul trebuie să fie async, iar Controller-ul de asemenea. Nu mixați sync cu async.

## Echivalențe Sync → Async

| Sync | Async | Pachet |
|---|---|---|
| `ToList()` | [`ToListAsync()`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.entityframeworkqueryableextensions.tolistasync) | `Microsoft.EntityFrameworkCore` |
| `FirstOrDefault()` | [`FirstOrDefaultAsync()`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.entityframeworkqueryableextensions.firstordefaultasync) | `Microsoft.EntityFrameworkCore` |
| `Find()` | [`FindAsync()`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.findasync) | `Microsoft.EntityFrameworkCore` |
| `SaveChanges()` | [`SaveChangesAsync()`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.savechangesasync) | `Microsoft.EntityFrameworkCore` |
| `Count()` | [`CountAsync()`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.entityframeworkqueryableextensions.countasync) | `Microsoft.EntityFrameworkCore` |

Documentație: [Async programming with EF Core](https://learn.microsoft.com/en-us/ef/core/miscellaneous/async)

# CancellationToken

## De ce CancellationToken?

Când un utilizator închide tab-ul browserului sau când un request depășește un timeout, ASP.NET Core semnalează anularea prin [`CancellationToken`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken). Fără propagarea acestui token, query-urile SQL continuă să se execute pe server chiar dacă nimeni nu mai așteaptă răspunsul — risipă de resurse.

```
Utilizator închide tab-ul
    → ASP.NET Core setează HttpContext.RequestAborted
        → Controller primește CancellationToken (model binding automat)
            → Service primește token-ul ca parametru
                → Repository primește token-ul ca parametru
                    → EF Core anulează comanda SQL
```

## Pattern-ul CancellationToken

```csharp
// FĂRĂ CancellationToken
public async Task<List<Article>> GetAllAsync()
{
    return await _dbSet.ToListAsync();
}

// CU CancellationToken
public async Task<List<Article>> GetAllAsync(CancellationToken cancellationToken = default)
{
    return await _dbSet.ToListAsync(cancellationToken);
}
```

Reguli:
- `CancellationToken` este **ultimul parametru**, cu valoarea implicită `default`
- Se transmite la **fiecare metodă async** din lanț: Controller → Service → Repository → EF Core
- ASP.NET Core injectează automat token-ul în acțiunile controller-ului din `HttpContext.RequestAborted` — nu trebuie să-l creăm manual
- La fel ca "async all the way", avem **"CancellationToken all the way"**

```csharp
// Controller — primește automat de la ASP.NET Core
public async Task<IActionResult> Index(CancellationToken cancellationToken)
{
    var articles = await _articleService.GetAllAsync(cancellationToken);
    // ...
}
```

> 🤔 **Ce se întâmplă dacă nu propagăm CancellationToken?** Query-ul SQL se execută complet pe server, rezultatul este procesat de EF Core, dar apoi este aruncat la gunoi pentru că nimeni nu mai citește response-ul HTTP. Cu CancellationToken, comanda SQL este anulată imediat.

# Modele actualizate �� BaseEntity

Toate modelele noastre au o proprietate `Id`. Extragem-o într-o clasă de bază pentru a putea crea constrângeri de tip în Repository.

## `Models/BaseEntity.cs`

```csharp
public class BaseEntity
{
    public int Id { get; set; }
}
```

> 🤔 **De ce nu punem `Id` direct în fiecare model?** Tehnic funcționează la fel. Beneficiul `BaseEntity` apare la `IRepository<T>`: dacă `T : BaseEntity`, știm garantat că orice entitate are `Id`, și putem scrie `FirstOrDefaultAsync(e => e.Id == id)` direct în implementarea generică. Fără `BaseEntity`, constraint-ul `where T : class` nu garantează existența `Id`.

## `Models/Article.cs`

```csharp
using System.ComponentModel.DataAnnotations;

public class Article : BaseEntity
{
    [Required(ErrorMessage = "Titlul este obligatoriu")]
    [MinLength(5, ErrorMessage = "Titlul trebuie să aibă minim 5 caractere")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Conținutul este obligatoriu")]
    [MinLength(20, ErrorMessage = "Conținutul trebuie să aibă minim 20 caractere")]
    public string Content { get; set; } = string.Empty;

    public DateTime PublishedAt { get; set; } = DateTime.Now;

    public string? ImagePath { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }
}
```

## `Models/Category.cs`

```csharp
using System.ComponentModel.DataAnnotations;

public class Category : BaseEntity
{
    [Required(ErrorMessage = "Numele categoriei este obligatoriu")]
    [MinLength(2, ErrorMessage = "Numele trebuie să aibă minim 2 caractere")]
    public string Name { get; set; } = string.Empty;

    public List<Article> Articles { get; set; } = new();
}
```

> **Notă despre migrare:** Dacă `ImagePath` nu exista în baza de date din Lab 5, creați o migrare nouă:
> ```bash
> dotnet ef migrations add AddImagePathToArticle
> dotnet ef database update
> ```

---
# Parte 2 — Arhitectura pe Straturi
---

# Repository Pattern

Repository-ul abstractizează accesul la baza de date. Controllers și Services nu vor mai ști de EF Core — vor lucra doar cu interfețe.

## Structura noilor foldere

```
Models/
    BaseEntity.cs               ← clasă de bază cu Id
    Article.cs
    Category.cs
    User.cs
Repositories/
    IRepository.cs              ← generic, pentru orice entitate
    Repository.cs               ← implementare generică
    IArticleRepository.cs       ← specific pentru Article
    ArticleRepository.cs        ← implementare cu Include/OrderBy
    IUnitOfWork.cs              ← coordonare SaveChanges
    UnitOfWork.cs               ← implementare cu DbContext
Services/
    IArticleService.cs
    ArticleService.cs
    ICategoryService.cs
    CategoryService.cs
Controllers/
    ArticlesController.cs
Views/
    Articles/
        Index.cshtml
        Details.cshtml
        Create.cshtml
        Edit.cshtml
        Delete.cshtml
    Shared/
        _Layout.cshtml
ViewModels/
    ArticleViewModel.cs
    CreateArticleViewModel.cs
    EditArticleViewModel.cs
```

## `Repositories/IRepository.cs` — Interfața generică

```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
}
```

Interfața este **generică** (`<T>`) cu constrângerea `where T : BaseEntity` — funcționează pentru orice entitate care moștenește `BaseEntity`: `Article`, `Category`, `User`.

Observați: **nu avem `SaveChangesAsync()` aici**. Salvarea datelor este responsabilitatea unui alt pattern — **Unit of Work** — pe care îl vom introduce imediat.

## `Repositories/Repository.cs` — Implementarea generică

```csharp
using Microsoft.EntityFrameworkCore;

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

Observați:
- Constrângerea `where T : BaseEntity` ne permite să scriem `e => e.Id == id` în `GetByIdAsync` — compilatorul știe că `T` are proprietatea `Id`
- `Update` și `Delete` sunt sincrone — EF Core doar marchează entitatea, nu face I/O
- `CancellationToken` este transmis la fiecare metodă async EF Core (`ToListAsync`, `FirstOrDefaultAsync`, `AddAsync`)
- Nu avem `SaveChangesAsync` — acesta aparține `IUnitOfWork`

> 🤔 **De ce avem atât `IRepository<T>` generic cât și `IArticleRepository` specific?** `IRepository<T>` acoperă operațiile CRUD de bază care sunt identice pentru orice entitate. `IArticleRepository` adaugă metode specifice care nu au sens pentru alte entități — cum ar fi `GetAllWithDetailsAsync()` care face `Include(Category)` și `Include(User)`. Separarea permite reutilizarea maximă fără a polua interfața generică.

# Unit of Work

## De ce Unit of Work?

Repository-ul este o **abstracție de colecție** — adaugă, caută, șterge entități. Dar **salvarea în baza de date** nu este responsabilitatea unei colecții. Dacă am pune `SaveChangesAsync()` în `IRepository<T>`, am amesteca două responsabilități diferite.

**Unit of Work** coordonează scrierea în baza de date pentru **toate repository-urile** dintr-o operație:

```
Service:  _articleRepository.Update(article);
          _tagRepository.AddAsync(tag);
          await _unitOfWork.SaveChangesAsync();   ← salvează TOT într-o singură tranzacție
```

Fără Unit of Work, fiecare repository ar salva independent — dacă al doilea `SaveChanges` eșuează, primul rămâne salvat, lăsând datele într-o stare inconsistentă.

> 🤔 **De ce funcționează?** Atât `UnitOfWork` cât și `Repository<T>` sunt înregistrate ca **Scoped** în DI. Într-un request HTTP, DI-ul creează un singur `AppDbContext` pe care îl partajează tuturor. Când Repository-ul marchează o entitate ca modificată, `UnitOfWork.SaveChangesAsync()` vede modificarea prin același `DbContext`.

## `Repositories/IUnitOfWork.cs`

```csharp
public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

## `Repositories/UnitOfWork.cs`

```csharp
using Lab06.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
```

> 🤔 **De ce nu punem `SaveChangesAsync` direct în Repository?** Repository-ul ar trebui să funcționeze ca o colecție (`Add`, `Remove`, `Find`). O colecție nu decide **când** se persistă datele — aceasta este decizia Service Layer-ului. Separarea permite operații care implică mai multe repository-uri într-o singură tranzacție.

## `Repositories/IArticleRepository.cs` — Interfață specifică

Repository-ul generic nu știe de `Include()` sau `OrderBy()`. Creăm o interfață specifică:

```csharp
public interface IArticleRepository : IRepository<Article>
{
    Task<List<Article>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<Article?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Article>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
}
```

## `Repositories/ArticleRepository.cs` — Implementare cu eager loading

```csharp
using Microsoft.EntityFrameworkCore;

public class ArticleRepository : Repository<Article>, IArticleRepository
{
    public ArticleRepository(AppDbContext context) : base(context) { }

    public async Task<List<Article>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Category)
            .Include(a => a.User)
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Article?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Category)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Article>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.CategoryId == categoryId)
            .Include(a => a.Category)
            .Include(a => a.User)
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync(cancellationToken);
    }
}
```

[`Include()`](https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager) încarcă relațiile din baza de date (eager loading). Fără `Include`, `article.Category` ar fi `null`.

# Service Layer

Serviciul conține **logica de business** și este singurul layer pe care Controller-ul îl apelează.

## `Services/IArticleService.cs`

```csharp
public interface IArticleService
{
    Task<List<Article>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Article?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Article article, CancellationToken cancellationToken = default);
    Task UpdateAsync(Article article, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
```

## `Services/ArticleService.cs`

```csharp
public class ArticleService : IArticleService
{
    private readonly IArticleRepository _articleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ArticleService(IArticleRepository articleRepository, IUnitOfWork unitOfWork)
    {
        _articleRepository = articleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Article>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _articleRepository.GetAllWithDetailsAsync(cancellationToken);
    }

    public async Task<Article?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _articleRepository.GetByIdWithDetailsAsync(id, cancellationToken);
    }

    public async Task AddAsync(Article article, CancellationToken cancellationToken = default)
    {
        article.PublishedAt = DateTime.Now;  // logică de business
        await _articleRepository.AddAsync(article, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Article article, CancellationToken cancellationToken = default)
    {
        _articleRepository.Update(article);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var article = await _articleRepository.GetByIdAsync(id, cancellationToken);
        if (article != null)
        {
            _articleRepository.Delete(article);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
```

Serviciul primește `IArticleRepository` și `IUnitOfWork` prin constructor (Dependency Injection). Nu accesează direct `AppDbContext`. Observați: **salvarea se face prin `_unitOfWork`**, nu prin repository — repository-ul marchează entitățile, Unit of Work le persistă.

## `Services/ICategoryService.cs`

Avem nevoie de categorii pentru dropdown-urile din formularele Create/Edit:

```csharp
public interface ICategoryService
{
    Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default);
}
```

## `Services/CategoryService.cs`

```csharp
public class CategoryService : ICategoryService
{
    private readonly IRepository<Category> _categoryRepository;

    public CategoryService(IRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _categoryRepository.GetAllAsync(cancellationToken);
    }
}
```

`CategoryService` folosește repository-ul generic `IRepository<Category>` — nu are nevoie de unul specific. De asemenea, **nu injectează `IUnitOfWork`** deoarece este un serviciu read-only (nu modifică date).

# Dependency Injection

## Cum funcționează DI în ASP.NET Core

Dependency Injection (DI) înseamnă că framework-ul **creează și furnizează** obiectele de care au nevoie clasele voastre, în loc să le creeze ele singure.

Fără DI:
```csharp
// Fiecare clasă creează singură ce are nevoie — greu de testat, greu de schimbat
public class ArticlesController
{
    public ArticlesController()
    {
        var context = new AppDbContext(...);
        var repo = new ArticleRepository(context);
        _service = new ArticleService(repo);
    }
}
```

Cu DI:
```csharp
// Framework-ul injectează ce e nevoie — classes cer, nu creează
public class ArticlesController
{
    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService; // primit "de afară"
    }
}
```

Când `ArticlesController` este creat, DI-ul vede că are nevoie de `IArticleService`. Caută înregistrarea, vede că e `ArticleService`. `ArticleService` are nevoie de `IArticleRepository` → `ArticleRepository`. `ArticleRepository` are nevoie de `AppDbContext`. DI-ul construiește automat **tot lanțul** — acesta se numește **dependency graph**.

## Înregistrare în `Program.cs` — versiunea completă

```csharp
var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Articles}/{action=Index}/{id?}");

app.Run();
```

`IUnitOfWork` este înregistrat ca **Scoped** — aceeași instanță per request HTTP, partajând `AppDbContext` cu toate repository-urile.

## Lifetime-uri

[`AddScoped`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.servicecollectionserviceextensions.addscoped) creează o instanță **per request HTTP**.

| Lifetime | Durata | Când se folosește |
|---|---|---|
| `AddTransient` | O instanță nouă la fiecare injecție | Servicii ușoare, fără stare |
| `AddScoped` | O instanță per request HTTP | Servicii cu `DbContext` |
| `AddSingleton` | O instanță pe toată durata aplicației | Configurări, cache-uri |

> 🤔 **Ce s-ar întâmpla dacă am înregistra `AppDbContext` ca `Singleton`?** `DbContext` nu este thread-safe — nu poate fi accesat simultan din mai multe cereri. Ca Singleton, doi utilizatori cu cereri simultane ar partaja același `DbContext`, ducând la erori imprevizibile. `AddScoped` garantează că fiecare cerere HTTP primește propriul `DbContext`.

## Open generic registration

```csharp
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```

Această linie înregistrează un **open generic** — spune DI-ului: "pentru orice `IRepository<T>`, creează un `Repository<T>`". Fără aceasta, ar trebui să scrieți:

```csharp
builder.Services.AddScoped<IRepository<Article>, Repository<Article>>();
builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
builder.Services.AddScoped<IRepository<User>, Repository<User>>();
// ...pentru fiecare entitate
```

---
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

# Referință — Metode și concepte noi

| Concept | Rol |
|---|---|
| [`BaseEntity`](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/seedwork-domain-model-base-classes-interfaces) | Clasă de bază cu `Id` — permite constrângeri de tip la `IRepository<T>` |
| [`IRepository<T>`](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design) | Interfață generică pentru data access (`where T : BaseEntity`) |
| [`IUnitOfWork`](https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application) | Coordonează `SaveChangesAsync` — separă persistarea de repository |
| [`CancellationToken`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken) | Propagă semnalul de anulare a request-ului prin toate layerele |
| [`Include()`](https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager) | Eager loading — încarcă relațiile |
| [`AddScoped(typeof(IRepository<>), typeof(Repository<>))`](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#open-generics) | Open generic DI registration |
| [`Controller`](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions) | Clasă de bază MVC (Views + JSON) |
| [`View()`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller.view) | Returnează un view Razor |
| [`RedirectToAction()`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase.redirecttoaction) | Redirect HTTP 302 |
| [`NotFound()`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase.notfound) | Returnează HTTP 404 |
| `[ValidateAntiForgeryToken]` | Protecție CSRF pe POST |
| `_Layout.cshtml` | Template comun pentru toate view-urile |
| `_ViewStart.cshtml` | Setează layout-ul implicit |
| `_ViewImports.cshtml` | Using-uri și Tag Helpers globale |

# Exerciții

1) Creați `IUserService` / `UserService` cu metoda `GetAllAsync()`. Refactorizați `LoadDropdownsAsync` din `ArticlesController` să folosească `IUserService` în loc de lista goală.

2) Creați `ICategoryRepository` care extinde `IRepository<Category>` și adăugați o metodă `GetByNameAsync(string name)`. Implementați-o. Refactorizați `CategoryService` să folosească `ICategoryRepository`.

3) Adăugați **paginare** în acțiunea `Index`:
  - Adăugați metode `CountAsync()` și `GetPagedAsync(int page, int pageSize)` în `IArticleService` și `IArticleRepository`
  - Controller-ul primește `?page=2` din query string
  - View-ul afișează butoane Previous/Next
  - Page size implicit: 5

4) Adăugați o pagină **Home** (`HomeController` cu `Views/Home/Index.cshtml`) care afișează:
  - Cele mai recente 3 articole (cards Bootstrap)
  - Numărul total de articole și categorii
  - Link-uri către lista completă
  - Actualizați navigarea din Layout
