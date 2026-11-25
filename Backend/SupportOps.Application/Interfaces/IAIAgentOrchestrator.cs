
using SupportOps.Application.DTOs.AI;
namespace SupportOps.Application.Interfaces;


public interface IAIAgentOrchestrator
{
    Task<AIAgentResponse> ExecuteAsync(
        string message,
        int userId,
        string role,
        CancellationToken cancellationToken = default);
}