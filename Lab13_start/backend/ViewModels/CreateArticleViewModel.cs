using System.ComponentModel.DataAnnotations;
using Lab12.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab12.ViewModels;

public class CreateArticleViewModel
{
    [Required(ErrorMessage = "Titlul este obligatoriu")]
    [MinLength(5, ErrorMessage = "Titlul trebuie să aibă minim 5 caractere")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Conținutul este obligatoriu")]
    [MinLength(20, ErrorMessage = "Conținutul trebuie să aibă minim 20 caractere")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "Categoria este obligatorie")]
    [Display(Name = "Categorie")]
    public int CategoryId { get; set; }

    public IFormFile? Upload { get; set; }

    public List<SelectListItem> Categories { get; set; } = new();

    // Tag-uri Many-to-Many
    public List<int> SelectedTagIds { get; set; } = new();
    public List<Tag> AvailableTags { get; set; } = new();
}
