using WorkforceManagement.Common;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Models.DTO.Request.User;
using WorkforceManagement.Models.DTO.Response.User;

namespace WorkforceManagement.UnitTests.Seeder
{
    public class UserTemplates
    {
        public static User GetUser(string seed)
        {
            var user = new User
            {
                Id = $"user-{seed}",
                UserName = $"user-{seed}"
            };
            return user;
        }

        public static UserCreateRequest GetUserCreateRequest(string seed = "", int teamId = 0)
        {
            var user = new UserCreateRequest
            {
                Username = $"user-{seed}",
                Password = $"user-{seed}",
                Email = $"user-{seed}@{ GlobalConstants.Host}",
                TeamId = teamId
            };
            return user;
        }

        public static UserUpdateRequest GetUserUpdateRequest(string seed = "", int teamId = 0)
        {
            var user = new UserUpdateRequest
            {
                UserId = $"user-{seed}",
                Username = $"user-{seed}",
                Email = $"user-{seed}@{GlobalConstants.Host}",
                TeamId = teamId
            };
            return user;
        }

        public static UserDayOffUpdateRequest GetUserDayOffUpdateRequest(string seed = "", int days = 1)
        {
            var user = new UserDayOffUpdateRequest
            {
                UserId = $"user-{seed}",
                Paid = days,
                Unpaid = days,
                SickLeave = days
            };
            return user;
        }

        public static UserResponse GetUserResponse(string seed)
        {
            var user = new UserResponse
            {
                Id = $"user-{seed}",
                Username = $"user-{seed}",
                Email = $"user-{seed}",
                Paid = 1,
                Unpaid = 1,
                SickLeave = 1,
            };
            return user;
        }
    }
}
