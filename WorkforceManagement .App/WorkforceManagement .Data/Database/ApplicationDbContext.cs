using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WorkforceManagement.Data.Entities;
using WorkforceManagement.Data.Entities.Common;

namespace WorkforceManagement.Data.Database
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        private static readonly MethodInfo SetIsDeletedQueryFilterMethod =
            typeof(ApplicationDbContext).GetMethod(
                nameof(SetIsDeletedQueryFilter),
                BindingFlags.NonPublic | BindingFlags.Static);

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Approval> Approvals { get; set; }

        public DbSet<TimeOffRequest> TimeOffRequests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set global query filter for not deleted entities only
            List<IMutableEntityType> entityTypes = modelBuilder.Model.GetEntityTypes().ToList();
            IEnumerable<IMutableEntityType> deletableEntityTypes = entityTypes
                .Where(et => et.ClrType != null && typeof(IBaseDeletableModel).IsAssignableFrom(et.ClrType));
            foreach (var deletableEntityType in deletableEntityTypes)
            {
                var method = SetIsDeletedQueryFilterMethod.MakeGenericMethod(deletableEntityType.ClrType);
                method.Invoke(null, new object[] { modelBuilder });
            }

            // User
            modelBuilder.Entity<User>()
                .HasMany(u => u.Teams)
                .WithMany(t => t.Members);

            modelBuilder.Entity<User>()
               .HasIndex(u => u.Email)
               .IsUnique();

            // Team
            modelBuilder.Entity<Team>()
                 .HasOne(t => t.TeamLeader)
                 .WithMany()
                 .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Team>()
                .Property(u => u.CreatedOn)
                .HasDefaultValueSql("getdate()");

            // Approval
            modelBuilder.Entity<Approval>()
                .HasOne(a => a.Approver)
                .WithMany(u => u.Approvals)
                .OnDelete(DeleteBehavior.ClientCascade);
        }

        private static void SetIsDeletedQueryFilter<T>(ModelBuilder builder)
            where T : class, IBaseDeletableModel
        {
            builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
