const fs = require('fs');
const path = require('path');

function generateGuid(index, prefix) {
    // A simple deterministic GUID generator based on index and prefix
    const hex = index.toString(16).padStart(4, '0');
    return `${prefix}0000-0000-0000-0000-00000000${hex}`;
}

const roles = [
    'PM', 'PO', 'DEV', 'SM', 'PA', 'Accountant', 'Marketing', 'System Admin', 
    'QA/Tester', 'UI/UX Designer', 'Stakeholder/Client', 'DevOps / Deployment',
    'HR Manager', 'Sales Representative', 'Customer Support', 'Business Analyst',
    'Content Writer', 'Legal Advisor', 'Data Scientist', 'Security Engineer'
];

let sql = `-- =======================================================
-- SEED DATA SCRIPT FOR TASK MANAGEMENT DATABASE
-- Generates at least 20 rows per table with explicit FKs
-- =======================================================\n\n`;

const USERS_PREFIX = '11111111';
const ROLES_PREFIX = '22222222';
const PERM_PREFIX = '33333333';
const DEPT_PREFIX = '44444444';
const PROJ_PREFIX = '55555555';
const SPRINT_PREFIX = '66666666';
const TYPE_PREFIX = '77777777';
const STAT_PREFIX = '88888888';
const TASK_PREFIX = '99999999';
const COMM_PREFIX = 'aaaaaaaa';
const ATTA_PREFIX = 'bbbbbbbb';
const AUDIT_PREFIX = 'cccccccc';
const NOTIF_PREFIX = 'dddddddd';
const REVIEW_PREFIX = 'eeeeeeee';
const WALLET_PREFIX = 'ffffffff'; // Actually user id, so we use user ids
const TRAN_PREFIX = '10101010';
const PROMPT_PREFIX = '20202020';
const USAGE_PREFIX = '30303030';
const FEED_PREFIX = '40404040';
const DATASET_PREFIX = '50505050';
const TIME_PREFIX = '70707070';
const ROW_COUNT = 20;

// 1. Users
sql += `-- Users\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, USERS_PREFIX);
    sql += `INSERT INTO Users (Id, Email, PasswordHash, FullName, IsActive, CreatedAt, UpdatedAt) VALUES ('${id}', 'user${i}@test.com', 'hash${i}', 'User ${i}', 1, GETDATE(), GETDATE());\n`;
}
sql += '\n';

// 2. Roles
sql += `-- Roles\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, ROLES_PREFIX);
    sql += `INSERT INTO Roles (Id, Name, Description) VALUES ('${id}', '${roles[i-1]}', 'Description for ${roles[i-1]}');\n`;
}
sql += '\n';

// 3. UserRoles
sql += `-- UserRoles\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let uid = generateGuid(i, USERS_PREFIX);
    let rid = generateGuid(i, ROLES_PREFIX);
    sql += `INSERT INTO UserRoles (UserId, RoleId) VALUES ('${uid}', '${rid}');\n`;
}
sql += '\n';

// 4. Permissions
sql += `-- Permissions\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, PERM_PREFIX);
    sql += `INSERT INTO Permissions (Id, Name, Code, Module) VALUES ('${id}', 'Permission ${i}', 'PERM_${i}', 'Module${(i%5)+1}');\n`;
}
sql += '\n';

// 5. RolePermissions
sql += `-- RolePermissions\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let rid = generateGuid(i, ROLES_PREFIX);
    let pid = generateGuid(i, PERM_PREFIX);
    sql += `INSERT INTO RolePermissions (RoleId, PermissionId) VALUES ('${rid}', '${pid}');\n`;
}
sql += '\n';

// 6. Departments
sql += `-- Departments\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, DEPT_PREFIX);
    let managerId = generateGuid(i, USERS_PREFIX); // Use user i as manager
    sql += `INSERT INTO Departments (Id, Name, Description, ManagerId, CreatedAt, UpdatedAt) VALUES ('${id}', 'Department ${i}', 'Dept Desc ${i}', '${managerId}', GETDATE(), GETDATE());\n`;
}
sql += '\n';

