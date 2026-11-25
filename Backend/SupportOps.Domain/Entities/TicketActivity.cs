namespace SupportOps.Domain.Entities;

public class TicketActivity
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    // Created, Assigned, StatusChanged, PriorityChanged, CommentAdded, Resolved
    public string ActivityType { get; set; } = string.Empty;

    // Example: Status, Priority, AssignedToUserId
    public string? FieldName { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? Comment { get; set; }

    // Authenticated user who performed this activity
    public int ChangedByUserId { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Ticket Ticket { get; set; } = null!;
}