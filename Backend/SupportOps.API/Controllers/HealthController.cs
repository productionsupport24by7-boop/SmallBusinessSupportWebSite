using Microsoft.AspNetCore.Mvc;

namespace SupportOps.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
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