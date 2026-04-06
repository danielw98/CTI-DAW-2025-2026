using Lab07.Models;
using Lab07.Repositories;

namespace Lab07.Services;

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
