using Nager.Date;
using Nager.Date.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkforceManagement.Services.Interfaces;

namespace WorkforceManagement.Services.Services
{
    public class DateService : IDateService
    {
        public bool CheckDate(DateTime dateTime)
        {
            DateTime date = new DateTime(2021, 1, 1);

            var updateDate = date.ToString("MM dd");

            if (dateTime.ToString("MM dd") == updateDate)
            {
                return true;
            }

            return false;
        }

        public int CheckforWorkingDays(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> holidays = CheckForHolidays(startDate, endDate, CountryCode.BG);
            int count = 0;
            while (startDate <= endDate)
            {
                if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    if (!holidays.Any(x => x.Date == startDate.Date))
                    {
                        count++;
                    }
                }
                startDate = startDate.AddDays(1);
            }
            return count;
        }

        // Bulgaria CountryCode = BG
        // Macedonia CountryCode = MK
        private List<PublicHoliday> CheckForHolidays(DateTime startDate, DateTime endDate, CountryCode countryCode)
        {
            List<PublicHoliday> publicHolidays = DateSystem.GetPublicHolidays(startDate, endDate, countryCode).ToList();

            return publicHolidays;
        }
    }
}
