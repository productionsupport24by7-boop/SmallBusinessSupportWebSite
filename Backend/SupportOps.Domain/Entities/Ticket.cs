namespace SupportOps.Domain.Entities;

public class Ticket
{
    public int Id { get; set; }

    public string TicketNumber { get; set; } = string.Empty;

    public int CustomerId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Priority { get; set; } = "Medium";

    public string Status { get; set; } = "Open";

    public string Category { get; set; } = "Other";

    public int? AssignedToUserId { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedOn { get; set; }

    public DateTime? ResolvedOn { get; set; }
}