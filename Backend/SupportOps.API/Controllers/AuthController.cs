using Microsoft.AspNetCore.Mvc;
using SupportOps.Application.DTOs;
using SupportOps.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using SupportOps.API.Models;

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


    // Your AllowAnonymous attributes are important because the controller has:[Authorize(Roles = "Admin")] Keep them on both login and registration.
    // Your controller currently has class-level Admin authorization. The registration endpoint must explicitly allow anonymous access.
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authenticationService.RegisterAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Registration failed.",
                errors = result.Errors
            });
        }

        // Your controller is correct. I recommend returning HTTP 201 Created for successful registration instead of 200 OK.
        // return Ok(new
        // {
        //     message = "Registration successful. Your account has been created."
        // });
        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "Registration successful. Your account has been created."
        });
    }
}