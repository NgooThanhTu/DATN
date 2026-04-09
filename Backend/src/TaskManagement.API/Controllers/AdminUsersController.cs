using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize]
    public class AdminUsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersByProjects([FromQuery] Guid? projectId, [FromQuery] string? search)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid currentUserId))
                    return Unauthorized(new { statusCode = 401, message = "Unauthorized." });

                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == currentUserId);

                var isAdmin = user?.UserRoles?.Any(ur => ur.Role != null && ur.Role.Name.Contains("Admin", StringComparison.OrdinalIgnoreCase)) ?? false;

                var pmProjectIds = new List<Guid>();
                if (!isAdmin && user != null)
                {
                    pmProjectIds = await _context.ProjectMembers
                        .Where(pm => pm.UserId == currentUserId && (pm.ProjectRole == "PM" || pm.ProjectRole == "PROJECT_MANAGER" || pm.ProjectRole == "Admin"))
                        .Select(pm => pm.ProjectId)
                        .ToListAsync();
                }

                if (!isAdmin && !pmProjectIds.Any())
                    return Ok(new { statusCode = 200, message = "Success", data = new List<object>() });

                var query = _context.ProjectMembers
                    .Include(pm => pm.User)
                    .Include(pm => pm.Project)
                    .AsQueryable();

                if (!isAdmin)
                {
                    query = query.Where(pm => pmProjectIds.Contains(pm.ProjectId));
                }

                if (projectId.HasValue)
                {
                    query = query.Where(pm => pm.ProjectId == projectId.Value);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    var s = search.ToLower();
                    query = query.Where(pm => pm.User.FullName.ToLower().Contains(s) || 
                                              pm.User.Email.ToLower().Contains(s));
                }

                var projectMembers = await query.ToListAsync();

                var users = projectMembers.Select(pm => new {
                    id = pm.UserId,
                    projectId = pm.ProjectId,
                    projectName = pm.Project.Name,
                    name = string.IsNullOrEmpty(pm.User.FullName) ? pm.User.Email.Split('@')[0] : pm.User.FullName,
                    email = pm.User.Email,
                    phone = "",
                    role = pm.ProjectRole,
                    status = pm.Status ? "Active" : "Inactive",
                    avatar = pm.User.AvatarUrl ?? $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(pm.User.FullName ?? pm.User.Email)}&background=random"
                }).ToList();

                return Ok(new { statusCode = 200, message = "Success", data = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = ex.Message });
            }
        }
    }
}
