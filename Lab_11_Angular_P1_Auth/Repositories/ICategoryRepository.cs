using Lab11.Models;

namespace Lab11.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
