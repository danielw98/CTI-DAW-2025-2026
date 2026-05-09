namespace Lab10.Models;

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
    public DateTime PublishedAt { get; set; } = DateTime.Now;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    // Identity — AuthorId este string (GUID), nu int
    public string? AuthorId { get; set; }
    public ApplicationUser? Author { get; set; }

    public string? ImagePath { get; set; }

    // TODO Lab 10 (Ex. 1): Adaugati navigare Many-to-Many spre Tag
    // - public List<Tag> Tags { get; set; } = new();
}
