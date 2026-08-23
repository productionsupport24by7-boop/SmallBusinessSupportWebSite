namespace SupportOps.Application.DTOs;

public class ContactRequestDto
{
    public string FullName { get; set; } = string.Empty;

    public string Company { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Service { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}