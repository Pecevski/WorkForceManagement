using Moq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Models.DTO.Request.Team;
using WorkforceManagement.Models.DTO.Response.Team;
using WorkforceManagement.Services.Contracts;
using WorkforceManagement.Services.Services;
using WorkforceManagement.UnitTests.Seeder;
using Xunit;

namespace WorkforceManagement.UnitTests.ServicesTests
{
    public class TeamServiceTests : IClassFixture<TeamDBFixture>
    {
        private TeamDBFixture _fixture;

        public TeamServiceTests(TeamDBFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetById_ValidTeamId_ReturnsTeam()
        {
            // arrange
            var sut = new TeamService(_fixture.DbContext, null);
            var fakeTeamId = 1;
            var fakeTeam = _fixture.DbContext.Teams.FirstOrDefault(t => t.Id == fakeTeamId);

            // act
            var result = await sut.GetById(1);

            // assert
            Assert.Equal(fakeTeam.Id, result.Id);
        }

        [Fact]
        public async Task GetById_InvalidTeamId_ReturnsNull()
        {
            // arrange
            var sut = new TeamService(_fixture.DbContext, null);
            var fakeTeamId = 0;

            // act
            var result = await sut.GetById(fakeTeamId);

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAll_Teams_ReturnFailed()
        {
            //arrange
            var sut = new TeamService(_fixture.DbContext, null);
            var userServiceMock = new Mock<IUserService>();
            var user = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser1");
            userServiceMock.Setup(u => u.GetUserById(It.IsAny<string>())).ReturnsAsync(user);

            var listOfTeams = new List<TeamResponse>()
            {
               new TeamResponse {Id = 1, Title = "FirstTeam", Description = "First Team", TeamLeaderId = user.Id },
               new TeamResponse {Id = 2, Title = "SecondTeam", Description = "Second Team", TeamLeaderId = user.Id },
            };

            //act
            var result = await sut.GetAllTeams();

            //assert
            Assert.NotEqual(listOfTeams, result);
        }

        [Fact]
        public async Task Delete_Team_ReturnTrue()
        {
            // arrange
            var sut = new TeamService(_fixture.DbContext, null);
            var fakeTeamId = 2;
            var fakeTeam = _fixture.DbContext.Teams.FirstOrDefault(t => t.Id == fakeTeamId);

            //act

            var result = await sut.DeleteTeam(fakeTeam.Id);

            //assert
            Assert.True(result);
        }

        [Fact]
        public async Task Delete_Team_ReturnSucceed()
        {
            // arrange
            var sut = new TeamService(_fixture.DbContext, null);
            var fakeTeamId = 3;
            var fakeTeam = _fixture.DbContext.Teams.FirstOrDefault(t => t.Id == fakeTeamId);

            //act
            await sut.DeleteTeam(fakeTeam.Id);
            var result = await sut.GetById(3);

            //assert
            Assert.Null(result);
        }


        [Fact]
        public async Task CreateTeam_InvalidInput_ReturnsNullTeam()
        {
            //arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(false);
            var sut = new TeamService(_fixture.DbContext, userServiceMock.Object);

            var fakeTeam = new TeamRequest()
            {
                Title = "FakeTeam",
                Description = "FakeDescription",
                TeamLeaderId = "FakeLead"
            };

            //act
            var result = await sut.CreateTeam(fakeTeam);

            //assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateTeam_InvalidInput_ReturnsTeamLeadNull()
        {
            //arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(false);
            var sut = new TeamService(_fixture.DbContext, userServiceMock.Object);

            var fakeTeam = new TeamRequest()
            {
                Title = "FakeTeam",
                Description = "FakeDescription",
                TeamLeaderId = "FakeLead"
            };

            var teamLead = _fixture.DbContext.Users.Any(u => u.Id == fakeTeam.TeamLeaderId);

            //act
            var result = await sut.CreateTeam(fakeTeam);

            //assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateTeam_ValidInput_ReturnTeamResponse()
        {
            //arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(true);

            var user = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser1");
            userServiceMock.Setup(u => u.GetUserById(It.IsAny<string>())).ReturnsAsync(user);

            var sut = new TeamService(_fixture.DbContext, userServiceMock.Object);

            var testTeam = new TeamRequest()
            {
                Title = "FakeTeam",
                Description = "FakeDescription",
                TeamLeaderId = user.Id
            };

            //act
            var result = await sut.CreateTeam(testTeam);

            //assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateTeam_ValidInput_ReturnUpdateTeam()
        {
            //arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(false);

            var user = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser1");
            userServiceMock.Setup(u => u.GetUserById(It.IsAny<string>())).ReturnsAsync(user);

            var sut = new TeamService(_fixture.DbContext, userServiceMock.Object);
            var testTeamId = 1;
            var testTeam = new TeamRequest()
            {
                Title = "NewTeam",
                Description = "UpdateDescription",
                TeamLeaderId = user.Id
            };

            //act
            var create = await sut.UpdateTeam(testTeam, testTeamId);
            var result = create;

            //assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateTeam_InvalidInput_ReturnsNullTeam()
        {
            //arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(false);
            var sut = new TeamService(_fixture.DbContext, userServiceMock.Object);

            var fakeTeamId = 1;
            var fakeTeam = new TeamRequest()
            {
                Title = "FakeTeam",
                Description = "FakeDescription",
                TeamLeaderId = "FakeLead"
            };

            //act
            var result = await sut.UpdateTeam(fakeTeam, fakeTeamId);

            //assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateTeam_InvalidInput_ReturnsNullTeamLead()
        {
            //arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(false);
            var sut = new TeamService(_fixture.DbContext, userServiceMock.Object);

            var fakeTeamId = 1;
            var fakeTeam = new TeamRequest()
            {
                Title = "FakeTeam",
                Description = "FakeDescription",
                TeamLeaderId = "FakeLead"
            };

            var teamLead = _fixture.DbContext.Users.Any(u => u.Id == fakeTeam.TeamLeaderId);

            //act
            var result = await sut.UpdateTeam(fakeTeam, fakeTeamId);

            //assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AssignMemberToTeam_ValidInput_ReturnTrue()
        {
            //assign
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(true);

            var user = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser1");
            userServiceMock.Setup(u => u.GetUserById(It.IsAny<string>())).ReturnsAsync(user);

            var teamId = 1;
            var team = _fixture.DbContext.Teams.SingleOrDefault(t => t.Id == teamId);
            var sut = new TeamService(_fixture.DbContext, userServiceMock.Object);

            //act
            var result = await sut.AssignMemberToTeam(team.Id, user.Id);

            //assert
            Assert.True(result);
        }

        [Fact]
        public async Task AssignMemberToTeam_InvalidInput_ReturnFalse()
        {
            //assign
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(false);

            var user = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser1");
            userServiceMock.Setup(u => u.GetUserById(It.IsAny<string>())).ReturnsAsync((User)null);

            var teamId = 1;
            var team = _fixture.DbContext.Teams.SingleOrDefault(t => t.Id == teamId);

            var sut = new TeamService(_fixture.DbContext, userServiceMock.Object);

            //act
            var result = await sut.AssignMemberToTeam(team.Id, user.Id);

            //assert
            Assert.False(result);
        }

        [Fact]
        public async Task AssignMemberToTeam_InvalidTeamId_ReturnFalse()
        {
            //assign
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.UserIsAdmin(It.IsAny<User>())).ReturnsAsync(true);

            var user = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser1");
            userServiceMock.Setup(u => u.GetUserById(It.IsAny<string>())).ReturnsAsync((User)null);

            var teamId = 1;
            var team = _fixture.DbContext.Teams.SingleOrDefault(t => t.Id == teamId);

            var sut = new TeamService(_fixture.DbContext, userServiceMock.Object);

            //act
            var result = await sut.AssignMemberToTeam(team.Id, user.Id);

            //assert
            Assert.False(result);
        }

        [Fact]
        public async Task Remove_MemberFromTeam_ReturnTrue()
        {
            //assign
            var userServiceMock = new Mock<IUserService>();
            var user = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser1");
            userServiceMock.Setup(u => u.GetUserById(It.IsAny<string>())).ReturnsAsync(user);

            var teamId = 1;
            var team = _fixture.DbContext.Teams.SingleOrDefault(t => t.Id == teamId);
            var sut = new TeamService(_fixture.DbContext, userServiceMock.Object);

            //act
            var result = await sut.RemoveMemberFromTeam(team.Id, user.Id);

            //assert
            Assert.True(result);
        }

        [Fact]
        public async Task Remove_MemberFromTeam_ReturnFalse()
        {
            //assign
            var userServiceMock = new Mock<IUserService>();

            var user = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser2");
            userServiceMock.Setup(u => u.GetUserById(It.IsAny<string>())).ReturnsAsync((User)null);

            var teamId = 1;
            var team = _fixture.DbContext.Teams.SingleOrDefault(t => t.Id == teamId);

            var sut = new TeamService(_fixture.DbContext, userServiceMock.Object);

            //act
            var result = await sut.RemoveMemberFromTeam(team.Id, user.Id);

            //assert
            Assert.False(result);
        }

        [Fact]
        public async Task Remove_InvalidTeamId_ReturnFalse()
        {
            //assign
            var userServiceMock = new Mock<IUserService>();
            var user = _fixture.DbContext.Users.FirstOrDefault(u => u.UserName == "TestUser1");
            userServiceMock.Setup(u => u.GetUserById(It.IsAny<string>())).ReturnsAsync(user);

            var teamId = int.MaxValue;
            var sut = new TeamService(_fixture.DbContext, userServiceMock.Object);

            //act
            var result = await sut.RemoveMemberFromTeam(teamId, user.Id);

            //assert
            Assert.False(result);
        }
    }
}
