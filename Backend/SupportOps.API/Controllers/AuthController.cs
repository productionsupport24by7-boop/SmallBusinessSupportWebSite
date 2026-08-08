using Microsoft.AspNetCore.Mvc;
using SupportOps.Application.DTOs;
using SupportOps.Application.Interfaces;

namespace SupportOps.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    public async Task<IActionResult>(LoginRequest request)
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