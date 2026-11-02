namespace SupportOps.Application.Models;


// This keeps the Application layer independent of ASP.NET Identity types.
public class RegistrationResult
{
    public bool Succeeded { get; set; }

    public List<string> Errors { get; set; } = new();
}