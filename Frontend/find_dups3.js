import fs from 'fs';
import path from 'path';

function walk(dir) {
    let results = [];
    const list = fs.readdirSync(dir);
    list.forEach(file => {
        file = dir + '/' + file;
        const stat = fs.statSync(file);
        if (stat && stat.isDirectory()) {
            results = results.concat(walk(file));
        } else if (file.endsWith('.js') || file.endsWith('.vue')) {
            results.push(file);
        }
    });
    return results;
}

const files = walk('./src');
let outstr = "";

for (const file of files) {
    const code = fs.readFileSync(file, 'utf8');
    let match = code.match(/<script\s+setup.*?>([\s\S]*?)<\/script>/);
    let content = match ? match[1] : code;
    
    // Very simple check for duplicate declarations
    let regex = /^\s*(?:const|let|var)\s+({?[a-zA-Z0-9_$,\s]+}?)\s*=/gm;
    let vars = new Set();
    
    let m;
    while ((m = regex.exec(content)) !== null) {
        let varNameMatch = m[1].trim();
        let varNames = [];
        if (varNameMatch.startsWith('{')) {
            varNames = varNameMatch.replace(/[{}]/g, '').split(',').map(v => v.trim());
        } else {
            varNames = [varNameMatch];
        }
        for (let varName of varNames) {
            varName = varName.split(':')[0].split('=')[0].trim();
            if (varName && vars.has(varName)) {
                outstr += `Duplicate var decl found in ${file}: ${varName}\n`;
            }
            if (varName) vars.add(varName);
        }
    }

    let importRegex = /import\s+.*?([a-zA-Z0-9_$,\s{}]+)\s+from/g;
    while ((m = importRegex.exec(content)) !== null) {
        let varNameMatch = m[1].trim();
        let varNames = varNameMatch.replace(/[{}]/g, '').split(',').map(v => v.trim());
        for (let varName of varNames) {
            varName = varName.split(' as ').pop().trim();
            if (varName && vars.has(varName)) {
                outstr += `Duplicate (Import vs Decl) found in ${file}: ${varName}\n`;
            }
            if (varName === '') continue; // ignore
        }
    }
}

if (!outstr) outstr = "No duplicates found";
fs.writeFileSync('dups3.txt', outstr, 'utf8');
