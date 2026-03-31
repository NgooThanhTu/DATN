using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.Project;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProjectService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        {
            var userIdString = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return new List<ProjectDto>(); // Not mapped / no token => no projects
            }

            var projects = await _context.Projects
                .AsNoTracking()
                .Where(p => !p.IsDeleted && p.ProjectMembers.Any(pm => pm.UserId == userId && pm.Status))
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Type = "Dự án phần mềm", // Có thể mở rộng schema để lưu column này 
                    Icon = "fa-brands fa-jira" // Hardcode tạm cho trùng với design bản mock
                })
                .ToListAsync();

            return projects;
        }
    }
}
