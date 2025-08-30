using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Services.Interfaces;

namespace WorkforceManagement.Services.Services
{
    public class AuthenticationService : UserManager<User>, IAuthenticationService
    {
        public AuthenticationService(
            IUserStore<User> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<User>> logger) : base(
                store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {

        }

        public async Task<List<User>> GetAllAsync()
        {
            return await Users.ToListAsync();
        }

        public async Task<List<string>> GetUserRolesAsync(User user)
        {
            return (await GetRolesAsync(user)).ToList();
        }

        public async Task CreateUserAsync(User user, string password)
        {
            await CreateAsync(user, password);
        }

        public async Task<bool> IsUserInRole(string userId, string roleName)
        {
            User user = await FindByIdAsync(userId);
            return await IsInRoleAsync(user, roleName);
        }

        public async Task<bool> ValidateUserCredentials(User user, string password)
        {
            bool result = await CheckPasswordAsync(user, password);
            return result;

        }
    }
}