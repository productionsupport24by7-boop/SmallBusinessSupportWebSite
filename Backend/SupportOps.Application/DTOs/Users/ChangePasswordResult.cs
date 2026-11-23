namespace SupportOps.Application.DTOs.Users;

public class ChangePasswordResult
{
    public bool Succeeded { get; set; }

    public List<string> Errors { get; set; } = [];
}