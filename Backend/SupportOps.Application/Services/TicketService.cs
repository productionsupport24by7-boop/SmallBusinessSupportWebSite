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
            CreatedOn = DateTime.UtcNow,
            CreatedByUserId = customerId

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
    UpdateTicketRequest request,
    int userId,
    string role)
    {


        if (request.Title != null &&
            (string.IsNullOrWhiteSpace(request.Title) ||
             request.Title.Length > 200))
        {
            throw new ArgumentException("Invalid ticket title.");
        }

        if (request.Description != null &&
            (string.IsNullOrWhiteSpace(request.Description) ||
             request.Description.Length > 5000))
        {
            throw new ArgumentException("Invalid ticket description.");
        }

        var allowedPriorities = new[]
        {
    "Low", "Medium", "High", "Critical"
};

        var allowedStatuses = new[]
        {
    "Open", "In Progress", "Pending", "Resolved", "Closed"
};

        if (request.Priority != null &&
            !allowedPriorities.Contains(
                request.Priority, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid ticket priority.");
        }

        if (request.Status != null &&
            !allowedStatuses.Contains(
                request.Status, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid ticket status.");
        }

        var ticket = await _ticketRepository.GetByIdAsync(id);

        if (ticket == null)
            return false;

        bool isAdmin = role.Equals(
            "Admin", StringComparison.OrdinalIgnoreCase);

        bool isAgent = role.Equals(
            "SupportAgent", StringComparison.OrdinalIgnoreCase);

        bool isCustomer = role.Equals(
            "Customer", StringComparison.OrdinalIgnoreCase);

        // Customers can only update their own open tickets.
        if (isCustomer)
        {
            if (ticket.CustomerId != userId ||
                !ticket.Status.Equals(
                    "Open", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Customers may edit descriptive fields only.
            if (request.Title != null)
                ticket.Title = request.Title.Trim();

            if (request.Description != null)
                ticket.Description = request.Description.Trim();

            if (request.Priority != null)
                ticket.Priority = request.Priority;

            if (request.Category != null)
                ticket.Category = request.Category;
        }
        else if (isAgent)
        {
            // An agent can only update tickets assigned to them.
            if (ticket.AssignedToUserId != userId)
                return false;

            if (request.Title != null)
                ticket.Title = request.Title.Trim();

            if (request.Description != null)
                ticket.Description = request.Description.Trim();

            if (request.Priority != null)
                ticket.Priority = request.Priority;

            if (request.Category != null)
                ticket.Category = request.Category;

            if (request.Status != null)
                ticket.Status = request.Status;

            // Agents cannot change ticket assignment.
        }
        else if (isAdmin)
        {
            if (request.Title != null)
                ticket.Title = request.Title.Trim();

            if (request.Description != null)
                ticket.Description = request.Description.Trim();

            if (request.Priority != null)
                ticket.Priority = request.Priority;

            if (request.Category != null)
                ticket.Category = request.Category;

            if (request.Status != null)
                ticket.Status = request.Status;

            if (request.AssignedToUserId.HasValue)
                ticket.AssignedToUserId = request.AssignedToUserId;
        }
        else
        {
            return false;
        }

        ticket.UpdatedOn = DateTime.UtcNow;

        if (ticket.Status.Equals(
            "Resolved", StringComparison.OrdinalIgnoreCase))
        {
            ticket.ResolvedOn ??= DateTime.UtcNow;
        }
        else
        {
            ticket.ResolvedOn = null;
        }
        ticket.UpdatedByUserId = userId;

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
            UpdatedByUserId = ticket.UpdatedByUserId,
            ResolvedByUserId = ticket.ResolvedByUserId,
            CreatedOn = ticket.CreatedOn,
            UpdatedOn = ticket.UpdatedOn,
            ResolvedOn = ticket.ResolvedOn
        };
    }




    public async Task<List<TicketResponse>> GetResponsibleTicketsAsync(int customerId)
    {
        var tickets = await _ticketRepository.GetResponsibleTicketsAsync(customerId);

        return tickets
            .Select(MapToResponse)
            .ToList();
        throw new NotImplementedException();
    }
}