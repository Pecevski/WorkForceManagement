using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Models.DTO.Request.Team;
using WorkforceManagement.Models.DTO.Response.Team;
using WorkforceManagement.Services.Contracts;
using WorkforceManagement.Services.Interfaces;
using WorkforceManagement.WebApi.Controllers;
using Xunit;

namespace WorkforceManagement.UnitTests.ControllersTests
{
    public class TeamControllerTests
    {
        [Fact]
        public async Task GetTeam_ValidInput_ReturnTeams()
        {
            //arrange
            ICollection<TeamResponse> teams = new Collection<TeamResponse>()
                {
                    new TeamResponse { Title = "TeamsOne", Description = "First Team", },
                    new TeamResponse { Title = "TeamTwo", Description = "Second Team", },
                    new TeamResponse { Title = "TeamThree", Description = "Third Team", }
                };
            var userServiceMock = new Mock<IUserService>();
            var teamServiceMock = new Mock<ITeamService>();
            teamServiceMock.Setup(t => t.GetAllTeams()).ReturnsAsync(teams);
            var sut = new TeamsController(teamServiceMock.Object, userServiceMock.Object);

            //act
            var result = await sut.GetAll();

            //assert
            Assert.IsType<Collection<TeamResponse>>(result);
        }

        [Fact]
        public async Task GetTeam_ValidId_ReturnTeam()
        {
            //arrange
            var team = new TeamResponse();
            var userServiceMock = new Mock<IUserService>();
            var teamServiceMock = new Mock<ITeamService>();
            teamServiceMock.Setup(t => t.GetById(It.IsAny<int>())).ReturnsAsync(team);
            var sut = new TeamsController(teamServiceMock.Object, userServiceMock.Object);

            //act
            var result = await sut.Get(team.Id);

            //assert
            Assert.IsType<TeamResponse>(result.Value);
        }

        [Fact]
        public async Task GetTeam_InValidId_ReturnNull()
        {
            //arrange
            var team = new TeamResponse();
            var userServiceMock = new Mock<IUserService>();
            var teamServiceMock = new Mock<ITeamService>();
            teamServiceMock.Setup(t => t.GetById(It.IsAny<int>())).ReturnsAsync((TeamResponse)null);
            var sut = new TeamsController(teamServiceMock.Object, userServiceMock.Object);

            //act
            var result = await sut.Get(team.Id);

            //assert
            Assert.Null(result.Value);
        }


        [Fact]
        public async Task CreateTeam_Valid_ReturnsOk()
        {
            //arrange
            var response = new TeamResponse();
            var userServiceMock = new Mock<IUserService>();
            var teamServiceMock = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceMock.Object, userServiceMock.Object);

            teamServiceMock.Setup(x => x.CreateTeam(It.IsAny<TeamRequest>())).ReturnsAsync(response);

            // act
            var result = await sut.Create(It.IsAny<TeamRequest>());

            // assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task CreateTeam_Invalid_ReturnsBadRequest()
        {
            // arrange
            var userServiceMock = new Mock<IUserService>();
            var teamServiceMock = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceMock.Object, userServiceMock.Object);
            teamServiceMock.Setup(x => x.CreateTeam(It.IsAny<TeamRequest>())).ReturnsAsync((TeamResponse)null);

            // act
            var result = await sut.Create(It.IsAny<TeamRequest>());

            // assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateTeam_Valid_ReturnsOk()
        {
            //arrange
            var response = new TeamResponse();
            var userServiceMock = new Mock<IUserService>();
            var teamServiceMock = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceMock.Object, userServiceMock.Object);

            teamServiceMock.Setup(x => x.UpdateTeam(It.IsAny<TeamRequest>(), It.IsAny<int>())).ReturnsAsync(response);

            // act
            var result = await sut.Update(It.IsAny<TeamRequest>(), It.IsAny<int>());

            // assert
            OkObjectResult objectResponse = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, objectResponse.StatusCode);
        }

        [Fact]
        public async Task UpdateTeam_Invalid_ReturnsBadRequest()
        {
            // arrange
            var userServiceMock = new Mock<IUserService>();
            var teamServiceMock = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceMock.Object, userServiceMock.Object);
            teamServiceMock.Setup(x => x.UpdateTeam(It.IsAny<TeamRequest>(), It.IsAny<int>())).ReturnsAsync((TeamResponse)null);

            // act
            var result = await sut.Update(It.IsAny<TeamRequest>(), It.IsAny<int>());

            // assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteTeam_Valid_ReturnsOk()
        {
            //arrange
            var response = new TeamResponse();
            var userServiceMock = new Mock<IUserService>();
            var teamServiceMock = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceMock.Object, userServiceMock.Object);
            teamServiceMock.Setup(x => x.DeleteTeam(It.IsAny<int>())).ReturnsAsync(true);

            // act
            var result = await sut.Delete(It.IsAny<int>());

            // assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteTeam_InValid_ReturnsBadRequest()
        {
            //arrange
            var response = new TeamResponse();
            var userServiceMock = new Mock<IUserService>();
            var teamServiceMock = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceMock.Object, userServiceMock.Object);
            teamServiceMock.Setup(x => x.DeleteTeam(It.IsAny<int>())).ReturnsAsync(false);

            // act
            var result = await sut.Delete(It.IsAny<int>());

            // assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task AddmemberToTeam_Valid_ReturnsOk()
        {
            //arrange
            var response = new TeamResponse();
            var userServiceMock = new Mock<IUserService>();
            var teamServiceMock = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceMock.Object, userServiceMock.Object);

            teamServiceMock.Setup(x => x.AssignMemberToTeam(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);

            // act
            var result = await sut.AddMemberToTeam(It.IsAny<int>(), It.IsAny<string>());

            // assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AddmemberToTeam_InValid_ReturnsBadRequest()
        {
            //arrange
            var response = new TeamResponse();
            var userServiceMock = new Mock<IUserService>();
            var teamServiceMock = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceMock.Object, userServiceMock.Object);

            teamServiceMock.Setup(x => x.AssignMemberToTeam(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(false);

            // act
            var result = await sut.AddMemberToTeam(It.IsAny<int>(), It.IsAny<string>());

            // assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task RemovememberToTeam_Valid_ReturnsOk()
        {
            //arrange
            var response = new TeamResponse();
            var userServiceMock = new Mock<IUserService>();
            var teamServiceMock = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceMock.Object, userServiceMock.Object);

            teamServiceMock.Setup(x => x.RemoveMemberFromTeam(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);

            // act
            var result = await sut.RemoveMember(It.IsAny<int>(), It.IsAny<string>());

            // assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RemovememberToTeam_InValid_ReturnsBadRequest()
        {
            //arrange
            var response = new TeamResponse();
            var userServiceMock = new Mock<IUserService>();
            var teamServiceMock = new Mock<ITeamService>();
            var sut = new TeamsController(teamServiceMock.Object, userServiceMock.Object);

            teamServiceMock.Setup(x => x.RemoveMemberFromTeam(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(false);

            // act
            var result = await sut.RemoveMember(It.IsAny<int>(), It.IsAny<string>());

            // assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}

