using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.DTOs.Department;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _context;

        public DepartmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentResponseDto>> GetAllAsync()
        {
            // Global Query Filter tự động loại IsDeleted == true
            return await _context.Departments
                .Include(d => d.Manager)
                .Select(d => new DepartmentResponseDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    ManagerId = d.ManagerId,
                    ManagerName = d.Manager != null ? d.Manager.FullName : null,
                    IsActive = d.IsActive,
                    MemberCount = d.DepartmentMembers.Count(),
                    CreatedAt = d.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<DepartmentResponseDto?> GetByIdAsync(Guid id)
        {
            return await _context.Departments
                .Include(d => d.Manager)
                .Where(d => d.Id == id)
                .Select(d => new DepartmentResponseDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    ManagerId = d.ManagerId,
                    ManagerName = d.Manager != null ? d.Manager.FullName : null,
                    IsActive = d.IsActive,
                    MemberCount = d.DepartmentMembers.Count(),
                    CreatedAt = d.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<DepartmentResponseDto> CreateAsync(CreateDepartmentDto dto)
        {
            // Validate ManagerId nếu có
            if (dto.ManagerId.HasValue)
            {
                var managerExists = await _context.Users.AnyAsync(u => u.Id == dto.ManagerId.Value && !u.IsDeleted);
                if (!managerExists)
                    throw new ArgumentException("Manager không tồn tại.");
            }

            var department = new Department
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                ManagerId = dto.ManagerId,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return (await GetByIdAsync(department.Id))!;
        }

        public async Task<DepartmentResponseDto> UpdateAsync(Guid id, UpdateDepartmentDto dto)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
            if (department == null)
                throw new ArgumentException("Phòng ban không tồn tại.");

            // Validate ManagerId nếu có
            if (dto.ManagerId.HasValue)
            {
                var managerExists = await _context.Users.AnyAsync(u => u.Id == dto.ManagerId.Value && !u.IsDeleted);
                if (!managerExists)
                    throw new ArgumentException("Manager không tồn tại.");
            }

            department.Name = dto.Name;
            department.ManagerId = dto.ManagerId;

            await _context.SaveChangesAsync();

            return (await GetByIdAsync(department.Id))!;
        }

        /// <summary>
        /// 5.1 Archive: Vô hiệu hóa tạm thời - ẩn khỏi dropdown tạo dự án mới
        /// </summary>
        public async Task ArchiveAsync(Guid id)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
            if (department == null)
                throw new ArgumentException("Phòng ban không tồn tại.");

            department.IsActive = false;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Khôi phục phòng ban đã bị Archive
        /// </summary>
        public async Task RestoreAsync(Guid id)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
            if (department == null)
                throw new ArgumentException("Phòng ban không tồn tại.");

            department.IsActive = true;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 5.1 Soft Delete: Đánh dấu xóa, Global Query Filter sẽ tự ẩn
        /// </summary>
        public async Task SoftDeleteAsync(Guid id)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
            if (department == null)
                throw new ArgumentException("Phòng ban không tồn tại.");

            department.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }
}
