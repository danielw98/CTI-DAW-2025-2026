using Lab10.Models;

namespace Lab10.Services;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default);
}
