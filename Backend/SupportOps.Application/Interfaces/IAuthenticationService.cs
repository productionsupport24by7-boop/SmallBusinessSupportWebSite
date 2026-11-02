using SupportOps.Application.Models;

namespace SupportOps.Application.Interfaces;


public interface IAuthenticationService
{
    Task<string?> LoginAsync(string email, string password);
    // Add the registration method:
    Task<RegistrationResult> RegisterAsync(
        string firstName,
        string lastName,
        string email,
        string password);
}