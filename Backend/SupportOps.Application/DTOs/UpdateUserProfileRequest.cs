using System.ComponentModel.DataAnnotations;

namespace SupportOps.Application.DTOs.Users;

public class UpdateUserProfileRequest
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }
}