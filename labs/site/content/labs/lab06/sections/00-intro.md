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
