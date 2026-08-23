using Microsoft.AspNetCore.Mvc;
using SupportOps.Application.DTOs;
using SupportOps.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace SupportOps.API.Controllers;

[ApiController]
[Route("api/auth")]
[Authorize(Roles = "Admin")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var token = await _authenticationService.LoginAsync(
            request.Email,
            request.Password);

        if (token == null)
        {
            return Unauthorized(new
            {
                Message = "Invalid email or password."
            });
        }

        return Ok(new LoginResponse
        {
            Token = token,
            Expires = DateTime.UtcNow.AddMinutes(60)
        });
    }
}