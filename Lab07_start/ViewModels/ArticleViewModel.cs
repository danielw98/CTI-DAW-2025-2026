namespace Lab07.ViewModels;

public class ArticleViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; } = DateTime.Now;
    public string CategoryName { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
}
