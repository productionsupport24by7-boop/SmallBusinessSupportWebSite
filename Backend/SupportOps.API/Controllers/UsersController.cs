using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportOps.Application.DTOs.Users;
using SupportOps.Application.Interfaces;
using System.Security.Claims;

namespace SupportOps.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ISupportAgentService _supportAgentService;
    private readonly IUserProfileService _userProfileService;

    public UsersController(
        ISupportAgentService supportAgentService,
        IUserProfileService userProfileService)
    {
        _supportAgentService = supportAgentService;
        _userProfileService = userProfileService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var profile = await _userProfileService.GetProfileAsync(userId);

        if (profile == null)
            return NotFound(new { message = "User profile not found." });

        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile(
        [FromBody] UpdateUserProfileRequest request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var updated = await _userProfileService.UpdateProfileAsync(
            userId, request);

        if (!updated)
            return BadRequest(new
            {
                message = "Unable to update user profile."
            });

        return NoContent();
    }

    [HttpGet("support-agents")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetSupportAgents()
    {
        var agents =
            await _supportAgentService.GetAllSupportAgentsAsync();

        return Ok(agents);
    }

    private bool TryGetUserId(out int userId)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(userIdClaim, out userId);
    }
}