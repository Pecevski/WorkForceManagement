using System;
using System.Collections.Generic;

namespace WorkforceManagement.Models.DTO.Response
{
    public class TimeOffRequestResponseDTO
    {
        public int TimeOffRequestId { get; set; }

        public string Requester { get; set; }

        public string RequestType { get; set; }

        public string Reason { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime CreatedOn { get; set; }

        public string RequestStatus { get; set; }

        public List<string> Approvals { get; set; }
    }
}