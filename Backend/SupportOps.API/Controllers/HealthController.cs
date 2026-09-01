using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SupportOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HealthController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "Healthy",
            Application = "SupportOps",
            Version = "1.0.0",
            ServerTime = DateTime.UtcNow
        });
    }
}