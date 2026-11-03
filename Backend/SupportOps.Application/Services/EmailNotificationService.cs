// This implementation will:
// - Find the active template by its code.
// - Replace placeholders in the subject and body.
// - HTML-encode values inserted into the HTML body.
// - Save the completed email to the outbox as Pending.



using System.Text.Encodings.Web;
using SupportOps.Application.Interfaces;
using SupportOps.Domain.Entities;

namespace SupportOps.Application.Services;

public class EmailNotificationService : IEmailNotificationService
{
    private readonly IEmailTemplateRepository _templateRepository;
    private readonly IEmailOutboxRepository _outboxRepository;

    public EmailNotificationService(
        IEmailTemplateRepository templateRepository,
        IEmailOutboxRepository outboxRepository)
    {
        _templateRepository = templateRepository;
        _outboxRepository = outboxRepository;
    }

    public async Task QueueEmailAsync(
        string templateCode,
        string toEmail,
        IDictionary<string, string> placeholders)
    {
        var template = await _templateRepository
            .GetActiveTemplateAsync(templateCode);

        if (template == null)
        {
            throw new InvalidOperationException(
                $"Active email template '{templateCode}' was not found.");
        }

        var subject = template.Subject;
        var body = template.Body;

        foreach (var placeholder in placeholders)
        {
            var token = "{{" + placeholder.Key + "}}";

            subject = subject.Replace(
                token,
                placeholder.Value,
                StringComparison.OrdinalIgnoreCase);

            body = body.Replace(
                token,
                HtmlEncoder.Default.Encode(placeholder.Value),
                StringComparison.OrdinalIgnoreCase);
        }

        var email = new EmailOutbox
        {
            ToEmail = toEmail,
            TemplateCode = templateCode,
            Subject = subject,
            Body = body,
            Status = "Pending",
            RetryCount = 0,
            CreatedOn = DateTime.UtcNow
        };

        await _outboxRepository.AddAsync(email);
    }
}