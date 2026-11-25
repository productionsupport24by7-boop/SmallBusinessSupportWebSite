using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportOps.Application.DTOs.Tickets;
using SupportOps.Application.Interfaces;

namespace SupportOps.API.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly ISupportAgentService _supportAgentService;
    private readonly ITicketActivityService _ticketActivityService;

    public TicketsController(ITicketService ticketService, ISupportAgentService supportAgentService, ITicketActivityService ticketActivityService   )
    {
        _ticketService = ticketService;
        _supportAgentService = supportAgentService;
        _ticketActivityService = ticketActivityService;
    }


    [HttpPut("{id:int}/assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignTicket(
    int id,
    [FromBody] AssignTicketRequest request)
    {
        if (request.AssignedToUserId <= 0)
        {
            return BadRequest(new
            {
                message = "A valid Support Agent must be selected."
            });
        }

        var isSupportAgent =
            await _supportAgentService.IsSupportAgentAsync(
                request.AssignedToUserId);

        if (!isSupportAgent)
        {
            return BadRequest(new
            {
                message = "The selected user is not an active Support Agent."
            });
        }

        var adminIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(adminIdClaim, out var adminId))
            return Unauthorized();

        var updated = await _ticketService.AssignTicketAsync(
            id,
            request.AssignedToUserId,
            adminId);

        if (!updated)
            return NotFound(new { message = "Ticket not found." });

        return NoContent();
    }


    [HttpPost]
    [Authorize(Roles = "Customer,SupportAgent,Admin")]
    public async Task<IActionResult> Create(
     [FromBody] CreateTicketRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = await _ticketService.CreateAsync(request, userId);

        return CreatedAtAction(
        nameof(GetById),
        new { id = result.Id },
        result);
    }



    [HttpGet("{id:int}")]
    [Authorize(Roles = "Customer,Admin,SupportAgent")]
    public async Task<IActionResult> GetById(int id)
    {

        var userIdClaim =
          User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            Console.WriteLine("Failed to parse user ID" + userId);
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(role))
        {
            Console.WriteLine("Failed to parse role" + role);
            return Unauthorized();
        }

        var ticket = await _ticketService.GetByIdAsync(id, userId, role);

        if (ticket == null)
        {
            return Unauthorized($"Not authorized to view ticket {id} for user {userId} as role {role} , request: {NotFound().ToString()}");
        }

        return Ok(ticket);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Customer,Admin,SupportAgent")]
    public async Task<IActionResult> Update(
     int id,
     UpdateTicketRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (!int.TryParse(userIdClaim, out var userId) ||
            string.IsNullOrWhiteSpace(role))
        {
            Console.WriteLine("Failed to parse user ID" + userId);
            Console.WriteLine("Failed to parse role" + role);
            return Unauthorized();
        }

        try
        {
            var updated = await _ticketService.UpdateAsync(
                id, request, userId, role);

            if (!updated)
                return NotFound();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,SupportAgent")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _ticketService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    // [HttpGet]
    // [Authorize(Roles = "Admin")]
    // public async Task<IActionResult> GetAll()
    // {
    //     var tickets = await _ticketService.GetAllAsync();

    //     return Ok(tickets);
    // }


    [HttpGet("my")]
    [Authorize(Roles = "Customer,SupportAgent,Admin")]
    public async Task<IActionResult> GetMyTickets()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (!int.TryParse(userIdClaim, out var userId) ||
            string.IsNullOrWhiteSpace(role))
        {
            return Unauthorized();
        }

        switch (role)
        {
            case "Admin":
                return Ok(await _ticketService.GetAllAsync());

            case "SupportAgent":
                return Ok(await _ticketService.GetResponsibleTicketsAsync(userId));

            case "Customer":
                return Ok(await _ticketService.GetCustomerTicketsAsync(userId));

            default:
                return Forbid();
        }
    }

    [HttpGet("{id:int}/activities")]
    [Authorize(Roles = "Admin,Customer,SupportAgent")]
    public async Task<IActionResult> GetTicketActivities(
    int id,
    CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (!int.TryParse(userIdClaim, out var userId) ||
            string.IsNullOrWhiteSpace(role))
        {
            return Unauthorized();
        }

        // Verify that this user is allowed to access this ticket.
        var ticket = await _ticketService.GetByIdAsync(
            id,
            userId,
            role);

        if (ticket == null)
        {
            // Do not disclose whether an inaccessible ticket exists.
            return NotFound();
        }

        var activities = await _ticketActivityService.GetActivitiesAsync(
            id,
            cancellationToken);

        return Ok(activities);
    }


}