using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SupportOps.Domain.Entities;

namespace SupportOps.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(SupportOpsDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Users.AnyAsync())
            return;

        var admin = new User
        {
            FirstName = "Ishwor",
            LastName = "Admin",
            Email = "admin@supportops.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = "Admin",
            IsActive = true
        };

        context.Users.Add(admin);

        await context.SaveChangesAsync();
    }
}