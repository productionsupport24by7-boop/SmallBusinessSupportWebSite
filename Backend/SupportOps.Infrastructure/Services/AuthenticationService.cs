
using Microsoft.AspNetCore.Identity;
using SupportOps.Application.Interfaces;
using SupportOps.Infrastructure.Identity;
using SupportOps.Application.Models;
using Microsoft.Extensions.Logging;

namespace SupportOps.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    private readonly IEmailNotificationService _emailNotificationService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IEmailNotificationService emailNotificationService,
    ILogger<AuthenticationService> logger
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _emailNotificationService = emailNotificationService;
        _logger = logger;
    }

    public async Task<string?> LoginAsync(
        string email,
        string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null || !user.IsActive)
            return null;

        var validPassword = await _userManager.CheckPasswordAsync(
            user,
            password);

        if (!validPassword)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        return await _jwtTokenGenerator.GenerateTokenAsync(
            user.Id,
            user.Email ?? string.Empty,
            $"{user.FirstName} {user.LastName}",
            roles);
    }


    // Add inside AuthenticationService

    public async Task<RegistrationResult> RegisterAsync(
    string firstName,
    string lastName,
    string email,
    string password)
    {
        var result = new RegistrationResult();

        firstName = firstName?.Trim() ?? string.Empty;
        lastName = lastName?.Trim() ?? string.Empty;
        email = email?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            result.Errors.Add("All registration fields are required.");
            return result;
        }

        email = email.ToUpperInvariant() == email
            ? email
            : email;

        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            result.Errors.Add("An account with this email already exists.");
            return result;
        }

        if (!await _roleManager.RoleExistsAsync("Customer"))
        {
            result.Errors.Add("Customer role is not configured.");
            return result;
        }

        var user = new ApplicationUser
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            UserName = email,
            IsActive = true,
            EmailConfirmed = false,
            CreatedOn = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            result.Errors.AddRange(
                createResult.Errors.Select(e => e.Description));

            return result;
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "Customer");

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            result.Errors.AddRange(
                roleResult.Errors.Select(e => e.Description));

            return result;
        }


        // Send welcome email asynchronously
        try
        {
            var placeholders = new Dictionary<string, string>
    {
        { "FirstName", user.FirstName },
        { "LastName", user.LastName },
        { "Email", user.Email ?? string.Empty }
    };

            await _emailNotificationService.QueueEmailAsync(
                "USER_REGISTRATION",
                user.Email!,
                placeholders);

            _logger.LogInformation(
                "Welcome email queued for {Email}", user.Email);
        }
        catch (Exception ex)
        {
            // Registration should not fail because of an email queue issue.
            _logger.LogError(
                ex,
                "Failed to queue welcome email for {Email}",
                user.Email);
        }
        result.Succeeded = true;
        return result;
    }
}