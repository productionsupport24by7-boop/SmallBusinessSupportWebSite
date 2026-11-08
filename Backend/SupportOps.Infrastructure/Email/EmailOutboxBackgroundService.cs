
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SupportOps.Application.Interfaces;
using SupportOps.Infrastructure.Data;

namespace SupportOps.Infrastructure.Email;

public class EmailOutboxBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmailOutboxBackgroundService> _logger;

    private const int MaxRetryCount = 5;
    private const int BatchSize = 10;

    public EmailOutboxBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<EmailOutboxBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email outbox worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingEmailsAsync(stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in email outbox worker.");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        _logger.LogInformation("Email outbox worker stopped.");
    }

    private async Task ProcessPendingEmailsAsync(
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<SupportOpsDbContext>();

        var emailSender = scope.ServiceProvider
            .GetRequiredService<IEmailSender>();

        var pendingEmails = await dbContext.EmailOutbox
            .Where(x => x.Status == "Pending"
                     && x.RetryCount < MaxRetryCount)
            .OrderBy(x => x.CreatedOn)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        foreach (var email in pendingEmails)
        {
            try
            {
                await emailSender.SendAsync(
                    email.ToEmail,
                    email.Subject,
                    email.Body,
                    cancellationToken);

                email.Status = "Sent";
                email.SentOn = DateTime.UtcNow;
                email.ErrorMessage = null;

                _logger.LogInformation(
                    "Email {EmailId} sent to {Recipient}.",
                    email.Id,
                    email.ToEmail);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                email.RetryCount++;
                email.ErrorMessage = ex.Message;

                email.Status = email.RetryCount >= MaxRetryCount
                    ? "Failed"
                    : "Pending";

                _logger.LogError(
                    ex,
                    "Failed to send email {EmailId}. Attempt {Attempt} of {MaxAttempts}.",
                    email.Id,
                    email.RetryCount,
                    MaxRetryCount);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
