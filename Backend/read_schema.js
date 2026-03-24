const fs = require('fs');
const path = require('path');
const dir = 'd:\\\\A\\\\QuanLyCongViec\\\\Backend\\\\src\\\\TaskManagement.Domain\\\\Entities';
const files = fs.readdirSync(dir).filter(f => f.endsWith('.cs'));
for (const file of files) {
    const content = fs.readFileSync(path.join(dir, file), 'utf8');
    const classNameMatch = content.match(/public class (\w+)/);
    if (!classNameMatch) continue;
    const className = classNameMatch[1];
    const props = [...content.matchAll(/public ([\w\?]+)\s+(\w+)\s*\{\s*get;\s*set;\s*\}/g)];
    const propNames = props.map(p => p[2]);
    console.log(className + ': ' + propNames.join(', '));
}
