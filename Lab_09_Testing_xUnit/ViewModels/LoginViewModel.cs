using System.ComponentModel.DataAnnotations;

namespace Lab09.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email-ul este obligatoriu")]
    [EmailAddress(ErrorMessage = "Format email invalid")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parola este obligatorie")]
    [DataType(DataType.Password)]
    [Display(Name = "Parolă")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Ține-mă minte")]
    public bool RememberMe { get; set; }
}
