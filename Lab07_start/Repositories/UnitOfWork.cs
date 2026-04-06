using Lab07.Data;
using Lab07.Models;

namespace Lab07.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private IArticleRepository? _articleRepository;
    private ICategoryRepository? _categoryRepository;
    private IRepository<User>? _userRepository;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IArticleRepository ArticleRepository
        => _articleRepository ??= new ArticleRepository(_context);

    public ICategoryRepository CategoryRepository
        => _categoryRepository ??= new CategoryRepository(_context);

    public IRepository<User> UserRepository
        => _userRepository ??= new Repository<User>(_context);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
