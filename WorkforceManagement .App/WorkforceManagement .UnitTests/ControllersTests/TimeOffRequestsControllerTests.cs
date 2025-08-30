using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Models.DTO.Request;
using WorkforceManagement.Models.DTO.Response;
using WorkforceManagement.Services.Contracts;
using WorkforceManagement.WebApi.Controllers;
using Xunit;

namespace WorkforceManagement.UnitTests.ControllersTests
{
    public class TimeOffRequestsControllerTests
    {
        [Fact]
        public async Task CreateTimeOffRequest_Valid_ReturnsCreated()
        {
            //arrange
            var response = new TimeOffRequestResponseDTO();
            var userServiceMock = new Mock<IUserService>();
            var timeOffRequestServiceMock = new Mock<ITimeOffRequestService>();
            var sut = new TimeOffRequestsController(userServiceMock.Object, timeOffRequestServiceMock.Object);
            userServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
            timeOffRequestServiceMock.Setup(x => x.CreateTimeOffRequest(It.IsAny<TimeOffRequestRequestDTO>(), It.IsAny<User>())).ReturnsAsync(response);

            // act
            var result = await sut.CreateTimeOffRequest(It.IsAny<TimeOffRequestRequestDTO>());

            // assert
            ObjectResult objectResponse = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResponse.StatusCode);
        }

        [Fact]
        public async Task CreateTimeOffRequest_Invalid_ReturnsBadRequest()
        {
            //arrange
            var userServiceMock = new Mock<IUserService>();
            var timeOffRequestServiceMock = new Mock<ITimeOffRequestService>();
            var sut = new TimeOffRequestsController(userServiceMock.Object, timeOffRequestServiceMock.Object);
            userServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
            timeOffRequestServiceMock.Setup(x => x.CreateTimeOffRequest(It.IsAny<TimeOffRequestRequestDTO>(), It.IsAny<User>())).ReturnsAsync((TimeOffRequestResponseDTO)null);

            // act
            var result = await sut.CreateTimeOffRequest(It.IsAny<TimeOffRequestRequestDTO>());

            // assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateTimeOffRequest_Valid_ReturnsOk()
        {
            //arrange
            var response = new TimeOffRequestResponseDTO();
            var userServiceMock = new Mock<IUserService>();
            var timeOffRequestServiceMock = new Mock<ITimeOffRequestService>();
            var sut = new TimeOffRequestsController(userServiceMock.Object, timeOffRequestServiceMock.Object);
            userServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
            timeOffRequestServiceMock.Setup(x => x.UpdateTimeOffRequest(It.IsAny<int>(), It.IsAny<TimeOffRequestRequestDTO>(), It.IsAny<User>())).ReturnsAsync(response);

            // act
            var result = await sut.UpdateTimeOffRequest(It.IsAny<int>(), It.IsAny<TimeOffRequestRequestDTO>());

            // assert
            ObjectResult objectResponse = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResponse.StatusCode);
        }

        [Fact]
        public async Task UpdateTimeOffRequest_Invalid_ReturnsBadRequest()
        {
            // arrange
            var userServiceMock = new Mock<IUserService>();
            var timeOffRequestServiceMock = new Mock<ITimeOffRequestService>();
            var sut = new TimeOffRequestsController(userServiceMock.Object, timeOffRequestServiceMock.Object);
            userServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
            timeOffRequestServiceMock.Setup(x 
                => x.UpdateTimeOffRequest(It.IsAny<int>(), It.IsAny<TimeOffRequestRequestDTO>(), It.IsAny<User>())).ReturnsAsync((TimeOffRequestResponseDTO)null);

            // act
            var result = await sut.UpdateTimeOffRequest(It.IsAny<int>(), It.IsAny<TimeOffRequestRequestDTO>());

            // assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ApproveRequest_Valid_ReturnsOk()
        {
            //arrange
            var response = new TimeOffRequestResponseDTO();
            var userServiceMock = new Mock<IUserService>();
            var timeOffRequestServiceMock = new Mock<ITimeOffRequestService>();
            var sut = new TimeOffRequestsController(userServiceMock.Object, timeOffRequestServiceMock.Object);
            userServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
            timeOffRequestServiceMock.Setup(x => x.ApproveTimeOffRequest(It.IsAny<int>(), It.IsAny<User>(), It.IsAny<bool>())).ReturnsAsync(response);

            // act
            var result = await sut.ApproveRequest(It.IsAny<int>(), It.IsAny<bool>());

            // assert
            ObjectResult objectResponse = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResponse.StatusCode);
        }

