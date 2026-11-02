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

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpPost]
    [Authorize(Roles = "Customer,SupportAgent,Admin")]
    public async Task<IActionResult> Create(
    CreateTicketRequest request)
    {
        var userIdClaim =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        var ticket = new CreateTicketRequest
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            Category = request.Category,

        };

        var result =
            await _ticketService.CreateAsync(ticket, int.Parse(userIdClaim));

        return Ok(result);
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
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role =
   User.FindFirstValue(ClaimTypes.Role);

        Console.WriteLine($"USER ID CLAIM: {userIdClaim}");
        Console.WriteLine($"ROLE CLAIM: {role}");



        if (!int.TryParse(userIdClaim, out var userId))
        {
            Console.WriteLine("Failed to parse user ID" + userId);
            return Unauthorized();
        }


        if (role == "Admin")
        {
            // Admins and Support Agents can view all tickets
            var alltickets = await _ticketService.GetAllAsync();
            return Ok(alltickets);
        }
        else if (role == "SupportAgent" || role == "Customer")
        {
            //Assign ticket only visible to Agent
            var responsibleTicket = await _ticketService.GetResponsibleTicketsAsync(userId);
            return Ok(responsibleTicket);
        }

        else
        {
            return Unauthorized();
        }


    }
}