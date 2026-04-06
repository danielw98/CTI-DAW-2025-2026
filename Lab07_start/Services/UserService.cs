using Lab07.Models;
using Lab07.Repositories;

namespace Lab07.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.UserRepository.GetAllAsync(cancellationToken);
    }
}
