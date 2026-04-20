using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TaskManagement.Application.DTOs.AI;
using TaskManagement.Application.DTOs.Common;
using TaskManagement.Application.DTOs.WorkTask;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableRateLimiting("FixedWindow")]
    public class AiController : ControllerBase
    {
        private readonly IAiService _aiService;

        public AiController(IAiService aiService)
        {
            _aiService = aiService;
        }

        [HttpGet("usage")]
        public async Task<IActionResult> Usage()
        {
            var userId = GetUserId();
            var usage = await _aiService.GetUsageAsync(userId);
            return Ok(ApiResponse<AiUsageDto>.Success(usage));
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] AiChatRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(ApiResponse<object>.Error("Message cannot be empty."));
            }

            try
            {
                var userId = GetUserId();
                var response = await _aiService.ChatAsync(userId, request);
                return Ok(ApiResponse<string>.Success(response));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        [HttpPost("generate-description")]
        public async Task<IActionResult> GenerateDescription([FromBody] AiGenerateDescriptionRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest(ApiResponse<object>.Error("Prompt khong duoc de trong."));
            }

            try
            {
                var userId = GetUserId();
                var description = await _aiService.GenerateDescriptionAsync(userId, request);
                return Ok(ApiResponse<string>.Success(description, "Generated"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        [HttpPost("breakdown-task")]
        public async Task<IActionResult> BreakdownTask([FromBody] AiBreakdownRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest(ApiResponse<object>.Error("Tieu de cong viec khong duoc de trong."));
            }

            try
            {
                var userId = GetUserId();

                if (request.CreateSubtasks)
                {
                    var created = await _aiService.BreakdownAndCreateSubtasksAsync(userId, request);
                    return Ok(ApiResponse<List<WorkTaskResponseDto>>.Success(created, "AI da phan ra va tao sub-work items."));
                }

                var subtasks = await _aiService.BreakdownTaskAsync(userId, request);
                return Ok(ApiResponse<List<AiSubTaskDto>>.Success(subtasks, "AI da phan tach cong viec thanh cong."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        private Guid GetUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException("Token khong hop le.");
            }

            return userId;
        }
    }
}
