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
                .Include(item => item.User)
                .FirstOrDefaultAsync(item => item.UserId == userId.Value);

            var transactions = await _context.PointTransactions
                .AsNoTracking()
                .Where(item => item.UserWalletUserId == userId.Value)
                .Include(item => item.WorkTask)
                .OrderByDescending(item => item.CreatedAt)
                .Take(80)
                .Select(item => new
                {
                    item.Id,
                    item.Amount,
                    item.Reason,
                    item.TransactionType,
                    item.WorkTaskId,
                    TaskTitle = item.WorkTask != null ? item.WorkTask.Title : null,
                    TaskSequenceId = item.WorkTask != null ? item.WorkTask.SequenceId : null,
                    item.CreatedAt
                })
                .ToListAsync();

            var assignedTaskSnapshots = await _context.WorkTasks
                .AsNoTracking()
                .Where(task => !task.IsDeleted && (task.AssignedUserId == userId.Value || task.ReporterId == userId.Value))
                .Select(task => new
                {
                    task.Id,
                    task.Title,
                    task.SequenceId,
                    task.Priority,
                    task.StoryPoints,
                    task.TotalEstimatedHours,
                    task.PlannedStartDate,
                    task.PlannedEndDate,
                    task.DueDate,
                    task.AssignedUserId,
                    task.ReporterId,
                    StatusName = task.TaskStatus.Name
                })
                .ToListAsync();

            var assignmentSnapshots = await _context.TaskAssignments
                .AsNoTracking()
                .Where(item => item.UserId == userId.Value)
                .Select(item => new
                {
                    item.WorkTaskId,
                    item.ProgressPercent,
                    item.ContributionWeight,
                    item.Status,
                    item.EstimatedHours,
                    item.TotalActualHours
                })
                .ToListAsync();

            var timeLogSnapshots = await _context.TimeLogs
                .AsNoTracking()
                .Where(item => item.UserId == userId.Value)
                .GroupBy(item => item.WorkTaskId)
                .Select(group => new
                {
                    WorkTaskId = group.Key,
                    LoggedHours = group.Sum(item => item.Hours)
                })
                .ToListAsync();

            var points = wallet?.TotalPoints ?? 0;
            var career = CalculateCareer(points);
            var summary = BuildSummary(transactions, assignedTaskSnapshots, assignmentSnapshots, timeLogSnapshots);

            return Ok(new
            {
                statusCode = 200,
                message = "Success",
                data = new
                {
                    wallet = new
                    {
                        userId = userId.Value,
                        totalPoints = points,
                        level = wallet?.Level ?? 1,
                        nextLevelAt = ((wallet?.Level ?? 1) * 1000),
                        userName = wallet?.User.FullName ?? wallet?.User.Email,
                        rankTitle = career.Title
                    },
                    career = new
                    {
                        level = career.Level,
                        title = career.Title,
                        pointsIntoLevel = career.PointsIntoLevel,
                        currentThreshold = career.CurrentThreshold,
                        nextThreshold = career.NextThreshold,
                        progressPercent = career.ProgressPercent
                    },
                    formula = new
                    {
                        expression = "Base effort x Efficiency x Quality x Contribution share",
                        earlyBonusPercent = 10,
                        contributionRule = "Diem duoc chia theo ty le dong gop trong task.",
                        actualHoursRule = "Actual hours uu tien lay tu time log, sau do fallback sang assignment actual hours.",
                        policy = new
                        {
                            parentRollup = "Task cha co the roll-up effort tu subtasks, nhung diem uu tien tinh tren task giao viec cuoi cung.",
                            multiAssignee = "Estimate cua task khong bi nhan doi; effort duoc chia theo contribution weight.",
                            reopenedWork = "Task bi rollback hoac reopen se bi tru diem thong qua giao dich rollback.",
                            carryOver = "Task qua cycle van giu estimate/actual, nhung quality modifier giam neu overdue."
                        },
                        sample = summary.SampleFormula
                    },
                    summary = new
                    {
                        totalTransactions = transactions.Count,
                        earnedPoints = transactions.Where(item => item.Amount > 0).Sum(item => item.Amount),
                        rollbackPoints = Math.Abs(transactions.Where(item => item.Amount < 0).Sum(item => item.Amount)),
                        completedTasks = summary.CompletedTasks,
                        earlyBonuses = summary.EarlyBonuses,
                        contributionPercent = summary.ContributionPercent,
                        estimatedHours = summary.EstimatedHours,
                        actualHours = summary.ActualHours,
                        loggedHours = summary.LoggedHours
                    },
                    spotlightTasks = summary.SpotlightTasks,
                    transactions,
                    recentAchievements = summary.RecentAchievements
                }
            });
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var leaders = await _context.UserWallets
                .AsNoTracking()
                .Include(item => item.User)
                .OrderByDescending(item => item.TotalPoints)
                .ThenBy(item => item.User.FullName)
                .Take(20)
                .ToListAsync();

            var payload = leaders.Select(item =>
            {
                var career = CalculateCareer(item.TotalPoints);
                return new
                {
                    item.UserId,
                    UserName = item.User.FullName ?? item.User.Email,
                    item.TotalPoints,
                    item.Level,
                    careerLevel = career.Level,
                    careerTitle = career.Title,
                    nextThreshold = career.NextThreshold
                };
            });

            return Ok(new { statusCode = 200, message = "Success", data = payload });
        }

        private Guid? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var id) ? id : null;
        }

        private static (int Level, string Title, int CurrentThreshold, int NextThreshold, int PointsIntoLevel, int ProgressPercent) CalculateCareer(int points)
        {
            var level = 1;
            var currentThreshold = 0;
            var nextThreshold = PointsForCareerLevel(level + 1);

            while (points >= nextThreshold)
            {
                level++;
                currentThreshold = nextThreshold;
                nextThreshold = PointsForCareerLevel(level + 1);
            }

            var span = Math.Max(1, nextThreshold - currentThreshold);
            var pointsIntoLevel = Math.Max(0, points - currentThreshold);
            var progress = Math.Min(100, (int)Math.Round((double)pointsIntoLevel / span * 100, MidpointRounding.AwayFromZero));

            return (level, CareerTitle(level), currentThreshold, nextThreshold, pointsIntoLevel, progress);
        }

        private static int PointsForCareerLevel(int level)
        {
            var normalizedLevel = Math.Max(1, level);
            return 250 * normalizedLevel * (normalizedLevel + 1);
        }

        private static string CareerTitle(int level)
        {
            if (level <= 1) return "Contributor";
            if (level == 2) return "Specialist";
            if (level == 3) return "Senior Specialist";
            if (level == 4) return "Lead";
            if (level == 5) return "Principal";
            return "Director";
        }

        private static RewardSummary BuildSummary(
            IEnumerable<dynamic> transactions,
            IEnumerable<dynamic> tasks,
            IEnumerable<dynamic> assignments,
            IEnumerable<dynamic> timeLogs)
        {
            var taskList = tasks.ToList();
            var assignmentList = assignments.ToList();
            var timeLogLookup = timeLogs.ToDictionary(item => (Guid)item.WorkTaskId, item => (double)item.LoggedHours);

            var completedTasks = taskList.Count(task => $"{task.StatusName ?? string.Empty}".ToUpperInvariant().Contains("DONE"));
            var earlyBonuses = transactions.Count(item => $"{item.TransactionType ?? string.Empty}".Contains("EarlyBonus", StringComparison.OrdinalIgnoreCase));
            var totalContributionWeight = assignmentList.Sum(item => Math.Max(0.0, (double)item.ContributionWeight));
            var contributionPercent = assignmentList.Count > 0
                ? Math.Round(totalContributionWeight / Math.Max(assignmentList.Count, 1) * 100, 1)
                : 0;
            var estimatedHours = assignmentList.Sum(item => Math.Max(0.0, (double)item.EstimatedHours));
            var actualHours = assignmentList.Sum(item => Math.Max(0.0, (double)item.TotalActualHours));
            var loggedHours = timeLogLookup.Values.Sum();

            var spotlightTasks = taskList
                .OrderByDescending(task => CalculateTaskFormula(task.StoryPoints, task.Priority, task.PlannedStartDate, task.PlannedEndDate, task.DueDate))
                .Take(4)
                .Select(task =>
                {
                    var assignment = assignmentList.FirstOrDefault(item => item.WorkTaskId == task.Id);
                    var formula = CalculateTaskFormula(task.StoryPoints, task.Priority, task.PlannedStartDate, task.PlannedEndDate, task.DueDate);
                    var share = assignment != null ? Math.Max(5, Math.Min(100, (int)Math.Round((double)assignment.ContributionWeight * 100, MidpointRounding.AwayFromZero))) : 100;
                    var assignmentEstimatedHours = assignment == null ? 0 : (double?)assignment.EstimatedHours ?? 0;
                    var assignmentActualHours = assignment == null ? 0 : (double?)assignment.TotalActualHours ?? 0;
                    var estimatedTaskHours = assignmentEstimatedHours > 0
                        ? assignmentEstimatedHours
                        : Math.Max(1, (double)task.TotalEstimatedHours);
                    var actualTaskHours = timeLogLookup.TryGetValue((Guid)task.Id, out var logged)
                        ? logged
                        : Math.Max(0.0, assignmentActualHours);
                    var efficiency = CalculateEfficiency(estimatedTaskHours, actualTaskHours);
                    var qualityModifier = CalculateQualityModifier(task.DueDate, $"{task.StatusName ?? string.Empty}");
                    var fairPoints = Math.Max(1, (int)Math.Round(formula * efficiency * qualityModifier * (share / 100.0), MidpointRounding.AwayFromZero));
                    return new
                    {
                        task.Id,
                        task.Title,
                        task.SequenceId,
                        task.Priority,
                        storyPoints = task.StoryPoints,
                        estimatedDays = EstimateDays(task.PlannedStartDate, task.PlannedEndDate, task.DueDate),
                        formulaPoints = formula,
                        contributionShare = share,
                        progressPercent = assignment != null ? (int)Math.Round((double)assignment.ProgressPercent, MidpointRounding.AwayFromZero) : ($"{task.StatusName ?? string.Empty}".ToUpperInvariant().Contains("DONE") ? 100 : 0),
                        estimatedHours = Math.Round(estimatedTaskHours, 1),
                        actualHours = Math.Round(actualTaskHours, 1),
                        efficiency = Math.Round(efficiency, 2),
                        qualityModifier = Math.Round(qualityModifier, 2),
                        fairPoints
                    };
                })
                .ToList();

            var recentAchievements = transactions
                .Where(item => item.Amount > 0)
                .Take(3)
                .Select(item => new
                {
                    item.Id,
                    title = item.TaskSequenceId ?? item.TaskTitle ?? "Work item",
                    item.Amount,
                    createdAt = item.CreatedAt,
                    item.Reason
                })
                .ToList();

            var sampleTask = spotlightTasks.FirstOrDefault();

            return new RewardSummary
            {
                CompletedTasks = completedTasks,
                EarlyBonuses = earlyBonuses,
                ContributionPercent = contributionPercent,
                SpotlightTasks = spotlightTasks,
                RecentAchievements = recentAchievements,
                SampleFormula = sampleTask == null
                    ? new
                    {
                        value = 3,
                        impact = 2,
                        days = 2,
                        total = 12,
                        note = "Task mau se xuat hien sau khi co du lieu that."
                    }
                    : new
                    {
                        value = Math.Max(1, (int)Math.Round((double)sampleTask.storyPoints, MidpointRounding.AwayFromZero)),
                        impact = PriorityImpact(sampleTask.Priority),
                        days = sampleTask.estimatedDays,
                        total = sampleTask.fairPoints,
                        note = $"Estimate {sampleTask.estimatedHours}h / Actual {sampleTask.actualHours}h / Efficiency x{sampleTask.efficiency} / Quality x{sampleTask.qualityModifier}."
                    }
            };
        }

        private static double CalculateEfficiency(double estimatedHours, double actualHours)
        {
            var safeEstimate = Math.Max(1, estimatedHours);
            if (actualHours <= 0)
            {
                return 1;
            }

            var ratio = safeEstimate / Math.Max(actualHours, 0.5);
            return Math.Clamp(ratio, 0.6, 1.25);
        }

        private static double CalculateQualityModifier(DateTime? dueDate, string statusName)
        {
            var normalized = statusName.ToUpperInvariant();
            var isDone = normalized.Contains("DONE") || normalized.Contains("COMPLETE");
            var overdue = dueDate.HasValue && DateTime.UtcNow.Date > dueDate.Value.Date;

            if (!isDone)
            {
                return overdue ? 0.75 : 0.9;
            }

            return overdue ? 0.9 : 1.1;
        }

        private static int CalculateTaskFormula(double storyPoints, int priority, DateTime? plannedStartDate, DateTime? plannedEndDate, DateTime? dueDate)
        {
            var value = Math.Max(1, (int)Math.Round(storyPoints <= 0 ? 1 : storyPoints, MidpointRounding.AwayFromZero));
            var impact = PriorityImpact(priority);
            var days = EstimateDays(plannedStartDate, plannedEndDate, dueDate);
            return value * impact * days;
        }

        private static int PriorityImpact(int priority)
        {
            return priority switch
            {
                1 => 4,
                2 => 3,
                3 => 2,
                _ => 1
            };
        }

        private static int EstimateDays(DateTime? plannedStartDate, DateTime? plannedEndDate, DateTime? dueDate)
        {
            var endDate = plannedEndDate ?? dueDate;
            if (plannedStartDate.HasValue && endDate.HasValue)
            {
                return Math.Max(1, (endDate.Value.Date - plannedStartDate.Value.Date).Days + 1);
            }

            return 1;
        }

        private sealed class RewardSummary
        {
            public int CompletedTasks { get; init; }
            public int EarlyBonuses { get; init; }
            public double ContributionPercent { get; init; }
            public double EstimatedHours { get; init; }
            public double ActualHours { get; init; }
            public double LoggedHours { get; init; }
            public object SampleFormula { get; init; } = null!;
            public IEnumerable<object> SpotlightTasks { get; init; } = Array.Empty<object>();
            public IEnumerable<object> RecentAchievements { get; init; } = Array.Empty<object>();
        }
    }
}
