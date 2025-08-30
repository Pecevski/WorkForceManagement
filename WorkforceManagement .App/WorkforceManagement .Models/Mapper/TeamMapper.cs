using WorkforceManagement.Data.Entities;
using WorkforceManagement.Models.DTO.Request.Team;
using WorkforceManagement.Models.DTO.Response.Team;

namespace WorkforceManagement.WebApi.Mapper
{
    public static class TeamMapper
    {
        public static TeamResponse MapTeam(Team team)
        {
            var teamResponse = new TeamResponse()
            {
                Id = team.Id,
                Title = team.Title,
                Description = team.Description,
                TeamLeaderId = team.TeamLeader.Id,
                CreatedOn = team.CreatedOn,
                LastModifiedOn = team.LastModifiedOn
            };

            return teamResponse;
        }

        public static Team MapTeamRequest(TeamRequest teamRequest)
        {
            var team = new Team()
            {
                Title = teamRequest.Title,
                Description = teamRequest.Description,
            };

            return team;
        }
    }
}

