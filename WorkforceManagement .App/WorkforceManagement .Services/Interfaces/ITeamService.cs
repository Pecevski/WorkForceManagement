using System.Collections.Generic;
using System.Threading.Tasks;
using WorkforceManagement.Models.DTO.Request.Team;
using WorkforceManagement.Models.DTO.Response.Team;

namespace WorkforceManagement.Services.Interfaces
{
    public interface ITeamService
    {
        Task<ICollection<TeamResponse>> GetAllTeams();
        Task<TeamResponse> GetById(int teamId);
        Task<TeamResponse> CreateTeam(TeamRequest team);
        Task<TeamResponse> UpdateTeam(TeamRequest team, int teamId);
        Task<bool> DeleteTeam(int teamId);
        Task<bool> AssignMemberToTeam(int teamId, string userId);
        Task<bool> RemoveMemberFromTeam(int teamId, string userId);

    }
}