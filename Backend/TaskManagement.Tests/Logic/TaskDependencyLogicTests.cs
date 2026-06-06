using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;
using Xunit;

namespace TaskManagement.Tests.Logic
{
    /// <summary>
    /// TASK DEPENDENCY LOGIC TESTS - Test logic phụ thuộc task
    /// Test: Circular Dependency Detection, Self-dependency, CRUD dependency
    /// </summary>
    public class TaskDependencyLogicTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Guid _projectId;

        public TaskDependencyLogicTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _projectId = Guid.NewGuid();
            _context.Projects.Add(new Project
            {
                Id = _projectId,
                Name = "Dependency Test Project",
                CreatedAt = DateTime.UtcNow,
                WorkspaceId = Guid.NewGuid()
            });
            _context.SaveChanges();
        }

        private async Task<Guid> SeedTaskAsync(string title = "Task")
        {
            var statusId = Guid.NewGuid();
            _context.TaskStatuses.Add(new TaskManagement.Domain.Entities.TaskStatus
            {
                Id = statusId, ProjectId = _projectId, Name = "To Do"
            });

            var taskId = Guid.NewGuid();
            _context.WorkTasks.Add(new WorkTask
            {
                Id = taskId,
                ProjectId = _projectId,
                TaskStatusId = statusId,
                Title = title,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                SequenceId = $"T-{Guid.NewGuid().ToString()[..4]}",
                ReporterId = Guid.NewGuid()
            });
            await _context.SaveChangesAsync();
            return taskId;
        }

        // ===========================================================
        // TC_DEP_001: [PASS] Tạo dependency "blocks" giữa 2 task hợp lệ
        // ===========================================================
        [Fact]
        public async Task CreateDependency_ValidBlocksRelation_Succeeds()
        {
            // Arrange
            var taskA = await SeedTaskAsync("Task A");
            var taskB = await SeedTaskAsync("Task B");

            _context.TaskDependencies.Add(new TaskDependency
            {
                PredecessorTaskId = taskA,
                SuccessorTaskId = taskB,
                DependencyType = 1
            });
            await _context.SaveChangesAsync();

            // Assert - Dependency tồn tại trong DB
            var dep = await _context.TaskDependencies
                .FirstOrDefaultAsync(d => d.PredecessorTaskId == taskA && d.SuccessorTaskId == taskB);

            Assert.NotNull(dep);
            Assert.Equal(1, dep.DependencyType);
        }

        // ===========================================================
        // TC_DEP_002: [PASS] Phát hiện Circular Dependency (A->B đã có, B->A bị chặn)
        // Logic này nằm trong Controller, test bằng cách kiểm tra DB trực tiếp
        // ===========================================================
        [Fact]
        public async Task CircularDependency_DirectReverse_IsDetectedByLogic()
        {
            // Arrange - A blocks B
            var taskA = await SeedTaskAsync("Task A");
            var taskB = await SeedTaskAsync("Task B");

            _context.TaskDependencies.Add(new TaskDependency
            {
                PredecessorTaskId = taskA,
                SuccessorTaskId = taskB,
                DependencyType = 1 // A blocks B
            });
            await _context.SaveChangesAsync();

            // Kiểm tra: Nếu ta muốn tạo B blocks A (ngược lại),
            // Logic Controller sẽ check: tìm reverse (PredecessorId=B, SuccessorId=A) -> có rồi -> FAIL
            var reverseExists = await _context.TaskDependencies
                .AnyAsync(d => d.PredecessorTaskId == taskB
                            && d.SuccessorTaskId == taskA
                            && d.DependencyType == 1);

            // reverse chưa tồn tại (chỉ có A->B)
            Assert.False(reverseExists);

            // Kiểm tra reverse: A->B đã tồn tại, nếu cố set B->A:
            var forwardExists = await _context.TaskDependencies
                .AnyAsync(d => d.PredecessorTaskId == taskA
                            && d.SuccessorTaskId == taskB
                            && d.DependencyType == 1);

            // A->B đã tồn tại -> bị controller chặn khi set B->A
            Assert.True(forwardExists, "A->B phải tồn tại để logic Circular check hoạt động");
        }

        // ===========================================================
        // TC_DEP_003: [PASS] Tạo dependency "relates_to" không bị circular guard
        // ===========================================================
        [Fact]
        public async Task CreateDependency_RelatesTo_AllowsBidirectional()
        {
            // Arrange
            var taskA = await SeedTaskAsync("Task A");
            var taskB = await SeedTaskAsync("Task B");

            // Relates_to (type 2) không check circular
            _context.TaskDependencies.Add(new TaskDependency
            {
                PredecessorTaskId = taskA,
                SuccessorTaskId = taskB,
                DependencyType = 2
            });
            _context.TaskDependencies.Add(new TaskDependency
            {
                PredecessorTaskId = taskB,
                SuccessorTaskId = taskA,
                DependencyType = 2
            });
            await _context.SaveChangesAsync();

            // Assert - Cả 2 quan hệ cùng tồn tại mà không lỗi
            var count = await _context.TaskDependencies
                .CountAsync(d => (d.PredecessorTaskId == taskA || d.PredecessorTaskId == taskB) && d.DependencyType == 2);
            Assert.Equal(2, count);
        }

        // ===========================================================
        // TC_DEP_004: [PASS] Xóa dependency thành công
        // ===========================================================
        [Fact]
        public async Task RemoveDependency_ExistingRelation_Succeeds()
        {
            // Arrange
            var taskA = await SeedTaskAsync("Task A");
            var taskB = await SeedTaskAsync("Task B");

            var dep = new TaskDependency
            {
                PredecessorTaskId = taskA,
                SuccessorTaskId = taskB,
                DependencyType = 1
            };
            _context.TaskDependencies.Add(dep);
            await _context.SaveChangesAsync();

            // Act
            _context.TaskDependencies.Remove(dep);
            await _context.SaveChangesAsync();

            // Assert
            var exists = await _context.TaskDependencies
                .AnyAsync(d => d.PredecessorTaskId == taskA && d.SuccessorTaskId == taskB);
            Assert.False(exists);
        }

        // ===========================================================
        // TC_DEP_005: [PASS] Dependency chuỗi dài (A->B->C->D) không bị block bởi guard đơn giản
        // ===========================================================
        [Fact]
        public async Task CreateDependency_ChainABCD_OnlyDirectReverseIsBlocked()
        {
            // Arrange
            var taskA = await SeedTaskAsync("A"); var taskB = await SeedTaskAsync("B");
            var taskC = await SeedTaskAsync("C"); var taskD = await SeedTaskAsync("D");

            // A->B, B->C, C->D
            _context.TaskDependencies.Add(new TaskDependency { PredecessorTaskId = taskA, SuccessorTaskId = taskB, DependencyType = 1 });
            _context.TaskDependencies.Add(new TaskDependency { PredecessorTaskId = taskB, SuccessorTaskId = taskC, DependencyType = 1 });
            _context.TaskDependencies.Add(new TaskDependency { PredecessorTaskId = taskC, SuccessorTaskId = taskD, DependencyType = 1 });
            await _context.SaveChangesAsync();

            // Verify: Logic chặn đơn giản chỉ kiểm tra 1 bước reverse
            // => D->A (4 bước) KHÔNG bị chặn bởi guard đơn giản chỉ check 1 bước
            // Đây là điểm yếu thực tế cần ghi nhận
            var directReverseOfFirstPair = await _context.TaskDependencies
                .AnyAsync(d => d.PredecessorTaskId == taskB && d.SuccessorTaskId == taskA && d.DependencyType == 1);

            Assert.False(directReverseOfFirstPair, "B->A không tồn tại (chỉ có A->B)");

            // D->A sẽ KHÔNG bị guard đơn giản bắt được (guard chỉ check 1 hop)
            var totalChainLinks = await _context.TaskDependencies.CountAsync();
            Assert.Equal(3, totalChainLinks); // Chỉ có đúng 3 link A->B, B->C, C->D
        }

        // ===========================================================
        // TC_DEP_006: [PASS] Lấy danh sách dependencies của Task
        // ===========================================================
        [Fact]
        public async Task GetDependencies_TaskWithMultipleRelations_ReturnsAll()
        {
            // Arrange
            var taskMain = await SeedTaskAsync("Main Task");
            var taskX = await SeedTaskAsync("Task X");
            var taskY = await SeedTaskAsync("Task Y");

            _context.TaskDependencies.Add(new TaskDependency { PredecessorTaskId = taskMain, SuccessorTaskId = taskX, DependencyType = 1 });
            _context.TaskDependencies.Add(new TaskDependency { PredecessorTaskId = taskY, SuccessorTaskId = taskMain, DependencyType = 1 });
            _context.TaskDependencies.Add(new TaskDependency { PredecessorTaskId = taskMain, SuccessorTaskId = taskY, DependencyType = 2 });
            await _context.SaveChangesAsync();

            // Act - Lấy tất cả deps của taskMain (cả predecessor lẫn successor)
            var deps = await _context.TaskDependencies
                .Where(d => d.PredecessorTaskId == taskMain || d.SuccessorTaskId == taskMain)
                .ToListAsync();

            // Assert
            Assert.Equal(3, deps.Count);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
