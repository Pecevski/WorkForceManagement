using System.ComponentModel.DataAnnotations;

namespace WorkforceManagement.Models.DTO.Request.Team
{
    public class TeamRequest
    {
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(150)]
        public string Description { get; set; }

        [Required]
        public string TeamLeaderId { get; set; }

    }
}
