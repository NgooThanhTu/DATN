const fs = require('fs');

let data = fs.readFileSync('../../seed_data.sql', 'utf8');

if (!data.includes('sp_msforeachtable')) {
    data = `EXEC sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';\n` + data + `\nEXEC sp_msforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';`;
    fs.writeFileSync('../../seed_data.sql', data);
    console.log("Added constraints bypass to seed_data.sql");
} else {
    console.log("Already bypassed");
}
