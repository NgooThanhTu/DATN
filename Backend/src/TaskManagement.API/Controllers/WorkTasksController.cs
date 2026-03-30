using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.WorkTask;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class WorkTasksController : ControllerBase
    {
        private readonly IWorkTaskService _workTaskService;

        public WorkTasksController(IWorkTaskService workTaskService)
        {
            _workTaskService = workTaskService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWorkTaskDto request)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid reporterId))
                {
                    // Fallback to empty if not authenticated, though usually should return Unauthorized.
                    reporterId = Guid.Empty;
                }
                
                var result = await _workTaskService.CreateAsync(reporterId, request);
                return CreatedAtAction(nameof(UpdateStatus), new { id = result.Id }, new { statusCode = 201, message = "Tạo tác vụ thành công.", data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { statusCode = 400, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { statusCode = 400, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Lỗi máy chủ nội bộ: " + ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusRequestDto request)
        {
            try
            {
                await _workTaskService.UpdateTaskStatusAsync(id, request);
                return Ok(new { statusCode = 200, message = "Success", data = "Cập nhật trạng thái tác vụ thành công." });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { statusCode = 409, message = "Dữ liệu đã bị người khác thay đổi. Vui lòng tải lại trang để tránh ghi đè (Anti-Overwrite)." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { statusCode = 400, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { statusCode = 400, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Lỗi máy chủ nội bộ: " + ex.Message });
            }
        }
    }
}
