using SupportOps.Application.DTOs.Tickets;

namespace SupportOps.Application.Interfaces;

public interface ITicketService
{
    Task<TicketResponse> CreateAsync(
        CreateTicketRequest request,
        int customerId);

    Task<List<TicketResponse>> GetAllAsync();

    Task<TicketResponse?> GetByIdAsync(int id,
    int userId,
    string role);

    Task<bool> UpdateAsync(
        int id,
        UpdateTicketRequest request, int userId,
    string role);
    Task<bool> DeleteAsync(int id);

    Task<List<TicketResponse>> GetCustomerTicketsAsync(int customerId);
    Task<List<TicketResponse>> GetResponsibleTicketsAsync(int agentId);

    Task<bool> AssignTicketAsync(
    int ticketId,
    int agentId,
    int adminUserId);

}