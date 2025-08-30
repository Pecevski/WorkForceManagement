using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using WorkforceManagement.Common;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Data.Database;
using System.Linq;

namespace WorkforceManagement.Data.Seeding
{
    public class UsersSeeder : ISeeder
    {
        public async Task SeedData(ApplicationDbContext applicationDbContext, IServiceProvider serviceProvider)
        {
            if (!applicationDbContext.Users.Any())
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
                await CreateAdmin(userManager);
            }
        }

        private async Task CreateAdmin(UserManager<User> userManager)
        {
            User user = new User
            {
                UserName = GlobalConstants.AdminUsername,
                EmailConfirmed = true,
                Email = GlobalConstants.AdminEmail
            };

            await userManager.CreateAsync(user, GlobalConstants.AdminPassword);
            await userManager.AddToRoleAsync(user, GlobalConstants.AdministratorRoleName);
        }
    }
}
