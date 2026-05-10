using Lab12.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Lab12.Tests.Integration;

public static class SeedTestData
{
    public const string RegularUserEmail = "user@test.com";
    public const string RegularUserPassword = "User@123";

    public static async Task Initialize(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        if (await userManager.FindByEmailAsync(RegularUserEmail) == null)
        {
            var user = new ApplicationUser
            {
                UserName = RegularUserEmail,
                Email = RegularUserEmail,
                FullName = "Regular Test User",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, RegularUserPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, "User");
        }
    }
}
