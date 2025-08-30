using System.IO;

namespace WorkforceManagement.Common
{
    public static class GlobalConstants
    {
        // Roles
        public const string AdministratorRoleName = "Administrator";

        // Administrator credentials
        public const string AdminUsername = "admin";
        public const string AdminPassword = "adminpass";
        public const string AdminEmail = "admin@demo.com";

        // Mail settings
        public const string Host = "demo.com";
        public const int  Port = 25;

        // Limit for days off
        public const int PaidDaysOff = 20;
        public const int UnpaidDaysOff = 40;
        public const int SickDaysOff = 90;

        // Directory where mails will be send 
        public static string MailsDirectory = Directory.GetParent(Directory.GetCurrentDirectory()) + "\\Mails";
    }
}
