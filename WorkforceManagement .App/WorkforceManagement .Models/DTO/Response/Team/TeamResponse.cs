using System;

namespace WorkforceManagement.Models.DTO.Response.Team
{
    public class TeamResponse
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        public string TeamLeaderId { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? LastModifiedOn { get; set; }
    }
}
