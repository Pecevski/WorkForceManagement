using System.Collections.Generic;
using System.Threading.Tasks;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Data.Entities.Enums;
using WorkforceManagement.Models.DTO.Request;
using WorkforceManagement.Models.DTO.Response;

namespace WorkforceManagement.Services.Contracts
{
    public interface ITimeOffRequestService
    {
        Task<TimeOffRequestResponseDTO> CreateTimeOffRequest(TimeOffRequestRequestDTO timeOffRequestRequestDTO, User currentUser);

        Task<TimeOffRequestResponseDTO> UpdateTimeOffRequest(int timeOffRequestId, TimeOffRequestRequestDTO timeOffRequestRequestDTO, User currentUser);

        Task<TimeOffRequestResponseDTO> ApproveTimeOffRequest(int approvalId, User currentUser, bool IsApproved);

        Task<TimeOffRequestResponseDTO> GetTimeOffRequest(int timeOffRequestId, User currentUser);

        Task<bool> DeleteTimeOffRequest(int timeOffRequestId, User currentUser);

        Task<bool> CurrentUserHasAuthorization(User requester, User currentUser);

        TimeOffRequest CreateNewTimeOffRequest(TimeOffRequestRequestDTO timeOffRequestRequestDTO, RequestType requestType, User requester);

        Task<bool> TeamLeadIsOutOfOffice(User teamLead, TimeOffRequest newTimeOffRequest);

        Approval CreateNewApproval(User teamLead);

        void SetTimeOffRequestStatus(TimeOffRequest newTimeOffRequest);

        HashSet<User> GetRequesterTeamLeads(User requester);

        bool SendMailRange(User sender, ICollection<User> receivers, TimeOffRequest timeOffRequest);

        Task SetDaysOffForNewYear();
        Task DeleteTimeOffRequestAfterSixMonths();

        IList<TimeOffRequest> GetAllPendingTimeOffRequests();
    }
}