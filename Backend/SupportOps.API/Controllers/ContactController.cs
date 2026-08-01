using Microsoft.AspNetCore.Mvc;
using SupportOps.Application.DTOs;
using SupportOps.Domain.Entities;
using SupportOps.Infrastructure.Persistence;
using SupportOps.Infrastructure.Data;

namespace SupportOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly SupportOpsDbContext _context;

    public ContactController(SupportOpsDbContext context)
    {
        _context = context;
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