        [Fact]
        public async Task ApproveRequest_Invalid_ReturnsBadRequest()
        {
            //arrange
            var userServiceMock = new Mock<IUserService>();
            var timeOffRequestServiceMock = new Mock<ITimeOffRequestService>();
            var sut = new TimeOffRequestsController(userServiceMock.Object, timeOffRequestServiceMock.Object);
            userServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
            timeOffRequestServiceMock.Setup(x => 
            x.ApproveTimeOffRequest(It.IsAny<int>(), It.IsAny<User>(), It.IsAny<bool>())).ReturnsAsync((TimeOffRequestResponseDTO)null);

            // act
            var result = await sut.ApproveRequest(It.IsAny<int>(), It.IsAny<bool>());

            // assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteTimeOffRequest_Valid_ReturnsOk()
        {
            //arrange
            var userServiceMock = new Mock<IUserService>();
            var timeOffRequestServiceMock = new Mock<ITimeOffRequestService>();
            var sut = new TimeOffRequestsController(userServiceMock.Object, timeOffRequestServiceMock.Object);
            userServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
            timeOffRequestServiceMock.Setup(x => x.DeleteTimeOffRequest(It.IsAny<int>(), It.IsAny<User>())).ReturnsAsync(true);

            // act
            var result = await sut.DeleteTimeOffRequest(It.IsAny<int>());

            // assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteTimeOffRequest_Invalid_ReturnsBadRequest()
        {
            // arrange
            var userServiceMock = new Mock<IUserService>();
            var timeOffRequestServiceMock = new Mock<ITimeOffRequestService>();
            var sut = new TimeOffRequestsController(userServiceMock.Object, timeOffRequestServiceMock.Object);
            userServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
            timeOffRequestServiceMock.Setup(x => x.DeleteTimeOffRequest(It.IsAny<int>(), It.IsAny<User>())).ReturnsAsync(false);

            // act
            var result = await sut.DeleteTimeOffRequest(It.IsAny<int>());

            // assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Get_Valid_ReturnsOk()
        {
            //arrange
            var response = new TimeOffRequestResponseDTO();
            var userServiceMock = new Mock<IUserService>();
            var timeOffRequestServiceMock = new Mock<ITimeOffRequestService>();
            var sut = new TimeOffRequestsController(userServiceMock.Object, timeOffRequestServiceMock.Object);
            userServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
            timeOffRequestServiceMock.Setup(x => x.GetTimeOffRequest(It.IsAny<int>(), It.IsAny<User>())).ReturnsAsync(response);

            // act
            var result = await sut.Get(It.IsAny<int>());

            // assert
            ObjectResult objectResponse = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResponse.StatusCode);
        }

        [Fact]
        public async Task Get_Invalid_ReturnsBadRequest()
        {
            //arrange
            var userServiceMock = new Mock<IUserService>();
            var timeOffRequestServiceMock = new Mock<ITimeOffRequestService>();
            var sut = new TimeOffRequestsController(userServiceMock.Object, timeOffRequestServiceMock.Object);
            userServiceMock.Setup(x => x.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new User());
            timeOffRequestServiceMock.Setup(x => x.GetTimeOffRequest(It.IsAny<int>(), It.IsAny<User>())).ReturnsAsync((TimeOffRequestResponseDTO)null);

            // act
            var result = await sut.Get(It.IsAny<int>());

            // assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}
