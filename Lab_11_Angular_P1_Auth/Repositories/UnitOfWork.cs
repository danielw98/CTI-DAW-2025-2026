using Lab11.Data;

namespace Lab11.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private IArticleRepository? _articleRepository;
    private ICategoryRepository? _categoryRepository;
    private ITagRepository? _tagRepository;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IArticleRepository ArticleRepository
        => _articleRepository ??= new ArticleRepository(_context);

    public ICategoryRepository CategoryRepository
        => _categoryRepository ??= new CategoryRepository(_context);

    public ITagRepository TagRepository
        => _tagRepository ??= new TagRepository(_context);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
