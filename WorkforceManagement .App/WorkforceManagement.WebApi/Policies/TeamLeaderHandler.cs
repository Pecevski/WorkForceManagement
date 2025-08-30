using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using WorkforceManagement.Data.Database;
using WorkforceManagement.Data.Entities;

namespace WorkforceManagement.WebApi.Policies
{
    public class TeamLeaderHandler : AuthorizationHandler<TeamLeaderRequirement>
    {
        private readonly IServiceProvider _serviceProvider;

        public TeamLeaderHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TeamLeaderRequirement requirement)
        {
            if (context.User.IsInRole(requirement.AdminRole))
            {
                context.Succeed(requirement);
                return Task.FromResult(0);
            }

            ApplicationDbContext dbContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();

            var currentUserId = userManager.GetUserId(context.User);

            var currentUser = dbContext.Users.Find(currentUserId);

            if (CurrentUserIsTeamLeader(dbContext, currentUser, requirement.CountOFLeadingTeams))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.FromResult(0);
        }

        private bool CurrentUserIsTeamLeader(ApplicationDbContext dbContext, User currentUser, int count)
        {
            var isTeamLead = dbContext.Teams.Count(x => x.TeamLeader == currentUser) >= count;
            return isTeamLead;
        }
    }
}