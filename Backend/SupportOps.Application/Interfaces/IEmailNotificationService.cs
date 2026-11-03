
namespace SupportOps.Application.Interfaces;

public interface IEmailNotificationService
{
    Task QueueEmailAsync(
        string templateCode,
        string toEmail,
        IDictionary<string, string> placeholders);
}
