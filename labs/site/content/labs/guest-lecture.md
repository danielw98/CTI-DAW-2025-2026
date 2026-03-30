---
title: "Guest Lecture — Clean Architecture & Vertical Slice în .NET"
slug: "guest-lecture"
order: 0
featured: true
youtubeId: "hLV6evKOK8M"
excerpt: "Prelegere susținută de un invitat din industrie: Clean Architecture, Vertical Slice, Repository Pattern și decizii de design în proiecte reale."
downloadUrl: "/downloads/Lab.zip"
---

# Guest Lecture — Clean Architecture & Vertical Slice în .NET

Prelegere susținută de un invitat din industrie despre structura proiectelor .NET în producție: Clean Architecture, Vertical Slice, Repository Pattern, Unit of Work și decizii de design motivate din experiență reală.

<div style="display:flex;gap:1rem;flex-wrap:wrap;margin:1.5rem 0;">
  <a href="/downloads/Lab.zip" style="display:inline-flex;align-items:center;gap:0.4rem;background:#1e3a5f;color:#7dd3fc;border:1px solid #3b82f644;border-radius:6px;padding:0.55rem 1.1rem;text-decoration:none;font-weight:600;font-size:0.9rem;">
    ⬇ Download proiect demo (Lab.zip)
  </a>
</div>

---

## Înregistrare video

<div style="position:relative;padding-bottom:56.25%;height:0;overflow:hidden;border-radius:10px;margin:1.5rem 0;">
  <iframe
    src="https://www.youtube.com/embed/hLV6evKOK8M"
    title="Guest Lecture — Clean Architecture în .NET"
    frameborder="0"
    allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
    allowfullscreen
    style="position:absolute;top:0;left:0;width:100%;height:100%;">
  </iframe>
</div>

---

## Structura prezentării

Prelegerea a pornit de la **problema concretă** a codului monolitic din Labs 4–5
(Razor Pages), a construit pas cu pas o soluție structurată pe layere și a explicat
motivația fiecărei decizii de design.

---

## 1. Problema cu codul monolitic (Razor Pages)

Când prezentarea, logica de business și accesul la date stau împreună (cum este în
varianta Razor Pages), apar limitări practice:

- **Portabilitate zero:** dacă vrei o aplicație mobilă, trebuie să rescrii totul —
  controller actions întorc `IActionResult` (tip specific ASP.NET), nu există
  interfețe reutilizabile.
- **Testabilitate scăzută:** nu poți testa logica fără să pornești întreaga stivă web.
- **Duplicare:** aceeași regulă de business rescrisă în mai multe locuri.
- **Cuplare tehnologică:** `DataAnnotations` (validări), `IFormFile` (upload) etc.
  sunt tipuri legate strict de ASP.NET — nu funcționează pe Android sau Desktop.

**Concluzia:** separăm lucrurile care se schimbă (UI, platforma) de lucrurile care
rămân stabile (logica de business, accesul la date).

---

## 2. Clean Architecture — imaginea de ansamblu

```
┌─────────────────────────────────────────────────┐
│               Presentation (MVC/API)            │  ← depinde de Application
├─────────────────────────────────────────────────┤
│                   Application                   │  ← depinde de Domain
│     (use cases / handlers, orchestrare)         │
├──────────────────────┬──────────────────────────┤
│   DataAccess         │   Infrastructure         │  ← ambele depind de Application
│   (EF Core repos,    │   (logging, email, JWT,   │
│    Unit of Work)     │    procesare plăți etc.)  │
├──────────────────────┴──────────────────────────┤
│                     Domain                      │  ← nu depinde de nimeni
│   (entități, reguli business, excepții, enums)  │
└─────────────────────────────────────────────────┘
```

**Regula fundamentală:** dependențele merg întotdeauna spre interior.
Un layer exterior știe de cel interior; cel interior nu știe că există layere
deasupra lui.

În proiectul demo:
```
Lab.Presentation  →  Lab.Application  →  Lab.Domain
Lab.DataAccess    →  Lab.Application
Lab.Infrastructure→  Lab.Application
```

---

## 3. Structura soluției demo