// 7. DepartmentMembers
sql += `-- DepartmentMembers\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let did = generateGuid(i, DEPT_PREFIX);
    let uid = generateGuid(i, USERS_PREFIX);
    sql += `INSERT INTO DepartmentMembers (DepartmentId, UserId, JoinedAt, RoleInDepartment) VALUES ('${did}', '${uid}', GETDATE(), 'Member');\n`;
}
sql += '\n';

// 8. Projects
sql += `-- Projects\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, PROJ_PREFIX);
    let creatorId = generateGuid(i, USERS_PREFIX);
    sql += `INSERT INTO Projects (Id, Name, Description, Status, CreatorId, CreatedAt, UpdatedAt) VALUES ('${id}', 'Project ${i}', 'Proj Desc ${i}', ${(i%3)}, '${creatorId}', GETDATE(), GETDATE());\n`;
}
sql += '\n';

// 9. ProjectMembers
sql += `-- ProjectMembers\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let pid = generateGuid(i, PROJ_PREFIX);
    let uid = generateGuid(i, USERS_PREFIX);
    sql += `INSERT INTO ProjectMembers (ProjectId, UserId, RoleInProject, JoinedAt) VALUES ('${pid}', '${uid}', 'Developer', GETDATE());\n`;
}
sql += '\n';

// 10. Sprints
sql += `-- Sprints\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, SPRINT_PREFIX);
    let pid = generateGuid(i, PROJ_PREFIX); // link sprint i to project i
    sql += `INSERT INTO Sprints (Id, Name, StartDate, EndDate, Goal, ProjectId, Status, CreatedAt, UpdatedAt) VALUES ('${id}', 'Sprint ${i}', GETDATE(), DATEADD(day, 14, GETDATE()), 'Goal ${i}', '${pid}', ${(i%2)}, GETDATE(), GETDATE());\n`;
}
sql += '\n';

// 11. TaskTypes
sql += `-- TaskTypes\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, TYPE_PREFIX);
    let pid = generateGuid(i, PROJ_PREFIX);
    sql += `INSERT INTO TaskTypes (Id, Name, ColorCode, Icon, ProjectId) VALUES ('${id}', 'Type ${i}', '#FFFFFF', 'icon-${i}', '${pid}');\n`;
}
sql += '\n';

// 12. TaskStatuses
sql += `-- TaskStatuses\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, STAT_PREFIX);
    let pid = generateGuid(i, PROJ_PREFIX);
    sql += `INSERT INTO TaskStatuses (Id, Name, ColorCode, Position, ProjectId) VALUES ('${id}', 'Status ${i}', '#000000', ${i}, '${pid}');\n`;
}
sql += '\n';

// 13. WorkTasks
sql += `-- WorkTasks\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, TASK_PREFIX);
    let pid = generateGuid((i%20)+1, PROJ_PREFIX);
    let sid = generateGuid((i%20)+1, SPRINT_PREFIX);
    let tid = generateGuid((i%20)+1, TYPE_PREFIX);
    let tsid = generateGuid((i%20)+1, STAT_PREFIX);
    let rid = generateGuid((i%20)+1, USERS_PREFIX);
    sql += `INSERT INTO WorkTasks (Id, Title, Description, Priority, StoryPoints, StartDate, DueDate, IsCompleted, ProjectId, SprintId, TaskTypeId, TaskStatusId, ReporterId, OriginalEstimate, RemainingEstimate, TimeSpent, CreatedAt, UpdatedAt) VALUES ('${id}', 'Task ${i}', 'Desc ${i}', ${(i%4)}, ${i%8}, GETDATE(), DATEADD(day, 5, GETDATE()), 0, '${pid}', '${sid}', '${tid}', '${tsid}', '${rid}', 8, 4, 4, GETDATE(), GETDATE());\n`;
}
sql += '\n';

// 14. TaskAssignments
sql += `-- TaskAssignments\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let tid = generateGuid(i, TASK_PREFIX);
    let uid = generateGuid(i, USERS_PREFIX);
    sql += `INSERT INTO TaskAssignments (WorkTaskId, UserId, AssignedAt) VALUES ('${tid}', '${uid}', GETDATE());\n`;
}
sql += '\n';

