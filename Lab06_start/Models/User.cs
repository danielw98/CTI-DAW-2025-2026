namespace Lab06.Models;

using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }
    [Required]
    [MinLength(3)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public List<Article> Articles { get; set; } = [];
}