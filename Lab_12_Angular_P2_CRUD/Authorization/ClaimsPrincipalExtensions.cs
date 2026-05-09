using System.Security.Claims;
using Lab12.Models;

namespace Lab12.Authorization;

public static class ClaimsPrincipalExtensions
{
    // Owner sau Admin pot modifica articolul. Folosit din ArticlesController (MVC)
    // si ArticlesApiController - tine logica intr-un singur loc.
    public static bool CanModifyArticle(this ClaimsPrincipal user, Article article)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return article.AuthorId == userId || user.IsInRole("Admin");
    }
}
