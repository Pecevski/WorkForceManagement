using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using WorkforceManagement.Common;
using WorkforceManagement.Data.Database;

namespace WorkforceManagement.Data.Seeding
{
    public class RolesSeeder : ISeeder
    {
        public async Task SeedData(ApplicationDbContext applicationDbContext, IServiceProvider serviceProvider)
        {
            if (!applicationDbContext.Roles.Any())
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await CreateRole(roleManager, GlobalConstants.AdministratorRoleName);
            }
        }

        private async Task CreateRole(RoleManager<IdentityRole> roleManager, string roleName)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}
