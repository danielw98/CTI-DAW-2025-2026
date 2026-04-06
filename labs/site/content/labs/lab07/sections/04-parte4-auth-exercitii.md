# Parte 4 — Autentificare: ViewModels, Controller, Views, Layout și Migrare

---

## Pasul 12 — ViewModels pentru autentificare (fișiere noi)

### `ViewModels/RegisterViewModel.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace Lab07.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Username-ul este obligatoriu")]
    [MinLength(3, ErrorMessage = "Username-ul trebuie să aibă minim 3 caractere")]
    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email-ul este obligatoriu")]
    [EmailAddress(ErrorMessage = "Format email invalid")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Numele complet este obligatoriu")]
    [MinLength(3, ErrorMessage = "Numele trebuie să aibă minim 3 caractere")]
    [Display(Name = "Nume complet")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parola este obligatorie")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Parola trebuie să aibă minim 6 caractere")]
    [Display(Name = "Parolă")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirmarea parolei este obligatorie")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Parolele nu coincid")]
    [Display(Name = "Confirmare parolă")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
```

### `ViewModels/LoginViewModel.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace Lab07.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email-ul este obligatoriu")]
    [EmailAddress(ErrorMessage = "Format email invalid")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parola este obligatorie")]
    [DataType(DataType.Password)]
    [Display(Name = "Parolă")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Ține-mă minte")]
    public bool RememberMe { get; set; }
}
```

---

## Pasul 13 — AuthController (fișier nou)

### `Controllers/AuthController.cs`

```csharp
using Lab07.Models;
using Lab07.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lab07.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email,
            FullName = model.FullName
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Articles");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        // Login cu email: căutăm user-ul după email, autentificăm cu UserName
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Email sau parolă incorectă.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Articles");
        }

        ModelState.AddModelError(string.Empty, "Email sau parolă incorectă.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
```

| Serviciu | Ce face |
|----------|---------|
| `UserManager<ApplicationUser>` | Creează useri, gestionează parole, roluri, claims |
| `SignInManager<ApplicationUser>` | Login, Logout, cookie management |

---

## Pasul 14 — Views pentru autentificare (fișiere noi)

### `Views/Auth/Register.cshtml`

```html
@model RegisterViewModel

@{
    ViewData["Title"] = "Înregistrare";
}

<div class="container mt-5">
    <div class="row justify-content-center">
        <div class="col-md-6">
            <h2>Înregistrare</h2>
            <div asp-validation-summary="ModelOnly" class="text-danger mb-3"></div>
            <form asp-controller="Auth" asp-action="Register" method="post">
                <div class="mb-3">
                    <label asp-for="UserName" class="form-label"></label>
                    <input asp-for="UserName" class="form-control" />
                    <span asp-validation-for="UserName" class="text-danger"></span>
                </div>
                <div class="mb-3">
                    <label asp-for="Email" class="form-label"></label>
                    <input asp-for="Email" class="form-control" />
                    <span asp-validation-for="Email" class="text-danger"></span>
                </div>
                <div class="mb-3">
                    <label asp-for="FullName" class="form-label"></label>
                    <input asp-for="FullName" class="form-control" />
                    <span asp-validation-for="FullName" class="text-danger"></span>
                </div>
                <div class="mb-3">
                    <label asp-for="Password" class="form-label"></label>
                    <input asp-for="Password" class="form-control" type="password" />
                    <span asp-validation-for="Password" class="text-danger"></span>
                </div>
                <div class="mb-3">
                    <label asp-for="ConfirmPassword" class="form-label"></label>
                    <input asp-for="ConfirmPassword" class="form-control" type="password" />
                    <span asp-validation-for="ConfirmPassword" class="text-danger"></span>
                </div>
                <button type="submit" class="btn btn-primary">Înregistrare</button>
                <a asp-controller="Auth" asp-action="Login" class="btn btn-secondary">Deja înregistrat?</a>
            </form>
        </div>
    </div>
</div>
```

### `Views/Auth/Login.cshtml`

```html
@model LoginViewModel

@{
    ViewData["Title"] = "Autentificare";
}

<div class="container mt-5">
    <div class="row justify-content-center">
        <div class="col-md-6">
            <h2>Autentificare</h2>
            <div asp-validation-summary="ModelOnly" class="text-danger mb-3"></div>
            <form asp-controller="Auth" asp-action="Login" method="post">
                @if (ViewData["ReturnUrl"] != null)
                {
                    <input type="hidden" name="returnUrl" value="@ViewData["ReturnUrl"]" />
                }
                <div class="mb-3">
                    <label asp-for="Email" class="form-label"></label>
                    <input asp-for="Email" class="form-control" />
                    <span asp-validation-for="Email" class="text-danger"></span>
                </div>
                <div class="mb-3">
                    <label asp-for="Password" class="form-label"></label>
                    <input asp-for="Password" class="form-control" type="password" />
                    <span asp-validation-for="Password" class="text-danger"></span>
                </div>
                <div class="mb-3 form-check">
                    <input asp-for="RememberMe" class="form-check-input" />
                    <label asp-for="RememberMe" class="form-check-label"></label>
                </div>
                <button type="submit" class="btn btn-primary">Autentificare</button>
                <a asp-controller="Auth" asp-action="Register" class="btn btn-secondary">Înregistrare</a>
            </form>
        </div>
    </div>
</div>
```

---

## Pasul 15 — Layout navbar cu Vizitator

În `Views/Shared/_Layout.cshtml`, înlocuiți navbar-ul cu:

```html
<nav class="navbar navbar-expand-lg navbar-dark bg-dark">
    <div class="container">
        <a class="navbar-brand" asp-controller="Home" asp-action="Index">News Portal</a>
        <div class="navbar-nav">
            <a class="nav-link" asp-controller="Home" asp-action="Index">Acasă</a>
            <a class="nav-link" asp-controller="Articles" asp-action="Index">Articole</a>
            @if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                <a class="nav-link" asp-controller="Articles" asp-action="Create">Articol nou</a>
            }
        </div>
        <div class="navbar-nav ms-auto">
            @if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                <span class="navbar-text me-3">@User.Identity.Name</span>
                <form asp-controller="Auth" asp-action="Logout" method="post" class="d-inline">
                    <button type="submit" class="btn btn-outline-light btn-sm">Logout</button>
                </form>
            }
            else
            {
                <span class="navbar-text me-3">Vizitator</span>
                <a class="nav-link" asp-controller="Auth" asp-action="Login">Login</a>
                <a class="nav-link" asp-controller="Auth" asp-action="Register">Register</a>
            }
        </div>
    </div>
