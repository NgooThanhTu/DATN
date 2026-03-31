import fs from 'fs';
import * as compilerSfc from '@vue/compiler-sfc';

function walk(dir) {
    let results = [];
    const list = fs.readdirSync(dir);
    list.forEach(file => {
        file = dir + '/' + file;
        const stat = fs.statSync(file);
        if (stat && stat.isDirectory()) {
            results = results.concat(walk(file));
        } else if (file.endsWith('.vue')) {
            results.push(file);
        }
    });
    return results;
}

const files = walk('./src');
let success = true;

for (const file of files) {
    const code = fs.readFileSync(file, 'utf8');
    const { descriptor, errors } = compilerSfc.parse(code, { filename: file });
    if (errors && errors.length) {
        console.error(`Error parsing ${file}:`, errors);
        success = false;
        continue;
    }
    
    if (descriptor.script || descriptor.scriptSetup) {
        try {
            compilerSfc.compileScript(descriptor, { id: file });
        } catch (e) {
            console.error(`Syntax Error in script of ${file}:`);
            console.error(e.message);
            success = false;
        }
    }
}

if (success) {
    console.log("All Vue files compiled successfully.");
}
