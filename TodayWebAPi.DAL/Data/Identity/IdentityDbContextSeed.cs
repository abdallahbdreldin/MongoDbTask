using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using System.Security.Claims;

namespace TodayWebAPi.DAL.Data.Identity
{
    public class IdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            var roles = new[] { "Admin", "customer" };
            foreach (var roleName in roles)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var role = new Role
                    {
                        Id = ObjectId.GenerateNewId().ToString(), 
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    };
                    await roleManager.CreateAsync(role);

                    var claim = new Claim("Permission", roleName == "Admin" ? "FullAccess" : "LimitedAccess");
                    await roleManager.AddClaimAsync(role, claim);
                }
            }

            
            var user1 = await userManager.FindByEmailAsync("abdullahbdreldin@gmail.com");
            if (user1 == null)
            {
                user1 = new User
                {
                    DisplayName = "Abdullah",
                    Email = "abdullahbdreldin@gmail.com",
                    UserName = "abdullahbdreldin"
                };

                var result = await userManager.CreateAsync(user1, "Password@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user1, "customer");
                }
            }
            else
            {
                var rolesForUser = await userManager.GetRolesAsync(user1);
                if (!rolesForUser.Contains("customer"))
                {
                    await userManager.AddToRoleAsync(user1, "customer");
                }
            }

            var user2 = await userManager.FindByEmailAsync("secondadmin@example.com");
            if (user2 == null)
            {
                user2 = new User
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    DisplayName = "Second Admin",
                    Email = "secondadmin@example.com",
                    UserName = "secondadmin"
                };

                var result = await userManager.CreateAsync(user2, "Password@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user2, "Admin");
                }
            }
            else
            {
                var rolesForUser = await userManager.GetRolesAsync(user2);
                if (!rolesForUser.Contains("Admin"))
                {
                    await userManager.AddToRoleAsync(user2, "Admin");
                }
            }
        }
    }
}
