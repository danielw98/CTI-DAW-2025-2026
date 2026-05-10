namespace Lab12.Services;

using Lab12.Models;

public interface ITagService
{
    Task<List<Tag>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Tag>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}
