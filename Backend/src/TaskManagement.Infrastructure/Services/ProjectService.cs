using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.Project;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        {
            // Tạm thời lấy toàn bộ project do hiện tại đang bật bypass đăng nhập.
            // Nếu có đăng nhập, ta sẽ WHERE theo ProjectMembers.Any(pm => pm.UserId == currentUserId)
            var projects = await _context.Projects
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
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
