using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Services;
using Xunit;

namespace TaskManagement.Tests.Logic
{
    /// <summary>
    /// GAMIFICATION LOGIC TESTS - Test điểm thưởng / phạt
    /// Kiểm tra nghiệp vụ: Cộng điểm, Trừ điểm, Level Up, Idempotency (không cộng 2 lần)
    /// </summary>
    public class GamificationLogicTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GamificationService _gamificationService;
        private readonly Guid _projectId;
        private readonly Guid _userId;
        private readonly Guid _reporterId;

        public GamificationLogicTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            _context = new ApplicationDbContext(options);
            _gamificationService = new GamificationService(_context);

            _projectId = Guid.NewGuid();
            _userId = Guid.NewGuid();
            _reporterId = Guid.NewGuid();

            // Seed dữ liệu nền
            _context.Projects.Add(new Project
            {
                Id = _projectId,
                Name = "Test Project Gamification",
                CreatedAt = DateTime.UtcNow,
                WorkspaceId = Guid.NewGuid()
            });
            _context.SaveChanges();
        }

        private async Task<Guid> SeedTaskAsync(DateTime? dueDate = null, double storyPoints = 5, bool noAssignment = true)
        {
            var taskId = Guid.NewGuid();
            var statusId = Guid.NewGuid();

            _context.TaskStatuses.Add(new TaskManagement.Domain.Entities.TaskStatus
            {
                Id = statusId, ProjectId = _projectId, Name = "In Progress"
            });

            _context.WorkTasks.Add(new WorkTask
            {
                Id = taskId,
                ProjectId = _projectId,
                TaskStatusId = statusId,
                Title = "Test Task",
                StoryPoints = (float)storyPoints,
                DueDate = dueDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                SequenceId = $"TC-{Guid.NewGuid().ToString()[..4]}",
                ReporterId = _reporterId,
                AssignedUserId = _userId
            });

            await _context.SaveChangesAsync();
            return taskId;
        }

        // ===========================================================
        // TC_GAMIFY_001: [PASS] Cộng điểm khi Task chuyển sang Done (không quá hạn)
        // ===========================================================
        [Fact]
        public async Task ApplyStatusChange_TodoToDone_AddsPointsToWallet()
        {
            // Arrange
            var taskId = await SeedTaskAsync(dueDate: DateTime.Now.AddDays(5)); // Chưa quá hạn

            // Act
            await _gamificationService.ApplyStatusChangeRewardsAsync(taskId, _userId, "In Progress", "Done");

            // Assert
            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == _userId);
            Assert.NotNull(wallet);
            Assert.True(wallet.TotalPoints > 0, "Wallet phải có điểm sau khi Done task");
        }

        // ===========================================================
        // TC_GAMIFY_002: [PASS] Không cộng điểm 2 lần (Idempotency)
        // ===========================================================
        [Fact]
        public async Task ApplyStatusChange_DoneTwice_OnlyRewardsOnce()
        {
            // Arrange
            var taskId = await SeedTaskAsync(dueDate: DateTime.Now.AddDays(5));

            // Act - Lần 1
            await _gamificationService.ApplyStatusChangeRewardsAsync(taskId, _userId, "In Progress", "Done");
            var wallet1 = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == _userId);
            var pointsAfterFirst = wallet1?.TotalPoints ?? 0;

            // Act - Lần 2 (kéo về ToDo rồi Done lại)
            await _gamificationService.ApplyStatusChangeRewardsAsync(taskId, _userId, "Done", "In Progress"); // Rollback
            await _gamificationService.ApplyStatusChangeRewardsAsync(taskId, _userId, "In Progress", "Done"); // Done lần 2

            var wallet2 = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == _userId);

            // Assert - Tổng điểm sau rollback+redo không được lớn hơn lần đầu
            Assert.NotNull(wallet2);
            // Điểm sau rollback = 0, sau khi done lại = pointsAfterFirst
            Assert.True(wallet2.TotalPoints <= pointsAfterFirst,
                $"Điểm không được tăng thêm lần 2: {wallet2.TotalPoints} should be <= {pointsAfterFirst}");
        }

        // ===========================================================
        // TC_GAMIFY_003: [PASS] Trừ điểm khi Rollback Done -> ToDo (điểm không âm)
        // ===========================================================
        [Fact]
        public async Task ApplyStatusChange_DoneToTodo_RollbackPoints_WalletNotNegative()
        {
            // Arrange
            var taskId = await SeedTaskAsync(dueDate: DateTime.Now.AddDays(5));

            // First - earn some points
            await _gamificationService.ApplyStatusChangeRewardsAsync(taskId, _userId, "In Progress", "Done");

            // Act - Rollback
            await _gamificationService.ApplyStatusChangeRewardsAsync(taskId, _userId, "Done", "In Progress");

            // Assert - Ví không bao giờ âm
            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == _userId);
            Assert.NotNull(wallet);
            Assert.True(wallet.TotalPoints >= 0, "Ví điểm không được âm sau Rollback!");
        }

        // ===========================================================
        // TC_GAMIFY_004: [PASS] Không cộng điểm khi Status không đổi (Todo -> Todo)
        // ===========================================================
        [Fact]
        public async Task ApplyStatusChange_SameStatus_NoPointsAdded()
        {
            // Arrange
            var taskId = await SeedTaskAsync();

            // Act - Cùng trạng thái -> không có gì xảy ra
            await _gamificationService.ApplyStatusChangeRewardsAsync(taskId, _userId, "In Progress", "In Progress");

            // Assert - Không có wallet nào được tạo
            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == _userId);
            Assert.Null(wallet); // Không có giao dịch gì cả
        }

        // ===========================================================
        // TC_GAMIFY_005: [PASS] Task không tồn tại -> ThrowException
        // ===========================================================
        [Fact]
        public async Task ApplyStatusChange_NonExistentTask_ThrowsArgumentException()
        {
            // Arrange
            var fakeTaskId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _gamificationService.ApplyStatusChangeRewardsAsync(fakeTaskId, _userId, "In Progress", "Done"));
        }

        // ===========================================================
        // TC_GAMIFY_006: [PASS] Xác nhận CalculateLevel logic
        //   Level 1: 0-499, Level 2: 500+, Level 3: 1500+...
        //   Formula: Level N threshold = 250 * N * (N + 1)
        //   Level 2 = 250*2*3 = 1500? Let's verify via test
        // ===========================================================
        [Fact]
        public async Task ApplyStatusChange_ManyTasks_LevelIncreasesCorrectly()
        {
            // Arrange - tạo nhiều task để earn đủ điểm level up
            var walletEntity = new UserWallet
            {
                UserId = _userId,
                TotalPoints = 1498, // Sắp chạm mốc (Level 2 threshold = 1500)
                Level = 1
            };
            _context.UserWallets.Add(walletEntity);
            await _context.SaveChangesAsync();

            // Task mới với đủ điểm để push qua mốc
            var taskId = await SeedTaskAsync(dueDate: DateTime.Now.AddDays(5), storyPoints: 20);

            // Act
            await _gamificationService.ApplyStatusChangeRewardsAsync(taskId, _userId, "In Progress", "Done");

            // Assert - Level phải tăng lên 2
            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == _userId);
            Assert.NotNull(wallet);
            Assert.True(wallet.Level >= 2, $"Level nên >= 2 sau khi vượt mốc. Hiện tại: Level {wallet.Level}, Points: {wallet.TotalPoints}");
        }

        // ===========================================================
        // TC_GAMIFY_007: [PASS] Task có ConTask (subtask) -> Không cộng điểm
        // ===========================================================
        [Fact]
        public async Task ApplyStatusChange_TaskWithChildren_SkipsReward()
        {
            // Arrange - Tạo task cha
            var parentStatusId = Guid.NewGuid();
            _context.TaskStatuses.Add(new TaskManagement.Domain.Entities.TaskStatus
            {
                Id = parentStatusId, ProjectId = _projectId, Name = "In Progress"
            });

            var parentTaskId = Guid.NewGuid();
            _context.WorkTasks.Add(new WorkTask
            {
                Id = parentTaskId,
                ProjectId = _projectId,
                TaskStatusId = parentStatusId,
                Title = "Parent Task",
                DueDate = DateTime.Now.AddDays(5),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                SequenceId = "PT-001",
                ReporterId = _reporterId,
                AssignedUserId = _userId
            });

            // Tạo subtask (con)
            var childStatusId = Guid.NewGuid();
            _context.TaskStatuses.Add(new TaskManagement.Domain.Entities.TaskStatus
            {
                Id = childStatusId, ProjectId = _projectId, Name = "To Do"
            });
            _context.WorkTasks.Add(new WorkTask
            {
                Id = Guid.NewGuid(),
                ProjectId = _projectId,
                ParentTaskId = parentTaskId, // ← Là subtask của parentTask
                TaskStatusId = childStatusId,
                Title = "Subtask con",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                SequenceId = "PT-001-S1",
                ReporterId = _reporterId
            });
            await _context.SaveChangesAsync();

            // Act
            await _gamificationService.ApplyStatusChangeRewardsAsync(parentTaskId, _userId, "In Progress", "Done");

            // Assert - Không được cộng điểm vì Task cha có con
            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == _userId);
            Assert.Null(wallet); // Không có ví được tạo -> không cộng điểm
        }

        // ===========================================================
        // TC_GAMIFY_008: [PASS] Phạt điểm khi Task đã quá hạn (LatePenalty)
        // ===========================================================
        [Fact]
        public async Task ApplyDueDatePenaltyAsync_OverdueTask_DeductsPoints()
        {
            // Arrange - Task quá hạn
            var taskId = await SeedTaskAsync(dueDate: DateTime.Now.AddDays(-5)); // Quá hạn 5 ngày trước

            // Tạo ví trước với 100 điểm
            _context.UserWallets.Add(new UserWallet
            {
                UserId = _userId, TotalPoints = 100, Level = 1
            });
            await _context.SaveChangesAsync();

            // Act
            await _gamificationService.ApplyDueDatePenaltyAsync(taskId, Guid.NewGuid());

            // Assert - Điểm phải bị trừ
            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == _userId);
            Assert.NotNull(wallet);
            Assert.True(wallet.TotalPoints < 100, $"Điểm phải bị trừ từ 100. Hiện tại: {wallet.TotalPoints}");
            Assert.True(wallet.TotalPoints >= 0, "Ví không được âm!");
        }

        // ===========================================================
        // TC_GAMIFY_009: [PASS] Không phạt điểm 2 lần cho cùng 1 Task (Idempotency Penalty)
        // ===========================================================
        [Fact]
        public async Task ApplyDueDatePenaltyAsync_CalledTwice_OnlyPenalizesOnce()
        {
            // Arrange
            var taskId = await SeedTaskAsync(dueDate: DateTime.Now.AddDays(-5));

            _context.UserWallets.Add(new UserWallet
            {
                UserId = _userId, TotalPoints = 200, Level = 1
            });
            await _context.SaveChangesAsync();

            // Act - Gọi 2 lần
            await _gamificationService.ApplyDueDatePenaltyAsync(taskId, Guid.NewGuid());
            var wallet1 = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == _userId);
            var pointsAfterFirst = wallet1!.TotalPoints;

            await _gamificationService.ApplyDueDatePenaltyAsync(taskId, Guid.NewGuid()); // Lần 2
            var wallet2 = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == _userId);

            // Assert - Điểm không bị trừ thêm lần 2
            Assert.Equal(pointsAfterFirst, wallet2!.TotalPoints);
        }

        // ===========================================================
        // TC_GAMIFY_010: [PASS] Ví điểm không âm dù bị phạt nhiều lần
        // ===========================================================
        [Fact]
        public async Task ApplyDueDatePenalty_TaskWithZeroPoints_WalletStaysAtZero()
        {
            // Arrange - Ví chỉ có 5 điểm nhưng penalty sẽ cao hơn
            var taskId = await SeedTaskAsync(dueDate: DateTime.Now.AddDays(-10), storyPoints: 100);

            _context.UserWallets.Add(new UserWallet
            {
                UserId = _userId, TotalPoints = 5, Level = 1
            });
            await _context.SaveChangesAsync();

            // Act
            await _gamificationService.ApplyDueDatePenaltyAsync(taskId, Guid.NewGuid());

            // Assert - Ví sàn là 0, không được âm
            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserId == _userId);
            Assert.NotNull(wallet);
            Assert.Equal(0, wallet.TotalPoints);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
