namespace SupportOps.Infrastructure.AI;

public class OllamaOptions
{
    public const string SectionName = "Ollama";

    public string BaseUrl { get; set; } = "http://localhost:11434/";

    public string Model { get; set; } = "qwen3:4b";
}