using System.ComponentModel.DataAnnotations;

namespace Lab11.DTOs;

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password);
