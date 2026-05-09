using Lab09.Models;

namespace Lab09.Services;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default);
}