```
Lab/
├── Lab.Application/
│   ├── Common/
│   │   ├── DataAccess/       IRepository<T>, IArticleRepository, IUnitOfWork, BaseEntity
│   │   ├── Entities/         ArticleEntity, UserEntity, CategoryEntity
│   │   ├── Errors/           Response<TData>
│   │   ├── Logging/          ILogger
│   │   ├── Mappings/         ArticleModelMapping
│   │   └── Models/           ArticleModel, UserModel, CategoryModel
│   └── Core/
│       └── Articles/         ICreateArticleHandler, CreateArticleHandler
│
├── Lab.DataAccess/
│   └── Core/
│       ├── EfCore/           AppDbContext
│       ├── Repositories/     ArticleRepository (unele metode notImplemented)
│       └── UnitOfWork.cs
│
├── Lab.Infrastructure/
│   └── Core/
│       └── Logging/          Logger (implementare Console.WriteLine)
│
└── Lab.Domain/
    ├── Common/
    │   └── Exceptions/       InvalidDiscountException
    └── Core/
        └── Invoices/         InvoiceService (exemplu demo, nu legat de articole)
```

---

## 4. Presentation Layer — responsabilități

Singurul layer care se înlocuiește complet dacă schimbi platforma (web → mobil).

**Ce face:**
- Preia input de la utilizator (formular, request HTTP)
- Mapează din **Contract** (Request/Response) în **Application Model**
- Apelează un use case din Application layer
- Afișează rezultatul (sau eroarea) utilizatorului

**Ce NU face:**
- Nu conține logică de business
- Nu accesează baza de date direct
- Nu validează datele (în afară de validare de prezentare: `[Required]` pe contracte)

### Contracte (Request / Response)

**Contractele** sunt clasele publice expuse utilizatorilor sau sistemelor externe.
Odată publicate, nu se mai schimbă — sau dacă se schimbă, se **versionează** API-ul
(v1, v2, ...) pentru a nu strica clienții existenți.

```csharp
// Contracts/Requests/CreateArticleRequest.cs
public class CreateArticleRequest
{
    [Required]
    public string Titlu { get; set; }
    public int CategoryId { get; set; }
}

// Contracts/Responses/ArticleResponse.cs
public class ArticleResponse
{
    public int Id { get; set; }
    public string Titlu { get; set; }
    public string CategoryName { get; set; }
}
```

> În proiecte mai mari, contractele se pun într-un proiect separat (ex.
> `Lab.Contracts`) referențiat atât din Presentation, cât și din Application.
> În demo, au stat în Presentation din motive de timp.

### Composition Root (Program.cs)

Locul unde se înregistrează toate serviciile în containerul de DI. Se numește
**composition root** pentru că aici se „compune" toată aplicația.

```csharp
builder.Services.AddTransient<ICreateArticleHandler, CreateArticleHandler>();
builder.Services.AddSingleton<ILogger, Logger>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
```

---

## 5. Mapare între layere

La fiecare graniță dintre layere are loc o conversie de tip:

```
Request          (Presentation)
  → ArticleModel  (Application)   [decuplat de contract]
    → ArticleEntity (DataAccess)  [decuplat de model intern]
  ← ArticleModel  (Application)
← Response       (Presentation)
```

**De ce?** Dacă mâine redenumești o proprietate internă (`Titlu` → `Title`),
contractul publicat rămâne neschimbat.

```csharp
public static class ArticleModelMapping
{
    public static ArticleEntity ToRepository(ArticleModel model)
    {
        return new ArticleEntity
        {
            Titlu = model.Titlu,
            CategoryId = model.CategoryId
        };
    }
}
```

---

## 6. Dependency Injection — cicluri de viață

| Lifetime | Creare | Când se folosește |
|----------|--------|-------------------|
| `Transient` | La fiecare injecție | Obiecte fără stare: validatoare, mappere |
| `Scoped` | O dată per request HTTP | `DbContext`, Repository, Handler |
| `Singleton` | O dată per rulare a aplicației | Logger, configurații, cache |

**Regulă critică:** nu injecta un serviciu `Scoped` (ex. `DbContext`) într-un
`Singleton` — vei primi eroare la runtime sau comportament nedefinit.

---

## 7. Application Layer — responsabilități

Layerul care „orchestrează" tot ce se întâmplă.

