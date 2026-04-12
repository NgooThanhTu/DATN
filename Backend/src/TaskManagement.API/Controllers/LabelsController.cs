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
    public class LabelsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LabelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("labels")]
        public async Task<IActionResult> GetByProject(Guid projectId)
        {
            var labels = await _context.Labels
                .AsNoTracking()
                .Where(l => l.ProjectId == projectId)
                .Select(l => new
                {
                    l.Id,
                    l.Name,
                    l.ColorCode,
                    l.Description,
                    IssueCount = l.IssueLabels.Count(),
                    l.CreatedAt
                })
                .OrderBy(l => l.Name)
                .ToListAsync();

            return Ok(new { statusCode = 200, message = "Success", data = labels });
        }

        [HttpPost("labels")]
        public async Task<IActionResult> Create(Guid projectId, [FromBody] CreateLabelRequest request)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null)
                return BadRequest(new { statusCode = 400, message = "Dự án không tồn tại." });

            var label = new Label
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                WorkspaceId = project.WorkspaceId,
                Name = request.Name,
                ColorCode = request.ColorCode ?? "#3b82f6",
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.Labels.Add(label);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByProject), new { projectId },
                new { statusCode = 201, message = "Tạo nhãn thành công.", data = new { label.Id, label.Name, label.ColorCode } });
        }

        [HttpPut("labels/{labelId}")]
        public async Task<IActionResult> Update(Guid projectId, Guid labelId, [FromBody] UpdateLabelRequest request)
        {
            var label = await _context.Labels.FirstOrDefaultAsync(l => l.Id == labelId && l.ProjectId == projectId);
            if (label == null)
                return NotFound(new { statusCode = 404, message = "Nhãn không tồn tại." });

            label.Name = request.Name ?? label.Name;
            label.ColorCode = request.ColorCode ?? label.ColorCode;
            label.Description = request.Description ?? label.Description;

            await _context.SaveChangesAsync();

            return Ok(new { statusCode = 200, message = "Cập nhật thành công." });
        }

        [HttpDelete("labels/{labelId}")]
        public async Task<IActionResult> Delete(Guid projectId, Guid labelId)
        {
            var label = await _context.Labels.FirstOrDefaultAsync(l => l.Id == labelId && l.ProjectId == projectId);
            if (label == null)
                return NotFound(new { statusCode = 404, message = "Nhãn không tồn tại." });

            _context.Labels.Remove(label);
            await _context.SaveChangesAsync();

            return Ok(new { statusCode = 200, message = "Xóa thành công." });
        }

        // === Issue-Label Assignment ===

        [HttpPost("tasks/{taskId}/labels")]
        public async Task<IActionResult> AssignLabel(Guid projectId, Guid taskId, [FromBody] AssignLabelRequest request)
        {
            var exists = await _context.IssueLabels.AnyAsync(il => il.WorkTaskId == taskId && il.LabelId == request.LabelId);
            if (exists)
                return BadRequest(new { statusCode = 400, message = "Nhãn đã được gán cho công việc này." });

            _context.IssueLabels.Add(new IssueLabel
            {
                WorkTaskId = taskId,
                LabelId = request.LabelId,
                AssignedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            return Ok(new { statusCode = 200, message = "Gán nhãn thành công." });
        }

        [HttpDelete("tasks/{taskId}/labels/{labelId}")]
        public async Task<IActionResult> RemoveLabel(Guid projectId, Guid taskId, Guid labelId)
        {
            var link = await _context.IssueLabels.FirstOrDefaultAsync(il => il.WorkTaskId == taskId && il.LabelId == labelId);
            if (link == null)
                return NotFound(new { statusCode = 404, message = "Nhãn chưa được gán." });

            _context.IssueLabels.Remove(link);
            await _context.SaveChangesAsync();

            return Ok(new { statusCode = 200, message = "Gỡ nhãn thành công." });
        }
    }

    public class CreateLabelRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? ColorCode { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateLabelRequest
    {
        public string? Name { get; set; }
        public string? ColorCode { get; set; }
        public string? Description { get; set; }
    }

    public class AssignLabelRequest
    {
        public Guid LabelId { get; set; }
    }
}
