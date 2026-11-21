using SupportOps.Application.DTOs.Users;

namespace SupportOps.Application.Interfaces;

public interface IUserProfileService
{
    Task<UserProfileResponse?> GetProfileAsync(int userId);

    Task<bool> UpdateProfileAsync(
        int userId,
        UpdateUserProfileRequest request);
}