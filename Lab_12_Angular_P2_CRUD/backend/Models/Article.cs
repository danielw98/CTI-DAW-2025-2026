namespace Lab12.Models;

using System.ComponentModel.DataAnnotations;

public class Article : BaseEntity
{
    [Required]
    [MinLength(5)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MinLength(20)]
    public string Content { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    // Identity — AuthorId este string (GUID), nu int
    public string? AuthorId { get; set; }
    public ApplicationUser? Author { get; set; }

    public string? ImagePath { get; set; }

    // Navigare Many-to-Many catre Tag
    public List<Tag> Tags { get; set; } = new();
}
