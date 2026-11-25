namespace SupportOps.Infrastructure.Email;

public class EmailOutboxOptions
{
    public const string SectionName = "EmailOutbox";

    public bool Enabled { get; set; } = false;

    public int PollingIntervalSeconds { get; set; } = 30;

    public int BatchSize { get; set; } = 10;
}