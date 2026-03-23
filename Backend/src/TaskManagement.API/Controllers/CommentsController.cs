using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;

using TaskManagement.API.Filters;
using Ganss.Xss;
using Microsoft.AspNetCore.SignalR;
using TaskManagement.API.Hubs;

namespace TaskManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/projects/{projectId}/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<KanbanHub> _hubContext;

        public CommentsController(ApplicationDbContext context, IHubContext<KanbanHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost]
        [ProjectAuthorize("Member,Admin,PM,DEV,Developer,PROJECT_MANAGER")]
        public async Task<IActionResult> AddComment(Guid projectId, [FromBody] string content, [FromQuery] Guid taskId)
        {
            if (string.IsNullOrWhiteSpace(content)) return BadRequest("Comment content cannot be empty.");

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // XSS Prevention using HtmlSanitizer
            var sanitizer = new HtmlSanitizer();
            var sanitizedContent = sanitizer.Sanitize(content);

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                WorkTaskId = taskId,
                UserId = userId,
                Content = sanitizedContent,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            
            // Broadcast real-time comment
            await _hubContext.Clients.Group(projectId.ToString()).SendAsync("CommentAdded", taskId, comment);

            return Ok(comment);
        }
    }
}
