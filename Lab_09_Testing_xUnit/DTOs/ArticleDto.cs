namespace Lab09.DTOs;

public record ArticleDto(
    int Id,
    string Title,
    string Content,
    DateTime PublishedAt,
    string CategoryName,
    string AuthorName);
