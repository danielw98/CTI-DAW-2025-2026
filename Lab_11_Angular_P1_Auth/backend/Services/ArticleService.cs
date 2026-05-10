using Lab11.Models;
using Lab11.Repositories;

namespace Lab11.Services;

public class ArticleService : IArticleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ArticleService> _logger;

    public ArticleService(IUnitOfWork unitOfWork, ILogger<ArticleService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<List<Article>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.ArticleRepository.GetAllWithDetailsAsync(cancellationToken);
    }

    public async Task<Article?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.ArticleRepository.GetByIdWithDetailsAsync(id, cancellationToken);
    }

    public async Task AddAsync(Article article, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating article {Title} by author {AuthorId}",
            article.Title, article.AuthorId);

        article.PublishedAt = DateTime.Now;
        await _unitOfWork.ArticleRepository.AddAsync(article, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Article created with id {ArticleId}", article.Id);
    }

    public async Task UpdateAsync(Article article, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating article {ArticleId}", article.Id);
        _unitOfWork.ArticleRepository.Update(article);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var article = await _unitOfWork.ArticleRepository.GetByIdAsync(id, cancellationToken);
        if (article != null)
        {
            _logger.LogInformation("Deleting article {ArticleId}", id);
            _unitOfWork.ArticleRepository.Delete(article);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.ArticleRepository.CountAsync(cancellationToken);
    }

    public async Task<List<Article>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.ArticleRepository.GetPagedAsync(page, pageSize, cancellationToken);
    }
}
