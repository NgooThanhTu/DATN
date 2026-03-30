using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs.Common;
using TaskManagement.Application.DTOs.Department;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/departments")]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var departments = await _departmentService.GetAllAsync();
            return Ok(ApiResponse<List<DepartmentResponseDto>>.Success(departments));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var department = await _departmentService.GetByIdAsync(id);
            if (department == null)
                return NotFound(ApiResponse<object>.Error("Phòng ban không tồn tại.", 404));

            return Ok(ApiResponse<DepartmentResponseDto>.Success(department));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentDto dto)
        {
            try
            {
                var result = await _departmentService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id },
                    ApiResponse<DepartmentResponseDto>.Created(result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentDto dto)
        {
            try
            {
                var result = await _departmentService.UpdateAsync(id, dto);
                return Ok(ApiResponse<DepartmentResponseDto>.Success(result, "Cập nhật thành công."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// 5.1 Archive: Vô hiệu hóa tạm thời (ẩn khỏi dropdown, vẫn xem lịch sử)
        /// </summary>
        [HttpPut("{id}/archive")]
        public async Task<IActionResult> Archive(Guid id)
        {
            try
            {
                await _departmentService.ArchiveAsync(id);
                return Ok(ApiResponse<object>.Success(null!, "Phòng ban đã được vô hiệu hóa."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Khôi phục phòng ban đã bị Archive
        /// </summary>
        [HttpPut("{id}/restore")]
        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                await _departmentService.RestoreAsync(id);
                return Ok(ApiResponse<object>.Success(null!, "Phòng ban đã được khôi phục."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// 5.1 Soft Delete: Đánh dấu xóa (Global Query Filter sẽ tự ẩn)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            try
            {
                await _departmentService.SoftDeleteAsync(id);
                return Ok(ApiResponse<object>.Success(null!, "Phòng ban đã được xóa."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }
    }
}
