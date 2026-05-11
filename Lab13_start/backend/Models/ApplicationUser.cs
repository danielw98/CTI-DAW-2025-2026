using Microsoft.AspNetCore.Identity;

namespace Lab12.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    public List<Article> Articles { get; set; } = [];
}
