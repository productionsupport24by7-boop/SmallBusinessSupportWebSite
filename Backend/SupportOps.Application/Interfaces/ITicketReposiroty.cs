using SupportOps.Domain.Entities;

namespace SupportOps.Application.Interfaces;

public interface ITicketRepository
{
    Task<Ticket> CreateAsync(Ticket ticket);

    Task<List<Ticket>> GetAllAsync();

    Task<Ticket?> GetByIdAsync(int id);

    Task<bool> UpdateAsync(Ticket ticket);
    Task<bool> DeleteAsync(int id);
}