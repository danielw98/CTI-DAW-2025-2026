---
title: "DAW Laborator 04 - EF Core Razor Pages"
slug: "lab04"
order: 4
excerpt: "Introducere EF Core + Razor Pages: DbContext, migrations, DI si prima pagina de listare a stirilor."
downloadUrl: "/downloads/DAW-Laborator-04-EF-Core-Razor-Pages.pdf"
repoUrl: "https://github.com/danielw98/DAW-2025-2026/tree/main/labs/lab04"
---

CTI – Dezvoltarea Aplicațiilor Web – Laborator 4

Entity Framework Core + Razor Pages – Proiect: News Portal


# Obiective

În acest laborator vom construi baza aplicației care va fi folosită pe parcursul semestrului.

Aplicația va fi un **portal simplu de știri** (News Portal), implementat cu **Razor Pages** și **Entity Framework Core**.

Obiectivele laboratorului:

- Configurarea Entity Framework Core
- Definirea modelelor și relațiilor între entități
- Utilizarea `DbContext` și `DbSet`
- Configurarea Dependency Injection
- Crearea bazei de date folosind migrations
- Afișarea datelor într-o pagină Razor Pages
- Introducerea conceptelor de bază pentru lucrul cu baza de date într-o aplicație web

# Tehnologii utilizate

- ASP.NET Core
- Razor Pages
- Entity Framework Core
- SQL Server (LocalDB)

# Crearea proiectului

Creați un proiect nou de tip **ASP.NET Core Web App (Razor Pages)**.

Selectați **.NET 8**. Rulați aplicația pentru a verifica că funcționează.

# Instalarea pachetelor NuGet

Instalați următoarele pachete:

- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Tools`

Aceste pachete permit conectarea la baza de date, generarea migrărilor, și executarea comenzilor EF.

# Structura proiectului

Adăugați următoarele foldere și fișiere:

```
Models/
    Article.cs
    Category.cs
Data/
    AppDbContext.cs
Pages/
    Articles/
        Index.cshtml
        Index.cshtml.cs
```

# Modelele aplicației

Vom avea două tabele: **Categories** și **Articles**. O categorie poate avea mai multe articole.

## Modelul `Category`

```csharp
using System.ComponentModel.DataAnnotations;

public class Category
{
    public int Id { get; set; }

    [Required]
    [MinLength(2)]
    public string Name { get; set; } = string.Empty;

    public List<Article> Articles { get; set; } = [];
}
```

## Modelul `Article`

```csharp
using System.ComponentModel.DataAnnotations;

public class Article
{
    public int Id { get; set; }

    [Required]
    [MinLength(5)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MinLength(20)]
    public string Content { get; set; } = string.Empty;

    public DateTime PublishedAt { get; set; } = DateTime.Now;

    public int CategoryId { get; set; }

    public Category Category { get; set; }  = null!;
}
```

# DbContext

[`DbContext`](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/) reprezintă conexiunea dintre aplicație și baza de date. Este clasa principală prin care interacționăm cu baza de date în Entity Framework Core.

Adăugați fișierul `Data/AppDbContext.cs`:

```csharp
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Article> Articles { get; set; }

    public DbSet<Category> Categories { get; set; }
}
```

- Clasa `AppDbContext` **moștenește** `DbContext` — clasa de bază din EF Core care gestionează conexiunea și operațiile cu baza de date.
- **Constructorul** primește un obiect `DbContextOptions<AppDbContext>` care conține configurarea (tipul bazei de date, connection string-ul etc.) și îl pasează clasei de bază prin `: base(options)`.
- Fiecare proprietate de tip [`DbSet<T>`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbset-1) corespunde unui **tabel** din baza de date. `DbSet<Article>` → tabelul `Articles`, `DbSet<Category>` → tabelul `Categories`. Prin aceste proprietăți vom face query-uri și vom adăuga/modifica/șterge date.

# Connection String

În fișierul `appsettings.json` adăugați un câmp pentru connection string:

```json
"ConnectionStrings": {
  "Lab04": ""
}
```

Deschideți **SQL Server Object Explorer** (View → SQL Server Object Explorer) și selectați instanța **(localdb)\MSSQLLocalDB**. Click dreapta → Properties și copiați valoarea **Connection String**.

Lipiți valoarea copiată în câmpul `"Lab04"` din `appsettings.json`. Apoi setați valoarea de la `Initial Catalog` la `NewsDb`.

Rezultatul final ar trebui să arate astfel:

```json
"ConnectionStrings": {
  "Lab04": "Server=(localdb)\\MSSQLLocalDB;Database=NewsDb;Trusted_Connection=True;"
}
```

# Configurarea DbContext

În `Program.cs` adăugați, **după** `builder.Services.AddRazorPages()`:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Lab04")));
```

