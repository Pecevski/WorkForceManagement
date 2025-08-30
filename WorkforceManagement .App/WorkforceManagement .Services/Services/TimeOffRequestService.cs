using Microsoft.EntityFrameworkCore;
using Nager.Date;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkforceManagement.Common;
using WorkforceManagement.Data.Database;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Data.Entities.Enums;
using WorkforceManagement.Models.DTO.Request;
using WorkforceManagement.Models.DTO.Response;
using WorkforceManagement.Models.Mapper;
using WorkforceManagement.Services.Contracts;
using WorkforceManagement.Services.Interfaces;

namespace WorkforceManagement.Services.Services
{
    public class TimeOffRequestService : ITimeOffRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IMailService _mailService;
        private readonly IDateService _dateService;

        public TimeOffRequestService(ApplicationDbContext context,
            IUserService userService,
            IMailService mailService,
            IDateService dateService)
        {
            _context = context;
            _userService = userService;
            _mailService = mailService;
            _dateService = dateService;
        }

        public async Task<TimeOffRequestResponseDTO> CreateTimeOffRequest(TimeOffRequestRequestDTO timeOffRequestRequestDTO, User currentUser)
        {
            User requester = await _userService.GetUserById(timeOffRequestRequestDTO.RequesterUserId);
            if (requester == null)
            {
                return null;
            }

            RequestType requestType;

            if (!Enum.TryParse(timeOffRequestRequestDTO.RequestType, true, out requestType))
            {
                return null;
            }

            if (await CurrentUserHasAuthorization(requester, currentUser))
            {
                HashSet<User> emailRecievers = new HashSet<User>();
                var newTimeOffRequest = CreateNewTimeOffRequest(timeOffRequestRequestDTO, requestType, requester);
                var requesterTeamLeads = GetRequesterTeamLeads(requester);

                //Auto approve and Send Emails for SickLeave Requests
                if (requestType == RequestType.SickLeave)
                {
                    newTimeOffRequest.Status = RequestStatus.Approved;
                }
                // Auto approve and Send Email to Requester when he is TeamLead only and has no other leads
                else if (requesterTeamLeads.Count == 0)
                {
                    newTimeOffRequest.Status = RequestStatus.Approved;
                }

                // Create approvals for each teamlead

                foreach (var teamLead in requesterTeamLeads)
                {
                    emailRecievers.Add(teamLead);
                    var newApproval = CreateNewApproval(teamLead);

                    // if the current teamLead is out of office
                    if (await TeamLeadIsOutOfOffice(teamLead, newTimeOffRequest))
                    {
                        newApproval.IsApproved = true;
                        SetTimeOffRequestStatus(newTimeOffRequest);
                    }

                    newTimeOffRequest.Approvals.Add(newApproval);
                }

                emailRecievers.UnionWith((await GetAllTeamMates(requester)).ToList());

                SendMailRange(requester, emailRecievers, newTimeOffRequest);

                if (!DeductRequestedWorkingDays(newTimeOffRequest))
                {
                    return null;
                }

                await _context.TimeOffRequests.AddAsync(newTimeOffRequest);
                await _context.SaveChangesAsync();

                return TimeOffRequestMapper.MapTimeOffRequest(newTimeOffRequest);
            }

            return null;
        }

