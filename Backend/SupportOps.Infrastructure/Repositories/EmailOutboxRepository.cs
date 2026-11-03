using Microsoft.EntityFrameworkCore;
using SupportOps.Application.Interfaces;
using SupportOps.Domain.Entities;
using SupportOps.Infrastructure.Data;

namespace SupportOps.Infrastructure.Repositories;

public class EmailOutboxRepository : IEmailOutboxRepository
{
    private readonly SupportOpsDbContext _context;

    public EmailOutboxRepository(SupportOpsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(EmailOutbox email)
    {
        await _context.EmailOutbox.AddAsync(email);
        await _context.SaveChangesAsync();
    }
}