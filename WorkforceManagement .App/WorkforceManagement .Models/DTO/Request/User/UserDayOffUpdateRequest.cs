using System.ComponentModel.DataAnnotations;

namespace WorkforceManagement.Models.DTO.Request.User
{
    public class UserDayOffUpdateRequest
    {
        [Required]
        public string UserId { get; set; }

        public int Paid{ get; set; }

        public int Unpaid { get; set; }

        public int SickLeave { get; set; }
    }
}
