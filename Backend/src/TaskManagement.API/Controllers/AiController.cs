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

        public class ChatRequest
        {
            public string Message { get; set; } = string.Empty;
            public List<ChatMessage>? History { get; set; }
        }

        public class ChatMessage
        {
            public string Role { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
        }

        public class GenerateRequest
        {
            public string Prompt { get; set; } = string.Empty;
            public string? Context { get; set; }
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

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrEmpty(request.Message))
            {
                return BadRequest(ApiResponse<object>.Error("Message cannot be empty."));
            }

            // Simulate AI delay
            await Task.Delay(2000);

            string responseContent = "";
            string lowerMsg = request.Message.ToLower();

            if (lowerMsg.Contains("tóm tắt") || lowerMsg.Contains("summary"))
            {
                responseContent = "Dự án của bạn hiện đang có 12 công việc cần làm, 3 công việc đang thực hiện và 5 công việc đã hoàn thành trong tuần này.";
            }
            else if (lowerMsg.Contains("bug") || lowerMsg.Contains("lỗi"))
            {
                responseContent = "Các lỗi gần đây thường liên quan đến phân quyền RBAC và luồng xử lý WebSocket. Bạn nên kiểm tra lại Identity Server.";
            }
            else
            {
                responseContent = "Mình hiểu rồi. Mình là Brain AI, trợ lý quản lý dự án ảo của bạn. Bạn có muốn mình tạo công việc mới, tóm tắt sự kiện, hay hỗ trợ bạn viết user story không?";
            }

            return Ok(ApiResponse<string>.Success(responseContent, "Success"));
        }

        [HttpPost("generate-description")]
        public async Task<IActionResult> GenerateDescription([FromBody] GenerateRequest request)
        {
            if (string.IsNullOrEmpty(request.Prompt))
            {
                return BadRequest(ApiResponse<object>.Error("Prompt không được để trống."));
            }

            await Task.Delay(1500);

            string aiDescription = $"**Mô tả được tạo tự động cho: {request.Prompt}**\n\n- Yêu cầu chính: Đảm bảo tính năng hoạt động theo đúng tài liệu thiết kế.\n- Tiêu chí hoàn thành (DoD): Code coverage > 80%, không có lỗi nghiêm trọng.\n- Ghi chú: Yêu cầu này được Brain AI tạo để hỗ trợ đội ngũ.";

            return Ok(ApiResponse<string>.Success(aiDescription, "Generated"));
        }
    }
}