        // ToDo delete all aproves aswell
        public async Task<bool> DeleteTimeOffRequest(int timeOffRequestId, User currentUser)
        {
            var timeOffRequestToDelete = await _context.TimeOffRequests.FirstOrDefaultAsync(tor => tor.Id == timeOffRequestId);

            if (timeOffRequestToDelete == null)
            {
                return false;
            }

            if (await CurrentUserHasAuthorization(timeOffRequestToDelete.Requester, currentUser))
            {
                timeOffRequestToDelete.IsDeleted = true;
                timeOffRequestToDelete.DeletedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<TimeOffRequestResponseDTO> GetTimeOffRequest(int timeOffRequestId, User currentUser)
        {
            var timeOffRequestFromDB = await _context.TimeOffRequests.FirstOrDefaultAsync(tor => tor.Id == timeOffRequestId);

            if (timeOffRequestFromDB == null)
            {
                return null;
            }

            if (await CurrentUserHasAuthorization(timeOffRequestFromDB.Requester, currentUser))
            {
                return TimeOffRequestMapper.MapTimeOffRequest(timeOffRequestFromDB);
            }

            return null;
        }

        public async Task<TimeOffRequestResponseDTO> UpdateTimeOffRequest(int timeOffRequestId, TimeOffRequestRequestDTO timeOffRequestRequestDTO, User currentUser)
        {
            var timeOffRequestToEdit = await _context.TimeOffRequests.FirstOrDefaultAsync(tor => tor.Id == timeOffRequestId);

            if (timeOffRequestToEdit == null ||
                timeOffRequestToEdit.Status == RequestStatus.Approved ||
                timeOffRequestToEdit.Status == RequestStatus.Rejected)
            {
                return null;
            }

            if (await CurrentUserHasAuthorization(timeOffRequestToEdit.Requester, currentUser))
            {
                RequestType newRequestType;

                if (!Enum.TryParse(timeOffRequestRequestDTO.RequestType, true, out newRequestType))
                {
                    return null;
                }

                HashSet<User> emailRecievers = new HashSet<User>();

                //Auto approve and Send Emails for SickLeave Requests
                if (newRequestType == RequestType.SickLeave)
                {
                    timeOffRequestToEdit.Status = RequestStatus.Approved;
                }
                else
                {
                    var approvals = timeOffRequestToEdit.Approvals.ToList();

                    for (int i = 0; i < approvals.Count; i++)
                    {
                        approvals[i].IsApproved = false;
                    }
                }

                timeOffRequestToEdit.Type = newRequestType;
                timeOffRequestToEdit.Reason = timeOffRequestRequestDTO.Reason;
                timeOffRequestToEdit.StartDate = timeOffRequestRequestDTO.StartDate;
                timeOffRequestToEdit.EndDate = timeOffRequestRequestDTO.EndDate;

                // Gets all team mates, team leaders and himself
                emailRecievers.UnionWith(timeOffRequestToEdit.Approvals.Select(a => a.Approver).ToList());
                emailRecievers.UnionWith((await GetAllTeamMates(timeOffRequestToEdit.Requester)).ToList());
                emailRecievers.Add(timeOffRequestToEdit.Requester);

                SendMailRange(timeOffRequestToEdit.Requester, emailRecievers, timeOffRequestToEdit);

                _context.TimeOffRequests.Update(timeOffRequestToEdit);
                await _context.SaveChangesAsync();

                return TimeOffRequestMapper.MapTimeOffRequest(timeOffRequestToEdit);
            }

            return null;
        }

        public async Task<TimeOffRequestResponseDTO> ApproveTimeOffRequest(int timeOffRequestId, User currentUser, bool isApproved)
        {
            var timeOffRequest = await _context.TimeOffRequests.FirstOrDefaultAsync(a => a.Id == timeOffRequestId);

            if (timeOffRequest == null)
            {
                return null;
            }

            var approval = timeOffRequest.Approvals.FirstOrDefault(a => a.Approver == currentUser);

            if (approval == null)
            {
                return null;
            }

            // If it is already rejected
            if (approval.TimeOffRequest.Status == RequestStatus.Rejected)
            {
                return null;
            }

            approval.IsApproved = isApproved;

            if (isApproved == false)
            {
                approval.TimeOffRequest.Status = RequestStatus.Rejected;

                // email for rejected request
                _mailService.SendEmail(approval.Approver, approval.TimeOffRequest.Requester.Email, approval.TimeOffRequest, EmailType.Rejected);

                ReturnDaysToUser(timeOffRequest);
            }
            else
            {
                if (approval.TimeOffRequest.Approvals.All(a => a.IsApproved == true))
                {
                    approval.TimeOffRequest.Status = RequestStatus.Approved;

                    // email for aprroved request
                    _mailService.SendEmail(approval.Approver, approval.TimeOffRequest.Requester.Email, approval.TimeOffRequest, EmailType.Approved);
                }
                else if (approval.TimeOffRequest.Status == RequestStatus.Created && approval.TimeOffRequest.Approvals.Count > 1)
                {
                    approval.TimeOffRequest.Status = RequestStatus.Awaiting;
                }
            }

            _context.TimeOffRequests.Update(timeOffRequest);
            await _context.SaveChangesAsync();

            return TimeOffRequestMapper.MapTimeOffRequest(approval.TimeOffRequest);
        }

        public bool SendMailRange(User sender, ICollection<User> receivers, TimeOffRequest timeOffRequest)
        {
            bool allEmailsSended = true;

            foreach (var receiver in receivers)
            {
                bool emailSended = _mailService.SendEmail(sender, receiver.Email, timeOffRequest, EmailType.Default);
                if (!emailSended)
                {
                    allEmailsSended = false;
                }
            }

            return allEmailsSended;
        }

        public async Task<HashSet<User>> GetAllTeamMates(User requester)
        {
            var requesterTeams = await _context.Teams
                .Where(t => t.Members.Contains(requester)
                ).ToListAsync();

            var requesterTeamMates = new HashSet<User>();

            foreach (var team in requesterTeams)
            {
                foreach (var user in team.Members)
                {
                    if (user != requester)
                    {
                        requesterTeamMates.Add(user);
                    }
                }
            }

            return requesterTeamMates;
        }

        public async Task<bool> CurrentUserHasAuthorization(User requester, User currentUser)
        {
            if (requester.Id == currentUser.Id || await _userService.UserIsAdmin(currentUser))
            {
                return true;
            }

            return false;
        }

        public async Task<bool> TeamLeadIsOutOfOffice(User teamLead, TimeOffRequest newTimeOffRequest)
        {
            var outOffOffice = await _context.TimeOffRequests
                .Where(tor => tor.Requester == teamLead &&
                tor.Status == RequestStatus.Approved &&
                tor.StartDate <= newTimeOffRequest.CreatedOn &&
                tor.EndDate > newTimeOffRequest.CreatedOn &&
                tor.StartDate <= newTimeOffRequest.StartDate &&
                tor.EndDate > newTimeOffRequest.StartDate)
                .FirstOrDefaultAsync();

            if (outOffOffice != null)
            {
                return true;
            }

            return false;
        }

        public HashSet<User> GetRequesterTeamLeads(User requester)
        {
            var teamLeads = _context.Teams
                   .Where(t => t.Members.Contains(requester))
                   .Select(t => t.TeamLeader)
                   .ToHashSet();

            if (teamLeads.Contains(requester))
            {
                teamLeads.Remove(requester);
            }

            return teamLeads;
        }

        public TimeOffRequest CreateNewTimeOffRequest(TimeOffRequestRequestDTO timeOffRequestRequestDTO, RequestType requestType, User requester)
        {
            return new TimeOffRequest()
            {
                CreatedOn = DateTime.UtcNow,
                Reason = timeOffRequestRequestDTO.Reason,
                Type = requestType,
                StartDate = timeOffRequestRequestDTO.StartDate,
                EndDate = timeOffRequestRequestDTO.EndDate,
                Status = RequestStatus.Created,
                Requester = requester
            };
        }

        public void SetTimeOffRequestStatus(TimeOffRequest newTimeOffRequest)
        {
            if (newTimeOffRequest.Approvals.All(a => a.IsApproved == true))
            {
                newTimeOffRequest.Status = RequestStatus.Approved;
            }
            else
            {
                newTimeOffRequest.Status = RequestStatus.Awaiting;
            }
        }

        public Approval CreateNewApproval(User teamLead)
        {
            var newApproval = new Approval();
            newApproval.Approver = teamLead;

            return newApproval;
        }

        public async Task SetDaysOffForNewYear()
        {
            var listUsers = await _context.Users.ToListAsync();

            var todayDate = DateTime.UtcNow;
            var checkDate = _dateService.CheckDate(todayDate);

            if (checkDate)
            {
                foreach (var user in listUsers)
                {
                    user.PaidDaysOff += GlobalConstants.PaidDaysOff;
                    user.SickDaysOff = GlobalConstants.SickDaysOff;
                    user.UnpaidDaysOff = GlobalConstants.UnpaidDaysOff;
                }
                _context.UpdateRange(listUsers);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteTimeOffRequestAfterSixMonths()
        {
            var date = DateTime.UtcNow.AddMonths(-6);

            var listUsers = await _context.Users.IgnoreQueryFilters().Where(u => u.DeletedOn > date).ToListAsync();

            foreach (var user in listUsers)
            {
                foreach (var request in user.TimeOffRequests)
                {
                    request.IsDeleted = true;
                    request.DeletedOn = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
        }

        public IList<TimeOffRequest> GetAllPendingTimeOffRequests()
        {
            return _context.TimeOffRequests
                   .Where(tor => tor.Status == RequestStatus.Awaiting || tor.Status == RequestStatus.Created)
                   .ToList();
        }

        private bool DeductRequestedWorkingDays(TimeOffRequest newTimeOffRequest)
        {
            User requester = newTimeOffRequest.Requester;

            if (requester == null)
            {
                return false;
            }

            var requestedWorkingDaysOff = _dateService.CheckforWorkingDays(newTimeOffRequest.StartDate, newTimeOffRequest.EndDate);

            var requestTypeIs = newTimeOffRequest.Type;

            switch (requestTypeIs)
            {
                case RequestType.Paid:
                    if (requester.PaidDaysOff >= requestedWorkingDaysOff)
                    {
                        requester.PaidDaysOff -= requestedWorkingDaysOff;
                        return true;
                    }
                    break;
                case RequestType.Unpaid:
                    if (requester.UnpaidDaysOff >= requestedWorkingDaysOff)
                    {
                        requester.UnpaidDaysOff -= requestedWorkingDaysOff;
                        return true;
                    }
                    break;
                case RequestType.SickLeave:
                    if (requester.SickDaysOff >= requestedWorkingDaysOff)
                    {
                        requester.SickDaysOff -= requestedWorkingDaysOff;
                        return true;
                    }
                    break;
                default:
                    break;
            }

            return false;
        }

        private void ReturnDaysToUser(TimeOffRequest timeOffRequest)
        {
            User requester = timeOffRequest.Requester;

            if (requester == null)
            {
                return;
            }

            var requestedWorkingDaysOff = _dateService.CheckforWorkingDays(timeOffRequest.StartDate, timeOffRequest.EndDate);

            var requestTypeIs = timeOffRequest.Type;

            switch (requestTypeIs)
            {
                case RequestType.Paid:
                    requester.PaidDaysOff += requestedWorkingDaysOff;
                    break;
                case RequestType.Unpaid:
                    requester.UnpaidDaysOff += requestedWorkingDaysOff;
                    break;
                case RequestType.SickLeave:
                    requester.SickDaysOff += requestedWorkingDaysOff;
                    break;
                default:
                    break;
            }
        }
    }
}