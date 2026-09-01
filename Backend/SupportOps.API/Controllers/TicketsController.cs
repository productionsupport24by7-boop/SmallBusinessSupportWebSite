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
            User.FindFirstValue(ClaimTypes.NameIdentifier);


        Console.WriteLine(userIdClaim);

        if (!int.TryParse(userIdClaim, out var customerId))
        {
            return Unauthorized();
        }

        var result = await _ticketService.CreateAsync(
            request,
            customerId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,SupportAgent")]
    public async Task<IActionResult> GetAll()
    {
        var tickets = await _ticketService.GetAllAsync();

        return Ok(tickets);
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
        if (role != "Admin" && role != "SupportAgent" && role == "Customer")
        {

        }
        var ticket = await _ticketService.GetByIdAsync(id, userId, role);

        if (ticket == null)
        {
            return Unauthorized($"Not authorized to view ticket {id} for user {userId} as role {role} , request: {NotFound().ToString()}");
        }

        return Ok(ticket);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,SupportAgent")]
    public async Task<IActionResult> Update(
        int id,
        UpdateTicketRequest request)
    {
        var updated = await _ticketService.UpdateAsync(
            id,
            request);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
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

    [HttpGet("my")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMyTickets()
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        foreach (var claim in User.Claims)
        {
            Console.WriteLine(
                $"CLAIM: {claim.Type} = {claim.Value}");
        }

        if (!int.TryParse(userIdClaim, out var userId))
        {
            Console.WriteLine("Failed to parse user ID" + userId);
            return Unauthorized();
        }

        var tickets =
            await _ticketService.GetMyTicketsAsync(userId);

        return Ok(tickets);
    }
}