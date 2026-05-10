namespace Lab12.Repositories;

public interface IUnitOfWork
{
    IArticleRepository ArticleRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    ITagRepository TagRepository { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
