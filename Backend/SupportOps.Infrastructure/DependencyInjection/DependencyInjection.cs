using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportOps.Application.Interfaces;
using SupportOps.Infrastructure.AI;
using SupportOps.Infrastructure.Data;
using SupportOps.Infrastructure.Services;


namespace SupportOps.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<SupportOpsDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        // Ollama AI configuration
        services.Configure<OllamaOptions>(
            configuration.GetSection(OllamaOptions.SectionName));

        // Ollama AI service
        services.AddHttpClient<IAIService, OllamaAIService>();

        services.AddScoped<ITicketActivityService, TicketActivityService>();


        return services;
    }


}