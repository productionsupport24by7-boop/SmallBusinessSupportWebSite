namespace SupportOps.Application.DTOs.Users;

public class UserProfileResponse
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public List<string> Roles { get; set; } = [];
}