Această linie înregistrează `AppDbContext` în **containerul de Dependency Injection**. Framework-ul va ști cum să creeze instanțe ale contextului oriunde este cerut.

- `AddDbContext<AppDbContext>(...)` — înregistrează contextul ca serviciu disponibil prin DI.
- `UseSqlServer(...)` — specifică providerul de baze de date (SQL Server).
- `GetConnectionString("Lab04")` — citește connection string-ul cu cheia `"Lab04"` din `appsettings.json`.

# Migrations

**Migrations** sunt mecanismul prin care EF Core traduce modelele C# în structura bazei de date. La fiecare modificare a modelelor, creăm o nouă migrare care descrie schimbările.

În **Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console):

```
Add-Migration InitialCreate
Update-Database
```

- `Add-Migration InitialCreate` — generează un fișier cu instrucțiunile SQL necesare pentru a crea tabelele (pe baza `DbSet`-urilor din `AppDbContext`).
- `Update-Database` — aplică migrarea și creează efectiv baza de date și tabelele în SQL Server.

# Introducerea datelor inițiale

Înainte de a continua, vom adăuga manual câteva înregistrări pentru a avea date de test.

Deschideți **SQL Server Object Explorer** → expandați baza de date `NewsDb` → Tables. Click dreapta pe tabelul `Categories` → **View Data** și adăugați câteva categorii:

| Id | Name |
|---|---|
| 1 | Tehnologie |
| 2 | Sport |
| 3 | Cultură |

Apoi faceți același lucru pentru tabelul `Articles`:

| Id | Title | Content | PublishedAt | CategoryId |
|---|---|---|---|---|
| 1 | Inteligența artificială în educație | Universitățile experimentează noi metode de predare. | 2026-03-10 | 1 |
| 2 | Startul sezonului de Formula 1 | Echipele prezintă noile monoposturi pentru sezon. | 2026-03-15 | 2 |
| 3 | Festival de film european | Proiecții speciale și regizori invitați. | 2026-03-09 | 3 |
| 4 | Noua generație de procesoare | Performanțe mai bune și consum redus de energie. | 2026-03-12 | 1 |
| 5 | Turneu internațional de tenis | Jucători din topul mondial participă la competiție. | 2026-03-11 | 2 |

> **Notă:** `CategoryId` trebuie să corespundă unui `Id` existent din tabelul `Categories`.

# Dependency Injection

Deoarece am înregistrat `AppDbContext` în containerul de DI (în `Program.cs`), îl putem primi automat prin constructor în orice clasă gestionată de framework.

În clasa `IndexModel` (code-behind-ul paginii Razor) adăugăm:

```csharp
private readonly AppDbContext _context;

public IndexModel(AppDbContext context)
{
    _context = context;
}
```

Framework-ul creează automat instanța contextului și o pasează constructorului.

# Razor Pages – Pagina Index

Pagina Index va afișa lista știrilor.

## `Index.cshtml.cs`

```csharp
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Article> Articles { get; set; }

    public void OnGet()
    {
        Articles = _context.Articles.ToList();
    }
}
```

- Proprietatea `Articles` este publică — valorile setate aici vor fi accesibile din fișierul `.cshtml` prin `Model.Articles`.
- Metoda `OnGet()` este apelată automat când pagina primește un request **GET**.

### Trimiterea datelor suplimentare cu `ViewData` / `ViewBag`

Pe lângă proprietățile publice, putem trimite date către pagină folosind `ViewData` — un dicționar `string → object`:

```csharp
public void OnGet()
{
    ViewData["Title"] = "Lista de stiri";
    ViewData["Count"] = _context.Articles.Count();

    Articles = _context.Articles.ToList();
}
```

În `.cshtml` accesăm valorile astfel:

```html
<h1>@ViewData["Title"]</h1>
<p>Total: @ViewData["Count"] articole</p>
```

`ViewBag` este un wrapper dinamic peste `ViewData`. Permite acces cu sintaxă de proprietate în loc de dicționar:

```csharp
// in .cshtml.cs
ViewData["Title"] = "Stiri";       // ViewData - dictionar

// in .cshtml - ambele variante functioneaza:
@ViewData["Title"]
@ViewBag.Title
```

`ViewBag` și `ViewData` partajează aceleași date — sunt două moduri diferite de a accesa același dicționar.

### Primirea parametrilor din URL (query string)

Metoda `OnGet` poate primi parametri direct din URL. De exemplu, pentru `/Articles?category=sport`:

```csharp
public void OnGet(string? category)
{
    // category va avea valoarea "sport" din query string
}
```

Framework-ul face automat legătura între numele parametrului din metodă și cel din URL (**model binding**).

## `Index.cshtml`