// 15. TaskDependencies
sql += `-- TaskDependencies\n`;
for (let i = 1; i <= ROW_COUNT-1; i++) {
    // Make task i depend on task i+1
    let pid = generateGuid(i+1, TASK_PREFIX);
    let sid = generateGuid(i, TASK_PREFIX);
    sql += `INSERT INTO TaskDependencies (PredecessorTaskId, SuccessorTaskId, DependencyType) VALUES ('${pid}', '${sid}', 0);\n`;
}
// 20th row specifically
sql += `INSERT INTO TaskDependencies (PredecessorTaskId, SuccessorTaskId, DependencyType) VALUES ('${generateGuid(1, TASK_PREFIX)}', '${generateGuid(20, TASK_PREFIX)}', 0);\n`;
sql += '\n';

// 16. Comments
sql += `-- Comments\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, COMM_PREFIX);
    let tid = generateGuid(i, TASK_PREFIX);
    let uid = generateGuid(i, USERS_PREFIX);
    sql += `INSERT INTO Comments (Id, Content, WorkTaskId, UserId, CreatedAt, UpdatedAt, IsEdited) VALUES ('${id}', 'Comment ${i}', '${tid}', '${uid}', GETDATE(), GETDATE(), 0);\n`;
}
sql += '\n';

// 17. Attachments
sql += `-- Attachments\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, ATTA_PREFIX);
    let tid = generateGuid(i, TASK_PREFIX);
    let uid = generateGuid(i, USERS_PREFIX);
    sql += `INSERT INTO Attachments (Id, FileName, FileUrl, FileSize, FileType, WorkTaskId, UserId, UploadedAt) VALUES ('${id}', 'file${i}.pdf', '/files/file${i}.pdf', 1024, 'application/pdf', '${tid}', '${uid}', GETDATE());\n`;
}
sql += '\n';

// 18. AuditLogs
sql += `-- AuditLogs\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, AUDIT_PREFIX);
    let tid = generateGuid(i, TASK_PREFIX);
    let uid = generateGuid(i, USERS_PREFIX);
    sql += `INSERT INTO AuditLogs (Id, Action, EntityName, EntityId, OldValues, NewValues, UserId, WorkTaskId, Timestamp) VALUES ('${id}', 'Update', 'WorkTask', '${tid}', '{}', '{}', '${uid}', '${tid}', GETDATE());\n`;
}
sql += '\n';

// 19. Notifications
sql += `-- Notifications\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, NOTIF_PREFIX);
    let uid = generateGuid(i, USERS_PREFIX);
    sql += `INSERT INTO Notifications (Id, Title, Content, Type, IsRead, UserId, CreatedAt, ReferenceId, ReferenceType) VALUES ('${id}', 'Notif ${i}', 'Content ${i}', 0, 0, '${uid}', GETDATE(), NULL, NULL);\n`;
}
sql += '\n';

// 20. PerformanceReviews
sql += `-- PerformanceReviews\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, REVIEW_PREFIX);
    let reviewer = generateGuid(i, USERS_PREFIX);
    // reviewee is next user, or user 1 for the last one
    let reviewee = generateGuid(i === ROW_COUNT ? 1 : i + 1, USERS_PREFIX);
    sql += `INSERT INTO PerformanceReviews (Id, ReviewerId, RevieweeId, Score, Feedback, ReviewPeriod, CreatedAt) VALUES ('${id}', '${reviewer}', '${reviewee}', ${(i%10)+1}, 'Feedback ${i}', '2026-Q1', GETDATE());\n`;
}
sql += '\n';

// 21. UserWallets
sql += `-- UserWallets\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let uid = generateGuid(i, USERS_PREFIX);
    sql += `INSERT INTO UserWallets (UserId, Balance, TotalEarned, TotalSpent, UpdatedAt) VALUES ('${uid}', 1000, 1000, 0, GETDATE());\n`;
}
sql += '\n';

