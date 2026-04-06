using Lab07.Models;

namespace Lab07.Services;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default);
}
