namespace Lab10.ViewModels;

public class ArticleViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; } = DateTime.Now;
    public string CategoryName { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string? ImagePath { get; set; }

    // TODO Lab 10 (Ex. 1): Adaugati lista de tag-uri pentru afisare in Details / Index
    // - public List<string> Tags { get; set; } = new();
    // - se populeaza in Mappings din article.Tags.Select(t => t.Name)
}
