using FlashShop.Identity.Api.Entities;
using FlashShop.Shared.Constants;
using Microsoft.AspNetCore.Identity;

namespace FlashShop.Identity.Api.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var dbContext = serviceProvider.GetRequiredService<IdentityDbContext>();

        string[] roles = [Roles.Admin, Roles.Customer];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
        }

        const string adminEmail = "admin@flashshop.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                FullName = "System Administrator",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Roles.Admin);
                dbContext.Wallets.Add(new Wallet
                {
                    Id = Guid.NewGuid(),
                    UserId = admin.Id,
                    Balance = 0,
                    Currency = "VND",
                    UpdatedAt = DateTime.UtcNow
                });
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
