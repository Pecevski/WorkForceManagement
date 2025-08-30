using Microsoft.AspNetCore.Authorization;

namespace WorkforceManagement.WebApi.Policies
{
    public class TeamLeaderRequirement : IAuthorizationRequirement
    {
        public TeamLeaderRequirement(int countOFLeadingTeams, string adminRole)
        {
            CountOFLeadingTeams = countOFLeadingTeams;
            AdminRole = adminRole;
        }

        public int CountOFLeadingTeams { get; set; }

        public string AdminRole { get; set; }
    }
}