using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/gamification")]
    [Authorize]
    public class GamificationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GamificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyRewards()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "Token khong hop le." });
            }

            var wallet = await _context.UserWallets
                .AsNoTracking()
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.UserId == userId.Value);

            var transactions = await _context.PointTransactions
                .AsNoTracking()
                .Where(t => t.UserWalletUserId == userId.Value)
                .Include(t => t.WorkTask)
                .OrderByDescending(t => t.CreatedAt)
                .Take(50)
                .Select(t => new
                {
                    t.Id,
                    t.Amount,
                    t.Reason,
                    t.TransactionType,
                    t.WorkTaskId,
                    TaskTitle = t.WorkTask != null ? t.WorkTask.Title : null,
                    TaskSequenceId = t.WorkTask != null ? t.WorkTask.SequenceId : null,
                    t.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                statusCode = 200,
                message = "Success",
                data = new
                {
                    wallet = new
                    {
                        userId = userId.Value,
                        totalPoints = wallet?.TotalPoints ?? 0,
                        level = wallet?.Level ?? 1,
                        nextLevelAt = ((wallet?.Level ?? 1) * 1000),
                        userName = wallet?.User.FullName ?? wallet?.User.Email
                    },
                    transactions
                }
            });
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var leaders = await _context.UserWallets
                .AsNoTracking()
                .Include(w => w.User)
                .OrderByDescending(w => w.TotalPoints)
                .ThenBy(w => w.User.FullName)
                .Take(20)
                .Select(w => new
                {
                    w.UserId,
                    UserName = w.User.FullName ?? w.User.Email,
                    w.TotalPoints,
                    w.Level
                })
                .ToListAsync();

            return Ok(new { statusCode = 200, message = "Success", data = leaders });
        }

        private Guid? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var id) ? id : null;
        }
    }
}
