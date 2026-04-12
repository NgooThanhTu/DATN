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
    public class PagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("pages")]
        public async Task<IActionResult> GetByProject(Guid projectId)
        {
            var pages = await _context.Pages
                .AsNoTracking()
                .Where(p => p.ProjectId == projectId && !p.IsArchived)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.SortOrder,
                    p.IsLocked,
                    CreatedByName = p.CreatedBy.FullName,
                    p.CreatedAt,
                    p.UpdatedAt
                })
                .OrderBy(p => p.SortOrder)
                .ToListAsync();

            return Ok(new { statusCode = 200, message = "Success", data = pages });
        }

        [HttpGet("pages/{pageId}")]
        public async Task<IActionResult> GetById(Guid projectId, Guid pageId)
        {
            var page = await _context.Pages
                .Include(p => p.CreatedBy)
                .Include(p => p.UpdatedBy)
                .FirstOrDefaultAsync(p => p.Id == pageId && p.ProjectId == projectId);

            if (page == null)
                return NotFound(new { statusCode = 404, message = "Trang không tồn tại." });

            return Ok(new
            {
                statusCode = 200,
                message = "Success",
                data = new
                {
                    page.Id,
                    page.Title,
                    page.Content,
                    page.SortOrder,
                    page.IsLocked,
                    page.IsArchived,
                    CreatedByName = page.CreatedBy?.FullName,
                    UpdatedByName = page.UpdatedBy?.FullName,
                    page.CreatedAt,
                    page.UpdatedAt
                }
            });
        }

        [HttpPost("pages")]
        public async Task<IActionResult> Create(Guid projectId, [FromBody] CreatePageRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid parsedUserId))
                return Unauthorized(new { statusCode = 401, message = "Vui lòng đăng nhập." });

            var maxSort = await _context.Pages
                .Where(p => p.ProjectId == projectId)
                .MaxAsync(p => (int?)p.SortOrder) ?? 0;

            var page = new Page
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Title = request.Title,
                Content = request.Content,
                SortOrder = maxSort + 1,
                CreatedById = parsedUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Pages.Add(page);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { projectId, pageId = page.Id },
                new { statusCode = 201, message = "Tạo trang thành công.", data = new { page.Id, page.Title } });
        }

        [HttpPut("pages/{pageId}")]
        public async Task<IActionResult> Update(Guid projectId, Guid pageId, [FromBody] UpdatePageRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userId, out Guid parsedUserId);

            var page = await _context.Pages.FirstOrDefaultAsync(p => p.Id == pageId && p.ProjectId == projectId);
            if (page == null)
                return NotFound(new { statusCode = 404, message = "Trang không tồn tại." });

            if (page.IsLocked)
                return BadRequest(new { statusCode = 400, message = "Trang đã bị khóa chỉnh sửa." });

            page.Title = request.Title ?? page.Title;
            page.Content = request.Content ?? page.Content;
            page.UpdatedById = parsedUserId;
            page.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { statusCode = 200, message = "Cập nhật thành công." });
        }

        [HttpPut("pages/{pageId}/archive")]
        public async Task<IActionResult> Archive(Guid projectId, Guid pageId)
        {
            var page = await _context.Pages.FirstOrDefaultAsync(p => p.Id == pageId && p.ProjectId == projectId);
            if (page == null)
                return NotFound(new { statusCode = 404, message = "Trang không tồn tại." });

            page.IsArchived = !page.IsArchived;
            page.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { statusCode = 200, message = page.IsArchived ? "Đã lưu trữ." : "Đã khôi phục." });
        }
    }

    public class CreatePageRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
    }

    public class UpdatePageRequest
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
    }
}
