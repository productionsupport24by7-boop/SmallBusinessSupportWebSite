using SupportOps.Domain.Entities;

namespace SupportOps.Application.Interfaces;

public interface IEmailTemplateRepository
{
    Task<EmailTemplate?> GetActiveTemplateAsync(string templateCode);
}