using Microsoft.EntityFrameworkCore;
using SupportOps.Application.Interfaces;
using SupportOps.Domain.Entities;
using SupportOps.Infrastructure.Data;

namespace SupportOps.Infrastructure.Repositories;

public class EmailTemplateRepository : IEmailTemplateRepository
{
    private readonly SupportOpsDbContext _context;

    public EmailTemplateRepository(SupportOpsDbContext context)
    {
        _context = context;
    }

    public Task<EmailTemplate?> GetActiveTemplateAsync(string templateCode)
    {
        return _context.EmailTemplates.FirstOrDefaultAsync(
            x => x.TemplateCode == templateCode && x.IsActive);
    }
}