// 22. PointTransactions
sql += `-- PointTransactions\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, TRAN_PREFIX);
    let uid = generateGuid(i, USERS_PREFIX);
    sql += `INSERT INTO PointTransactions (Id, UserWalletUserId, Amount, TransactionType, ReferenceId, Description, CreatedAt) VALUES ('${id}', '${uid}', 10, 0, NULL, 'Transaction ${i}', GETDATE());\n`;
}
sql += '\n';

// 23. AIPromptTemplates
sql += `-- AIPromptTemplates\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, PROMPT_PREFIX);
    sql += `INSERT INTO AIPromptTemplates (Id, Name, SystemPrompt, UserPromptTemplate, Description, IsActive, CreatedAt, UpdatedAt) VALUES ('${id}', 'Prompt ${i}', 'Sys prompt', 'User prompt', 'Desc', 1, GETDATE(), GETDATE());\n`;
}
sql += '\n';

// 24. AITokenUsages
sql += `-- AITokenUsages\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, USAGE_PREFIX);
    let uid = generateGuid(i, USERS_PREFIX);
    sql += `INSERT INTO AITokenUsages (Id, UserId, Feature, PromptTokens, CompletionTokens, TotalTokens, ModelName, RequestTime, ProcessingTimeMs) VALUES ('${id}', '${uid}', 'Feature ${i}', 10, 20, 30, 'Model', GETDATE(), 1000);\n`;
}
sql += '\n';

// 25. AIFeedbacks
sql += `-- AIFeedbacks\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, FEED_PREFIX);
    let uid = generateGuid(i, USERS_PREFIX);
    sql += `INSERT INTO AIFeedbacks (Id, UserId, Feature, IsHelpful, FeedbackText, RequestPayload, ResponsePayload, CreatedAt) VALUES ('${id}', '${uid}', 'Feature ${i}', 1, 'Helpful', '{}', '{}', GETDATE());\n`;
}
sql += '\n';

// 26. AITrainingDatasets
sql += `-- AITrainingDatasets\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, DATASET_PREFIX);
    let uid = generateGuid(i, USERS_PREFIX);
    sql += `INSERT INTO AITrainingDatasets (Id, DataType, OriginalContent, ImprovedContent, Status, CreatorId, CreatedAt, UpdatedAt) VALUES ('${id}', 0, 'Origin', 'Improved', 0, '${uid}', GETDATE(), GETDATE());\n`;
}
sql += '\n';

// 27. TaskVectorEmbeddings
sql += `-- TaskVectorEmbeddings\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let tid = generateGuid(i, TASK_PREFIX);
    sql += `INSERT INTO TaskVectorEmbeddings (WorkTaskId, VectorData, ModelVersion, LastUpdated) VALUES ('${tid}', '[]', 'v1', GETDATE());\n`;
}
sql += '\n';

// 28. TimeLogs
sql += `-- TimeLogs\n`;
for (let i = 1; i <= ROW_COUNT; i++) {
    let id = generateGuid(i, TIME_PREFIX);
    let tid = generateGuid(i, TASK_PREFIX);
    let uid = generateGuid(i, USERS_PREFIX);
    sql += `INSERT INTO TimeLogs (Id, WorkTaskId, UserId, TimeSpent, Description, LogDate, CreatedAt, UpdatedAt) VALUES ('${id}', '${tid}', '${uid}', 2, 'Log ${i}', GETDATE(), GETDATE(), GETDATE());\n`;
}
sql += '\n';

fs.writeFileSync(path.join(__dirname, 'seed_data.sql'), sql);
console.log('Successfully generated seed_data.sql');
