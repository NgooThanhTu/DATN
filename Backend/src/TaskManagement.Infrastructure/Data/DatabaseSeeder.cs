using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskStatus = TaskManagement.Domain.Entities.TaskStatus;

namespace TaskManagement.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Seed Roles
            if (!await context.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role { Id = Guid.Parse("22222222-0000-0000-0000-000000000001"), Name = "PM", Description = "Project Manager" },
                    new Role { Id = Guid.Parse("22222222-0000-0000-0000-000000000002"), Name = "PO", Description = "Product Owner" },
                    new Role { Id = Guid.Parse("22222222-0000-0000-0000-000000000003"), Name = "Developer", Description = "Developer" },
                    new Role { Id = Guid.Parse("22222222-0000-0000-0000-000000000008"), Name = "System Admin", Description = "System Admin" }
                };
                context.Roles.AddRange(roles);
                await context.SaveChangesAsync();
            }
        }
    }
}
