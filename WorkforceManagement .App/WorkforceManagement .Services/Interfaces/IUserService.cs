using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Models.DTO.Request.User;
using WorkforceManagement.Models.DTO.Response.User;

namespace WorkforceManagement.Services.Contracts
{
    public interface IUserService
    {
        Task<UserResponse> CreateUserAsync(UserCreateRequest inputDto);

        Task<UserResponse> UpdateUserAsync(UserUpdateRequest inputDto);

        Task<UserResponse> UpdateUserDayOffsAsync(UserDayOffUpdateRequest inputDto);

        Task<bool> DeleteUserAsync(string userId);

        Task<List<UserResponse>> GetAllAsync();

        Task<User> GetCurrentUserAsync(ClaimsPrincipal user);

        Task<bool> AssignUserAsAdmin(string username);

        Task<User> GetUserById(string requesterUserId);

        Task<bool> UserIsAdmin(User user);

        Task<DayOffResponse> GetUserDaysOffAsync(string id);
    }
}