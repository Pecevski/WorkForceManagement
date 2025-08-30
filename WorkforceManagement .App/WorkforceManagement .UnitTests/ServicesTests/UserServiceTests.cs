using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkforceManagement.Data.Database;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Data.Entities.Enums;
using WorkforceManagement.Models.DTO.Request.User;
using WorkforceManagement.Services.Services;
using WorkforceManagement.UnitTests.Seeder;
using Xunit;

namespace WorkforceManagement.UnitTests.ServicesTests
{
    public class UserServiceTests : IDisposable
    {
        public DbContextOptions<ApplicationDbContext> _options;
        ApplicationDbContext _dbContext;

        public UserServiceTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb").Options;

            _dbContext = new ApplicationDbContext(_options);
        }

        [Fact]
        public async Task CreateUser_Valid_ReturnsUserResponse()
        {
            // arrange
            var model = UserTemplates.GetUserCreateRequest();

            var manager = MockUserManager(new List<User>());

            var sut = new UserService(_dbContext, manager.Object);

            // act
            var result = await sut.CreateUserAsync(model);

            // assert
            Assert.NotNull(result);
            Assert.Equal(result.Username, model.Username);
        }

        [Fact]
        public async Task CreateUser_ValidTeamId_ReturnsUserResponse()
        {
            // arrange
            var team = new Team
            {
                Title = "Test",
                Description = "Test",
                TeamLeader = new User()
            };
            await _dbContext.Teams.AddAsync(team);
            await _dbContext.SaveChangesAsync();

            var model = UserTemplates.GetUserCreateRequest();

            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);

            // act
            var result = await sut.CreateUserAsync(model);

