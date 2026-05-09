using Lab10.Models;
using Lab10.Repositories;

namespace Lab10.Services;

// TODO Lab 10 (Ex. 3): Adaugati structured logging in service
// - injectati ILogger<ArticleService> in constructor
// - in AddAsync, UpdateAsync, DeleteAsync: _logger.LogInformation cu placeholder-e nominale
//   (ex: _logger.LogInformation("Creating article {Title} by {AuthorId}", article.Title, article.AuthorId);)
// - ATENTIE: testele Lab 9 (Lab10.Tests/Services/ArticleServiceTests.cs) trebuie sa treaca
//   NullLogger<ArticleService>.Instance este solutia (vezi materialul, Pasul 19)
public class ArticleService : IArticleService
{
    private readonly IUnitOfWork _unitOfWork;

    public ArticleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
        article.PublishedAt = DateTime.Now;
        await _unitOfWork.ArticleRepository.AddAsync(article, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Article article, CancellationToken cancellationToken = default)
    {
        _unitOfWork.ArticleRepository.Update(article);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var article = await _unitOfWork.ArticleRepository.GetByIdAsync(id, cancellationToken);
        if (article != null)
        {
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
