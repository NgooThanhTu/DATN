using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructure.Data;
using TaskManagement.API.Filters;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [SystemAuthorize(roles: "SuperAdmin, Admin, Developer, DEV")]
    public class AdminUsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] string? search)
        {
            try
            {
                var query = _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    var s = search.ToLower();
                    query = query.Where(u => u.FullName.ToLower().Contains(s) || u.Email.ToLower().Contains(s));
                }

                var users = await query.Select(u => new
                {
                    id = u.Id,
                    name = string.IsNullOrEmpty(u.FullName) ? u.Email.Split('@')[0] : u.FullName,
                    email = u.Email,
                    isActive = u.IsActive,
                    avatar = u.AvatarUrl,
                    roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                    createdAt = u.CreatedAt
                }).ToListAsync();

                return Ok(new { statusCode = 200, message = "Success", data = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] TaskManagement.Application.DTOs.Admin.AddUserRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { statusCode = 400, message = "Dữ liệu không hợp lệ." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate if email exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted);
                if (existingUser == null)
                {
                    return BadRequest(new { statusCode = 400, message = "Tài khoản chưa được khởi tạo." });
                }

                // Cập nhật Role hệ thống nếu cần
                var roleEntity = await _context.Roles.FirstOrDefaultAsync(r => r.Name == request.Role);
                if (roleEntity == null)
                {
                    roleEntity = new TaskManagement.Domain.Entities.Role { Id = Guid.NewGuid(), Name = request.Role };
                    _context.Roles.Add(roleEntity);
                    await _context.SaveChangesAsync();
                }

                var existingRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == existingUser.Id && ur.RoleId == roleEntity.Id);
                if (existingRole == null)
                {
                    _context.UserRoles.Add(new TaskManagement.Domain.Entities.UserRole
                    {
                        UserId = existingUser.Id,
                        RoleId = roleEntity.Id
                    });
                }

                // Assign to Project if ProjectId provided
                if (request.ProjectId.HasValue)
                {
                    var projectExists = await _context.Projects.AnyAsync(p => p.Id == request.ProjectId.Value);
                    if (!projectExists)
                    {
                        return BadRequest(new { statusCode = 404, message = "Dự án không tồn tại." });
                    }

                    // Insert pending ProjectMember
                    var pmExists = await _context.ProjectMembers.AnyAsync(pm => pm.ProjectId == request.ProjectId.Value && pm.UserId == existingUser.Id);
                    if (!pmExists)
                    {
                        _context.ProjectMembers.Add(new TaskManagement.Domain.Entities.ProjectMember
                        {
                            ProjectId = request.ProjectId.Value,
                            UserId = existingUser.Id,
                            ProjectRole = "DEV",
                            JoinedAt = DateTime.UtcNow,
                            Status = false // Pending accept
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Mock Send Email Invite
                Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
                Console.WriteLine($"║ LỜI MỜI NHÂN SỰ ĐƯỢC TẠO CHO: {request.Email}");
                Console.WriteLine($"║ LINK CHẤP NHẬN: http://localhost:5173/accept-invite?email={request.Email}");
                Console.WriteLine("╚════════════════════════════════════════════════════════════╝");

                return Ok(new { statusCode = 200, message = "Đã gửi lời mời tham gia thành công. Chờ nhân sự xác nhận." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { statusCode = 500, message = ex.Message });
            }
        }

        [HttpPut("{userId}/suspend")]
        public async Task<IActionResult> SuspendUser(Guid userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return NotFound(new { statusCode = 404, message = "User not found." });
                }

                // Suspend the user
                user.IsActive = false;
                
                // Clear the current active refreshToken logic from the User entity for now
                // When we create RefreshTokens table in phase 3, we will add the "IsRevoked" query here
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;

                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();

                return Ok(new { statusCode = 200, message = "User suspended successfully." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { statusCode = 500, message = ex.Message });
            }
        }
    }
}
