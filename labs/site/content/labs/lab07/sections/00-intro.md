CTI — Dezvoltarea Aplicațiilor Web — Laborator 7

**Autentificare și Autorizare cu ASP.NET Core Identity**

---

# Obiective

Laboratorul continuă proiectul **News Portal** din Lab 6. La finalul acestui laborator, aplicația va avea **user registration, login și role-based access control**.

Obiectivele laboratorului:

- Instalarea și configurarea **ASP.NET Core Identity**
- Crearea `ApplicationUser` care extinde `IdentityUser`
- Modificarea `AppDbContext` pentru a moșteni `IdentityDbContext`
- Implementarea **Register**, **Login** și **Logout** cu MVC
- Crearea și seeding-ul **rolurilor**: Admin, User
- Conceptul de **Vizitator** — utilizatorul neautentificat
- Atributul `[Authorize]` pentru protejarea controller actions
- **Proprietatea conținutului**: doar autorul sau admin-ul poate edita/șterge

# Recapitulare Lab 6

Din laboratorul anterior avem:

- Modelele: `Article`, `Category`, `User` cu `BaseEntity`
- `IArticleService` / `ArticleService`, `ICategoryService` / `CategoryService`
- `IUnitOfWork` / `UnitOfWork`, Repository Pattern
- `ArticlesController` MVC cu Views și ViewModels
- `HomeController`, paginare, `IUserService` / `UserService`

---

# Autentificare vs Autorizare

| Concept | Întrebare | Exemplu |
|---------|-----------|---------|
| **Autentificare** | Cine ești? | Login cu email și parolă |
| **Autorizare** | Ce ai voie? | Doar admin-ul poate șterge orice articol |

## Rolurile din aplicație

| Rol | Descriere | Cum se obține |
|-----|-----------|---------------|
| **Vizitator** | Vede articole, nu poate crea/edita/șterge | Implicit — orice utilizator neautentificat |
| **User** | Poate crea articole, editează doar ale lui | Se atribuie automat la Register |
| **Admin** | Poate edita/șterge orice articol | Creat prin SeedData |

> **Vizitator** nu este un rol Identity stocat în baza de date. Este starea implicită — un utilizator care nu s-a autentificat.

---

> **Notă despre ordinea pașilor:** Nu se poate rula `dotnet ef migrations add` dacă codul nu compilează. De aceea, **toate modificările de cod** (Models, DbContext, ViewModels, Views, Controllers, Program.cs, SeedData) se fac **înainte** de migrare.
