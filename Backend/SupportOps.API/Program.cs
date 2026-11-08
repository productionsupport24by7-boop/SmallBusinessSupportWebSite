
using Microsoft.EntityFrameworkCore;
using SupportOps.API.Configuration;
using SupportOps.Infrastructure.DependencyInjection;
using SupportOps.Infrastructure.Data;
using SupportOps.Application.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SupportOps.Application.Interfaces;
using SupportOps.Application.Services;
using SupportOps.Infrastructure.Repositories;
using SupportOps.API.Models;




using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SupportOps.Infrastructure.Identity;


using SupportOps.Infrastructure.Services;
using SupportOps.Infrastructure.Security;
using SupportOps.Infrastructure.Email;

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

//manage identity and authentication
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);
    })
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<SupportOpsDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();
// Authentication
//removing below user repository service registrations 
// builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();

builder.Services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();

builder.Services.AddScoped<IEmailOutboxRepository, EmailOutboxRepository>();

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

builder.Services.AddHostedService<EmailOutboxBackgroundService>();


var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>()!;


builder.Services.AddSingleton(jwtSettings);

// he JwtTokenGenerator can remain a singleton because UserManager<ApplicationUser> is scoped and cannot be injected into a singleton.
// Important correction: Change its registration to: This is necessary because the new token generator depends on the scoped UserManager<ApplicationUser>.

// builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();


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
                Encoding.UTF8.GetBytes(jwtSettings.Key)),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });



builder.Services.AddAuthorization();



var app = builder.Build();
app.UseSwaggerConfiguration();
//app.UseHttpsRedirection();
app.UseCors("Angular");
app.UseAuthentication();
app.UseAuthorization();
//app.MapSwagger();
app.MapControllers();

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

//this will seed the database with initial data if it is empty
//add application user roles and a default admin user
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SupportOpsDbContext>();

    //changes after adding asp.net identity and authentication
    //await DbSeeder.SeedAsync(db);
    var userManager = scope.ServiceProvider
    .GetRequiredService<UserManager<ApplicationUser>>();

    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole<int>>>();

    await DbSeeder.SeedAsync(db, userManager, roleManager);


}

await app.RunAsync();

namespace SupportOps.API.Models
{
    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}

