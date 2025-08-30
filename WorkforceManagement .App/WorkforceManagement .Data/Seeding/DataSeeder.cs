using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkforceManagement.Data.Database;

namespace WorkforceManagement.Data.Seeding
{
    public class DataSeeder : ISeeder
    {
        private readonly ICollection<ISeeder> _seeders;

        public DataSeeder(ICollection<ISeeder> seeders)
        {
            _seeders = seeders;
        }

        public async Task SeedData(ApplicationDbContext applicationDbContext, IServiceProvider serviceProvider)
        {
            foreach (var seeder in _seeders)
            {
                await seeder.SeedData(applicationDbContext, serviceProvider);
            }
        }
    }
}
