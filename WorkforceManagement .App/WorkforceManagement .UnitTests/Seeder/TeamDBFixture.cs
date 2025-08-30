using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkforceManagement.Data.Database;
using WorkforceManagement.Data.Entities;

namespace WorkforceManagement.UnitTests.Seeder
{
    public class TeamDBFixture : IDisposable
    {
        public DbContextOptions<ApplicationDbContext> options;

        public ApplicationDbContext DbContext { get; private set; }

        public TeamDBFixture()
        {
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "TestDB").Options;

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
                    new Team { Id = 1, Title = "TeamsOne", Description = "First Team", TeamLeader = users[1], Members = users },
                    new Team { Id = 2, Title = "TeamTwo", Description = "Second Team", TeamLeader = users[2]},
                    new Team { Id = 3, Title = "TeamThree", Description = "Third Team", TeamLeader = users[3]}
                };

                context.AddRange(users);
                context.AddRange(teams);

                context.SaveChanges();
            }
        }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
        }
    }
}
