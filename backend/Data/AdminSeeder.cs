using backend.Constants;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public static class AdminSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var adminEmail = configuration["SeedAdmin:Email"];
        var adminPassword = configuration["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var normalizedEmail = adminEmail.Trim().ToLowerInvariant();

        var adminAlreadyExists = await context.Users
            .AnyAsync(user => user.Email == normalizedEmail);

        if (adminAlreadyExists)
        {
            return;
        }

        var admin = new User
        {
            Name = "Admin",
            Email = normalizedEmail,
            Role = Roles.Admin
        };

        var passwordHasher = new PasswordHasher<User>();
        admin.PasswordHash = passwordHasher.HashPassword(admin, adminPassword);

        context.Users.Add(admin);
        await context.SaveChangesAsync();
    }
}