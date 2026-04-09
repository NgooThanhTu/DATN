using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Application.DTOs.Auth;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                return Unauthorized(new { statusCode = 401, message = "Unauthorized." });

            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
                
            if (user == null) return NotFound(new { message = "User not found" });

            // We read extra fields if available, otherwise fallback.
            // Since User entity might not have JobTitle, Organization, Department natively,
            // we will simulate them via SystemSetting or allow the schema to adapt.
            // Since we can't change the db schema without migrations, we will store profile extra data
            // into SystemSettings where Key = "Profile_" + userId.
            
            var extraProfileSetting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == "Profile_" + userId.ToString());

            var extra = new UpdateProfileRequest();

            if (extraProfileSetting != null && !string.IsNullOrEmpty(extraProfileSetting.Value))
            {
                try {
                    extra = System.Text.Json.JsonSerializer.Deserialize<UpdateProfileRequest>(extraProfileSetting.Value) ?? extra;
                } catch { }
            }

            return Ok(new
            {
                statusCode = 200,
                data = new
                {
                    id = user.Id,
                    email = user.Email,
                    fullName = user.FullName,
                    avatarUrl = user.AvatarUrl,
                    publicName = string.IsNullOrEmpty(extra.PublicName) ? user.FullName : extra.PublicName,
                    jobTitle = extra.JobTitle,
                    departmentName = extra.DepartmentName,
                    organizationName = extra.OrganizationName,
                    collaborationRules = extra.CollaborationRules
                }
            });
        }

        public class UpdateProfileRequest
        {
            public string FullName { get; set; } = string.Empty;
            public string PublicName { get; set; } = string.Empty;
            public string JobTitle { get; set; } = string.Empty;
            public string DepartmentName { get; set; } = string.Empty;
            public string OrganizationName { get; set; } = string.Empty;
            public string CollaborationRules { get; set; } = string.Empty;
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                return Unauthorized(new { statusCode = 401, message = "Unauthorized." });

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound(new { message = "User not found" });

            user.FullName = request.FullName;
            user.UpdatedAt = DateTime.UtcNow;

            var extraProfileSetting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == "Profile_" + userId.ToString());
            
            if (extraProfileSetting == null)
            {
                extraProfileSetting = new TaskManagement.Domain.Entities.SystemSetting
                {
                    Id = Guid.NewGuid(),
                    Key = "Profile_" + userId.ToString(),
                    SettingGroup = "UserProfile"
                };
                _context.SystemSettings.Add(extraProfileSetting);
            }

            var extraProfileData = new
            {
                publicName = request.PublicName,
                jobTitle = request.JobTitle,
                departmentName = request.DepartmentName,
                organizationName = request.OrganizationName,
                collaborationRules = request.CollaborationRules
            };

            extraProfileSetting.Value = System.Text.Json.JsonSerializer.Serialize(extraProfileData);
            extraProfileSetting.LastModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { statusCode = 200, message = "Profile updated successfully" });
        }
    }
}
