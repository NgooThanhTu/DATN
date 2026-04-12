const fs = require('fs');

let data = fs.readFileSync('../../seed_data.sql', 'utf8');

// Fix ProjectMembers
data = data.replace(/INSERT INTO ProjectMembers \(ProjectId, UserId, ProjectRole, JoinedAt, Status\) VALUES \('(.*?)', '77777777-0000-0000-0000-000000000001', '.*?', 0, 'Public', '(.*?)', '(.*?)', GETDATE\(\), 1\);/g, 
  "INSERT INTO ProjectMembers (ProjectId, UserId, ProjectRole, JoinedAt, Status) VALUES ('$1', '$2', '$3', GETDATE(), 1);");

// Fix Sprints
data = data.replace(/INSERT INTO Sprints \(Id, Name, StartDate, EndDate, ProjectId, Status, CreatedAt\) VALUES \('(.*?)', '77777777-0000-0000-0000-000000000001', '.*?', 65536, '(.*?)', GETDATE\(\), DATEADD\(day, 14, GETDATE\(\)\), '(.*?)', (0|1), GETDATE\(\)\);/g,
  "INSERT INTO Sprints (Id, Name, StartDate, EndDate, ProjectId, Status, CreatedAt) VALUES ('$1', '$2', GETDATE(), DATEADD(day, 14, GETDATE()), '$3', $4, GETDATE());");

fs.writeFileSync('../../seed_data.sql', data);
console.log("Fixed seed_data.sql");
