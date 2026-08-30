using Microsoft.AspNetCore.Mvc;
using SupportOps.Application.Security;
using SupportOps.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
namespace SupportOps.API.Controllers;


[ApiController]
[Route("api/test")]
// Require authorization by default for this controller. Token generation endpoints
// are explicitly marked [AllowAnonymous] below.
[Authorize]
public class TestController : ControllerBase
{
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public TestController(JwtTokenGenerator jwtTokenGenerator)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    [HttpGet("token")]
    [AllowAnonymous]
    public IActionResult GenerateToken()
    {
        var user = new User
        {
            Id = 1,
            FirstName = "Ishwor",
            LastName = "Dangol",
            Email = "admin@supportops.com",
            Role = "Admin"
        };

        var token = _jwtTokenGenerator.GenerateToken(user);

        return Ok(new
        {
            Token = token
        });
    }

    [HttpPost("token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GenerateToken(User user)
    {
        // var user = new User
        // {
        //     Id = 1,
        //     FirstName = "Ishwor",
        //     LastName = "Dangol",
        //     Email = "admin@supportops.com",
        //     Role = "Admin"
        // };

        // Do not trust role or id coming from client input. Enforce safe defaults.
        user = new User
        {
            Id = user?.Id ?? 0,
            FirstName = user?.FirstName,
            LastName = user?.LastName,
            Email = user?.Email,
            Role = "User"
        };

        var token = _jwtTokenGenerator.GenerateToken(user);

        return Ok(new
        {
            Token = token
        });
    }
}