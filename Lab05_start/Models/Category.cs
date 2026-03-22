namespace Lab05.Models;

using System.ComponentModel.DataAnnotations;
public class Category
{
    public int Id { get; set; }
    [Required]
    [MinLength(2)]
    public string Name { get; set; } = string.Empty;
    public List<Article> Articles { get; set; } = [];
}