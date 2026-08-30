using SupportOps.Application.DTOs.Tickets;
using SupportOps.Application.Interfaces;
using SupportOps.Domain.Entities;

namespace SupportOps.Application.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;

    public TicketService(ITicketRepository repository)
    {
        _ticketRepository = repository;
    }

    public async Task<TicketResponse> CreateAsync(
        CreateTicketRequest request,
        int customerId)
    {
        var ticket = new Ticket
        {
            TicketNumber = GenerateTicketNumber(),
            CustomerId = customerId,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            Category = request.Category,
            Status = "Open",
            CreatedOn = DateTime.UtcNow
        };

        await _ticketRepository.CreateAsync(ticket);

        return MapToResponse(ticket);
    }

    public async Task<List<TicketResponse>> GetAllAsync()
    {
        var tickets = await _ticketRepository.GetAllAsync();

        return tickets
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<TicketResponse?> GetByIdAsync(int id, int userId, string role)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);

        if (ticket == null)
        {
            return null;
        }

        // Customers can only view their own tickets
        if (role == "Customer" &&
            ticket.CustomerId != userId)
        {
                // Service layer cannot return IActionResult; indicate no access by returning null
                return null;
        }

        return MapToResponse(ticket);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateTicketRequest request)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);

        if (ticket == null)
        {
            return false;
        }

        if (request.Title != null)
            ticket.Title = request.Title;

        if (request.Description != null)
            ticket.Description = request.Description;

        if (request.Priority != null)
            ticket.Priority = request.Priority;

        if (request.Status != null)
            ticket.Status = request.Status;

        if (request.Category != null)
            ticket.Category = request.Category;

        if (request.AssignedToUserId.HasValue)
            ticket.AssignedToUserId = request.AssignedToUserId;

        ticket.UpdatedOn = DateTime.UtcNow;

        if (ticket.Status == "Resolved" &&
            ticket.ResolvedOn == null)
        {
            ticket.ResolvedOn = DateTime.UtcNow;
        }

        return await _ticketRepository.UpdateAsync(ticket);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _ticketRepository.DeleteAsync(id);
    }

    private static string GenerateTicketNumber()
    {
        return $"INC-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }

    private static TicketResponse MapToResponse(Ticket ticket)
    {
        return new TicketResponse
        {
            Id = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            CustomerId = ticket.CustomerId,
            Title = ticket.Title,
            Description = ticket.Description,
            Priority = ticket.Priority,
            Status = ticket.Status,
            Category = ticket.Category,
            AssignedToUserId = ticket.AssignedToUserId,
            CreatedOn = ticket.CreatedOn,
            UpdatedOn = ticket.UpdatedOn,
            ResolvedOn = ticket.ResolvedOn
        };
    }
}