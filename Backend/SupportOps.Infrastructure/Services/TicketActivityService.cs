using Microsoft.EntityFrameworkCore;
using SupportOps.Application.DTOs.Tickets;
using SupportOps.Application.Interfaces;
using SupportOps.Domain.Entities;
using SupportOps.Infrastructure.Data;
using SupportOps.Infrastructure.Identity;

namespace SupportOps.Infrastructure.Services;

public class TicketActivityService : ITicketActivityService
{
    private readonly SupportOpsDbContext _dbContext;

    public TicketActivityService(SupportOpsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task RecordActivityAsync(
        int ticketId,
        string activityType,
        int changedByUserId,
        string? fieldName = null,
        string? oldValue = null,
        string? newValue = null,
        string? comment = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(activityType))
        {
            throw new ArgumentException(
                "Activity type is required.",
                nameof(activityType));
        }

        var activity = new TicketActivity
        {
            TicketId = ticketId,
            ActivityType = activityType.Trim(),
            FieldName = fieldName,
            OldValue = oldValue,
            NewValue = newValue,
            Comment = comment,
            ChangedByUserId = changedByUserId,
            CreatedOn = DateTime.UtcNow
        };

        _dbContext.TicketActivities.Add(activity);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<TicketActivityResponse>> GetActivitiesAsync(
        int ticketId,
        CancellationToken cancellationToken = default)
    {
        var activities = await _dbContext.TicketActivities
            .AsNoTracking()
            .Where(x => x.TicketId == ticketId)
            .OrderByDescending(x => x.CreatedOn)
            .ToListAsync(cancellationToken);

        if (activities.Count == 0)
        {
            return [];
        }

        var userIds = activities
            .Select(x => x.ChangedByUserId)
            .Distinct()
            .ToList();

        var users = await _dbContext.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(
                u => u.Id,
                cancellationToken);

        return activities.Select(activity =>
        {
            users.TryGetValue(
                activity.ChangedByUserId,
                out ApplicationUser? user);

            var changedByName = user == null
                ? "Unknown User"
                : $"{user.FirstName} {user.LastName}".Trim();

            if (string.IsNullOrWhiteSpace(changedByName))
            {
                changedByName = user?.Email ?? "Unknown User";
            }

            return new TicketActivityResponse
            {
                Id = activity.Id,
                TicketId = activity.TicketId,
                ActivityType = activity.ActivityType,
                FieldName = activity.FieldName,
                OldValue = activity.OldValue,
                NewValue = activity.NewValue,
                Comment = activity.Comment,
                ChangedByUserId = activity.ChangedByUserId,
                ChangedByName = changedByName,
                CreatedOn = activity.CreatedOn
            };
        }).ToList();
    }
}