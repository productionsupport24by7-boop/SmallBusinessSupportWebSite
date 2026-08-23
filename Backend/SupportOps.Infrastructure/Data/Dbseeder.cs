using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using SupportOps.Domain.Entities;

namespace SupportOps.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(SupportOpsDbContext context)
    {
        await context.Database.MigrateAsync();

        // if (await context.Users.AnyAsync())
        //     return;

        if (!await context.Users.AnyAsync(x => x.Role == "Admin"))
        {
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
        }

        if (!await context.Users.AnyAsync(x => x.Role == "Customer"))
        {
            var customer = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@supportops.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer@123"),
                Role = "Customer",
                IsActive = true
            };
            context.Users.Add(customer);
        }

        if (!await context.Users.AnyAsync(x => x.Role == "SupportAgent"))
        {
            var supportAgent = new User
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@supportops.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("SupportAgent@123"),
                Role = "SupportAgent",
                IsActive = true
            };
            context.Users.Add(supportAgent);

        }

        await context.SaveChangesAsync();



    }
}