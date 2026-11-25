using SupportOps.Application.DTOs.Tickets;

namespace SupportOps.Application.Interfaces;

public interface ITicketActivityService
{
    Task RecordActivityAsync(
        int ticketId,
        string activityType,
        int changedByUserId,
        string? fieldName = null,
        string? oldValue = null,
        string? newValue = null,
        string? comment = null,
        CancellationToken cancellationToken = default);

    Task<List<TicketActivityResponse>> GetActivitiesAsync(
        int ticketId,
        CancellationToken cancellationToken = default);
}