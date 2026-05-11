using System.ComponentModel.DataAnnotations.Schema;

namespace Lab12.Models;

[Table("ArticleTags")]
public class ArticleTag
{
    public int ArticleId { get; set; }
    public Article Article { get; set; } = null!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
