using Microsoft.AspNetCore.Identity;
using SupportOps.Application.DTOs.Users;
using SupportOps.Application.Interfaces;
using SupportOps.Infrastructure.Identity;

namespace SupportOps.Infrastructure.Services;

public class UserProfileService : IUserProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserProfileService(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserProfileResponse?> GetProfileAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new UserProfileResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            Roles = roles.ToList()
        };
    }

    public async Task<bool> UpdateProfileAsync(
        int userId,
        UpdateUserProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return false;

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();

        user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
            ? null
            : request.PhoneNumber.Trim();

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded;
    }

    public async Task<ChangePasswordResult> ChangePasswordAsync(
      int userId,
      ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return new ChangePasswordResult
            {
                Succeeded = false,
                Errors = ["User account was not found."]
            };
        }

        var result = await _userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword);

        return new ChangePasswordResult
        {
            Succeeded = result.Succeeded,
            Errors = result.Errors
                .Select(error => error.Description)
                .ToList()
        };
    }


}