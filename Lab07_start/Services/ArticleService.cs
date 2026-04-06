using Lab07.Models;
using Lab07.Repositories;

namespace Lab07.Services;

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
