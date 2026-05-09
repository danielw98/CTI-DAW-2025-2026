using Lab10.Data;

namespace Lab10.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private IArticleRepository? _articleRepository;
    private ICategoryRepository? _categoryRepository;
    // TODO Lab 10 (Ex. 1): Adaugati cache field pentru _tagRepository (private ITagRepository? _tagRepository;)

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IArticleRepository ArticleRepository
        => _articleRepository ??= new ArticleRepository(_context);

    public ICategoryRepository CategoryRepository
        => _categoryRepository ??= new CategoryRepository(_context);

    // TODO Lab 10 (Ex. 1): Implementati ITagRepository TagRepository
    // - urmati pattern-ul de mai sus: lazy-init prin ??= new TagRepository(_context)

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
