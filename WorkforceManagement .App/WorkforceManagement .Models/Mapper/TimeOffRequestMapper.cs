using System.Collections.Generic;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Models.DTO.Response;

namespace WorkforceManagement.Models.Mapper
{
    public static class TimeOffRequestMapper
    {
        public static TimeOffRequestResponseDTO MapTimeOffRequest(TimeOffRequest timeOffRequest)
        {
            var approvals = new List<string>();

            foreach (var approval in timeOffRequest.Approvals)
            {
                var newApproval = approval.Approver.UserName + "    approved: " + approval.IsApproved;
                approvals.Add(newApproval);
            }

            return new TimeOffRequestResponseDTO()
            {
                TimeOffRequestId = timeOffRequest.Id,
                RequestType = timeOffRequest.Type.ToString(),
                Requester = timeOffRequest.Requester.UserName,
                Reason = timeOffRequest.Reason,
                StartDate = timeOffRequest.StartDate.Date,
                EndDate = timeOffRequest.EndDate.Date,
                CreatedOn = timeOffRequest.CreatedOn.Date,
                RequestStatus = timeOffRequest.Status.ToString(),
                Approvals = approvals
            };
        }
    }
}
