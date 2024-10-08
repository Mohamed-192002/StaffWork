using Microsoft.AspNetCore.Identity;
using StaffWork.Core.Consts;

namespace StaffWork.Infrastructure.Seeding
{
    public static class DefalutRoles
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(AppRoles.SuperAdmin));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Employee));
            }
        }
    }
}
