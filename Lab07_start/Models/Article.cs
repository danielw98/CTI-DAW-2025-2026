namespace Lab07.Models;

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

    public int? UserId { get; set; }
    public User? User { get; set; }

    public string? ImagePath { get; set; }
}
