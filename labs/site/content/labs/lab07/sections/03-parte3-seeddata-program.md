# Parte 3 — SeedData și Program.cs

---

## Pasul 10 — Rescrierea SeedData (async)

Înlocuiți complet `Data/SeedData.cs`. SeedData devine **async** și include crearea rolurilor și a adminului.

> **Ordinea în SeedData contează:** Rolurile trebuie create **înainte** de utilizatorul admin, altfel `AddToRoleAsync` eșuează.

```csharp
using Lab07.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lab07.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        context.Database.Migrate();

        // Seed roles — înainte de admin user
        string[] roleNames = ["Admin", "User"];
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        // Seed admin user — UserName și Email sunt diferite
        var adminEmail = "admin@newsportal.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                FullName = "Administrator",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Seed categories and articles - precum inainte
    }
}
```

---

## Pasul 11 — Actualizare Program.cs

### 11a. Adăugați using-uri

```csharp
using Microsoft.AspNetCore.Identity;
```

### 11b. Adăugați Identity după AddDbContext

```csharp
// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Cookie redirect paths
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/Login";
});
```

### 11c. Scoateți IUserService din DI

```csharp
// Ștergeți:
// builder.Services.AddScoped<IUserService, UserService>();
```

### 11d. Înlocuiți seed-ul sync cu apelul async

```csharp
// Ștergeți blocul vechi:
// using (var scope = app.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     SeedData.Initialize(context);
// }

// Adăugați în loc:
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.InitializeAsync(services);
}
```

### 11e. Adăugați middleware de autentificare

După `app.UseRouting()`, adăugați în ordine:

```csharp
// Ordinea contează!
app.UseAuthentication();
app.UseAuthorization();
```

> `UseAuthentication()` trebuie să fie **înaintea** `UseAuthorization()`. Dacă sunt inversate, autorizarea nu va funcționa.
