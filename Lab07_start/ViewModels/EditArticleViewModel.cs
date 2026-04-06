namespace Lab07.ViewModels;

public class EditArticleViewModel : CreateArticleViewModel
{
    public int Id { get; set; }
    public string? ExistingImagePath { get; set; }
}
