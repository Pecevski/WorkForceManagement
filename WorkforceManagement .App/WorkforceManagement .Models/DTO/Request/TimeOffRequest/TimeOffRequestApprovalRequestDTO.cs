using System.ComponentModel.DataAnnotations;

namespace WorkforceManagement.Models.DTO.Request.TimeOffRequest
{
    public class TimeOffRequestApprovalRequestDTO
    {
        [Required]
        public bool IsApproved { get; set; }
    }
}
