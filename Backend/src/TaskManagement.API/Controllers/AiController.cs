using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TaskManagement.Application.DTOs.Common;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AiController : ControllerBase
    {
        public class BreakdownRequest
        {
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }

        public class SubTaskDto
        {
            public string Title { get; set; } = string.Empty;
            public double EstHours { get; set; }
        }

        [HttpPost("breakdown-task")]
        public async Task<IActionResult> BreakdownTask([FromBody] BreakdownRequest request)
        {
            if (string.IsNullOrEmpty(request.Title))
            {
                return BadRequest(ApiResponse<object>.Error("Tiêu đề công việc không được để trống."));
            }

            // MOCK AI SERVICE (Simulate 1-2s delay for realism)
            await Task.Delay(1500);

            var result = new List<SubTaskDto>();
            string lowerStr = request.Title.ToLower() + " " + request.Description.ToLower();

            // Very simple Mock heuristic for demo
            if (lowerStr.Contains("login") || lowerStr.Contains("đăng nhập"))
            {
                result.Add(new SubTaskDto { Title = "Phát triển giao diện Login UI & Form Validation", EstHours = 4 });
                result.Add(new SubTaskDto { Title = "Tích hợp gọi JWT API và xử lý Token lưu vào LocalStorage", EstHours = 3 });
            }
            else if (lowerStr.Contains("api") || lowerStr.Contains("backend"))
            {
                result.Add(new SubTaskDto { Title = "Thiết kế DTOs & Models cho API", EstHours = 2 });
                result.Add(new SubTaskDto { Title = "Code logic Controller", EstHours = 3 });
                result.Add(new SubTaskDto { Title = "Viết Unit Test cho Edge cases", EstHours = 2 });
            }
            else
            {
                result.Add(new SubTaskDto { Title = "Phân tích yêu cầu và Mockup UI: " + request.Title, EstHours = 2 });
                result.Add(new SubTaskDto { Title = "Thiết kế cấu trúc Database", EstHours = 2 });
                result.Add(new SubTaskDto { Title = "Triển khai Logic lập trình (Code)", EstHours = 4 });
            }

            // Trả về theo định dạng JSON như AI gọi thành công
            return Ok(ApiResponse<List<SubTaskDto>>.Success(result, "Phân tách công việc bằng AI thành công."));
        }
    }
}
