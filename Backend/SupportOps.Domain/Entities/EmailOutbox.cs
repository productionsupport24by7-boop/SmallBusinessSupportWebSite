namespace SupportOps.Domain.Entities;

public class EmailOutbox
{
    public int Id { get; set; }

    public string ToEmail { get; set; } = string.Empty;

    public string TemplateCode { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public int RetryCount { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public DateTime? SentOn { get; set; }
}