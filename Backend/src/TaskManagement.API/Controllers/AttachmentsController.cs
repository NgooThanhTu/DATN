using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using TaskManagement.API.Filters;
using Microsoft.AspNetCore.SignalR;
using TaskManagement.API.Hubs;

namespace TaskManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/projects/{projectId}/[controller]")]
    public class AttachmentsController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<KanbanHub> _hubContext;

        public AttachmentsController(IFileService fileService, ApplicationDbContext context, IHubContext<KanbanHub> hubContext)
        {
            _fileService = fileService;
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost("upload/{taskId}")]
        [ProjectAuthorize("Admin,PM,DEV,Developer,PROJECT_MANAGER")]
        public async Task<IActionResult> Upload(Guid projectId, Guid taskId, IFormFile file)
        {
            var task = await _context.WorkTasks.FindAsync(taskId);
            if (task == null) return NotFound();

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Security: In a real app, check if user has permission for this task's project
            
            using (var stream = file.OpenReadStream())
            {
                var savedFileName = await _fileService.UploadFileAsync(stream, file.FileName);
                if (string.IsNullOrEmpty(savedFileName)) return BadRequest("File upload failed.");

                var attachment = new Attachment
                {
                    Id = Guid.NewGuid(),
                    WorkTaskId = taskId,
                    UserId = userId,
                    FileName = file.FileName,
                    FileUrl = savedFileName, // This is the Guid-based name
                    FileSize = file.Length,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Attachments.Add(attachment);
                await _context.SaveChangesAsync();
                
                // Broadcast real-time attachment notification
                await _hubContext.Clients.Group(projectId.ToString()).SendAsync("FileUploaded", taskId, attachment);

                return Ok(attachment);
            }
        }

        [HttpGet("{attachmentId}")]
        [ProjectAuthorize("Member,Admin,PM,DEV,Developer,PROJECT_MANAGER")]
        public async Task<IActionResult> Download(Guid projectId, Guid attachmentId)
        {
            // Secure access: Check if attachment belongs to a task in the specified project
            var attachment = await _context.Attachments
                .Include(a => a.WorkTask)
                .FirstOrDefaultAsync(a => a.Id == attachmentId);
            
            if (attachment == null || attachment.WorkTask.ProjectId != projectId) 
                return NotFound("Attachment not found or access denied.");
 
             var fileData = await _fileService.DownloadFileAsync(attachment.FileUrl);
             if (fileData == null) return NotFound("Physical file not found.");
 
             return File(fileData.Value.Bytes, fileData.Value.ContentType, attachment.FileName);
         }
    }
}
