-- Tắt kiểm tra khóa ngoại để xóa an toàn
EXEC sp_MSforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT all"
GO

DELETE FROM TimeLogs;
DELETE FROM TaskVectorEmbeddings;
DELETE FROM AITrainingDatasets;
DELETE FROM AIFeedbacks;
DELETE FROM AITokenUsages;
DELETE FROM AIPromptTemplates;
DELETE FROM PointTransactions;
DELETE FROM UserWallets;
DELETE FROM PerformanceReviews;
DELETE FROM Notifications;
DELETE FROM AuditLogs;
DELETE FROM Attachments;
DELETE FROM Comments;
DELETE FROM TaskDependencies;
DELETE FROM TaskAssignments;
DELETE FROM WorkTasks;
DELETE FROM TaskStatuses;
DELETE FROM TaskTypes;
DELETE FROM Sprints;
DELETE FROM ProjectMembers;
DELETE FROM Projects;
DELETE FROM DepartmentMembers;
DELETE FROM Departments;
DELETE FROM RolePermissions;
DELETE FROM Permissions;
DELETE FROM UserRoles;
DELETE FROM Roles;
DELETE FROM Users;

-- Bật lại kiểm tra khóa ngoại
EXEC sp_MSforeachtable "ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all"
GO
