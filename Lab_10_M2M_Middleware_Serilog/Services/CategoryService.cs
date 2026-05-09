using Lab10.Models;
using Lab10.Repositories;

namespace Lab10.Services;

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
