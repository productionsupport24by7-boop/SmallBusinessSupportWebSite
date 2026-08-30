namespace SupportOps.Application.DTOs.Tickets;

public class CreateTicketRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Priority { get; set; } = "Medium";

    public string Category { get; set; } = "Other";
}