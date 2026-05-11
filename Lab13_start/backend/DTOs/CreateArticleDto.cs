using System.ComponentModel.DataAnnotations;

namespace Lab12.DTOs;

public record CreateArticleDto(
    [Required, MinLength(5)] string Title,
    [Required, MinLength(20)] string Content,
    [Required] int CategoryId);