```csharp
public async Task<Response<ArticleModel>> HandleAsync(
    ArticleModel model, CancellationToken cancellationToken)
{
    Response<ArticleModel> response = new();
    try
    {
        // 1. Autentificare / Autorizare
        // 2. Validări de input
        if (string.IsNullOrWhiteSpace(model.Titlu))
        {
            response.ErrorMessage = "Titlul nu poate fi gol!";
            return response;
        }
        // 3. Logică de domeniu
        // 4. Persistare
        await _unitOfWork.ArticleRepository.AddAsync(
            model.ToRepository(), cancellationToken);
        await _unitOfWork.SaveChangesAsync();
        response.Data = model;
    }
    catch (InvalidDiscountException)
    {
        response.ErrorMessage = "Clientul selectat nu poate avea discount";
    }
    catch (Exception ex)
    {
        response.ErrorMessage = "Eroare: " + ex.Message;
    }
    return response;
}
```

### Response\<T\> — erori fără excepții necontrolate

```csharp
public class Response<TData>
{
    public string? ErrorMessage { get; set; }
    public TData? Data { get; set; }
}
```

Controller-ul verifică simplu:
```csharp
var result = await _handler.HandleAsync(model, cancellationToken);
if (result.ErrorMessage != null)
    return BadRequest(result.ErrorMessage);
return Ok(result.Data);
```

### Validări: Application vs Domain

| Tip | Exemplu | Unde |
|-----|---------|------|
| Application validation | Titlul nu e gol, email-ul are `@` | Application handler |
| Domain validation (invariant) | Doar userii cu ID 10–20 au discount 25% | Domain layer |

---

## 8. Logging — Dependency Inversion

Interfața se definește în Application (layer interior), implementarea în
Infrastructure (layer exterior):

```csharp
// Lab.Application/Common/Logging/ILogger.cs
public interface ILogger
{
    void Error(string message);
}

// Lab.Infrastructure/Core/Logging/Logger.cs
public class Logger : ILogger
{
    public void Error(string message) => Console.WriteLine($"[ERROR] {message}");
}
```

| Nivel | Utilizare |
|-------|-----------|
| Trace / Debug | Detalii maxime; activat doar la depanare |
| Information | Comportament normal (nivel implicit) |
| Warning | Ceva neașteptat, dar aplicația continuă |
| Error | Eroare gestionată, utilizatorul e afectat |
| Exception | Excepție neașteptată, posibil critică |

---

## 9. Repository Pattern + Unit of Work

```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    Task AddAsync(T entity, CancellationToken cancellationToken);
    void Update(T entity);
    void Delete(T entity);
}

public interface IUnitOfWork
{
    IArticleRepository ArticleRepository { get; }
    Task SaveChangesAsync();
}
```

`SaveChanges` stă pe Unit of Work, nu pe repository, pentru atomicitate:

```csharp
await _unitOfWork.ArticleRepository.AddAsync(article, ct);
await _unitOfWork.MetadataRepository.AddAsync(meta, ct);
await _unitOfWork.SaveChangesAsync(); // o singură tranzacție
```

---

## 10. CancellationToken

Propagă token-ul prin întreg lanțul asincron, de la controller până la EF Core:

```csharp
public async Task<IActionResult> Create(
    CreateArticleRequest request, CancellationToken cancellationToken)
{
    var model = request.ToApplicationModel();
    var result = await _handler.HandleAsync(model, cancellationToken);
    // ...
}
```

---

## 11. Ordinea recomandată de implementare

1. **Domain** — nu depinde de nimeni, poate fi testat imediat
2. **Infrastructure** — servicii externe (logging, email etc.)
3. **Application** — orchestrare use cases, testabil fără UI
4. **Presentation** — ales ultimul, când știi deja ce interfețe există

---

## 12. Demo incomplet — ce a rămas neterminat

| Fișier | Problemă |
|--------|----------|
| `CreateArticleHandler.cs` | `response.Data =` → linie cu sintaxă incompletă |
| `CreateArticleHandler.cs` | `InvoiceService.AddInvoice()` aruncă mereu `InvalidDiscountException` — demo de domeniu |
| `ArticleModel.cs` | `ToRepository()` definit ca `internal`, aruncă `NotImplementedException` |
| `ArticleRepository.cs` | Toate metodele exceptând `AddAsync` și `GetAllAsync` aruncă `NotImplementedException` |
| `UnitOfWork.cs` | `ArticleRepository` creează instanță nouă la fiecare acces (corect: câmp privat) |
| `Program.cs` | Nu există — DI nu este configurat, aplicația nu pornește |

---

## Resurse recomandate

- **"Clean Architecture"** — Robert C. Martin
- **"Domain-Driven Design"** — Eric Evans
- Documentație Microsoft: [Clean Architecture cu ASP.NET Core](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/)
