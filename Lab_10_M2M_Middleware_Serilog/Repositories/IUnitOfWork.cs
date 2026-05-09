namespace Lab10.Repositories;

public interface IUnitOfWork
{
    IArticleRepository ArticleRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    // TODO Lab 10 (Ex. 1): Adaugati ITagRepository TagRepository { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
