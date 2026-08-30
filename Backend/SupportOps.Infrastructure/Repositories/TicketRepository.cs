using Microsoft.EntityFrameworkCore;
using SupportOps.Application.Interfaces;
using SupportOps.Domain.Entities;
using SupportOps.Infrastructure.Data;

namespace SupportOps.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly SupportOpsDbContext _context;

    public TicketRepository(SupportOpsDbContext context)
    {
        _context = context;
    }

    public async Task<Ticket> CreateAsync(Ticket ticket)
    {
        _context.Tickets.Add(ticket);

        await _context.SaveChangesAsync();

        return ticket;
    }

    public async Task<List<Ticket>> GetAllAsync()
    {
        return await _context.Tickets
            .OrderByDescending(x => x.CreatedOn)
            .ToListAsync();
    }

    public async Task<Ticket?> GetByIdAsync(int id)
    {
        return await _context.Tickets
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> UpdateAsync(Ticket ticket)
    {
        _context.Tickets.Update(ticket);

        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == id);

        if (ticket == null)
        {
            return false;
        }

        _context.Tickets.Remove(ticket);

        await _context.SaveChangesAsync();

        return true;
    }
}