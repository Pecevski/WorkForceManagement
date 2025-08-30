using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using WorkforceManagement.Data.Database;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Data.Entities.Enums;

namespace WorkforceManagement.UnitTests.Seeder
{
    public class DatabaseFixture : IDisposable
    {
        public DbContextOptions<ApplicationDbContext> options;

        public ApplicationDbContext DbContext { get; private set; }

        public DatabaseFixture()
        {
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "TestDataBase").Options;

            DbContext = new ApplicationDbContext(options);

            SeedDb();
        }

        private void SeedDb()
        {
            using (var context = new ApplicationDbContext(options))
            {
                var users = new List<User>
                {
                    new User { UserName = "TestAdmin"},
                    new User { UserName = "TestLeader1"},
                    new User { UserName = "TestLeader2"},
                    new User { UserName = "TestLeader3"},
                    new User { UserName = "TestUser1"},
                    new User { UserName = "TestUser2"},
                    new User { UserName = "TestUser3"}
                };

                var teams = new List<Team>()
                {
                    new Team { Id = 1, Title = "Team1", Description = "Team One", TeamLeader = users[1], Members = users.GetRange(4, 3) },
                    new Team { Id = 2, Title = "Team2", Description = "Team Two", TeamLeader = users[2], Members = users.GetRange(4, 3) },
                    new Team { Id = 3, Title = "Team3", Description = "Team Three", TeamLeader = users[3], Members = users.GetRange(4, 3) }
                };

                var timeOffRequests = new List<TimeOffRequest>()
                {
                    new TimeOffRequest { Id = 1, Reason = "Paid Holiday", Requester = users[4], Status = RequestStatus.Created, Type = RequestType.Paid },
                    new TimeOffRequest { Id = 2, Reason = "Unpaid Holiday", Requester = users[5], Status = RequestStatus.Created, Type = RequestType.Unpaid },
                    new TimeOffRequest { Id = 3, Reason = "SickLeave Holiday", Requester = users[6], Status = RequestStatus.Created, Type = RequestType.SickLeave },
                    new TimeOffRequest { Id = 4, Reason = "Paid Holiday", Requester = users[4], Status = RequestStatus.Rejected, Type = RequestType.Paid },
                    new TimeOffRequest { Id = 5, Reason = "Unpaid Holiday", Requester = users[5], Status = RequestStatus.Awaiting, Type = RequestType.Unpaid },
                    new TimeOffRequest { Id = 6, Reason = "SickLeave Holiday", Requester = users[6], Status = RequestStatus.Approved, Type = RequestType.SickLeave },
                    new TimeOffRequest { Id = 7, Reason = "Paid Holiday", Requester = users[4], Status = RequestStatus.Created, Type = RequestType.Paid, StartDate = new DateTime(2221, 1, 2), EndDate = new DateTime(2221, 1, 3)},
                    new TimeOffRequest { Id = 8, Reason = "Unpaid Holiday", Requester = users[5], Status = RequestStatus.Rejected, Type = RequestType.Unpaid },
                    new TimeOffRequest { Id = 9, Reason = "SickLeave Holiday", Requester = users[6], Status = RequestStatus.Created, Type = RequestType.SickLeave },
                };

                var approvals = new List<Approval>()
                {
                    new Approval { Id = 1, TimeOffRequest = timeOffRequests[7], Approver = users[1], IsApproved = false },
                    new Approval { Id = 2, TimeOffRequest = timeOffRequests[7], Approver = users[2], IsApproved = false },
                    new Approval { Id = 3, TimeOffRequest = timeOffRequests[7], Approver = users[3], IsApproved = false },
                    new Approval { Id = 4, TimeOffRequest = timeOffRequests[8], Approver = users[1], IsApproved = false },
                    new Approval { Id = 5, TimeOffRequest = timeOffRequests[8], Approver = users[2], IsApproved = false },
                    new Approval { Id = 6, TimeOffRequest = timeOffRequests[8], Approver = users[3], IsApproved = false }
                };

                timeOffRequests[6].Approvals = approvals.GetRange(0, 3);
                timeOffRequests[7].Approvals = approvals.GetRange(3, 3);

                context.AddRange(users);
                context.AddRange(teams);
                context.AddRange(timeOffRequests);
                context.AddRange(approvals);

                context.SaveChanges();
            }
        }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
        }
    }
}
