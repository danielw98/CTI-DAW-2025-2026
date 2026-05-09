using Lab11.Data;
using Lab11.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab11.Repositories;

public class ArticleRepository : Repository<Article>, IArticleRepository
{
    public ArticleRepository(AppDbContext context) : base(context) { }

    public async Task<List<Article>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Author)
            .Include(a => a.Tags)
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Article?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Author)
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Article>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Articles
            .Where(a => a.CategoryId == categoryId)
            .Include(a => a.Category)
            .Include(a => a.Author)
            .Include(a => a.Tags)
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Articles.CountAsync(cancellationToken);
    }

    public async Task<List<Article>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.Author)
            .Include(a => a.Tags)
            .OrderByDescending(a => a.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
