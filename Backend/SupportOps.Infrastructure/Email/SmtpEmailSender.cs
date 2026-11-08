using System;
using System.Collections.Generic;
using System.Text;


using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SupportOps.Application.Interfaces;

namespace SupportOps.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;

    public SmtpEmailSender(IOptions<SmtpSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(
            _settings.FromName,
            _settings.FromEmail));

        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        message.Body = new BodyBuilder
        {
            HtmlBody = htmlBody
        }.ToMessageBody();

        using var client = new SmtpClient();

        var secureOption = _settings.UseStartTls
            ? SecureSocketOptions.StartTls
            : SecureSocketOptions.Auto;

        await client.ConnectAsync(
            _settings.Host,
            _settings.Port,
            secureOption,
            cancellationToken);

        await client.AuthenticateAsync(
            _settings.UserName,
            _settings.Password,
            cancellationToken);

        await client.SendAsync(message, cancellationToken);

        await client.DisconnectAsync(
            true,
            cancellationToken);
    }
}