            // assert
            Assert.NotNull(result);
            Assert.Equal(result.Username, model.Username);
        }

        [Fact]
        public async Task CreateUser_InValidTeamId_ReturnsNull()
        {
            // arrange
            var model = new UserCreateRequest
            {
                Username = "Test",
                Password = "Test",
                Email = "test@test.com",
                TeamId = 1
            };

            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);

            // act
            var result = await sut.CreateUserAsync(model);

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AssignUserAsAdmin_Valid_ReturnsTrue()
        {
            // arrange
            var user = UserTemplates.GetUser("test");

            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);

            manager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);

            // act
            var result = await sut.AssignUserAsAdmin(user.UserName);

            // assert
            Assert.True(result);
        }

        [Fact]
        public async Task AssignUserAsAdmin_InvalidUsername_ReturnsFalse()
        {
            // arrange
            var user = UserTemplates.GetUser("test");

            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);

            manager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            // act
            var result = await sut.AssignUserAsAdmin(user.UserName);

            // assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteUserAsync_Valid_ReturnsTrue()
        {
            // arrange
            var user = new User
            {
                Id = "test"
            };

            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);

            _dbContext.Users.Add(user);
            // act
            var result = await sut.DeleteUserAsync(user.Id);

            // assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUserAsync_InvalidId_ReturnsFalse()
        {
            // arrange
            var user = new User
            {
                Id = "test"
            };

            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            // act
            var result = await sut.DeleteUserAsync("invlaid");

            // assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetAllAsync_Valid_ReturnsListUserResponse()
        {
            // arrange
            var user = new User
            {
                Id = "test"
            };

            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            // act
            var result = await sut.GetAllAsync();

            // assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetCurrentUserAsync_Valid_ReturnsUser()
        {
            // arrange
            var user = UserTemplates.GetUser("test");
            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);

            manager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            // act
            var result = await sut.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>());

            // assert
            Assert.Equal(user, result);
        }

        [Fact]
        public async Task GetUserById_Valid_ReturnsUser()
        {
            // arrange
            var user = UserTemplates.GetUser("test");
            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);

            manager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            // act
            var result = await sut.GetUserById(It.IsAny<string>());

            // assert
            Assert.Equal(user, result);
        }

        [Fact]
        public async Task GetUserById_Invalid_ReturnsNull()
        {
            // arrange
            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);

            manager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            // act
            var result = await sut.GetUserById(It.IsAny<string>());

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserDaysOffAsync_Valid_ReturnsUserResponse()
        {
            // arrange
            var user = UserTemplates.GetUser("test");
            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            // act
            var result = await sut.GetUserDaysOffAsync(user.Id);

            // assert
            Assert.Equal(user.PaidDaysOff, result.Paid);
        }

        [Fact]
        public async Task GetUserDaysOffAsync_Invalid_ReturnsNull()
        {
            // arrange
            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);

            // act
            var result = await sut.GetUserDaysOffAsync("invalid");

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUserAsync_Valid_ReturnsUserResponse()
        {
            // arrange
            var user = UserTemplates.GetUser("test");
            var userUpdateRequest = UserTemplates.GetUserUpdateRequest("test");
            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            // act
            var result = await sut.UpdateUserAsync(userUpdateRequest);

            // assert
            Assert.Equal(userUpdateRequest.Username, result.Username);
        }

        [Fact]
        public async Task UpdateUserAsync_ValidUTeamId_ReturnsUserRespons()
        {
            // arrange
            var user = UserTemplates.GetUser("test");
            var userUpdateRequest = UserTemplates.GetUserUpdateRequest("test", 1);
            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);
            _dbContext.Teams.Add(new Team { Id = 1 });
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            // act
            var result = await sut.UpdateUserAsync(userUpdateRequest);

            // assert
            Assert.Equal(userUpdateRequest.Username, result.Username);
        }

        [Fact]
        public async Task UpdateUserAsync_InvalidUserId_ReturnsNull()
        {
            // arrange
            var userUpdateRequest = UserTemplates.GetUserUpdateRequest("test");
            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);

            // act
            var result = await sut.UpdateUserAsync(userUpdateRequest);

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUserAsync_InvalidUTeamId_ReturnsNull()
        {
            // arrange
            var user = UserTemplates.GetUser("test");
            var userUpdateRequest = UserTemplates.GetUserUpdateRequest("test", 2);
            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            // act
            var result = await sut.UpdateUserAsync(userUpdateRequest);

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUserDayOffsAsync_Valid_ReturnsUserResponse()
        {
            // arrange
            var user = UserTemplates.GetUser("test");
            var userUpdateRequest = UserTemplates.GetUserDayOffUpdateRequest("test", 2);
            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);
            manager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            // act
            var result = await sut.UpdateUserDayOffsAsync(userUpdateRequest);

            // assert
            Assert.Equal(user.UserName, result.Username);
            Assert.Equal(userUpdateRequest.Paid, result.Paid);
        }

        [Fact]
        public async Task UpdateUserDayOffsAsync_InvalidUserId_ReturnsNull()
        {
            // arrange
            var userUpdateRequest = UserTemplates.GetUserDayOffUpdateRequest("test");
            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);

            // act
            var result = await sut.UpdateUserDayOffsAsync(userUpdateRequest);

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUserDayOffsAsync_InvalidDays_ReturnsNull()
        {
            // arrange
            var user = UserTemplates.GetUser("test");
            var userUpdateRequest = UserTemplates.GetUserDayOffUpdateRequest("test", 500);
            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);
            manager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            // act
            var result = await sut.UpdateUserDayOffsAsync(userUpdateRequest);

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UserIsAdmin_Valid_ReturnsTrue()
        {
            // arrange
            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);
            manager.Setup(x => x.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);

            // act
            var result = await sut.UserIsAdmin(It.IsAny<User>());

            // assert
            Assert.True(result);
        }

        [Fact]
        public async Task UserIsAdmin_Invalid_ReturnsFalse()
        {
            // arrange
            var manager = MockUserManager(new List<User>());
            var sut = new UserService(_dbContext, manager.Object);
            manager.Setup(x => x.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

            // act
            var result = await sut.UserIsAdmin(It.IsAny<User>());

            // assert
            Assert.False(result);
        }

        public static Mock<UserManager<User>> MockUserManager(List<User> users)
        {
            var store = new Mock<IUserStore<User>>();
            var manager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            return manager;
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
        }
    }
}
