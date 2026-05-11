using System.ComponentModel.DataAnnotations;

namespace Lab12.DTOs;

public record RegisterDto(
    [Required, EmailAddress] string Email,
    [Required, MinLength(3)] string FullName,
    [Required, MinLength(6)] string Password);
