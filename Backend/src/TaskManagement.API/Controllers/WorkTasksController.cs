using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagement.API.Filters;
using TaskManagement.Application.DTOs.WorkTask;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    public class WorkTasksController : ControllerBase
    {
        private readonly IWorkTaskService _workTaskService;

        public WorkTasksController(IWorkTaskService workTaskService)
        {
            _workTaskService = workTaskService;
        }

        /// <summary>
        /// GET /api/projects/{projectId}/WorkTasks
        /// Lấy danh sách tasks của dự án. Chỉ thành viên dự án mới được phép.
        /// </summary>
        [HttpGet("api/projects/{projectId}/WorkTasks")]
        [Authorize]
        public async Task<IActionResult> GetTasksByProject(Guid projectId)
        {
            try
            {
                var tasks = await _workTaskService.GetTasksByProjectIdAsync(projectId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Lỗi máy chủ: " + ex.Message });
            }
        }

        /// <summary>
        /// GET /api/tasks/my-tasks
        /// Lấy danh sách tasks được giao cho user đang đăng nhập (dùng cho Dashboard).
        /// </summary>
        [HttpGet("api/tasks/my-tasks")]
        [Authorize]
        public async Task<IActionResult> GetMyTasks()
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    return Unauthorized(new { statusCode = 401, message = "Token không hợp lệ." });
                }

                var tasks = await _workTaskService.GetMyTasksAsync(userId);
                return Ok(new { statusCode = 200, message = "Success", data = tasks });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { statusCode = 500, message = "Lỗi máy chủ: " + ex.Message });
            }
        }

        /// <summary>
        /// PUT /api/tasks/{id}/status
        /// Cập nhật trạng thái task (kèm các ràng buộc State Machine, Dependencies, Parent-Subtask).
        /// </summary>
        [HttpPut("api/tasks/{id}/status")]
        [Authorize]
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

