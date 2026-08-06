namespace SupportOps.API.Configuration;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    public static WebApplication UseSwaggerConfiguration(this WebApplication app)
    {
        // 2. Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) // Good practice to limit it to development
        {
            app.UseSwagger();   // Serves the generated OpenAPI spec as a JSON endpoint
            app.UseSwaggerUI(); // Serves the web UI using that JSON endpoint
        }

        return app;
    }
}