using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Domain.Entities;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/auditlogs")]
    [Authorize]
    public class AuditLogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuditLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuditLogs(
            [FromQuery] Guid? projectId, 
            [FromQuery] Guid? taskId, 
            [FromQuery] string? timeFilter, 
            [FromQuery] string? search,
            [FromQuery] int page = 1, [FromQuery] int limit = 100)
        {
            try
            {
                // Admin-only guard
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { statusCode = 401, message = "Unauthorized." });

                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId);
                
                var isAdmin = user?.UserRoles?.Any(ur => ur.Role.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase)) ?? false;

                var pmProjectIds = new List<Guid>();
                if (!isAdmin && user != null)
                {
                    pmProjectIds = await _context.ProjectMembers
                        .Where(pm => pm.UserId == userId && (pm.ProjectRole == "PM" || pm.ProjectRole == "PROJECT_MANAGER" || pm.ProjectRole == "PO" || pm.ProjectRole == "Admin"))
                        .Select(pm => pm.ProjectId)
                        .ToListAsync();
                }
                
                if (!isAdmin && !pmProjectIds.Any())
                    return StatusCode(403, new { statusCode = 403, message = "Chỉ Admin hoặc PM mới được phép xem Audit Log." });

                var query = _context.AuditLogs
                    .Include(al => al.User)
                    .Include(al => al.WorkTask)
                    .ThenInclude(t => t.Project)
                    .AsQueryable();

                // 1. Phân quyền dữ liệu (Data Authorization)
                if (!isAdmin)
                {
                    query = query.Where(al => pmProjectIds.Contains(al.WorkTask.ProjectId));
                }

                // 2. Bộ lọc (Filters)
                if (projectId.HasValue)
                {
                    query = query.Where(al => al.WorkTask.ProjectId == projectId.Value);
                }
                if (taskId.HasValue)
                {
                    query = query.Where(al => al.WorkTaskId == taskId.Value);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    var s = search.ToLower();
                    query = query.Where(al => al.WorkTask.Title.ToLower().Contains(s) || 
                                              al.User.FullName.ToLower().Contains(s) || 
                                              al.User.Email.ToLower().Contains(s));
                }
                if (!string.IsNullOrEmpty(timeFilter))
                {
                    var now = DateTime.UtcNow;
                    if (timeFilter == "24h")
                        query = query.Where(al => al.CreatedAt >= now.AddHours(-24));
                    else if (timeFilter == "30d")
                        query = query.Where(al => al.CreatedAt >= now.AddDays(-30));
                }

                var skip = (page - 1) * limit;

                query = query.OrderByDescending(al => al.CreatedAt);

                var total = await query.CountAsync();
                
                var rawLogs = await query
                    .Skip(skip)
                    .Take(limit)
                    .Select(al => new {
                        Id = al.Id,
                        CreatedAt = al.CreatedAt,
                        FullName = al.User.FullName,
                        Email = al.User.Email,
                        OldValue = al.OldValue,
                        NewValue = al.NewValue,
                        FieldChanged = al.FieldChanged,
                        TaskTitle = al.WorkTask.Title,
                        TaskId = al.WorkTaskId,
                        ProjectName = al.WorkTask.Project.Name
                    })
                    .ToListAsync();
                    
                var logs = rawLogs.Select(al => new {
                        id = "LOG-" + al.Id.ToString().Substring(0, 8).ToUpper(),
                        timestamp = al.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                        user = al.FullName ?? al.Email,
                        action = string.IsNullOrEmpty(al.OldValue) && al.NewValue != null ? "create" : "update",
                        resource = $"[{al.ProjectName}] {al.TaskTitle}",
                        targetId = al.TaskTitle.Length > 10 ? "TASK-" + al.TaskTitle.Substring(0, 10) : "TASK-" + al.TaskTitle,
                        projectName = al.ProjectName,
                        status = "success",
                        summary = $"Thay đổi trường '{al.FieldChanged}'",
                        details = new {
                            field = al.FieldChanged,
                            oldValue = al.OldValue,
                            newValue = al.NewValue,
                            taskId = al.TaskId
                        }
                    }).ToList();

                return Ok(new { statusCode = 200, message = "Success", data = new { items = logs, total } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = ex.Message });
            }
        }
    }
}
