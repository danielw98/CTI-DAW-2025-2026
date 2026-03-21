using System.ComponentModel.DataAnnotations;

namespace Lab04.Models;

public class Category
{
    public int Id { get; set; }
    [Required]
    [MinLength(2)]
    public string Name { get; set; } = string.Empty;
}
 