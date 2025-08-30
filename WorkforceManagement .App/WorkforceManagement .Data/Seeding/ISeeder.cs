using System;
using System.Threading.Tasks;
using WorkforceManagement.Data.Database;

namespace WorkforceManagement.Data.Seeding
{
    public interface ISeeder
    {
        Task SeedData(ApplicationDbContext applicationDbContext, IServiceProvider serviceProvider);
    }
}
