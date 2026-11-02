using System.ComponentModel.DataAnnotations;

namespace SupportOps.API.Models;


// Sprint: User registration
// We'll implement the first version of registration with these rules:
// - Anyone can register through the public registration endpoint.
// - Every new user automatically receives the Customer role.
// - Users cannot choose their own role during registration.
// - Identity securely hashes passwords.
// - Duplicate email addresses are rejected.
// - Admin and SupportAgent accounts remain managed separately.
public class RegisterRequest
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    [StringLength(100)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;
}