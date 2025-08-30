using System;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkforceManagement.Data.Database;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Models.DTO.Request.User;
using WorkforceManagement.Models.DTO.Response.User;
using WorkforceManagement.Services.Contracts;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WorkforceManagement.Common;
using WorkforceManagement.Data.Entities.Enums;

namespace WorkforceManagement.Services.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<User> _userManager;

        public UserService(ApplicationDbContext applicationDbContext, UserManager<User> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        public async Task<bool> AssignUserAsAdmin(string username)
        {
            User user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return false;
            }
            await _userManager.AddToRoleAsync(user, GlobalConstants.AdministratorRoleName);
            return true;
        }

        public async Task<UserResponse> CreateUserAsync(UserCreateRequest inputDto)
        {
            User user = new User
            {
                UserName = inputDto.Username,
                EmailConfirmed = true,
                Email = inputDto.Email,
            };

            if (inputDto.TeamId != 0)
            {
                Team team = await _applicationDbContext.Teams.FindAsync(inputDto.TeamId);
                if (team == null)
                {
                    return null;
                }
                user.Teams.Add(team);
            }

            await CalculateDaysOffForNewUser(user);

            await _userManager.CreateAsync(user, inputDto.Password);

            UserResponse result = new UserResponse
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Paid = user.PaidDaysOff,
                Unpaid = user.UnpaidDaysOff,
                SickLeave = user.SickDaysOff
            };

            return result;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            User user = await _applicationDbContext.Users.FindAsync(userId);

            if (user == null)
            {
                return false;
            }

            CancelPendingTimeOffRequests(user);

            string deletedEmail = user.Id + "@deleted.com";

            user.UserName = userId;
            user.NormalizedUserName = userId;
            user.Email = deletedEmail;
            user.NormalizedEmail = deletedEmail;
            user.PhoneNumber = userId;

            user.IsDeleted = true;
            user.DeletedOn = DateTime.UtcNow;

            await _applicationDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<UserResponse>> GetAllAsync()
        {
            List<UserResponse> result = await _applicationDbContext.Users
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    Username = u.UserName,
                    Email = u.Email,
                    Paid = u.PaidDaysOff,
                    Unpaid = u.UnpaidDaysOff,
                    SickLeave = u.SickDaysOff
                }).ToListAsync();

            return result;

        }

        public Task<User> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            return _userManager.GetUserAsync(user);
        }

        public async Task<User> GetUserById(string requesterUserId)
        {
            return await _userManager.FindByIdAsync(requesterUserId);
        }

        public async Task<DayOffResponse> GetUserDaysOffAsync(string id)
        {
            var user = await _applicationDbContext.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

            var result = new DayOffResponse
            {
                Paid = user.PaidDaysOff,
                Unpaid = user.UnpaidDaysOff,
                SickLeave = user.SickDaysOff
            };

            return result;
        }

        public async Task<UserResponse> UpdateUserAsync(UserUpdateRequest inputDto)
        {
            User user = await _applicationDbContext.Users.FindAsync(inputDto.UserId);
            if (user == null)
            {
                return null;
            }

            if (inputDto.TeamId != 0)
            {
                Team team = await _applicationDbContext.Teams.FindAsync(inputDto.TeamId);
                if (team == null)
                {
                    return null;
                }
                user.Teams.Add(team);
            }

            user.UserName = inputDto.Username;
            user.Email = inputDto.Email;

            await _userManager.UpdateAsync(user);

            UserResponse result = new UserResponse
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Paid = user.PaidDaysOff,
                Unpaid = user.UnpaidDaysOff,
                SickLeave = user.SickDaysOff
            };
            return result;
        }

        public async Task<UserResponse> UpdateUserDayOffsAsync(UserDayOffUpdateRequest inputDto)
        {
            User user = await GetUserById(inputDto.UserId);
            if (user == null)
            {
                return null;
            }

            user.PaidDaysOff += inputDto.Paid;
            user.UnpaidDaysOff += inputDto.Unpaid;
            user.SickDaysOff += inputDto.SickLeave;

            if (user.UnpaidDaysOff > 40 ||
                user.SickDaysOff > 90 ||
                user.PaidDaysOff < 0 ||
                user.UnpaidDaysOff < 0 ||
                user.SickDaysOff < 0)
            {
                return null;
            }

            await _userManager.UpdateAsync(user);

            var result = new UserResponse
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Paid = user.PaidDaysOff,
                Unpaid = user.UnpaidDaysOff,
                SickLeave = user.SickDaysOff
            };
            return result;
        }

        public async Task<bool> UserIsAdmin(User user)
        {
            return await _userManager.IsInRoleAsync(user, GlobalConstants.AdministratorRoleName);
        }

        private void CancelPendingTimeOffRequests(User user)
        {
            foreach (var timeOffRequest in user.TimeOffRequests)
            {
                if (timeOffRequest.Status == RequestStatus.Awaiting ||
                    timeOffRequest.Status == RequestStatus.Created)
                {
                    timeOffRequest.Status = RequestStatus.Canceled;
                }
            }
        }

        private Task CalculateDaysOffForNewUser(User user)
        {
            int currentMonth = DateTime.UtcNow.Month;
            double daysOffCoefficient = 12.0 / currentMonth;

            user.PaidDaysOff = GlobalConstants.PaidDaysOff + 1 - (int)(GlobalConstants.PaidDaysOff / daysOffCoefficient);
            user.UnpaidDaysOff = GlobalConstants.UnpaidDaysOff + 1 - (int)(GlobalConstants.UnpaidDaysOff / daysOffCoefficient);
            user.SickDaysOff = GlobalConstants.SickDaysOff;

            return Task.CompletedTask;
        }
    }
}