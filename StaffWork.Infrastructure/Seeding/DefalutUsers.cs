using Microsoft.AspNetCore.Identity;
using StaffWork.Core.Models;
namespace StaffWork.Seeding.Consts
{
    public static class DefaultUsers
    {
        public static async Task SeedUsers(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            User superAdmin = new()
            {
                UserName = "SuperAdmin",
                FullName = "SuperAdmin",
                ImageUrl = "/assets/images/avatar.png",
                IsActive = true,
                Password = "P@ssword123"
            };

            var user = await userManager.FindByNameAsync(superAdmin.UserName);

            if (user is null)
            {
                await userManager.CreateAsync(superAdmin, "P@ssword123");
                var roles = roleManager.Roles.Select(r => r.Name).ToList();
                await userManager.AddToRolesAsync(superAdmin, roles);
            }
        }
    }
}
