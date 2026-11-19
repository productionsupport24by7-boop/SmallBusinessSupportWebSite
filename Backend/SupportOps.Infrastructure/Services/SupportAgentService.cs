
using Microsoft.AspNetCore.Identity;
using SupportOps.Application.DTOs.Users;
using SupportOps.Application.Interfaces;
using SupportOps.Infrastructure.Identity;

namespace SupportOps.Infrastructure.Services;

public class SupportAgentService : ISupportAgentService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public SupportAgentService(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<SupportAgentResponse>>
        GetAllSupportAgentsAsync()
    {
        var users = await _userManager.GetUsersInRoleAsync(
            "SupportAgent");

        return users
            .Select(user => new SupportAgentResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty
            })
            .OrderBy(user => user.FirstName)
            .ThenBy(user => user.LastName)
            .ToList();
    }

    public async Task<bool> IsSupportAgentAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(
            userId.ToString());

        if (user == null || !user.IsActive)
            return false;

        return await _userManager.IsInRoleAsync(
            user, "SupportAgent");
    }
}
