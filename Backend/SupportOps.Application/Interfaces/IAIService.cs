namespace SupportOps.Application.Interfaces;

public interface IAIService
{
    Task<string> GenerateResponseAsync(
        string prompt,
        CancellationToken cancellationToken = default);
}