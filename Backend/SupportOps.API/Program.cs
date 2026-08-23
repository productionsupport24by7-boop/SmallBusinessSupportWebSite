
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupportOps.API.Configuration;
using SupportOps.Infrastructure.DependencyInjection;
using SupportOps.Infrastructure.Data;
using SupportOps.Application.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using SupportOps.Application.Interfaces;
using SupportOps.Application.Services;
using SupportOps.Infrastructure.Repositories;
using SupportOps.API.Models;

//create a builder for the web application
var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(
    args.Length > 0 ? $"Command-line arguments: {string.Join(", ", args)}" : "No command-line arguments provided.");
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

//add services to builder like controllers,dbcontext swagger, dependency injection and CORS configuration
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddCorsConfiguration();


// Database
builder.Services.AddDbContext<SupportOpsDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();



// Console.WriteLine(
//     builder.Configuration.GetConnectionString("DefaultConnection"));


var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>()!;
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton<JwtTokenGenerator>();


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });





builder.Services.AddAuthorization();



var app = builder.Build();


//this will seed the database with initial data if it is empty
//add application user roles and a default admin user
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SupportOpsDbContext>();

    await DbSeeder.SeedAsync(db);
}


app.UseSwaggerConfiguration();


//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.UseCors("Angular");
//app.MapSwagger();
app.MapControllers();
await app.RunAsync();

namespace SupportOps.API.Models
{
    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}

