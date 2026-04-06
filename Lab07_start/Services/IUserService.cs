using Lab07.Models;

namespace Lab07.Services;

public interface IUserService
{
    Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default);
}
