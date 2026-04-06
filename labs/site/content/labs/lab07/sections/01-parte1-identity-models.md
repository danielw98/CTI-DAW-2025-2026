# Parte 1 — Identity: Setup și Modele

---

## Pasul 1 — NuGet package

```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

---

## Pasul 2 — Creare ApplicationUser

### `Models/ApplicationUser.cs` — fișier nou

```csharp
using Microsoft.AspNetCore.Identity;

namespace Lab07.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public List<Article> Articles { get; set; } = [];
}
```

`IdentityUser` oferă: `Id` (string/GUID), `UserName`, `Email`, `PasswordHash`, etc.

> `UserName` și `Email` sunt câmpuri **separate**. `UserName` este ceea ce se afișează în navbar (ex: `"admin"`), `Email` este adresa (ex: `"admin@newsportal.com"`).

---

## Pasul 3 — Actualizare Article.cs

În `Models/Article.cs`, înlocuiți referința la `User` cu `ApplicationUser`:

```csharp
// Ștergeți aceste 2 linii:
// public int? UserId { get; set; }
// public User? User { get; set; }

// Adăugați în loc:
public string? AuthorId { get; set; }
public ApplicationUser? Author { get; set; }
```

> `AuthorId` este `string?` (GUID), nu `int?` — `IdentityUser` folosește GUID ca primary key.

---

## Pasul 4 — Actualizare CreateArticleViewModel

În `ViewModels/CreateArticleViewModel.cs`, ștergeți câmpul `UserId` și lista `Users`:

```csharp
// Ștergeți aceste linii:
// [Display(Name = "Autor")]
// public int? UserId { get; set; }
// public List<SelectListItem> Users { get; set; } = new();
```

Autorul nu se mai alege dintr-un dropdown — va fi setat automat din sesiunea utilizatorului curent.

> `EditArticleViewModel` moștenește din `CreateArticleViewModel`, deci e actualizat automat.

---

## Pasul 5 — Actualizare Views: Create și Edit

### `Views/Articles/Create.cshtml`

Ștergeți blocul `<div>` cu dropdown-ul pentru UserId:

```html
<!-- Ștergeți tot acest bloc: -->
<div class="mb-3">
    <label asp-for="UserId" class="form-label"></label>
    <select asp-for="UserId" class="form-select"
            asp-items="Model.Users">
        <option value="">-- Selectați --</option>
    </select>
    <span asp-validation-for="UserId" class="text-danger"></span>
</div>
```

### `Views/Articles/Edit.cshtml`

Ștergeți același bloc cu dropdown-ul pentru UserId:

```html
<!-- Ștergeți tot acest bloc: -->
<div class="mb-3">
    <label asp-for="UserId" class="form-label"></label>
    <select asp-for="UserId" class="form-select"
            asp-items="Model.Users">
        <option value="">-- Selectați --</option>
    </select>
    <span asp-validation-for="UserId" class="text-danger"></span>
</div>
```
