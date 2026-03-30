---
title: "DAW Laborator 05 - CRUD"
slug: "lab05"
order: 5
excerpt: "CRUD complet in Razor Pages: Details, Create, Edit, Delete, rutare cu parametri si paginare simpla."
downloadUrl: "/downloads/DAW-Laborator-05-CRUD.pdf"
repoUrl: "https://github.com/danielw98/DAW-2025-2026/tree/main/labs/lab05"
---

CTI – Dezvoltarea Aplicațiilor Web – Laborator 5

Razor Pages — CRUD complet, rutare, Tag Helpers, paginare


# Obiective

Laboratorul continuă proiectul **News Portal** din Lab 4. La finalul acestui laborator, aplicația va avea un **CRUD complet** pentru știri, cu validare, rutare cu parametri și paginare.

Obiectivele laboratorului:

- Înțelegerea Tag Helpers: `asp-for`, `asp-page`, `asp-route-id`, `asp-validation-for`
- Rutare cu parametri (`@page "{id:int}"`)
- Atributul `[BindProperty]` pentru model binding în formulare
- Tipul returnat `IActionResult` pentru `OnGet` / `OnPost`
- Implementare completă: Details, Create, Edit, Delete
- Paginare simplă cu `Skip()` / `Take()`
- Adăugarea modelului `User` și legătura cu `Article`

# Recapitulare Lab 4

Din laboratorul anterior avem:

- Modelele `Article` și `Category` cu `DataAnnotations`
- `AppDbContext` cu `DbSet<Article>` și `DbSet<Category>`
- Connection string + DI configurat
- Migrations create și aplicate
- Pagina **Index** care afișează lista articolelor

Structura curentă a folderului `Pages/`:

```
Pages/
    Index.cshtml            ← lista articolelor (pagina principala, ruta /)
    Index.cshtml.cs
```

La finalul acestui laborator:

```
Models/
    Article.cs
    Category.cs
    User.cs                ← NOU
Pages/
    Index.cshtml            ← lista articolelor
    Index.cshtml.cs
    Articles/
        Details.cshtml
        Details.cshtml.cs
        Create.cshtml
        Create.cshtml.cs
        Edit.cshtml
        Edit.cshtml.cs
        Delete.cshtml
        Delete.cshtml.cs
```

# Modelul `User`

Înainte de a implementa CRUD-ul, adăugăm un model nou: `User`. Acesta va fi folosit în laboratoarele viitoare pentru autentificare și pentru a identifica autorul fiecărui articol.

## `Models/User.cs`

```csharp
using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required]
    [MinLength(3)]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public List<Article> Articles { get; set; } = new();
}
```

# Actualizare `Article` — adăugare FK către `User`

Adăugați următoarele proprietăți în clasa `Article`:

```csharp
public int? UserId { get; set; }

public User? User { get; set; }
```

# Actualizare `AppDbContext`

Adăugați noul `DbSet` în `AppDbContext`:

```csharp
public DbSet<User> Users { get; set; }
```

# Migrare

După aceste modificări, creați și aplicați o migrare nouă:

```
Add-Migration AddUser
Update-Database
```

# Tag Helpers

[Tag Helpers](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/intro) permit scrierea de atribute speciale pe elementele HTML, care sunt procesate pe server.

