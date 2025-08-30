using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkforceManagement.Data.Database;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Models.DTO.Request.Team;
using WorkforceManagement.Models.DTO.Response.Team;
using WorkforceManagement.Services.Contracts;
using WorkforceManagement.Services.Interfaces;
using WorkforceManagement.WebApi.Mapper;

namespace WorkforceManagement.Services.Services
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserService _userService;
        public TeamService(ApplicationDbContext applicationDbContext, IUserService userService)
        {
            _applicationDbContext = applicationDbContext;
            _userService = userService;
        }

        public async Task<ICollection<TeamResponse>> GetAllTeams()
        {
            ICollection<TeamResponse> teams = new List<TeamResponse>();

            ICollection<Team> allTeams = await _applicationDbContext.Teams.ToListAsync();

            foreach (var team in allTeams)
            {
                TeamResponse TeamResponse = TeamMapper.MapTeam(team);
                teams.Add(TeamResponse);
            }

            return teams;
        }

        public async Task<TeamResponse> GetById(int teamId)
        {
            Team team = await _applicationDbContext.Teams.SingleOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
            {
                return null;
            }

            return TeamMapper.MapTeam(team);
        }

        public async Task<TeamResponse> CreateTeam(TeamRequest teamToCreate)
        {
            Team team = TeamMapper.MapTeamRequest(teamToCreate);

            if (team == null)
            {
                return null;
            }

            User teamLeader = await _userService.GetUserById(teamToCreate.TeamLeaderId);

            if (teamLeader == null)
            {
                return null;
            }

            team.TeamLeader = teamLeader;

            await _applicationDbContext.Teams.AddAsync(team);
            await _applicationDbContext.SaveChangesAsync();

            return TeamMapper.MapTeam(team);
        }

        public async Task<TeamResponse> UpdateTeam(TeamRequest teamRequest, int teamId)
        {
            Team updateTeam = await _applicationDbContext.Teams.FindAsync(teamId);

            if (updateTeam == null)
            {
                return null;
            }

            User teamLeader = await _userService.GetUserById(teamRequest.TeamLeaderId);

            if (teamLeader == null)
            {
                return null;
            }

            updateTeam.Title = teamRequest.Title;
            updateTeam.Description = teamRequest.Description;
            updateTeam.TeamLeader = teamLeader;
            updateTeam.LastModifiedOn = DateTime.UtcNow;

            _applicationDbContext.Teams.Update(updateTeam);
            await _applicationDbContext.SaveChangesAsync();

            return TeamMapper.MapTeam(updateTeam);
        }

        public async Task<bool> DeleteTeam(int teamId)
        {
            Team removeTeam = await _applicationDbContext.Teams.FindAsync(teamId);
            if (removeTeam == null)
            {
                return false;
            }

            removeTeam.IsDeleted = true;
            removeTeam.DeletedOn = DateTime.UtcNow;
            await _applicationDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AssignMemberToTeam(int teamId, string userId)
        {
            Team team = await _applicationDbContext.Teams.SingleOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
            {
                return false;
            }

            User member = await _userService.GetUserById(userId);

            if (member == null)
            {
                return false;
            }

            team.Members.Add(member);
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveMemberFromTeam(int teamId, string userId)
        {
            Team team = await _applicationDbContext.Teams.SingleOrDefaultAsync(t => t.Id == teamId);

            if (team == null || !team.Members.Any(u => u.Id == userId))
            {
                return false;
            }

            User member = await _userService.GetUserById(userId);

            if (member == null)
            {
                return false;
            }

            team.Members.Remove(member);
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }
    }
}
