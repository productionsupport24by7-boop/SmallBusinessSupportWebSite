public static class CorsConfiguration
{
    public static IServiceCollection AddCorsConfiguration(
        this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("Angular", policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return services;
    }
}