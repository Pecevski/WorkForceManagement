using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WorkforceManagement.Common;
using WorkforceManagement.Models.DTO.Request.Team;
using WorkforceManagement.Models.DTO.Response.Team;
using WorkforceManagement.Services.Contracts;
using WorkforceManagement.Services.Interfaces;

namespace WorkforceManagement.WebApi.Controllers
{
    [ApiController]
    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Route("api/[controller]")]

    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly IUserService _userService;

        public TeamsController(ITeamService teamService, IUserService userService)
        {
            _teamService = teamService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IEnumerable<TeamResponse>> GetAll()
        {
            return await _teamService.GetAllTeams();
        }

        [HttpGet]
        [Route("{teamId}")]
        public async Task<ActionResult<TeamResponse>> Get(int teamId)
        {
            return await _teamService.GetById(teamId);
        }


        [HttpPost]
        [ActionName(nameof(Get))]
        public async Task<IActionResult> Create(TeamRequest teamRequest)
        {
            TeamResponse createTeam = await _teamService.CreateTeam(teamRequest);

            if (createTeam != null)
            {
                return CreatedAtAction("Get", "Teams", new { id = createTeam.Id }, teamRequest);
            }

            return BadRequest("Team already exist.");
        }

        [HttpPut]
        [Route("{teamId}")]
        public async Task<IActionResult> Update(TeamRequest teamRequest, int teamId)
        {
            var updateTeam = await _teamService.UpdateTeam(teamRequest, teamId);
            if (updateTeam != null)
            {
                return Ok("Team updated.");
            }

            return BadRequest();
        }

        [HttpDelete]
        [Route("Delete/{teamId}")]
        public async Task<IActionResult> Delete(int teamId)
        {
            bool result = await _teamService.DeleteTeam(teamId);
            if (result)
            {
                return Ok("Team removed");
            }

            return BadRequest();
        }

        [HttpPut]
        [Route("AddMemberToTeam")]
        public async Task<IActionResult> AddMemberToTeam(int teamId, string userId)
        {

            if (await _teamService.AssignMemberToTeam(teamId, userId))
            {
                return Ok("Memeber assigned to Team");
            }

            return BadRequest();
        }

        [HttpPut]
        [Route("RemoveMember")]
        public async Task<IActionResult> RemoveMember([Required]int teamId, string userId)
        {

            if (await _teamService.RemoveMemberFromTeam(teamId, userId))
            {
                return Ok("Memeber removed from Team");
            }

            return BadRequest();
        }

    }
}