| Tag Helper | Rol | Exemplu |
|---|---|---|
| [`asp-page`](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/built-in/anchor-tag-helper) | Generează link către Razor Page | `<a asp-page="Details">` |
| [`asp-route-{param}`](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/built-in/anchor-tag-helper#asp-route-value) | Trimite un parametru în rută | `<a asp-route-id="@article.Id">` |
| [`asp-for`](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms#the-input-tag-helper) | Leagă un `<input>` de o proprietate a modelului | `<input asp-for="Article.Title" />` |
| [`asp-validation-for`](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms#the-validation-message-tag-helper) | Afișează mesajul de validare pentru o proprietate | `<span asp-validation-for="Article.Title"></span>` |
| [`asp-validation-summary`](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms#the-validation-summary-tag-helper) | Afișează toate erorile de validare | `<div asp-validation-summary="All"></div>` |
| [`asp-items`](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms#the-select-tag-helper) | Populează un `<select>` cu opțiuni | `<select asp-for="Article.CategoryId" asp-items="Model.Categories">` |

Exemplu — link către Details cu id:

```html
<a asp-page="Details" asp-route-id="@article.Id">Detalii</a>
```

Exemplu — input legat de model:

```html
<input asp-for="Article.Title" class="form-control" />
<span asp-validation-for="Article.Title" class="text-danger"></span>
```

Tag helper-ul `asp-for` generează automat atributele `name`, `id`, `value` și `type` pe baza proprietății modelului.

# Rutare cu parametri

Directiva `@page` poate include parametri de rută cu constrângeri de tip:

```html
@page "{id:int}"
```

Aceasta generează ruta `/Articles/Details/3`, unde `3` este valoarea parametrului `id`. Constrângerea `:int` asigură că doar valori numerice sunt acceptate.

## Referințe absolute și relative la pagini

Când folosim `asp-page` sau `RedirectToPage`, calea poate fi **relativă** sau **absolută**:

- **Relativă** (`"Index"`) — rezolvată față de folderul paginii curente. Din `Pages/Articles/Edit.cshtml`, `"Index"` → `Pages/Articles/Index.cshtml`.
- **Absolută** (`"/Index"`) — rezolvată față de folderul `Pages/`. `"/Index"` → `Pages/Index.cshtml` indiferent de unde e apelat.

Deoarece pagina Index se află la `Pages/Index.cshtml`, iar paginile CRUD se află în `Pages/Articles/`, toate referințele spre Index trebuie să folosească calea absolută:

```csharp
return RedirectToPage("/Index");   // corect — merge la Pages/Index.cshtml
// return RedirectToPage("Index"); // gresit — ar cauta Pages/Articles/Index.cshtml
```

Același principiu se aplică și în `.cshtml`:

```html
<a asp-page="/Index">← Înapoi</a>
<a asp-page="/Articles/Details" asp-route-id="@article.Id">Detalii</a>
```

Parametrul din rută este mapat automat pe un parametru al metodei handler:

```csharp
public IActionResult OnGet(int id)
{
    // id este extras din URL
}
```

# `[BindProperty]`

Atributul [`[BindProperty]`](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/#the-bindproperty-attribute) permite model binding automat din formular.

Când utilizatorul trimite un formular (POST), framework-ul populează automat proprietatea decorată cu `[BindProperty]` cu datele din formular.

```csharp
[BindProperty]
public Article Article { get; set; }
```

Fără `[BindProperty]`, proprietatea ar rămâne `null` la POST.

# `IActionResult`

Metodele `OnGet` și `OnPost` pot returna [`IActionResult`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult), ceea ce permite returnarea de rezultate diferite:

| Metodă | Efect |
|---|---|
| `Page()` | Returnează pagina curentă (afișează view-ul) |
| `RedirectToPage("Index")` | Redirect (HTTP 302) către pagina Index |
| `NotFound()` | Returnează HTTP 404 |

Exemplu:

```csharp
public IActionResult OnGet(int id)
{
    Article = _context.Articles.Find(id);

    if (Article == null)
    {
        return NotFound();
    }

    return Page();
}
```

# Pagina Details

Afișează o singură știre, identificată prin `id` din URL.

## `Details.cshtml.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;

    public DetailsModel(AppDbContext context)
    {
        _context = context;
    }

    public Article Article { get; set; }

    public IActionResult OnGet(int id)
    {
        Article = _context.Articles
            .Include(a => a.Category)
            .FirstOrDefault(a => a.Id == id);

        if (Article == null)
        {
            return NotFound();
        }

        return Page();
    }
}
```

## `Details.cshtml`

```html
@page "{id:int}"
@model DetailsModel

<h1>@Model.Article.Title</h1>

<p><strong>Category:</strong> @Model.Article.Category.Name</p>
<p><strong>Data publicării:</strong> @Model.Article.PublishedAt.ToShortDateString()</p>
<p>@Model.Article.Content</p>

<a asp-page="/Index">← Înapoi la listă</a>
```

# Pagina Create

Permite adăugarea unei știri noi.

## `List<SelectListItem>` pentru categorii

Pentru dropdown-ul de categorii, construim manual o [`List<SelectListItem>`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.rendering.selectlistitem):

```csharp
public List<SelectListItem> Categories { get; set; }
```

Se inițializează atât în `OnGet` cât și în `OnPost` (în caz de erori de validare):

```csharp
Categories = _context.Categories
    .Select(c => new SelectListItem
    {
        Value = c.Id.ToString(),
        Text = c.Name
    })
    .ToList();
```

Fiecare `SelectListItem` are `Value` (valoarea trimisă la POST) și `Text` (textul afișat în dropdown). Interogarea LINQ proiectează fiecare `Category` íntr-un `SelectListItem`.

## `Create.cshtml.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;

    public CreateModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Article Article { get; set; }

    public List<SelectListItem> Categories { get; set; }

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

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            LoadCategories();
            return Page();
        }

        Article.PublishedAt = DateTime.Now;

        _context.Articles.Add(Article);
        _context.SaveChanges();

        return RedirectToPage("/Index");
    }
}
```

## `Create.cshtml`

```html
@page
@model CreateModel

<h1>Adaugă articol</h1>

<div asp-validation-summary="All" class="text-danger"></div>

<form method="post">
    <div>
        <label asp-for="Article.Title"></label>
        <input asp-for="Article.Title" class="form-control" />
        <span asp-validation-for="Article.Title" class="text-danger"></span>
    </div>

    <div>
        <label asp-for="Article.Content"></label>
        <textarea asp-for="Article.Content" class="form-control"></textarea>
        <span asp-validation-for="Article.Content" class="text-danger"></span>
    </div>

    <div>
        <label asp-for="Article.CategoryId"></label>
        <select asp-for="Article.CategoryId" asp-items="Model.Categories" class="form-control">
            <option value="">-- Selectați categoria --</option>
        </select>
        <span asp-validation-for="Article.CategoryId" class="text-danger"></span>
    </div>

    <button type="submit" class="btn btn-primary">Salvează</button>
</form>

<a asp-page="/Index">← Înapoi la listă</a>
```

Câmpul `PublishedAt` este setat automat în `OnPost`, nu apare în formular.

Tag helper-ul `asp-for` generează:
- `name="Article.Title"` — necesar pentru model binding
- `id="Article_Title"` — pentru legătura cu `<label>`
- `value` — precompletează câmpul (util la Edit)
- validare HTML5 pe baza `DataAnnotations`

# Pagina Edit

Permite modificarea unei știri existente. Formularul este similar cu Create, dar precompletează câmpurile cu valorile existente.

## `Edit.cshtml.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

public class EditModel : PageModel
{
    private readonly AppDbContext _context;

    public EditModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Article Article { get; set; }

    public List<SelectListItem> Categories { get; set; }

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

    public IActionResult OnGet(int id)
    {
        Article = _context.Articles.Find(id);

        if (Article == null)
        {
            return NotFound();
        }

        LoadCategories();
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            LoadCategories();
            return Page();
        }

        _context.Update(Article);
        _context.SaveChanges();

        return RedirectToPage("/Index");
    }
}
```

**Atenție:** `[BindProperty]` populează obiectul `Article` din formular, inclusiv `Id`-ul. Fără câmpul hidden pentru `Id`, EF Core ar crea o resursă nouă în loc să o actualizeze pe cea existentă.

Metoda [`_context.Update()`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.update) marchează entitatea ca modificată. La apelul `SaveChanges()`, EF generează un `UPDATE` SQL.

## `Edit.cshtml`

```html
@page "{id:int}"
@model EditModel

<h1>Editează articolul</h1>

<div asp-validation-summary="All" class="text-danger"></div>

<form method="post">
    <input type="hidden" asp-for="Article.Id" />

    <div>
        <label asp-for="Article.Title"></label>
        <input asp-for="Article.Title" class="form-control" />
        <span asp-validation-for="Article.Title" class="text-danger"></span>
    </div>

    <div>
        <label asp-for="Article.Content"></label>
        <textarea asp-for="Article.Content" class="form-control"></textarea>
        <span asp-validation-for="Article.Content" class="text-danger"></span>
    </div>

    <div>
        <label asp-for="Article.CategoryId"></label>
        <select asp-for="Article.CategoryId" asp-items="Model.Categories" class="form-control">
            <option value="">-- Selectați categoria --</option>
        </select>
        <span asp-validation-for="Article.CategoryId" class="text-danger"></span>
    </div>

    <button type="submit" class="btn btn-success">Actualizează</button>
</form>

<a asp-page="/Index">← Înapoi la listă</a>
```

Câmpul `<input type="hidden" asp-for="Article.Id" />` este esențial — fără el, `Article.Id` ar fi `0` la POST, iar EF Core ar insera o entitate nouă în loc să o actualizeze.

# Pagina Delete

Afișează o confirmare înainte de ștergere. GET afișează detaliile, POST execută ștergerea.

## `Delete.cshtml.cs`

```csharp
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

    public Article Article { get; set; }

    public IActionResult OnGet(int id)
    {
        Article = _context.Articles
            .Include(a => a.Category)
            .FirstOrDefault(a => a.Id == id);

        if (Article == null)
        {
            return NotFound();
        }

        return Page();
    }

    public IActionResult OnPost(int id)
    {
        var article = _context.Articles.Find(id);

        if (article == null)
        {
            return NotFound();
        }

        _context.Articles.Remove(article);
        _context.SaveChanges();

        return RedirectToPage("/Index");
    }
}
```

## `Delete.cshtml`

```html
@page "{id:int}"
@model DeleteModel

