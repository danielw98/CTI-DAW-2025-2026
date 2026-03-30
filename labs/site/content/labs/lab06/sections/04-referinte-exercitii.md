# Referință — Metode și concepte noi

| Concept | Rol |
|---|---|
| [`BaseEntity`](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/seedwork-domain-model-base-classes-interfaces) | Clasă de bază cu `Id` — permite constrângeri de tip la `IRepository<T>` |
| [`IRepository<T>`](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design) | Interfață generică pentru data access (`where T : BaseEntity`) |
| [`IUnitOfWork`](https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application) | Coordonează `SaveChangesAsync` — separă persistarea de repository |
| [`CancellationToken`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken) | Propagă semnalul de anulare a request-ului prin toate layerele |
| [`Include()`](https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager) | Eager loading — încarcă relațiile |
| [`AddScoped<IUnitOfWork, UnitOfWork>()`](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) | Înregistrare Unit of Work în DI — repository-urile sunt create intern de UoW |
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
