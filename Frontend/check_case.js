import fs from 'fs';
import path from 'path';

function walk(dir) {
    let results = [];
    const list = fs.readdirSync(dir);
    list.forEach(file => {
        const fullPath = dir + '/' + file;
        const stat = fs.statSync(fullPath);
        if (stat && stat.isDirectory()) {
            results = results.concat(walk(fullPath));
        } else if (file.endsWith('.js') || file.endsWith('.vue')) {
            results.push(fullPath);
        }
    });
    return results;
}

const files = walk('./src');
let foundError = false;

for (const file of files) {
    const code = fs.readFileSync(file, 'utf8');
    const importRegex = /import\s+.*?from\s+['"](.*?)['"]/g;
    let match;
    while ((match = importRegex.exec(code)) !== null) {
        let importPath = match[1];
        if (!importPath.startsWith('.') && !importPath.startsWith('@/')) continue;
        
        // resolve path
        let targetPath = '';
        if (importPath.startsWith('@/')) {
            targetPath = path.resolve('src', importPath.substring(2));
        } else {
            targetPath = path.resolve(path.dirname(file), importPath);
        }

        // Add extensions since imports might omit them
        let extensions = ['', '.js', '.vue'];
        let matchedActual = false;
        
        for (let ext of extensions) {
            let p = targetPath + ext;
            if (fs.existsSync(p)) {
                 // Check exact case
                 const dir = path.dirname(p);
                 const base = path.basename(p);
                 if (fs.existsSync(dir)) {
                     const actualFiles = fs.readdirSync(dir);
                     if (!actualFiles.includes(base)) {
                         console.error(`CASE MISMATCH in ${file}: imports '${importPath}' but actual file is something else (probably ${actualFiles.find(f => f.toLowerCase() === base.toLowerCase())})`);
                         foundError = true;
                     }
                 }
                 matchedActual = true;
                 break;
            }
        }
    }
}

if (!foundError) {
    console.log("No import case mismatch found.");
}