<h1>Ștergere articol</h1>

<p>Sigur doriți să ștergeți acest articol?</p>

<div>
    <h3>@Model.Article.Title</h3>
    <p><strong>Category:</strong> @Model.Article.Category.Name</p>
    <p><strong>Data:</strong> @Model.Article.PublishedAt.ToShortDateString()</p>
    <p>@Model.Article.Content</p>
</div>

<form method="post">
    <button type="submit" class="btn btn-danger">Confirmă ștergerea</button>
</form>

<a asp-page="/Index">← Înapoi la listă</a>
```

Delete-ul folosește **POST** pentru acțiunea de ștergere, nu GET. O ștergere prin GET ar fi o vulnerabilitate (un link simplu ar putea șterge date).

# Actualizarea paginii Index

Actualizăm pagina Index pentru a include linkuri către Details, Edit, Delete, precum și un link către Create.

## `Index.cshtml` — versiunea completă

```html
@page
@model IndexModel

<h1>Articles</h1>

<p><a asp-page="/Articles/Create" class="btn btn-primary">+ Adaugă articol</a></p>

<table class="table">
    <thead>
        <tr>
            <th>Title</th>
            <th>Category</th>
            <th>Data</th>
            <th>Acțiuni</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var article in Model.Articles)
        {
            <tr>
                <td>@article.Title</td>
                <td>@article.Category.Name</td>
                <td>@article.PublishedAt.ToShortDateString()</td>
                <td>
                    <a asp-page="/Articles/Details" asp-route-id="@article.Id">Detalii</a> |
                    <a asp-page="/Articles/Edit" asp-route-id="@article.Id">Editează</a> |
                    <a asp-page="/Articles/Delete" asp-route-id="@article.Id">Șterge</a>
                </td>
            </tr>
        }
    </tbody>
