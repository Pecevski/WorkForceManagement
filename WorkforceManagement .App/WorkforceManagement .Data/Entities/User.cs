using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using WorkforceManagement.Data.Entities.Common;

namespace WorkforceManagement.Data.Entities
{
    public class User : IdentityUser, IBaseDeletableModel
    {
        public User()
        {
            Teams = new HashSet<Team>();
            TimeOffRequests = new HashSet<TimeOffRequest>();
            Approvals = new HashSet<Approval>();
        }

        public DateTime? DeletedOn { get; set; }

        public bool IsDeleted { get; set; }

        public int PaidDaysOff { get; set; }

        public int SickDaysOff { get; set; }

        public int UnpaidDaysOff { get; set; }

        public virtual ICollection<Team> Teams { get; set; }

        public virtual ICollection<TimeOffRequest> TimeOffRequests { get; set; }

        public virtual ICollection<Approval> Approvals { get; set; }
    }
}