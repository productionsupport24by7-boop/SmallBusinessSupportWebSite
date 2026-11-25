namespace SupportOps.Application.DTOs.AI;

public class AIAgentResponse
{
    public string Answer { get; set; } = string.Empty;

    public List<string> ToolsExecuted { get; set; } = [];
}