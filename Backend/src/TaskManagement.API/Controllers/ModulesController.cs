using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Domain.Entities;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId}")]
    [Authorize]
    public class ModulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ModulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("modules")]
        public async Task<IActionResult> GetByProject(Guid projectId)
        {
            var modules = await _context.Modules
                .AsNoTracking()
                .Where(m => m.ProjectId == projectId)
                .Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.Description,
                    m.Status,
                    m.StartDate,
                    m.TargetDate,
                    LeadName = m.Lead != null ? m.Lead.FullName : null,
                    IssueCount = m.IssueModules.Count(),
                    m.CreatedAt,
                    m.UpdatedAt
                })
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return Ok(new { statusCode = 200, message = "Success", data = modules });
        }

        [HttpPost("modules")]
        public async Task<IActionResult> Create(Guid projectId, [FromBody] CreateModuleRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid parsedUserId))
                return Unauthorized(new { statusCode = 401, message = "Vui lòng đăng nhập." });

            var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists)
                return BadRequest(new { statusCode = 400, message = "Dự án không tồn tại." });

            var module = new Module
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                TargetDate = request.TargetDate,
                Status = "Backlog",
                LeadId = parsedUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Modules.Add(module);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByProject), new { projectId },
                new { statusCode = 201, message = "Tạo Module thành công.", data = new { module.Id, module.Name } });
        }

        [HttpPut("modules/{moduleId}")]
        public async Task<IActionResult> Update(Guid projectId, Guid moduleId, [FromBody] UpdateModuleRequest request)
        {
            var module = await _context.Modules.FirstOrDefaultAsync(m => m.Id == moduleId && m.ProjectId == projectId);
            if (module == null)
                return NotFound(new { statusCode = 404, message = "Module không tồn tại." });

            module.Name = request.Name ?? module.Name;
            module.Description = request.Description ?? module.Description;
            module.Status = request.Status ?? module.Status;
            module.StartDate = request.StartDate ?? module.StartDate;
            module.TargetDate = request.TargetDate ?? module.TargetDate;
            module.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { statusCode = 200, message = "Cập nhật thành công." });
        }

        [HttpDelete("modules/{moduleId}")]
        public async Task<IActionResult> Delete(Guid projectId, Guid moduleId)
        {
            var module = await _context.Modules.FirstOrDefaultAsync(m => m.Id == moduleId && m.ProjectId == projectId);
            if (module == null)
                return NotFound(new { statusCode = 404, message = "Module không tồn tại." });

            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();

            return Ok(new { statusCode = 200, message = "Xóa thành công." });
        }
    }

    public class CreateModuleRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? TargetDate { get; set; }
    }

    public class UpdateModuleRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? TargetDate { get; set; }
    }
}
