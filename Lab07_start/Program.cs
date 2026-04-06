using Lab07.Data;
using Lab07.Repositories;
using Lab07.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Containerul DI (Dependency Injection) este configurat aici, înainte de construirea aplicației.
// Fiecare serviciu este înregistrat cu durata de viață Scoped — o instanță nouă per request HTTP.
// Lanțul de dependențe: Controller → IArticleService/ICategoryService → IUnitOfWork → AppDbContext.
// ASP.NET Core rezolvă automat aceste dependențe prin constructori, fără a fi nevoie de `new`.

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();

// MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.Initialize(context);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
