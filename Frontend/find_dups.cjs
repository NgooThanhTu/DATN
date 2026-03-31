const fs = require('fs');
const glob = require('glob'); // Not available, let's use raw fs

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
let found = false;

for (const file of files) {
    const code = fs.readFileSync(file, 'utf8');
    // Extract script setup content
    let match = code.match(/<script\s+setup.*?>([\s\S]*?)<\/script>/);
    let content = match ? match[1] : code;
    
    let regex = /(?:const|let|var)\s+({?[a-zA-Z0-9_$,\s]+}?)\s*=/g;
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
            // strip alias like "x as y"
            varName = varName.split(' as ').pop().trim();
            // strip default val like "x = 1"
            varName = varName.split('=')[0].trim();
            if (varName && vars.has(varName)) {
                console.log(`Duplicate found in ${file}: ${varName}`);
                found = true;
            }
            if (varName) vars.add(varName);
        }
    }
    
    // Also check imports vs declarations
    let importRegex = /import\s+(?:{?)([a-zA-Z0-9_$,\s]+)(?:}?)\s+from/g;
    while ((m = importRegex.exec(content)) !== null) {
        let varNameMatch = m[1].trim();
        let varNames = varNameMatch.replace(/[{}]/g, '').split(',').map(v => v.trim());
        for (let varName of varNames) {
            varName = varName.split(' as ').pop().trim();
            if (varName && vars.has(varName)) {
                console.log(`Duplicate found (Import vs Decl) in ${file}: ${varName}`);
                found = true;
            }
            if (varName) vars.add(varName);
        }
    }
}

if (!found) console.log("No duplicates found with naive regex.");
