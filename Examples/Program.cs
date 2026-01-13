using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SSRBusiness.BusinessClasses;
using SSRBusiness.Data;
using SSRBusiness.Entities;

namespace SSRBusiness.Examples;

/// <summary>
/// Example usage of the migrated SSRBusiness library.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("SSRBusiness .NET 8 - Example Usage");
        Console.WriteLine("===================================\n");

        // Load configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        // Set up DbContext
        var connectionString = configuration.GetConnectionString("SanSabaConnection");
        var optionsBuilder = new DbContextOptionsBuilder<SsrDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        await using var context = new SsrDbContext(optionsBuilder.Options);
        var userRepo = new UserRepository(context);

        // Example 1: List users
        Console.WriteLine("Example 1: List All Users");
        var users = await userRepo.GetUserListAsync();
        foreach (var user in users)
        {
            Console.WriteLine($"- {user.FirstName} {user.LastName} ({user.Email})");
        }

        // Example 2: Authenticate
        Console.WriteLine("\nExample 2: Authentication");
        Console.Write("Email: ");
        var email = Console.ReadLine() ?? "";
        Console.Write("Password: ");
        var password = Console.ReadLine() ?? "";

        var authUser = await userRepo.AuthenticateAndLoadAsync(email, password);
        Console.WriteLine(authUser != null ? "✓ Success!" : "✗ Failed");

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }
}
