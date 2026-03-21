using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab04.Models;

public class Article
{
    public int Id { get; set; }
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
}