</table>
```

# Paginare simplă cu Skip / Take

Pentru a nu afișa toate știrile pe o singură pagină, implementăm o paginare simplă.

## `Index.cshtml.cs` — cu paginare

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    private const int PageSize = 5;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Article> Articles { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }

    public void OnGet(int? pageNumber)
    {
        CurrentPage = pageNumber ?? 1;

        var totalItems = _context.Articles.Count();
        TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

        Articles = _context.Articles
            .Include(a => a.Category)
            .OrderByDescending(a => a.PublishedAt)
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();
    }
}
```

`Skip()` sare peste elementele din paginile anterioare, `Take()` selectează doar elementele paginii curente.

Parametrul `pageNumber` vine din query string: `/?pageNumber=2`.

## Linkuri de paginare în `Index.cshtml`

Adăugați la finalul paginii, după `</table>`:

```html
<nav>
    @if (Model.CurrentPage > 1)
    {
        <a asp-page="/Index" asp-route-pageNumber="@(Model.CurrentPage - 1)">← Pagina anterioară</a>
    }

    <span>Pagina @Model.CurrentPage din @Model.TotalPages</span>

    @if (Model.CurrentPage < Model.TotalPages)
    {
        <a asp-page="/Index" asp-route-pageNumber="@(Model.CurrentPage + 1)">Pagina următoare →</a>
    }
</nav>
```

# Fluxul CRUD complet

