using System.ComponentModel.DataAnnotations;

namespace Lab09.DTOs;

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password);
