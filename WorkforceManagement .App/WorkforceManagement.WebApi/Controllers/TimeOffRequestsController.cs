using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Models.DTO.Request;
using WorkforceManagement.Models.DTO.Response;
using WorkforceManagement.Services.Contracts;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WorkforceManagement.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TimeOffRequestsController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly ITimeOffRequestService _timeOffRequestService;

        public TimeOffRequestsController(IUserService userService, ITimeOffRequestService timeOffRequestService)
        {
            _userService = userService;
            _timeOffRequestService = timeOffRequestService;
        }

        // POST api/<TimeOffRequestsController>
        [HttpPost]
        public async Task<IActionResult> CreateTimeOffRequest(TimeOffRequestRequestDTO timeOffRequestRequestDTO)
        {
            User currentUser = await _userService.GetCurrentUserAsync(this.User);

            TimeOffRequestResponseDTO result = await _timeOffRequestService.CreateTimeOffRequest(timeOffRequestRequestDTO, currentUser);

            if (result != null)
            {
                return StatusCode(201, result);
            }

            return BadRequest("TimeOffRequest Submit Failed!");
        }

        // PUT api/<TimeOffRequestsController>/5
        [HttpPut("{timeOffRequestId}")]
        public async Task<IActionResult> UpdateTimeOffRequest(int timeOffRequestId, TimeOffRequestRequestDTO timeOffRequestRequestDT)
        {
            User currentUser = await _userService.GetCurrentUserAsync(this.User);

            TimeOffRequestResponseDTO result = await _timeOffRequestService.UpdateTimeOffRequest(timeOffRequestId, timeOffRequestRequestDT, currentUser);

            if (result != null)
            {
                return StatusCode(200, result);
            }

            return BadRequest("TimeOffRequest Update Failed!");
        }

        // PUT api/<TimeOffRequestsController>/5
        [HttpPut("ApproveRequest/{timeOffRequestId}/{isApproved}")]
        [ActionName("ApproveRequest")]
        public async Task<IActionResult> ApproveRequest(int timeOffRequestId, bool isApproved)
        {
            User currentUser = await _userService.GetCurrentUserAsync(this.User);

            TimeOffRequestResponseDTO result = await _timeOffRequestService.ApproveTimeOffRequest(timeOffRequestId, currentUser, isApproved);

            if (result != null)
            {
                return StatusCode(200, result);
            }

            return BadRequest("TimeOffRequest Update Failed!");
        }

        // DELETE api/<TimeOffRequestsController>/5
        [HttpDelete("{timeOffRequestId}")]
        public async Task<IActionResult> DeleteTimeOffRequest(int timeOffRequestId)
        {
            User currentUser = await _userService.GetCurrentUserAsync(this.User);

            if (await _timeOffRequestService.DeleteTimeOffRequest(timeOffRequestId, currentUser))
            {
                return Ok("TimeOffRequest Delete Succeeded!");
            }

            return BadRequest("TimeOffRequest Delete Failed!");
        }

        // GET api/<TimeOffRequestsController>/5
        [HttpGet("{timeOffRequestId}")]
        public async Task<IActionResult> Get(int timeOffRequestId)
        {
            User currentUser = await _userService.GetCurrentUserAsync(this.User);

            TimeOffRequestResponseDTO result = await _timeOffRequestService.GetTimeOffRequest(timeOffRequestId, currentUser);

            if (result != null)
            {
                return StatusCode(200, result);
            }

            return BadRequest();
        }
    }
}
