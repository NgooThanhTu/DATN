using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Domain.Entities;
using System.Security.Claims;
using TaskManagement.Domain.Constants;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
                return Unauthorized(new { message = "Invalid token" });

            var projects = await _context.Projects
                .Where(p => p.CreatorId == userId || p.ProjectMembers.Any(pm => pm.UserId == userId))
                .Select(p => new {
                    id = p.Id,
                    name = p.Name,
                    description = p.Description,
                    type = "Phần mềm",
                    icon = "fa-solid fa-layer-group",
                    color = "#579dff"
                })
                .ToListAsync();

            if (!projects.Any())
            {
                // Auto create a default project to unlock the UI for new users
                var p = new Project
                {
                    Id = Guid.NewGuid(),
                    Name = "Không gian làm việc cá nhân",
                    Description = "Dự án mặc định",
                    CreatorId = userId,
                    StartDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Status = true
                };
                _context.Projects.Add(p);
                _context.ProjectMembers.Add(new ProjectMember
                {
                    ProjectId = p.Id,
                    UserId = userId,
                    ProjectRole = ProjectRoles.PM,
                    JoinedAt = DateTime.UtcNow,
                    Status = true
                });
                await _context.SaveChangesAsync();
                
                projects.Add(new {
                    id = p.Id,
                    name = p.Name,
                    description = p.Description,
                    type = "Phần mềm",
                    icon = "fa-solid fa-layer-group",
                    color = "#579dff"
                });
            }

            return Ok(projects);
        }
    }
}