| Operație | Pagina | Metoda HTTP | Handler | EF Core |
|---|---|---|---|---|
| Listare | Index | GET | `OnGet` | `ToList()` |
| Detalii | Details | GET | `OnGet(int id)` | `FirstOrDefault()` |
| Creare | Create | GET + POST | `OnGet` + `OnPost` | `Add()` + `SaveChanges()` |
| Editare | Edit | GET + POST | `OnGet(int id)` + `OnPost` | `Update()` + `SaveChanges()` |
| Ștergere | Delete | GET + POST | `OnGet(int id)` + `OnPost(int id)` | `Remove()` + `SaveChanges()` |

# Referință — Tag Helpers utilizate

| Tag Helper | Descriere |
|---|---|
| [`asp-page`](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/built-in/anchor-tag-helper) | Link către o Razor Page |
| [`asp-route-{param}`](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/built-in/anchor-tag-helper#asp-route-value) | Parametru de rută |
| [`asp-for`](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms#the-input-tag-helper) | Binding input ↔ proprietate model |
| [`asp-items`](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms#the-select-tag-helper) | Populează `<select>` cu opțiuni |
| [`asp-validation-for`](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms#the-validation-message-tag-helper) | Mesaj de eroare per câmp |
| [`asp-validation-summary`](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms#the-validation-summary-tag-helper) | Sumar erori de validare |

# Referință — Metode noi utilizate

| Metodă | Rol |
|---|---|
| [`Find(id)`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.find) | Caută o entitate după cheie primară |
| [`FirstOrDefault()`](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.firstordefault) | Primul element sau `null` |
| [`Add(entity)`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbset-1.add) | Marchează entitatea pentru inserare |
| [`Update(entity)`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.update) | Marchează entitatea ca modificată |
| [`Remove(entity)`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbset-1.remove) | Marchează entitatea pentru ștergere |
| [`Skip(n)`](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.skip) | Sare peste primele n elemente |
| [`Take(n)`](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.take) | Selectează următoarele n elemente |

Documentație Tag Helpers: <https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/intro>

Documentație Razor Pages: <https://learn.microsoft.com/en-us/aspnet/core/razor-pages/>

# Exerciții

- **(1p)** Adăugați în pagina Index o coloană suplimentară care afișează primele 50 de caractere din `Continut` (sau tot conținutul dacă e mai scurt).

- **(1p)** Stilizați pagina Index. Fiecare știre sa fie afișată ca un card, nu ca rând în tabel:
  - **Titlu** (bold)
  - Primele 100 caractere din conținut
  - *Categorie — Data publicării* (italic)
  - Linkuri: Detalii | Editează | Șterge

- **(2p)** Adăugați o pagină **Create** pentru **Categorie** (`Pages/Categories/Create`).
  - Formularul trebuie să conțină un câmp pentru `Nume` cu validare
  - După salvare, redirect către pagina Index (`RedirectToPage("/Index")`)

- **(2p)** Adăugați filtrare pe pagina Index. Adăugați un dropdown cu categoriile și un buton „Filtrează". Doar știrile din categoria selectată sunt afișate. Dacă nu e selectată nicio categorie, se afișează toate.

  Indicații: folosiți un query string `/?categoryId=2` și un parametru opțional `int? categoryId` în `OnGet`.

- **(2p)** Modificați pagina Delete astfel încât ștergerea să funcționeze doar dacă utilizatorul confirmă prin tastarea titlului știrii. Adăugați un `<input>` în formularul de confirmare și validați pe server că textul introdus coincide cu `Article.Title`.

- **(2p)** Pagina **Edit** nu conține suport pentru imagine. Adăugați:
  - Afișarea imaginii curente (dacă există) deasupra formularului
  - Un câmp `<input type="file">` pentru încărcarea unei imagini noi
  - Dacă utilizatorul încarcă o imagine nouă, aceasta o înlocuiește pe cea veche; dacă nu încarcă nimic, imaginea existentă rămâne neschimbată
  - Nu uitați `enctype="multipart/form-data"` pe `<form>` (necesar pentru upload)

  Indicații: injectați `IWebHostEnvironment` în `EditModel`, adăugați `[BindProperty] public IFormFile? Upload`, și păstrați `ImagePath`-ul existent prin `<input type="hidden">` când nu se încarcă imagine nouă.
