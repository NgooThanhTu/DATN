const fs = require('fs');
const path = require('path');

const seedFile = path.resolve(__dirname, 'Backend/seed_data.sql');
let content = fs.readFileSync(seedFile, 'utf8');

// Insert default workspace at the top before Projects
const workspaceSql = `

    -- =======================================================
    -- Workspace Data
    -- =======================================================
    INSERT INTO Workspaces (Id, Name, Slug, OwnerId, Timezone, CreatedAt, UpdatedAt, IsDeleted)
    VALUES ('77777777-0000-0000-0000-000000000001', 'Default Workspace', 'default-ws', '11111111-0000-0000-0000-000000000001', 'Asia/Ho_Chi_Minh', GETDATE(), GETDATE(), 0);

    INSERT INTO WorkspaceMembers (WorkspaceId, UserId, WorkspaceRole, JoinedAt, IsActive)
    VALUES ('77777777-0000-0000-0000-000000000001', '11111111-0000-0000-0000-000000000001', 'OWNER', GETDATE(), 1);

    -- Add other users to workspace
`;

// Insert the workspace SQL right after the Users and Roles (let's say before Projects, or just at line 6 or somewhere)
content = content.replace('-- Projects', workspaceSql + '\n    -- Projects');

// Replace Projects inserts
content = content.replace(/INSERT INTO Projects \(Id, Name/g, 'INSERT INTO Projects (Id, WorkspaceId, Identifier, IssueSequence, NetworkType, Name');
content = content.replace(/VALUES \('55555555-0000-0000-0000-0000000000(\w{2})',/g, "VALUES ('55555555-0000-0000-0000-0000000000$1', '77777777-0000-0000-0000-000000000001', 'PRJ$1', 0, 'Public',");

// Replace WorkTasks inserts
content = content.replace(/INSERT INTO WorkTasks \(Id, ProjectId/g, 'INSERT INTO WorkTasks (Id, WorkspaceId, SequenceId, SortOrder, ProjectId');
content = content.replace(/VALUES \('66666666-0000-0000-0000-0000000000(\w{2})',/g, "VALUES ('66666666-0000-0000-0000-0000000000$1', '77777777-0000-0000-0000-000000000001', 'CUN-$1', 65536,");

// Add some new tables data at the very end
content += `
    -- =======================================================
    -- New Plane Entities (Labels, Modules, Intakes)
    -- =======================================================
    INSERT INTO Labels (Id, ProjectId, WorkspaceId, Name, ColorCode, CreatedAt)
    VALUES 
        (NEWID(), '55555555-0000-0000-0000-000000000001', '77777777-0000-0000-0000-000000000001', 'Bug', '#ef4444', GETDATE()),
        (NEWID(), '55555555-0000-0000-0000-000000000001', '77777777-0000-0000-0000-000000000001', 'Feature', '#3b82f6', GETDATE()),
        (NEWID(), '55555555-0000-0000-0000-000000000001', '77777777-0000-0000-0000-000000000001', 'Urgent', '#f59e0b', GETDATE());

    INSERT INTO Modules (Id, ProjectId, Name, Description, Status, CreatedAt, UpdatedAt)
    VALUES 
        (NEWID(), '55555555-0000-0000-0000-000000000001', 'Authentication System', 'Login, OAuth, and Registration', 'InProgress', GETDATE(), GETDATE()),
        (NEWID(), '55555555-0000-0000-0000-000000000001', 'Kanban Board Revamp', 'Migrating to Vue 3 and LexoRank', 'Planned', GETDATE(), GETDATE());

    INSERT INTO Intakes (Id, ProjectId, Title, Description, Source, Status, SubmittedById, CreatedAt)
    VALUES
        (NEWID(), '55555555-0000-0000-0000-000000000001', 'User feedback on dark mode', 'They said the contrast is too low', 'MANUAL', 'Pending', '11111111-0000-0000-0000-000000000002', GETDATE()),
        (NEWID(), '55555555-0000-0000-0000-000000000001', 'Bug: Cannot upload avatar', 'Upload fails with 500', 'FORM', 'Accepted', '11111111-0000-0000-0000-000000000002', GETDATE());
`;

fs.writeFileSync(seedFile, content);
console.log('Seed data patched successfully.');
