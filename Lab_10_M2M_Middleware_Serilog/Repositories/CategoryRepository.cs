using Lab10.Data;
using Lab10.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab10.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }
}
