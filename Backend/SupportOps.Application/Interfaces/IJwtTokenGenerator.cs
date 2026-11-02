namespace SupportOps.Application.Interfaces;

public interface IJwtTokenGenerator
{
    Task<string> GenerateTokenAsync(
        int userId,
        string email,
        string fullName,
        IEnumerable<string> roles);
}