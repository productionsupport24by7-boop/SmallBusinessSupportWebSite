namespace SupportOps.Application.DTOs.Tickets;

public class TicketActivityResponse
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public string ActivityType { get; set; } = string.Empty;

    public string? FieldName { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? Comment { get; set; }

    public int ChangedByUserId { get; set; }

    public string ChangedByName { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }
}