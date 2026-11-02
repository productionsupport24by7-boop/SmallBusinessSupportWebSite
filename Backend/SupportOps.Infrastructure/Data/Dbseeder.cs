
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SupportOps.Infrastructure.Identity;

namespace SupportOps.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        SupportOpsDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        await context.Database.MigrateAsync();

        string[] roles =
        {
            "Admin",
            "Customer",
            "SupportAgent"
        };

        // Create roles if they do not exist.
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new IdentityRole<int>(role));
            }
        }

        // Create default users.
        await CreateUserAsync(
            userManager,
            "Ishwor",
            "Admin",
            "admin@supportops.com",
            "Admin@123",
            "Admin");

        await CreateUserAsync(
            userManager,
            "John",
            "Doe",
            "john.doe@supportops.com",
            "Customer@123",
            "Customer");

        await CreateUserAsync(
            userManager,
            "Jane",
            "Smith",
            "jane.smith@supportops.com",
            "SupportAgent@123",
            "SupportAgent");
    }

    private static async Task CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string firstName,
        string lastName,
        string email,
        string password,
        string role)
    {
        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser != null)
            return;

        var user = new ApplicationUser
        {
            FirstName = firstName,
            LastName = lastName,
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            IsActive = true,
            CreatedOn = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Could not create user {email}: {errors}");
        }

        var roleResult = await userManager.AddToRoleAsync(user, role);

        if (!roleResult.Succeeded)
        {
            var errors = string.Join(
                "; ",
                roleResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Could not assign role {role} to {email}: {errors}");
        }
    }
}



// using BCrypt.Net;
// using Microsoft.EntityFrameworkCore;
// using SupportOps.Domain.Entities;

// namespace SupportOps.Infrastructure.Data;

// public static class DbSeeder
// {
//     public static async Task SeedAsync(SupportOpsDbContext context)
//     {
//         await context.Database.MigrateAsync();

//         // if (await context.Users.AnyAsync())
//         //     return;

//         if (!await context.Users.AnyAsync(x => x.Role == "Admin"))
//         {
//             var admin = new User
//             {
//                 FirstName = "Ishwor",
//                 LastName = "Admin",
//                 Email = "admin@supportops.com",
//                 PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
//                 Role = "Admin",
//                 IsActive = true
//             };

//             context.Users.Add(admin);
//         }

//         if (!await context.Users.AnyAsync(x => x.Role == "Customer"))
//         {
//             var customer = new User
//             {
//                 FirstName = "John",
//                 LastName = "Doe",
//                 Email = "john.doe@supportops.com",
//                 PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer@123"),
//                 Role = "Customer",
//                 IsActive = true
//             };
//             context.Users.Add(customer);
//         }

//         if (!await context.Users.AnyAsync(x => x.Role == "SupportAgent"))
//         {
//             var supportAgent = new User
//             {
//                 FirstName = "Jane",
//                 LastName = "Smith",
//                 Email = "jane.smith@supportops.com",
//                 PasswordHash = BCrypt.Net.BCrypt.HashPassword("SupportAgent@123"),
//                 Role = "SupportAgent",
//                 IsActive = true
//             };
//             context.Users.Add(supportAgent);

//         }

//         await context.SaveChangesAsync();



//     }
// }