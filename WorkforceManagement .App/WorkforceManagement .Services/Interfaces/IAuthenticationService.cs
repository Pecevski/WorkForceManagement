using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkforceManagement.Data.Entities;

namespace WorkforceManagement.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<User> GetUserAsync(ClaimsPrincipal claimsPrincipal);

        Task<bool> IsUserInRole(string userId, string roleId);

        Task<User> FindByNameAsync(string name);

        Task<User> FindByEmailAsync(string email);

        Task<User> FindByIdAsync(string id);

        Task<List<User>> GetAllAsync();

        Task CreateUserAsync(User user, string password);

        Task<List<string>> GetUserRolesAsync(User user);

        Task<bool> ValidateUserCredentials(User user, string password);
    }
}