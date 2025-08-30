using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WorkforceManagement.Models.DTO.Request.User;
using WorkforceManagement.Models.DTO.Response.User;
using WorkforceManagement.Services.Contracts;
using WorkforceManagement.UnitTests.Seeder;
using WorkforceManagement.WebApi.Controllers;
using Xunit;

namespace WorkforceManagement.UnitTests.ControllersTests
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task CreateUser_Valid_ReturnsCreаted()
        {
            //arrange
            var response = UserTemplates.GetUserResponse("test");
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<UserCreateRequest>())).ReturnsAsync(response);
            var sut = new UsersController(userServiceMock.Object);

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns("http://localhost");

            sut.Url = urlHelper.Object;

            // act
            var result = await sut.CreateUser(It.IsAny<UserCreateRequest>());

            // assert
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task CreateUser_Invalid_ReturnsBadRequest()
        {
            //arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<UserCreateRequest>())).ReturnsAsync((UserResponse)null);
            var sut = new UsersController(userServiceMock.Object);

            // act
            var result = await sut.CreateUser(It.IsAny<UserCreateRequest>());

            // assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task AssignUserAsAdmin_Valid_ReturnsOk()
        {
            //arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.AssignUserAsAdmin(It.IsAny<string>())).ReturnsAsync(true);
            var sut = new UsersController(userServiceMock.Object);

            // act
            var result = await sut.AssignUserAsAdmin(It.IsAny<string>());

            // assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AssignUserAsAdmin_Invalid_ReturnsBadRequest()
        {
            //arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.AssignUserAsAdmin(It.IsAny<string>())).ReturnsAsync(false);
            var sut = new UsersController(userServiceMock.Object);

            // act
            var result = await sut.AssignUserAsAdmin(It.IsAny<string>());

            // assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetAll_Valid_ReturnsListUserResponse()
        {
            //arrange
            var users = new List<UserResponse>
                {
                new UserResponse(),
                new UserResponse(),
                };
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(users);
            var sut = new UsersController(userServiceMock.Object);

            // act
            var result = await sut.GetAll();

            // assert
            Assert.NotNull(result);
            Assert.Equal(users.Count, result.Value.Count);
        }

        [Fact]
        public async Task GetMyDaysOff_Valid_ReturnsOk()
        {
            //arrange
            var user = UserTemplates.GetUser("test");
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.GetUserDaysOffAsync(It.IsAny<string>())).ReturnsAsync(new DayOffResponse());
            var sut = new UsersController(userServiceMock.Object);

            // act
            var result = await sut.GetMyDaysOff(It.IsAny<string>());

            // assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetMyDaysOff_Invalid_ReturnsBadRequest()
        {
            //arrange
            var user = UserTemplates.GetUser("test");
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.GetUserDaysOffAsync(It.IsAny<string>())).ReturnsAsync((DayOffResponse)null);
            var sut = new UsersController(userServiceMock.Object);

            // act
            var result = await sut.GetMyDaysOff(It.IsAny<string>());

            // assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateUser_Valid_ReturnsNoContent()
        {
            //arrange
            var response = UserTemplates.GetUserResponse("test");
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.UpdateUserAsync(It.IsAny<UserUpdateRequest>())).ReturnsAsync(response);
            var sut = new UsersController(userServiceMock.Object);

            // act
            var result = await sut.UpdateUser(It.IsAny<UserUpdateRequest>());

            // assert
            var res = result;
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateUser_Invalid_ReturnsBadRequest()
        {
            //arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.UpdateUserAsync(It.IsAny<UserUpdateRequest>())).ReturnsAsync((UserResponse)null);
            var sut = new UsersController(userServiceMock.Object);

            // act
            var result = await sut.UpdateUser(It.IsAny<UserUpdateRequest>());

            // assert
            var res = result;
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateUserDayOff_Valid_ReturnsNoContent()
        {
            //arrange
            var model = UserTemplates.GetUserResponse("test");
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.UpdateUserDayOffsAsync(It.IsAny<UserDayOffUpdateRequest>())).ReturnsAsync(model);
            var sut = new UsersController(userServiceMock.Object);

            // act
            var result = await sut.UpdateUserDayOff(It.IsAny<UserDayOffUpdateRequest>());

            // assert
            var res = result;
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateUserDayOff_Invalid_ReturnsBadRequest()
        {
            //arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.UpdateUserDayOffsAsync(It.IsAny<UserDayOffUpdateRequest>())).ReturnsAsync((UserResponse)null);
            var sut = new UsersController(userServiceMock.Object);

            // act
            var result = await sut.UpdateUserDayOff(It.IsAny<UserDayOffUpdateRequest>());

            // assert
            var res = result;
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteUser_Valid_ReturnsOk()
        {
            //arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.DeleteUserAsync(It.IsAny<string>())).ReturnsAsync(true);
            var sut = new UsersController(userServiceMock.Object);

            // act
            var result = await sut.DeleteUser(It.IsAny<string>());

            // assert
            var res = result;
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteUser_Invalid_ReturnsBadRequest()
        {
            //arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.DeleteUserAsync(It.IsAny<string>())).ReturnsAsync(false);
            var sut = new UsersController(userServiceMock.Object);

            // act
            var result = await sut.DeleteUser(It.IsAny<string>());

            // assert
            var res = result;
            Assert.IsType<BadRequestResult>(result);
        }
    }
}
