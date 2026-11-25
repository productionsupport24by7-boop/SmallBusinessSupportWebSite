using SupportOps.Application.DTOs.Tickets;
using SupportOps.Application.Interfaces;
using SupportOps.Domain.Entities;


namespace SupportOps.Application.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ITicketActivityService _ticketActivityService;

    public TicketService(ITicketRepository repository, ITicketActivityService ticketActivityService)
    {
        _ticketRepository = repository;
        _ticketActivityService = ticketActivityService;
    }

    public async Task<TicketResponse> CreateAsync(
        CreateTicketRequest request,
        int customerId)
    {
        ValidateCreateRequest(request);
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
        await _ticketActivityService.RecordActivityAsync(
    ticketId: ticket.Id,
    activityType: "Created",
    changedByUserId: customerId,
    comment: "Ticket created.");

        return MapToResponse(ticket);
    }

    public async Task<List<TicketResponse>> GetAllAsync()
    {
        var tickets = await _ticketRepository.GetAllAsync();

        return tickets
            .Select(MapToResponse)
            .ToList();
    }


    public async Task<TicketResponse?> GetByIdAsync(
        int id,
        int userId,
        string role)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);

        if (ticket == null)
        {
            return null;
        }

        // Admin can access all tickets.
        if (role == "Admin")
        {
            return MapToResponse(ticket);
        }

        // Customers can access only their own tickets.
        if (role == "Customer" &&
            ticket.CustomerId == userId)
        {
            return MapToResponse(ticket);
        }

        // Support agents can access only tickets assigned to them.
        if (role == "SupportAgent" &&
            ticket.AssignedToUserId == userId)
        {
            return MapToResponse(ticket);
        }

        // Reject all other roles and unauthorized access.
        return null;
    }


    public async Task<bool> UpdateAsync(
    int id,
    UpdateTicketRequest request,
    int userId,
    string role)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Validate input.
        if (request.Title != null &&
            (string.IsNullOrWhiteSpace(request.Title) ||
             request.Title.Trim().Length < 5 ||
             request.Title.Trim().Length > 200))
        {
            throw new ArgumentException("Invalid ticket title.");
        }

        if (request.Description != null &&
            (string.IsNullOrWhiteSpace(request.Description) ||
             request.Description.Trim().Length < 10 ||
             request.Description.Trim().Length > 5000))
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

        var allowedCategories = new[]
        {
        "Technical", "Application", "Database",
        "Access", "Infrastructure", "Other"
    };

        if (request.Priority != null &&
            !allowedPriorities.Contains(
                request.Priority.Trim(),
                StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid ticket priority.");
        }

        if (request.Status != null &&
            !allowedStatuses.Contains(
                request.Status.Trim(),
                StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid ticket status.");
        }

        if (request.Category != null &&
            !allowedCategories.Contains(
                request.Category.Trim(),
                StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid ticket category.");
        }

        if (request.ResolutionDescription != null &&
            (string.IsNullOrWhiteSpace(request.ResolutionDescription) ||
             request.ResolutionDescription.Trim().Length > 4000))
        {
            throw new ArgumentException(
                "Resolution description must be between 1 and 4000 characters.");
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

            // Customers cannot change operational or resolution fields.
            if (request.Status != null ||
                request.ResolutionDescription != null)
            {
                return false;
            }
        }
        else if (isAgent)
        {
            // Agents can only update tickets assigned to them.
            if (ticket.AssignedToUserId != userId)
                return false;
        }
        else if (!isAdmin)
        {
            return false;
        }

        var changes = new List<TicketFieldChange>();

        // Track and apply descriptive field changes.
        if (request.Title != null)
        {
            TrackChange(
                changes,
                nameof(Ticket.Title),
                ticket.Title,
                request.Title.Trim());

            ticket.Title = request.Title.Trim();
        }

        if (request.Description != null)
        {
            TrackChange(
                changes,
                nameof(Ticket.Description),
                ticket.Description,
                request.Description.Trim());

            ticket.Description = request.Description.Trim();
        }

        if (request.Priority != null)
        {
            TrackChange(
                changes,
                nameof(Ticket.Priority),
                ticket.Priority,
                request.Priority.Trim());

            ticket.Priority = request.Priority.Trim();
        }

        if (request.Category != null)
        {
            TrackChange(
                changes,
                nameof(Ticket.Category),
                ticket.Category,
                request.Category.Trim());

            ticket.Category = request.Category.Trim();
        }

        // Only Admin and assigned SupportAgent can change status.
        if (isAdmin || isAgent)
        {
            string newStatus = request.Status?.Trim() ?? ticket.Status;

            bool isBeingResolved =
                newStatus.Equals("Resolved", StringComparison.OrdinalIgnoreCase) &&
                !ticket.Status.Equals("Resolved", StringComparison.OrdinalIgnoreCase);

            // Require a resolution description when resolving a ticket.
            if (isBeingResolved &&
                string.IsNullOrWhiteSpace(request.ResolutionDescription))
            {
                throw new ArgumentException(
                    "Resolution description is required when resolving a ticket.");
            }

            if (request.Status != null)
            {
                TrackChange(
                    changes,
                    nameof(Ticket.Status),
                    ticket.Status,
                    newStatus);

                ticket.Status = newStatus;
            }


            if (request.ResolutionDescription != null &&
    !newStatus.Equals("Resolved", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Resolution description can only be added to a resolved ticket.");
            }
            // Update resolution description when supplied.
            if (request.ResolutionDescription != null)
            {
                TrackChange(
                    changes,
                    nameof(Ticket.ResolutionDescription),
                    ticket.ResolutionDescription,
                    request.ResolutionDescription.Trim());

                ticket.ResolutionDescription =
                    request.ResolutionDescription.Trim();
            }

            if (ticket.Status.Equals(
                "Resolved", StringComparison.OrdinalIgnoreCase))
            {
                ticket.ResolvedOn ??= DateTime.UtcNow;
                ticket.ResolvedByUserId ??= userId;
            }
            else if (!ticket.Status.Equals(
                "Closed", StringComparison.OrdinalIgnoreCase))
            {
                // Clear current resolution details if the ticket is reopened.
                if (ticket.ResolutionDescription != null)
                {
                    TrackChange(
                        changes,
                        nameof(Ticket.ResolutionDescription),
                        ticket.ResolutionDescription,
                        null);
                }

                ticket.ResolutionDescription = null;
                ticket.ResolvedOn = null;
                ticket.ResolvedByUserId = null;
            }
        }

        // No actual field changes: nothing to persist or audit.
        if (changes.Count == 0)
            return true;

        ticket.UpdatedOn = DateTime.UtcNow;
        ticket.UpdatedByUserId = userId;

        bool updated = await _ticketRepository.UpdateAsync(ticket);

        if (!updated)
            return false;

        // Record each changed field in the activity history.
        bool resolvedNow = changes.Any(x =>
            x.FieldName == nameof(Ticket.Status) &&
            string.Equals(
                x.NewValue,
                "Resolved",
                StringComparison.OrdinalIgnoreCase));

        string activityType = resolvedNow ? "Resolved" : "Updated";

        foreach (var change in changes)
        {
            await _ticketActivityService.RecordActivityAsync(
                ticketId: ticket.Id,
                activityType: activityType,
                changedByUserId: userId,
                fieldName: change.FieldName,
                oldValue: change.OldValue,
                newValue: change.NewValue);
        }

        return true;
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


    public async Task<List<TicketResponse>> GetCustomerTicketsAsync(int customerId)
    {
        var tickets = await _ticketRepository.GetCustomerTicketsAsync(customerId);

        return tickets
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<List<TicketResponse>> GetResponsibleTicketsAsync(int agentId)
    {
        var tickets = await _ticketRepository.GetResponsibleTicketsAsync(agentId);

        return tickets
            .Select(MapToResponse)
            .ToList();

    }

    private static void ValidateCreateRequest(
       CreateTicketRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Title) ||
            request.Title.Trim().Length < 5 ||
            request.Title.Length > 200)
        {
            throw new ArgumentException("Invalid ticket title.");
        }

        if (string.IsNullOrWhiteSpace(request.Description) ||
            request.Description.Trim().Length < 10 ||
            request.Description.Length > 5000)
        {
            throw new ArgumentException("Invalid ticket description.");
        }

        var allowedPriorities = new[]
        {
        "Low", "Medium", "High", "Critical"
    };

        var allowedCategories = new[]
        {
        "Technical", "Application", "Database",
        "Access", "Infrastructure", "Other"
    };

        if (!allowedPriorities.Contains(
            request.Priority,
            StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid ticket priority.");
        }

        if (!allowedCategories.Contains(
            request.Category,
            StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid ticket category.");
        }
    }

    public async Task<bool> AssignTicketAsync(
     int ticketId,
     int agentId,
     int adminUserId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);

        if (ticket == null)
            return false;

        if (ticket.AssignedToUserId == agentId)
            return true;

        var oldAgentId = ticket.AssignedToUserId;

        ticket.AssignedToUserId = agentId;
        ticket.UpdatedByUserId = adminUserId;
        ticket.UpdatedOn = DateTime.UtcNow;

        bool updated = await _ticketRepository.UpdateAsync(ticket);

        if (!updated)
            return false;

        await _ticketActivityService.RecordActivityAsync(
            ticketId: ticket.Id,
            activityType: "Assigned",
            changedByUserId: adminUserId,
            fieldName: nameof(Ticket.AssignedToUserId),
            oldValue: oldAgentId?.ToString(),
            newValue: agentId.ToString(),
            comment: "Ticket assigned or reassigned.");

        return true;
    }

    private static void TrackChange(
    List<TicketFieldChange> changes,
    string fieldName,
    string? oldValue,
    string? newValue)
    {
        if (string.Equals(oldValue, newValue, StringComparison.Ordinal))
            return;

        changes.Add(new TicketFieldChange(
            fieldName,
            oldValue,
            newValue));
    }

    private sealed record TicketFieldChange(
        string FieldName,
        string? OldValue,
        string? NewValue);

}