using System.ComponentModel.DataAnnotations;

namespace Lab10.DTOs;

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password);
