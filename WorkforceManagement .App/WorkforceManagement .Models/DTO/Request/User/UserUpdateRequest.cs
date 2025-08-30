using System.ComponentModel.DataAnnotations;

namespace WorkforceManagement.Models.DTO.Request.User
{
    public class UserUpdateRequest
    {
        public string UserId { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int? TeamId { get; set; }
    }
}
