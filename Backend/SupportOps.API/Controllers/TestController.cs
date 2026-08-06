using Microsoft.AspNetCore.Mvc;
using SupportOps.Application.Security;
using SupportOps.Domain.Entities;

namespace SupportOps.API.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public TestController(JwtTokenGenerator jwtTokenGenerator)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    [HttpGet("token")]
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
}