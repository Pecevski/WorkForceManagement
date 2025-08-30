using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkforceManagement.Common;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Models.DTO.Request.User;
using WorkforceManagement.Models.DTO.Response.User;
using WorkforceManagement.Services.Contracts;

namespace WorkforceManagement.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> CreateUser(UserCreateRequest inputModel)
        {
            UserResponse result = await _userService.CreateUserAsync(inputModel);
            if (result != null)
            {
                string url = this.Url.Link(null, new { Controller = "Users", Action = "GetAll" });
                return Created(url, result);
            }

            return BadRequest();
        }

        [HttpPut]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [Route("assign-admin")]
        public async Task<IActionResult> AssignUserAsAdmin(string username)
        {
            var result = await _userService.AssignUserAsAdmin(username);
            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpGet]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<ActionResult<List<UserResponse>>> GetAll()
        {
            List<UserResponse> result = await _userService.GetAllAsync();
            return result;
        }

        [HttpGet]
        [Authorize]
        [Route("DaysOff")]
        public async Task<IActionResult> GetMyDaysOff(string userId)
        {
            var result = await _userService.GetUserDaysOffAsync(userId);
            if (result != null)
            {
                return Ok(result);
            }

            return BadRequest();
        }

        [HttpPut]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> UpdateUser(UserUpdateRequest inputModel)
        {
            UserResponse result = await _userService.UpdateUserAsync(inputModel);
            if (result != null)
            {
                return NoContent();
            }
            return BadRequest();
        }

        [HttpPut]
        [Authorize(Policy = "IsTeamLeadOrAdmin")]
        [Route("Update-DaysOff")]
        public async Task<IActionResult> UpdateUserDayOff(UserDayOffUpdateRequest inputModel)
        {
            UserResponse result = await _userService.UpdateUserDayOffsAsync(inputModel);
            if (result != null)
            {
                return NoContent();
            }
            return BadRequest();
        }

        [HttpDelete]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            bool result = await _userService.DeleteUserAsync(id);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}