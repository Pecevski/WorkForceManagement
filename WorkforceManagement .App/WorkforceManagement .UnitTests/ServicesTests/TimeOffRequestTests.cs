using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Data.Entities.Enums;
using WorkforceManagement.Models.DTO.Request;
using WorkforceManagement.Models.DTO.Response;
using WorkforceManagement.Services.Contracts;
using WorkforceManagement.Services.Interfaces;
using WorkforceManagement.Services.Services;
using WorkforceManagement.UnitTests.Seeder;
using Xunit;

namespace WorkforceManagement.UnitTests.ServicesTests
{

    public class TimeOffRequestTests : IClassFixture<DatabaseFixture>
    {
        DatabaseFixture _fixture;

        public TimeOffRequestTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task DeleteTimeOffRequest_WithValidId_AndProperCreditentials_Succeed()
        {
            //Arrange
            var sut = new TimeOffRequestService(_fixture.DbContext, null, null, null);
            var requester = _fixture.DbContext.TimeOffRequests.FirstOrDefault(tor => tor.Id == 1).Requester;

            //Act
            var actual = await sut.DeleteTimeOffRequest(1, requester);

            //Assert
            Assert.True(actual);
        }

        [Fact]
        public async Task DeleteTimeOffRequest_WithValidId_AndNotProperCreditentials_Failed()
        {
            //Arrange
            var userServiceMock = new Mock<IUserService>();

            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(false);

            var sut = new TimeOffRequestService(_fixture.DbContext, userServiceMock.Object, null, null);
            var requester = _fixture.DbContext.TimeOffRequests.FirstOrDefault(tor => tor.Id == 3).Requester;

            //Act
            var actual = await sut.DeleteTimeOffRequest(2, requester);

            //Assert
            Assert.False(actual);
        }

        [Fact]
        public async Task DeleteTimeOffRequest_WithNotValidId_AndProperCreditentials_Failed()
        {
            //Arrange
            var sut = new TimeOffRequestService(_fixture.DbContext, null, null, null);
            var requester = _fixture.DbContext.TimeOffRequests.FirstOrDefault(tor => tor.Id == 1).Requester;

            //Act
            var actual = await sut.DeleteTimeOffRequest(int.MaxValue, requester);

            //Assert
            Assert.False(actual);
        }

        [Fact]
        public async Task CurrentUserHasAuthorization_WithAdminInput_Succeed()
        {
            //Arrange
            var userServiceMock = new Mock<IUserService>();

            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(true);

            var sut = new TimeOffRequestService(_fixture.DbContext, userServiceMock.Object, null, null);
            var requester = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser1");
            var currentUser = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser2");

            //Act
            var actual = await sut.CurrentUserHasAuthorization(requester, currentUser);

            //Assert
            Assert.True(actual);
        }

        [Fact]
        public async Task CurrentUserHasAuthorization_WithNotValidInput_Failed()
        {
            //Arrange
            var userServiceMock = new Mock<IUserService>();

            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(false);

            var sut = new TimeOffRequestService(_fixture.DbContext, userServiceMock.Object, null, null);
            var requester = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser1");
            var currentUser = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser2");
            //Act
            var actual = await sut.CurrentUserHasAuthorization(requester, currentUser);

            //Assert
            Assert.False(actual);
        }

        [Fact]
        public async Task CurrentUserHasAuthorization_WithValidInput_Succeed()
        {
            //Arrange
            var userServiceMock = new Mock<IUserService>();

            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(false);

            var sut = new TimeOffRequestService(_fixture.DbContext, userServiceMock.Object, null, null);
            var requester = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser1");
            var currentUser = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser1");
            //Act
            var actual = await sut.CurrentUserHasAuthorization(requester, currentUser);

            //Assert
            Assert.True(actual);
        }

        [Fact]
        public async Task GetTimeOffRequest_WithValidInput_Succeed()
        {
            //Arrange
            var userServiceMock = new Mock<IUserService>();

            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(false);

            var sut = new TimeOffRequestService(_fixture.DbContext, userServiceMock.Object, null, null);

            var timeOffRequestResponseModel = new TimeOffRequestResponseDTO()
            {
                Requester = "TestUser3"
            };

            var currentUser = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser3");

            //Act
            var actual = await sut.GetTimeOffRequest(3, currentUser);

            //Assert
            Assert.Equal(actual.Requester, timeOffRequestResponseModel.Requester);
        }

