using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab10.ViewModels;

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

    // TODO Lab 10 (Ex. 1): Adaugati multi-select pentru tag-uri
    // - public List<int> SelectedTagIds { get; set; } = new();
    // - public List<SelectListItem> AvailableTags { get; set; } = new();
    // SelectedTagIds vine din form (POST), AvailableTags se populeaza in LoadDropdownsAsync
}
