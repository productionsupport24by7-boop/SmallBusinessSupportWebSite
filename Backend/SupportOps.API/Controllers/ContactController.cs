using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportOps.Application.DTOs;
using SupportOps.Domain.Entities;
using SupportOps.Infrastructure.Persistence;
using SupportOps.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;

namespace SupportOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContactController : ControllerBase
{
    private readonly SupportOpsDbContext _context;

    public ContactController(SupportOpsDbContext context)
    {
        _context = context;
    }
    // GET: api/contact


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var contacts = await _context.ContactRequests
                .OrderByDescending(x => x.CreatedOn)
                .ToListAsync();

            return Ok(contacts);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new
                {
                    Message = "An error occurred while retrieving contact requests.",
                    Error = ex.Message
                });
        }
    }


    [HttpPost]
    public async Task<IActionResult> Create(ContactRequestDto request)
    {
        var entity = new ContactRequest
        {
            FullName = request.FullName,
            Company = request.Company,
            Email = request.Email,
            Phone = request.Phone,
            Service = request.Service,
            Message = request.Message
        };

        _context.ContactRequests.Add(entity);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Contact request submitted successfully."
        });
    }




}