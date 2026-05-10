namespace Lab11.Services;

using Lab11.Models;

public interface ITagService
{
    Task<List<Tag>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Tag>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}
