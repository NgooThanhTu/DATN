using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Domain.Entities;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/settings")]
    [Authorize]
    public class SystemSettingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SystemSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{group}")]
        public async Task<IActionResult> GetSettingsByGroup(string group)
        {
            // Optional: check admin roles if these are system-wide
            // For now, allow reading (or you can restrict to Admin/PM only)

            var settings = await _context.SystemSettings
                .Where(s => s.SettingGroup == group)
                .ToListAsync();

            var data = settings.ToDictionary(s => s.Key, s => s.Value);

            return Ok(new { statusCode = 200, data = data });
        }

        public class UpdateSettingRequest
        {
            public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
        }

        [HttpPut("{group}")]
        public async Task<IActionResult> UpdateSettingsByGroup(string group, [FromBody] UpdateSettingRequest request)
        {
            // Only Admins or PROJECT_MANAGERS should perhaps modify system-wide
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            
            var isAdmin = user?.UserRoles?.Any(ur => ur.Role.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase)) ?? false;
            var isPm = await _context.ProjectMembers.AnyAsync(pm => pm.UserId.ToString() == userId && (pm.ProjectRole == "PROJECT_MANAGER" || pm.ProjectRole == "PM" || pm.ProjectRole == "Admin"));
            
            if (!isAdmin && !isPm)
                return Unauthorized(new { message = "You do not have permission to change settings." });

            var existingSettings = await _context.SystemSettings
                .Where(s => s.SettingGroup == group)
                .ToListAsync();

            foreach (var kvp in request.Settings)
            {
                var setting = existingSettings.FirstOrDefault(s => s.Key == kvp.Key);
                if (setting != null)
                {
                    setting.Value = kvp.Value;
                    setting.LastModifiedAt = DateTime.UtcNow;
                }
                else
                {
                    _context.SystemSettings.Add(new SystemSetting
                    {
                        Id = Guid.NewGuid(),
                        SettingGroup = group,
                        Key = kvp.Key,
                        Value = kvp.Value,
                        LastModifiedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { statusCode = 200, message = "Settings updated successfully" });
        }
    }
}
