
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SupportOps.Application.DTOs.AI;
using SupportOps.Application.Interfaces;
using SupportOps.Infrastructure.AI;


namespace SupportOps.Infrastructure.Services;

public class SupportOpsAIAgentService : IAIAgentOrchestrator
{
    private readonly HttpClient _httpClient;
    private readonly OllamaOptions _options;
    private readonly ITicketService _ticketService;

    private const int MaxIterations = 3;

    public SupportOpsAIAgentService(
        HttpClient httpClient,
        IOptions<OllamaOptions> options,
        ITicketService ticketService)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _ticketService = ticketService;

        _httpClient.BaseAddress =
            new Uri(_options.BaseUrl.TrimEnd('/') + "/");

        _httpClient.Timeout = TimeSpan.FromMinutes(3);
    }

    public async Task<AIAgentResponse> ExecuteAsync(
        string message,
        int userId,
        string role,
        CancellationToken cancellationToken = default)
    {
        var messages = new List<OllamaMessage>
        {
            new()
            {
                Role = "system",
                Content = """
                    You are SupportOps AI, an IT production support agent.

                    You can investigate support tickets using the
                    get_ticket_details tool.

                    Instructions:
                    1. If the user asks about a specific ticket,
                       retrieve its details using the tool.
                    2. Never invent ticket information.
                    3. Analyze only the information returned by the tool.
                    4. Explain possible root causes and troubleshooting steps.
                    5. Clearly distinguish facts from assumptions.
                    6. Never claim that you changed a ticket.
                    7. If the requested ticket cannot be accessed,
                       explain that clearly.
                    """
            },
            new()
            {
                Role = "user",
                Content = message
            }
        };

        var toolsExecuted = new List<string>();

        for (var iteration = 0;
             iteration < MaxIterations;
             iteration++)
        {
            var request = new OllamaChatRequest
            {
                Model = _options.Model,
                Stream = false,
                Messages = messages,
                Tools = GetAvailableTools()
            };

            using var response = await _httpClient.PostAsJsonAsync(
                "api/chat",
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync
                <OllamaChatResponse>(
                    cancellationToken: cancellationToken);

            var assistantMessage = result?.Message;

            if (assistantMessage == null)
            {
                throw new InvalidOperationException(
                    "Ollama returned an empty response.");
            }

            // No tool calls: the agent has finished.
            if (assistantMessage.ToolCalls == null ||
                assistantMessage.ToolCalls.Count == 0)
            {
                return new AIAgentResponse
                {
                    Answer = assistantMessage.Content ?? string.Empty,
                    ToolsExecuted = toolsExecuted
                };
            }

            // Preserve the assistant tool-call message in conversation.
            messages.Add(assistantMessage);

            foreach (var toolCall in assistantMessage.ToolCalls)
            {
                var toolName = toolCall.Function.Name;

                string toolResult;

                switch (toolName)
                {
                    case "get_ticket_details":

                        toolResult = await GetTicketDetailsAsync(
                            toolCall.Function.Arguments,
                            userId,
                            role,
                            cancellationToken);

                        toolsExecuted.Add(toolName);
                        break;

                    default:
                        toolResult = JsonSerializer.Serialize(new
                        {
                            error = "Unknown tool requested."
                        });
                        break;
                }

                // Send the executed tool result back to the model.
                messages.Add(new OllamaMessage
                {
                    Role = "tool",
                    ToolName = toolName,
                    Content = toolResult
                });
            }
        }

        return new AIAgentResponse
        {
            Answer = "The agent reached its maximum tool execution limit. Please try a more specific question.",
            ToolsExecuted = toolsExecuted
        };
    }

    private async Task<string> GetTicketDetailsAsync(
        JsonElement arguments,
        int userId,
        string role,
        CancellationToken cancellationToken)
    {
        if (!arguments.TryGetProperty(
                "ticketId",
                out var ticketIdElement) ||
            !ticketIdElement.TryGetInt32(out var ticketId) ||
            ticketId <= 0)
        {
            return JsonSerializer.Serialize(new
            {
                error = "A valid ticket ID is required."
            });
        }

        // Always use the authenticated user's ID and role.
        // Never accept these values from the AI model.
        var ticket = await _ticketService.GetByIdAsync(
            ticketId,
            userId,
            role);

        if (ticket == null)
        {
            return JsonSerializer.Serialize(new
            {
                error = "Ticket not found or you do not have permission to access it."
            });
        }

        return JsonSerializer.Serialize(ticket);
    }

    private static List<OllamaTool> GetAvailableTools()
    {
        return
        [
            new OllamaTool
            {
                Type = "function",
                Function = new OllamaToolFunction
                {
                    Name = "get_ticket_details",
                    Description = "Retrieve the details of a support ticket using its ticket ID.",
                    Parameters = new
                    {
                        type = "object",
                        properties = new
                        {
                            ticketId = new
                            {
                                type = "integer",
                                description = "The ID of the support ticket."
                            }
                        },
                        required = new[] { "ticketId" }
                    }
                }
            }
        ];
    }

    private sealed class OllamaChatRequest
    {
        public string Model { get; set; } = string.Empty;
        public bool Stream { get; set; }

        public List<OllamaMessage> Messages { get; set; } = [];

        public List<OllamaTool> Tools { get; set; } = [];
    }

    private sealed class OllamaMessage
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("tool_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ToolName { get; set; }

        [JsonPropertyName("tool_calls")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<OllamaToolCall>? ToolCalls { get; set; }
    }

    private sealed class OllamaTool
    {
        public string Type { get; set; } = "function";
        public OllamaToolFunction Function { get; set; } = new();
    }

    private sealed class OllamaToolFunction
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public object Parameters { get; set; } = new();
    }

    private sealed class OllamaToolCall
    {
        public OllamaToolCallFunction Function { get; set; } = new();
    }

    private sealed class OllamaToolCallFunction
    {
        public string Name { get; set; } = string.Empty;
        public JsonElement Arguments { get; set; }
    }

    private sealed class OllamaChatResponse
    {
        public OllamaMessage? Message { get; set; }
    }
}
