namespace SupportOps.Application.DTOs.Tickets;

public class TicketResponse
{
    public int Id { get; set; }

    public string TicketNumber { get; set; } = string.Empty;

    public int CustomerId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public int? AssignedToUserId { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public DateTime? ResolvedOn { get; set; }
}