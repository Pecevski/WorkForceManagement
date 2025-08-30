using WorkforceManagement.Data.Entities;
using WorkforceManagement.Data.Entities.Enums;

namespace WorkforceManagement.Services.Interfaces
{
    public interface IMailService
    {
        bool SendEmail(User sender, string toAddress, TimeOffRequest requestToBeSend, EmailType type);
    }
}