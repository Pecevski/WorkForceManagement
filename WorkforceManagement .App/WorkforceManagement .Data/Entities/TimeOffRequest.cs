using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkforceManagement.Data.Entities.Common;
using WorkforceManagement.Data.Entities.Enums;

namespace WorkforceManagement.Data.Entities
{
    public class TimeOffRequest : BaseDeletableModel
    {
        public TimeOffRequest()
        {
            Approvals = new HashSet<Approval>();
        }

        public int Id { get; set; }

        public RequestType Type { get; set; }

        public RequestStatus Status { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime CreatedOn { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        public virtual User Requester { get; set; }

        public virtual ICollection<Approval> Approvals { get; set; }
    }
}
