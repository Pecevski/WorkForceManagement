namespace WorkforceManagement.Models.DTO.Response.User
{
    public class UserResponse : DayOffResponse
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }
    }
}