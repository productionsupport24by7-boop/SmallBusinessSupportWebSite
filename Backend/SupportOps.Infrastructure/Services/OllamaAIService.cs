
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SupportOps.Application.Interfaces;
using SupportOps.Infrastructure.AI;

namespace SupportOps.Infrastructure.Services;

public class OllamaAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly OllamaOptions _options;

    public OllamaAIService(
        HttpClient httpClient,
        IOptions<OllamaOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        _httpClient.BaseAddress =
            new Uri(_options.BaseUrl.TrimEnd('/') + "/");

        _httpClient.Timeout = TimeSpan.FromMinutes(3);
    }

    public async Task<string> GenerateResponseAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(prompt))
            throw new ArgumentException("Prompt cannot be empty.");

        var request = new OllamaChatRequest
        {
            Model = _options.Model,
            Stream = false,
            Messages =
            [
                new OllamaMessage
                {
                    Role = "system",
                    Content = """
                    You are SupportOps AI, an IT production support assistant.

                    Your responsibilities:
                    - Analyze technical problems.
                    - Explain possible root causes.
                    - Suggest practical troubleshooting steps.
                    - Provide clear and concise answers.

                    Rules:
                    - Never invent logs or system information.
                    - Distinguish confirmed facts from assumptions.
                    - Ask for more information when necessary.
                    - Do not claim to have executed an action unless
                      a backend tool confirms that it succeeded.
                    """
                },
                new OllamaMessage
                {
                    Role = "user",
                    Content = prompt
                }
            ]
        };

        using var response = await _httpClient.PostAsJsonAsync(
            "api/chat",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(
                cancellationToken);

            throw new InvalidOperationException(
                $"Ollama request failed: {response.StatusCode}. {error}");
        }

        var result = await response.Content.ReadFromJsonAsync
            <OllamaChatResponse>(
                cancellationToken: cancellationToken);

        return result?.Message?.Content
            ?? "Ollama did not return a response.";
    }

    private sealed class OllamaChatRequest
    {
        public string Model { get; set; } = string.Empty;
        public bool Stream { get; set; }
        public List<OllamaMessage> Messages { get; set; } = [];
    }

    private sealed class OllamaMessage
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    private sealed class OllamaChatResponse
    {
        public OllamaResponseMessage? Message { get; set; }
    }

    private sealed class OllamaResponseMessage
    {
        public string? Content { get; set; }
    }
}
