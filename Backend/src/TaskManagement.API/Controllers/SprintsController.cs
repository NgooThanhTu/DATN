using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs.Common;
using TaskManagement.Application.DTOs.Sprint;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId}/sprints")]
    [Authorize]
    public class SprintsController : ControllerBase
    {
        private readonly ISprintService _sprintService;

        public SprintsController(ISprintService sprintService)
        {
            _sprintService = sprintService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByProject(Guid projectId)
        {
            var sprints = await _sprintService.GetByProjectAsync(projectId);
            return Ok(ApiResponse<List<SprintResponseDto>>.Success(sprints));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid projectId, Guid id)
        {
            var sprint = await _sprintService.GetByIdAsync(id);
            if (sprint == null || sprint.ProjectId != projectId)
                return NotFound(ApiResponse<object>.Error("Sprint không tồn tại trong dự án này.", 404));

            return Ok(ApiResponse<SprintResponseDto>.Success(sprint));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid projectId, [FromBody] CreateSprintDto dto)
        {
            try
            {
                var result = await _sprintService.CreateAsync(projectId, dto);
                return CreatedAtAction(nameof(GetById), new { projectId, id = result.Id },
                    ApiResponse<SprintResponseDto>.Created(result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// 5.3 Sprint Overlap Guard: Start sprint, chặn nếu đã có sprint active
        /// </summary>
        [HttpPost("{id}/start")]
        public async Task<IActionResult> Start(Guid projectId, Guid id)
        {
            try
            {
                var result = await _sprintService.StartAsync(projectId, id);
                return Ok(ApiResponse<SprintResponseDto>.Success(result, "Sprint đã được bắt đầu."));
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

        /// <summary>
        /// 5.5 Close Sprint + Roll-over Tasks
        /// </summary>
        [HttpPost("{id}/close")]
        public async Task<IActionResult> Close(Guid projectId, Guid id, [FromBody] CloseSprintDto dto)
        {
            try
            {
                await _sprintService.CloseAsync(id, dto);
                return Ok(ApiResponse<object>.Success(null!, "Sprint đã được đóng. Các task chưa hoàn thành đã được chuyển."));
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
        /// <summary>
        /// 6.1 Burndown Chart
        /// </summary>
        [HttpGet("{id}/burndown")]
        public async Task<IActionResult> GetBurndown(Guid projectId, Guid id)
        {
            try
            {
                var result = await _sprintService.GetBurndownChartAsync(id);
                return Ok(ApiResponse<List<BurndownDataDto>>.Success(result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }
    }
}
