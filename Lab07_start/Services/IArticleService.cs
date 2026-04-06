using Lab07.Models;

namespace Lab07.Services;

public interface IArticleService
{
    Task<List<Article>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Article?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Article article, CancellationToken cancellationToken = default);
    Task UpdateAsync(Article article, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<List<Article>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}
