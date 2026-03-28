namespace Lab06.Models;

using System.ComponentModel.DataAnnotations;

public class Article
{
    public int Id { get; set; }
    [Required]
    [MinLength(5)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Introduceti continutul")]
    [MinLength(20, ErrorMessage = "Continutul articolului trebuie sa aiba cel putin 20 de caractere.")]
    public string Content { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime PublishedAt { get; set; } = DateTime.Now;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }

    public string? ImagePath { get; set; }
}
