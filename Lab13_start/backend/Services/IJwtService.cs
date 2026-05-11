using Lab12.Models;

namespace Lab12.Services;

public interface IJwtService
{
    Task<string> GenerateTokenAsync(ApplicationUser user);
    int ExpiresInSeconds { get; }
}
