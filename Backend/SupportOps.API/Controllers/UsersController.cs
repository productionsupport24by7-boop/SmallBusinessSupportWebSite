
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportOps.Application.DTOs.Tickets;
using SupportOps.Application.Interfaces;
using SupportOps.Application.Services;
using System.Security.Claims;

namespace SupportOps.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ISupportAgentService _supportAgentService;

    public UsersController(
        ISupportAgentService supportAgentService)
    {
        _supportAgentService = supportAgentService;
    }

    [HttpGet("support-agents")]
   // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetSupportAgents()
    {
        var agents =
            await _supportAgentService.GetAllSupportAgentsAsync();

        return Ok(agents);
    }

    

}