</nav>
```

> Logout este `<form method="post">`, nu `<a href>` — previne CSRF (nu vrem ca un link extern să facă logout).

---

## Pasul 16 — Migration și run

Acum că **tot codul compilează**, facem migrarea:

```bash
dotnet ef migrations add AddIdentity
dotnet ef database drop --force
dotnet ef database update
dotnet run
```

**Verificați:**
1. Aplicația pornește fără erori
2. Accesați `/Auth/Register` → creați un cont
3. Navbar arată username-ul, nu email-ul
4. Logout → navbar arată „Vizitator"
5. `/Auth/Login` cu `admin@newsportal.com` / `Admin@123` → navbar arată „admin"

---

# Exerciții (3p)

## Exercițiul 1: `[Authorize]` + AuthorId automat **(1p)**

- Adăugați `[Authorize]` pe `Create` (GET + POST), `Edit` (GET + POST), `Delete` (GET + POST) din `ArticlesController`
- La `Create` POST, setați `AuthorId` automat:

```csharp
using System.Security.Claims;

var article = new Article
{
    Title = viewModel.Title,
    Content = viewModel.Content,
    CategoryId = viewModel.CategoryId,
    AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier)
};
```

- Testați: accesați `/Articles/Create` neautentificat → redirect la `/Auth/Login?ReturnUrl=...` → login → redirect înapoi

## Exercițiul 2: Content Ownership **(1p)**

- Adăugați helper privat în `ArticlesController`:

```csharp
private bool IsOwnerOrAdmin(Article article)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    return article.AuthorId == userId || User.IsInRole("Admin");
}
```

- Adăugați verificare în `Edit` (GET + POST) și `Delete` (GET + POST):

```csharp
if (!IsOwnerOrAdmin(article))
    return Forbid();  // 403 Forbidden
```

- Testați cu 2 useri diferiți: user A creează articol → user B primește 403 la Edit/Delete → admin poate edita orice

> **401 vs 403:** `Unauthorized` (401) = nu ești autentificat. `Forbid` (403) = ești autentificat, dar nu ai dreptul.

## Exercițiul 3: Role-based visibility în Views **(1p)**

- În `Views/Articles/Index.cshtml`, afișați butoanele Edit/Delete doar utilizatorilor autentificați:

```html
@if (User.Identity != null && User.Identity.IsAuthenticated)
{
    <a asp-action="Edit" asp-route-id="@article.Id" class="btn btn-sm btn-warning">Editare</a>
    <a asp-action="Delete" asp-route-id="@article.Id" class="btn btn-sm btn-danger">Ștergere</a>
}
```

- Aplicați același pattern în `Views/Articles/Details.cshtml`
- Ștergeți `IUserService`, `UserService`, și `Models/User.cs` din proiect (nu mai sunt necesare)

> Ascunderea în View este pentru UX. Verificarea reală de securitate rămâne în controller cu `IsOwnerOrAdmin()`.

---

**Total exerciții: 3p**