        [Fact]
        public async Task GetTimeOffRequest_WithInValidInput_Failed()
        {
            //Arrange
            var userServiceMock = new Mock<IUserService>();

            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(false);

            var sut = new TimeOffRequestService(_fixture.DbContext, userServiceMock.Object, null, null);

            var currentUser = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser3");

            //Act
            var actual = await sut.GetTimeOffRequest(2, currentUser);

            //Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task GetTimeOffRequest_WithAdminCredentials_Succeed()
        {
            //Arrange
            var userServiceMock = new Mock<IUserService>();

            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(true);

            var sut = new TimeOffRequestService(_fixture.DbContext, userServiceMock.Object, null, null);

            var timeOffRequestResponseModel = new TimeOffRequestResponseDTO()
            {
                Requester = "TestUser3"
            };

            var currentUser = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser2");

            //Act
            var actual = await sut.GetTimeOffRequest(3, currentUser);

            //Assert
            Assert.Equal(actual.Requester, timeOffRequestResponseModel.Requester);
        }

        [Fact]
        public async Task UpdateTimeOffRequest_WithInvalidId_Failed()
        {
            //Arrange
            var sut = new TimeOffRequestService(_fixture.DbContext, null, null, null);

            //Act
            var actual = await sut.UpdateTimeOffRequest(int.MaxValue, null, null);

            //Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UpdateApprovedTimeOffRequest_Failed()
        {
            //Arrange
            var sut = new TimeOffRequestService(_fixture.DbContext, null, null, null);

            //Act
            var actual = await sut.UpdateTimeOffRequest(4, null, null);

            //Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UpdateRejectedTimeOffRequest_Failed()
        {
            //Arrange
            var sut = new TimeOffRequestService(_fixture.DbContext, null, null, null);

            //Act
            var actual = await sut.UpdateTimeOffRequest(6, null, null);

            //Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UpdateTimeOffRequest_WithIncorrectRequestType_Failed()
        {
            //Arrange
            var userServiceMock = new Mock<IUserService>();

            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(true);

            var sut = new TimeOffRequestService(_fixture.DbContext, userServiceMock.Object, null, null);

            var timeOffRequestRequestModel = new TimeOffRequestRequestDTO()
            {
                RequestType = "IncorrectRequestType"
            };

            var currentUser = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser3");

            //Act
            var actual = await sut.UpdateTimeOffRequest(1, timeOffRequestRequestModel, currentUser);

            //Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UpdateTimeOffRequest_WithSickLeaveRequestType_Succeed()
        {
            //Arrange
            var userServiceMock = new Mock<IUserService>();
            var emailServiceMock = new Mock<IMailService>();

            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(true);
            emailServiceMock.Setup(x => x.SendEmail(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<TimeOffRequest>(), EmailType.Default)).Returns(true);

            var sut = new TimeOffRequestService(_fixture.DbContext, userServiceMock.Object, emailServiceMock.Object, null);

            var timeOffRequestRequestModel = new TimeOffRequestRequestDTO()
            {
                RequestType = RequestType.SickLeave.ToString()
            };

            var timeOffRequestResponseModel = new TimeOffRequestResponseDTO()
            {
                RequestStatus = RequestStatus.Approved.ToString()
            };

            var currentUser = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser3");

            //Act
            var actual = await sut.UpdateTimeOffRequest(2, timeOffRequestRequestModel, currentUser);

            //Assert
            Assert.Equal(actual.RequestStatus, timeOffRequestResponseModel.RequestStatus);
        }

        [Fact]
        public async Task UpdateTimeOffRequest_WithValidInput_Succeed()
        {
            //Arrange
            var userServiceMock = new Mock<IUserService>();
            var emailServiceMock = new Mock<IMailService>();

            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(true);
            emailServiceMock.Setup(x => x.SendEmail(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<TimeOffRequest>(), EmailType.Default)).Returns(true);

            var sut = new TimeOffRequestService(_fixture.DbContext, userServiceMock.Object, emailServiceMock.Object, null);

            var timeOffRequestRequestModel = new TimeOffRequestRequestDTO()
            {
                RequestType = RequestType.Paid.ToString(),
                Reason = "New Reason",
                StartDate = new DateTime(2221, 1, 2),
                EndDate = new DateTime(2221, 1, 12)
            };

            var timeOffRequestResponseModel = new TimeOffRequestResponseDTO()
            {
                RequestType = RequestType.Paid.ToString(),
                Reason = "New Reason",
                StartDate = new DateTime(2221, 1, 2),
                EndDate = new DateTime(2221, 1, 12)
            };

            var currentUser = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser3");

            //Act
            var actual = await sut.UpdateTimeOffRequest(1, timeOffRequestRequestModel, currentUser);

            //Assert
            Assert.Equal(actual.RequestType, timeOffRequestResponseModel.RequestType);
            Assert.Equal(actual.Reason, timeOffRequestResponseModel.Reason);
            Assert.Equal(actual.StartDate, timeOffRequestResponseModel.StartDate);
            Assert.Equal(actual.EndDate, timeOffRequestResponseModel.EndDate);
        }

        [Fact]
        public void SendMailRange_WithValidInput_ShouldReturnTrue()
        {
            //Arrange
            var userServiceMock = new Mock<IUserService>();
            var mailServiceMock = new Mock<IMailService>();

            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(false);
            mailServiceMock.Setup(m => m.SendEmail(It.IsAny<User>(), It.IsNotNull<string>(), It.IsAny<TimeOffRequest>(), EmailType.Default)).Returns(true);

            var service = new TimeOffRequestService(_fixture.DbContext, userServiceMock.Object, mailServiceMock.Object, null);

            var sender = new User()
            {
                Email = "sender@demo.com"
            };

            var user1 = new User()
            {
                Email = "user1@demo.com"
            };

            HashSet<User> receivers = new HashSet<User>();
            receivers.Add(user1);

            var request = new TimeOffRequest()
            {
                Reason = "Test",
            };

            //Act
            var result = service.SendMailRange(sender, receivers, request);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ApproveTimeOffRequest_WithInvalidId_Failed()
        {
            //Arrange
            var sut = new TimeOffRequestService(_fixture.DbContext, null, null, null);

            //Act
            var actual = await sut.ApproveTimeOffRequest(int.MaxValue, null, true);

            //Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task ApproveTimeOffRequest_WithInvalidApproverId_Failed()
        {
            //Arrange
            var currentUser = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser3");
            var sut = new TimeOffRequestService(_fixture.DbContext, null, null, null);

            //Act
            var actual = await sut.ApproveTimeOffRequest(7, currentUser, true);

            //Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task ApproveAlreadyRejectedTimeOffRequest_Failed()
        {
            //Arrange
            var currentUser = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestLeader1");
            var sut = new TimeOffRequestService(_fixture.DbContext, null, null, null);

            //Act
            var actual = await sut.ApproveTimeOffRequest(8, currentUser, true);

            //Assert
            Assert.Null(actual);
        }


        [Fact]
        public async Task RejectedTimeOffRequest_Succeed()
        {
            //Arrange
            var currentUser = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestLeader1");

            var timeOffRequestResponseModel = new TimeOffRequestResponseDTO()
            {
                RequestStatus = RequestStatus.Rejected.ToString()
            };

            var dateServiceMock = new Mock<IDateService>();

            var mailServiceMock = new Mock<IMailService>();

            mailServiceMock.Setup(x => x.SendEmail(It.IsAny<User>(), It.IsAny<string>(),
                It.IsAny<TimeOffRequest>(), It.IsAny<EmailType>())).Returns(true);

            var sut = new TimeOffRequestService(_fixture.DbContext, null, mailServiceMock.Object, dateServiceMock.Object);

            //Act
            var actual = await sut.ApproveTimeOffRequest(7, currentUser, false);

            //Assert
            Assert.Equal(actual.RequestStatus, timeOffRequestResponseModel.RequestStatus);
        }

        [Fact]
        public void GetRequesterTeamLeads_Default_ReturnHashSetOfUser()
        {
            //assign
            var userServiceMock = new Mock<IUserService>();
            var dateServiceMock = new Mock<IDateService>();
            var mailServiceMock = new Mock<IMailService>();
            var currentUser = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser1");

            var sut = new TimeOffRequestService(_fixture.DbContext, null, mailServiceMock.Object, dateServiceMock.Object);
            //act
            var actual = sut.GetRequesterTeamLeads(currentUser);

            //assert
            Assert.Equal(3, actual.Count);
        }

        [Fact]
        public void GetRequesterTeamLeads_Default_ReturnemptyCollectionr()
        {
            //assign
            var userServiceMock = new Mock<IUserService>();
            var dateServiceMock = new Mock<IDateService>();
            var mailServiceMock = new Mock<IMailService>();
            var currentUser = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestLeader1");

            var sut = new TimeOffRequestService(_fixture.DbContext, null, mailServiceMock.Object, dateServiceMock.Object);
            //act
            var actual = sut.GetRequesterTeamLeads(currentUser);

            //assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task TeamLeadIsOutOfOffice_Invalid_ReturnFalse()
        {
            //assign
            var userServiceMock = new Mock<IUserService>();
            var dateServiceMock = new Mock<IDateService>();
            var mailServiceMock = new Mock<IMailService>();
            var currentUser = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser1");

            var request = new TimeOffRequest()
            {
                Reason = "Test",
            };

            var sut = new TimeOffRequestService(_fixture.DbContext, null, mailServiceMock.Object, dateServiceMock.Object);
            //act
            var actual = await sut.TeamLeadIsOutOfOffice(currentUser, request);

            //assert
            Assert.False(actual);
        }
    }
}