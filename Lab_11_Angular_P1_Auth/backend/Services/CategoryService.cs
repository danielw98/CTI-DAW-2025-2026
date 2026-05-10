using Lab11.Models;
using Lab11.Repositories;

namespace Lab11.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.CategoryRepository.GetAllAsync(cancellationToken);
    }
}
