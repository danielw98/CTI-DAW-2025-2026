namespace Lab07.ViewModels;

public class HomeViewModel
{
    public List<ArticleViewModel> RecentArticles { get; set; } = [];
    public int TotalArticles { get; set; }
    public int TotalCategories { get; set; }
}
