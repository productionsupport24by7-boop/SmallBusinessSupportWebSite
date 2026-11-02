using Microsoft.AspNetCore.Mvc;
using SupportOps.Application.Security;
using SupportOps.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SupportOps.Infrastructure.Security;
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

        var token = GenerateJwtTokenForUser(user);

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
            FirstName = user?.FirstName ?? string.Empty,
            LastName = user?.LastName ?? string.Empty,
            Email = user?.Email ?? string.Empty,
            Role = "User"
        };

        var token = GenerateJwtTokenForUser(user);

        return Ok(new
        {
            Token = token
        });
    }

    private string GenerateJwtTokenForUser(User user)
    {
        var methodNames = new[]
        {
            "GenerateToken",
            "GenerateJwtToken",
            "CreateToken",
            "CreateJwtToken"
        };

        foreach (var methodName in methodNames)
        {
            var method = _jwtTokenGenerator.GetType().GetMethod(methodName, new[] { typeof(User) })
                ?? _jwtTokenGenerator.GetType().GetMethod(methodName);

            if (method == null)
            {
                continue;
            }

            var arguments = method.GetParameters().Length == 0
                ? Array.Empty<object>()
                : new object[] { user };

            var result = method.Invoke(_jwtTokenGenerator, arguments);
            return result?.ToString() ?? throw new InvalidOperationException($"JWT token generation method '{methodName}' returned null.");
        }

        throw new InvalidOperationException("No JWT token generation method was found on JwtTokenGenerator.");
    }
}