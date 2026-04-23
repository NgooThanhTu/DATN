using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Application.DTOs.Common;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/stickies")]
    [Authorize]
    public class StickiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StickiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetStickies()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out Guid userId)) return Unauthorized();

            var stickies = await _context.StickyNotes
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.UpdatedAt)
                .ToListAsync();

            return Ok(ApiResponse<object>.Success(stickies));
        }

        [HttpPost]
        public async Task<IActionResult> CreateSticky([FromBody] StickyNoteDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out Guid userId)) return Unauthorized();

            var sticky = new StickyNote
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Content = dto.Content ?? "",
                Color = dto.Color ?? "#FEF08A",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.StickyNotes.Add(sticky);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Success(sticky));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSticky(Guid id, [FromBody] StickyNoteDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out Guid userId)) return Unauthorized();

            var sticky = await _context.StickyNotes.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (sticky == null) return NotFound(ApiResponse<object>.Error("Sticky note not found."));

            sticky.Content = dto.Content ?? sticky.Content;
            if (!string.IsNullOrEmpty(dto.Color)) sticky.Color = dto.Color;
            sticky.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<object>.Success(sticky));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSticky(Guid id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out Guid userId)) return Unauthorized();

            var sticky = await _context.StickyNotes.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (sticky == null) return NotFound(ApiResponse<object>.Error("Sticky note not found."));

            _context.StickyNotes.Remove(sticky);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Success(null!, "Deleted successfully"));
        }
    }

    public class StickyNoteDto
    {
        public string? Content { get; set; }
        public string? Color { get; set; }
    }
}
