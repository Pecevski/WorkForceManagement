using Nager.Date;
using System;

namespace WorkforceManagement.Services.Interfaces
{
    public interface IDateService
    {
        public bool CheckDate(DateTime dateTime);

        public int CheckforWorkingDays(DateTime startDate, DateTime endDate);
    }
}