using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedMockDataAsync(ApplicationDbContext context)
        {
            var defaultProject = await context.Projects.FirstOrDefaultAsync(p => p.Name == "Demo Plane Project");
            if (defaultProject != null)
            {
                var hasTasks = await context.WorkTasks.AnyAsync(t => t.ProjectId == defaultProject.Id);
                if (hasTasks) return;
            }

            var workspaceId = Guid.NewGuid();
            var ownerId = Guid.Parse("11111111-0000-0000-0000-000000000001");

            var ownerExists = await context.Users.AnyAsync(u => u.Id == ownerId);
            if (!ownerExists)
            {
                ownerId = Guid.NewGuid();
                context.Users.Add(new User
                {
                    Id = ownerId,
                    FullName = "Admin (Seeded)",
                    Email = "admin@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    IsActive = true
                });
            }

            var testUserId = Guid.NewGuid();
            var testUserExists = await context.Users.AnyAsync(u => u.Email == "test@example.com");
            if (!testUserExists)
            {
                context.Users.Add(new User
                {
                    Id = testUserId,
                    FullName = "Test User",
                    Email = "test@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                    IsActive = true
                });
            }

            var existingWorkspace = await context.Workspaces.FirstOrDefaultAsync(w => w.Slug == "cybwf");
            if (existingWorkspace == null)
            {
                var workspace = new Workspace
                {
                    Id = workspaceId,
                    Name = "Cybwf Workspace",
                    Slug = "cybwf",
                    OwnerId = ownerId,
                    Timezone = "Asia/Ho_Chi_Minh",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                context.Workspaces.Add(workspace);

                context.WorkspaceMembers.Add(new WorkspaceMember
                {
                    WorkspaceId = workspaceId,
                    UserId = ownerId,
                    WorkspaceRole = "OWNER",
                    JoinedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }
            else
            {
                workspaceId = existingWorkspace.Id;
            }

            var projectId = Guid.NewGuid();
            var project = new Project
            {
                Id = projectId,
                WorkspaceId = workspaceId,
                Identifier = "CYBWF",
                IssueSequence = 10,
                NetworkType = "Public",
                Name = "Demo Plane Project",
                Description = "A sample project to test the Plane-like UI",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Projects.Add(project);

            context.ProjectMembers.Add(new ProjectMember
            {
                ProjectId = projectId,
                UserId = ownerId,
                ProjectRole = "PM",
                JoinedAt = DateTime.UtcNow,
                Status = true
            });

            if (!testUserExists)
            {
                context.ProjectMembers.Add(new ProjectMember
                {
                    ProjectId = projectId,
                    UserId = testUserId,
                    ProjectRole = "PM",
                    JoinedAt = DateTime.UtcNow,
                    Status = true
                });

                context.WorkspaceMembers.Add(new WorkspaceMember
                {
                    WorkspaceId = workspaceId,
                    UserId = testUserId,
                    WorkspaceRole = "MEMBER",
                    JoinedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }

            var statusBacklog = new TaskManagement.Domain.Entities.TaskStatus { Id = Guid.NewGuid(), ProjectId = projectId, Name = "BACKLOG", Position = 0 };
            var statusTodo = new TaskManagement.Domain.Entities.TaskStatus { Id = Guid.NewGuid(), ProjectId = projectId, Name = "TO DO", Position = 1 };
            var statusProgress = new TaskManagement.Domain.Entities.TaskStatus { Id = Guid.NewGuid(), ProjectId = projectId, Name = "IN PROGRESS", Position = 2 };
            var statusReview = new TaskManagement.Domain.Entities.TaskStatus { Id = Guid.NewGuid(), ProjectId = projectId, Name = "IN REVIEW", Position = 3 };
            var statusDone = new TaskManagement.Domain.Entities.TaskStatus { Id = Guid.NewGuid(), ProjectId = projectId, Name = "DONE", Position = 4 };

            context.TaskStatuses.AddRange(statusBacklog, statusTodo, statusProgress, statusReview, statusDone);

            var typeTask = new TaskType { Id = Guid.NewGuid(), ProjectId = projectId, Name = "Task", ColorCode = "#3b82f6" };
            var typeBug = new TaskType { Id = Guid.NewGuid(), ProjectId = projectId, Name = "Bug", ColorCode = "#ef4444" };
            context.TaskTypes.AddRange(typeTask, typeBug);

            var tasks = new[]
            {
                new WorkTask
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    WorkspaceId = workspaceId,
                    Title = "Nghien cuu kien truc Plane",
                    Description = "Phan tich schema database",
                    Priority = 3,
                    TaskTypeId = typeTask.Id,
                    TaskStatusId = statusDone.Id,
                    ReporterId = ownerId,
                    SortOrder = 10000,
                    SequenceId = "CYBWF-1",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkTask
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    WorkspaceId = workspaceId,
                    Title = "Thiet ke giao dien Dark Mode",
                    Description = "#0D0D0D background",
                    Priority = 2,
                    TaskTypeId = typeTask.Id,
                    TaskStatusId = statusProgress.Id,
                    ReporterId = ownerId,
                    SortOrder = 20000,
                    SequenceId = "CYBWF-2",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkTask
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    WorkspaceId = workspaceId,
                    Title = "Viet API Kanban Reorder",
                    Description = "Testing Lexorank",
                    Priority = 1,
                    TaskTypeId = typeBug.Id,
                    TaskStatusId = statusReview.Id,
                    ReporterId = ownerId,
                    SortOrder = 30000,
                    SequenceId = "CYBWF-3",
                    CreatedAt = DateTime.UtcNow
                },
                new WorkTask
                {
                    Id = Guid.NewGuid(),
                    ProjectId = projectId,
                    WorkspaceId = workspaceId,
                    Title = "Len ke hoach Sprint 1",
                    Description = "Chuan bi backlog",
                    Priority = 4,
                    TaskTypeId = typeTask.Id,
                    TaskStatusId = statusTodo.Id,
                    ReporterId = ownerId,
                    SortOrder = 40000,
                    SequenceId = "CYBWF-4",
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.WorkTasks.AddRange(tasks);
            await context.SaveChangesAsync();
        }
    }
}
