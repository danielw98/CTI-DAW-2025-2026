namespace Lab07.Models;

using System.ComponentModel.DataAnnotations;

public class User : BaseEntity
{
    [Required]
    [MinLength(3)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public List<Article> Articles { get; set; } = [];
}
