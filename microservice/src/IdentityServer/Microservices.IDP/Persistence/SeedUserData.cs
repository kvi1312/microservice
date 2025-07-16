using System.Security.Claims;
using Duende.IdentityModel;
using IdentityServer.Common;
using IdentityServer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Persistence;

public class SeedUserData
{
    public static async Task EnsureSeedData(string connectionString)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<IdentityContext>(option => { option.UseSqlServer(connectionString); });

        services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

        using (var serviceProvider = services.BuildServiceProvider())
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                await CreateUser(scope, "Alice", "Smith", "123 wall street", Guid.NewGuid().ToString(),
                    "a2601197", "Administrator", "alicesmith@gmail.com");
            }
        }
    }

    private static async Task CreateUser(IServiceScope scope, string firstName, string lastName, string address, string id,
        string password, string role, string email)
    {
        Console.WriteLine("Seeding User");
        var userManagement = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = await userManagement.FindByNameAsync(email);
        if (user == null)
        {
            user = new User
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Address = address,
                EmailConfirmed = true,
                Id = id,
            };
            var result = await userManagement.CreateAsync(user, password);
            CheckResult(result);

            var addToRoleResult = await userManagement.AddToRoleAsync(user, role);
            CheckResult(addToRoleResult);

            result = userManagement.AddClaimsAsync(user, new Claim[]
            {
                new(SystemConstants.Claims.UserName, user.UserName),
                new(SystemConstants.Claims.FirstName, user.FirstName),
                new(SystemConstants.Claims.LastName, user.LastName),
                new(SystemConstants.Claims.Roles, role),
                new(JwtClaimTypes.Address, user.Address),
                new(JwtClaimTypes.Email, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id),
            }).Result;
            CheckResult(result);
        }
    }

    private static void CheckResult(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }
    }
}