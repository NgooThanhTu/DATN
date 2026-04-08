import fs from 'fs';
import { parse } from '@babel/parser';

function walk(dir) {
    let results = [];
    const list = fs.readdirSync(dir);
    list.forEach(file => {
        const fullPath = dir + '/' + file;
        const stat = fs.statSync(fullPath);
        if (stat && stat.isDirectory()) {
            results = results.concat(walk(fullPath));
        } else if (file.endsWith('.js')) {
            results.push(fullPath);
        }
    });
    return results;
}

const files = walk('./src');
let success = true;

for (const file of files) {
    const code = fs.readFileSync(file, 'utf8');
    try {
        parse(code, {
            sourceType: 'module',
            plugins: ['importAssertions']
        });
    } catch (e) {
        console.error(`Syntax Error in ${file}:`);
        console.error(e.message);
        success = false;
    }
}

if (success) {
    console.log("All JS files parsed successfully.");
}
