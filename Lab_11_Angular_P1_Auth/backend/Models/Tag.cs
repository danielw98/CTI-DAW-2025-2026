using System.ComponentModel.DataAnnotations;

namespace Lab11.Models;

public class Tag : BaseEntity
{
    [Required]
    [MinLength(2)]
    public string Name { get; set; } = string.Empty;

    // Navigare Many-to-Many catre Article
    public List<Article> Articles { get; set; } = new();
}
