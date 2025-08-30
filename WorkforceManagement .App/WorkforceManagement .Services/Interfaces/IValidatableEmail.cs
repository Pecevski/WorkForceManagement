using WorkforceManagement.Data.Entities;

namespace WorkforceManagement.Services.Interfaces
{
    public interface IValidatableEmail
    {
        bool IsValidUser(User user);

        bool IsValidRequest(TimeOffRequest request);

        bool IsValidEmail(string address);
    }
}