```html
@page
@model IndexModel

<h1>Stiri</h1>

<table>
    <thead>
        <tr>
            <th>Title</th>
            <th>Date</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var article in Model.Articles)
        {
            <tr>
                <td>@article.Title</td>
                <td>@article.PublishedAt.ToShortDateString()</td>
            </tr>
        }
    </tbody>
</table>
```

- `@page` — directivă obligatorie care marchează fișierul ca **Razor Page**. Fără ea, fișierul nu va fi tratat ca pagină și nu va răspunde la request-uri.
- `@model IndexModel` — specifică **tipul modelului** asociat paginii. Prin `Model` (cu M mare) accesăm instanța clasei `IndexModel` și proprietățile ei publice (ex: `Model.Articles`).
- `@foreach`, `@article.Title` — sintaxa Razor: prefixul `@` permite scrierea de expresii C# direct în HTML.

# Rutele în Razor Pages

În laboratorul anterior am definit rute explicit pe controllere cu `[Route()]` și `[HttpGet()]`. În **Razor Pages**, rutele sunt generate **automat** pe baza structurii de foldere din `Pages/`:

| Fișier | Ruta generată |
|---|---|
| `Pages/Index.cshtml` | `/` sau `/Index` |
| `Pages/Articles/Index.cshtml` | `/Articles` sau `/Articles/Index` |
| `Pages/Articles/Details.cshtml` | `/Articles/Details` |

Nu trebuie să configurăm nimic — framework-ul mapează automat calea fișierului la URL.

Pentru a adăuga **parametri în rută**, folosim directiva `@page` cu un template:

```html
@page "{id:int}"
```

Aceasta face ca pagina să răspundă la `/Articles/Details/5`, unde `5` este valoarea parametrului `id`. Parametrul va fi disponibil în metoda `OnGet(int id)`.

# Fluxul request-ului

```
Browser → Razor Page → DbContext → SQL Server → DbContext → Razor Page → HTML
```

# Referință — Metode EF Core utilizate

| Metodă | Rol |
|---|---|
| [`DbContext`](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/) | Conexiunea la baza de date |
| [`DbSet<T>`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbset-1) | Reprezintă un tabel |
| [`Include()`](https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager) | Încarcă relații (eager loading) |
| [`ToList()`](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.tolist) | Execută query-ul și returnează o listă |
| [`SaveChanges()`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.savechanges) | Salvează modificările |

# Referință — Metode Razor Pages utilizate

| Metodă | Rol |
|---|---|
| [`PageModel`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.razorpages.pagemodel) | Code-behind pentru pagină |
| `OnGet()` | Handler pentru request GET |
| [`Page()`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.razorpages.pagemodel.page) | Returnează pagina curentă |
| [`RedirectToPage()`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.razorpages.pagemodel.redirecttopage) | Redirect către altă pagină |
| `ModelState.IsValid` | Verifică validarea modelului |

Documentație EF Core: <https://learn.microsoft.com/en-us/ef/core/>

Documentație Razor Pages: <https://learn.microsoft.com/en-us/aspnet/core/razor-pages/>

# Exerciții

Pornind de la pagina Index prezentată mai sus, extindeți aplicația pas cu pas.

- **(1p)** Pagina Index afișează deja titlul și data, dar nu și **categoria** fiecărui articol. Adăugați o coloană nouă în tabel care să afișeze numele categoriei. Asigurați-vă că folosiți `Include()` pentru a încărca relația.

- **(1p)** Articolele ar trebui afișate cu cele mai recente primele. Modificați query-ul din `OnGet()` astfel încât lista să fie ordonată **descrescător** după `PublishedAt`.

- **(2p)** Creați o pagină **Details** (`Pages/Articles/Details.cshtml`) care afișează un singur articol cu toate informațiile (titlu, conținut, categorie, dată).
  - Folosiți `@page "{id:int}"` pentru a primi id-ul din URL (ex: `/Articles/Details/3`)
  - În `OnGet(int id)`, căutați articolul cu `FirstOrDefault()` sau `Find()`
  - Dacă articolul nu există, returnați `NotFound()`

- **(1p)** Reveniți la pagina Index și adăugați un **link către Details** pe fiecare rând din tabel. Folosiți un tag `<a>` cu atributul `href` care să includă id-ul articolului.

- **(2p)** Creați o pagină **Create** (`Pages/Articles/Create.cshtml`) pentru adăugarea unui articol nou.
  - Pagina trebuie să conțină un formular cu câmpuri pentru titlu, conținut și categorie
  - În `OnPost()`, verificați `ModelState.IsValid` înainte de salvare
  - După salvarea cu `SaveChanges()`, faceți redirect către pagina Index cu `RedirectToPage("Index")`
