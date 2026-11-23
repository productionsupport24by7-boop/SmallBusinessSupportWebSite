using SupportOps.Application.DTOs.Users;
using Microsoft.AspNetCore.Identity;

namespace SupportOps.Application.Interfaces;

public interface IUserProfileService
{
    Task<UserProfileResponse?> GetProfileAsync(int userId);

    Task<bool> UpdateProfileAsync(
        int userId,
        UpdateUserProfileRequest request);


Task<ChangePasswordResult> ChangePasswordAsync(
    int userId,
    ChangePasswordRequest request);

}