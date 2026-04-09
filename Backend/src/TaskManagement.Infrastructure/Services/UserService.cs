using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task DisableUserAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.DepartmentMemberships)
                .Include(u => u.ProjectMemberships)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("User not found.");

            // 1. Offboarding Security: Vô hiệu hóa và xóa ngay token
            user.IsActive = false;
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            // 2. Mất toàn quyền trên mọi Project (áp dụng "Individual Permission" rule bằng cách xoá khỏi Group/Department)
            if (user.DepartmentMemberships.Any())
            {
                _context.DepartmentMembers.RemoveRange(user.DepartmentMemberships);
            }
            
            // Xóa khỏi ProjectMember (Cache cũ)
            if (user.ProjectMemberships.Any())
            {
                _context.ProjectMembers.RemoveRange(user.ProjectMemberships);
            }

            await _context.SaveChangesAsync();
        }
    }
}
