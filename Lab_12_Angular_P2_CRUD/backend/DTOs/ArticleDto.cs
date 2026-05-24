namespace Lab12.DTOs;

public record ArticleDto(
    int Id,
    string Title,
    string Content,
    DateTime PublishedAt,
    string CategoryName,
    int CategoryId,
    string AuthorName,
    string? AuthorId,
    string? ImagePath,
    List<TagDto> Tags);
