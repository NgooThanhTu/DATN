using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Domain.Entities;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/workspaces")]
    [Authorize]
    public class WorkspacesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WorkspacesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy tất cả workspaces mà user hiện tại là thành viên
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMyWorkspaces()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid parsedUserId))
                return Unauthorized(new { statusCode = 401, message = "Vui lòng đăng nhập." });

            var workspaces = await _context.WorkspaceMembers
                .AsNoTracking()
                .Where(wm => wm.UserId == parsedUserId && wm.IsActive)
                .Select(wm => new
                {
                    wm.Workspace.Id,
                    wm.Workspace.Name,
                    wm.Workspace.Slug,
                    wm.Workspace.Logo,
                    wm.Workspace.Timezone,
                    wm.WorkspaceRole,
                    OwnerName = wm.Workspace.Owner.FullName,
                    MemberCount = wm.Workspace.Members.Count(m => m.IsActive),
                    ProjectCount = wm.Workspace.Projects.Count(p => !p.IsDeleted),
                    wm.Workspace.CreatedAt
                })
                .ToListAsync();

            return Ok(new { statusCode = 200, message = "Success", data = workspaces });
        }

        /// <summary>
        /// Tạo workspace mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWorkspaceRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid parsedUserId))
                return Unauthorized(new { statusCode = 401, message = "Vui lòng đăng nhập." });

            // Validate slug uniqueness
            var slugExists = await _context.Workspaces.AnyAsync(w => w.Slug == request.Slug);
            if (slugExists)
                return BadRequest(new { statusCode = 400, message = "Slug đã tồn tại. Vui lòng chọn tên khác." });

            var workspace = new Workspace
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Slug = request.Slug.ToLower().Trim(),
                OwnerId = parsedUserId,
                Timezone = request.Timezone ?? "Asia/Ho_Chi_Minh",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Workspaces.Add(workspace);

            // Auto-add creator as OWNER
            _context.WorkspaceMembers.Add(new WorkspaceMember
            {
                WorkspaceId = workspace.Id,
                UserId = parsedUserId,
                WorkspaceRole = "OWNER",
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            });

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMyWorkspaces), null,
                new { statusCode = 201, message = "Tạo workspace thành công.", data = new { workspace.Id, workspace.Name, workspace.Slug } });
        }

        /// <summary>
        /// Lấy thông tin workspace theo slug
        /// </summary>
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid parsedUserId))
                return Unauthorized(new { statusCode = 401, message = "Vui lòng đăng nhập." });

            var workspace = await _context.Workspaces
                .AsNoTracking()
                .Where(w => w.Slug == slug)
                .Select(w => new
                {
                    w.Id,
                    w.Name,
                    w.Slug,
                    w.Logo,
                    w.Timezone,
                    OwnerName = w.Owner.FullName,
                    MemberCount = w.Members.Count(m => m.IsActive),
                    ProjectCount = w.Projects.Count(p => !p.IsDeleted),
                    w.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (workspace == null)
                return NotFound(new { statusCode = 404, message = "Workspace không tồn tại." });

            // Check membership
            var isMember = await _context.WorkspaceMembers
                .AnyAsync(wm => wm.WorkspaceId == workspace.Id && wm.UserId == parsedUserId && wm.IsActive);

            if (!isMember)
                return StatusCode(403, new { statusCode = 403, message = "Bạn không phải thành viên của workspace này." });

            return Ok(new { statusCode = 200, message = "Success", data = workspace });
        }

        /// <summary>
        /// Thêm thành viên vào workspace
        /// </summary>
        [HttpPost("{workspaceId}/members")]
        public async Task<IActionResult> AddMember(Guid workspaceId, [FromBody] AddWorkspaceMemberRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid parsedUserId))
                return Unauthorized(new { statusCode = 401, message = "Vui lòng đăng nhập." });

            // Check requester is OWNER or ADMIN
            var requesterMembership = await _context.WorkspaceMembers
                .FirstOrDefaultAsync(wm => wm.WorkspaceId == workspaceId && wm.UserId == parsedUserId);

            if (requesterMembership == null || (requesterMembership.WorkspaceRole != "OWNER" && requesterMembership.WorkspaceRole != "ADMIN"))
                return StatusCode(403, new { statusCode = 403, message = "Bạn không có quyền thêm thành viên." });

            // Check user exists
            var targetUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (targetUser == null)
                return BadRequest(new { statusCode = 400, message = "Không tìm thấy người dùng với email này." });

            // Check not already member
            var existing = await _context.WorkspaceMembers
                .FirstOrDefaultAsync(wm => wm.WorkspaceId == workspaceId && wm.UserId == targetUser.Id);

            if (existing != null)
            {
                if (existing.IsActive)
                    return BadRequest(new { statusCode = 400, message = "Người dùng đã là thành viên." });
                existing.IsActive = true;
                existing.WorkspaceRole = request.Role ?? "MEMBER";
            }
            else
            {
                _context.WorkspaceMembers.Add(new WorkspaceMember
                {
                    WorkspaceId = workspaceId,
                    UserId = targetUser.Id,
                    WorkspaceRole = request.Role ?? "MEMBER",
                    JoinedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new { statusCode = 200, message = "Thêm thành viên thành công." });
        }

        /// <summary>
        /// Lấy danh sách thành viên workspace
        /// </summary>
        [HttpGet("{workspaceId}/members")]
        public async Task<IActionResult> GetMembers(Guid workspaceId)
        {
            var members = await _context.WorkspaceMembers
                .AsNoTracking()
                .Where(wm => wm.WorkspaceId == workspaceId && wm.IsActive)
                .Select(wm => new
                {
                    wm.UserId,
                    wm.User.FullName,
                    wm.User.Email,
                    wm.User.AvatarUrl,
                    wm.WorkspaceRole,
                    wm.JoinedAt
                })
                .OrderBy(m => m.FullName)
                .ToListAsync();

            return Ok(new { statusCode = 200, message = "Success", data = members });
        }
    }

    public class CreateWorkspaceRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Timezone { get; set; }
    }

    public class AddWorkspaceMemberRequest
    {
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
    }
}
