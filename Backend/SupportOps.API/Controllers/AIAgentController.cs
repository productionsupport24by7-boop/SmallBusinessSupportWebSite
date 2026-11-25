using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportOps.Application.DTOs.AI;
using SupportOps.Application.Interfaces;

namespace SupportOps.API.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize(Roles = "Admin,SupportAgent")]
public class AIAgentController : ControllerBase
{
    private readonly IAIService _aiService;

    public AIAgentController(IAIService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat(
        [FromBody] AIChatRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _aiService.GenerateResponseAsync(
            request.Message,
            cancellationToken);

        return Ok(new AIChatResponse
        {
            Response = result
        });
    }
}