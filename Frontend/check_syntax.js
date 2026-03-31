import fs from 'fs';
import path from 'path';
import { parse } from '@babel/parser';
import * as compilerSfc from '@vue/compiler-sfc';

const dir = './src';

function walk(directory) {
    let results = [];
    const list = fs.readdirSync(directory);
    list.forEach(function (file) {
        file = directory + '/' + file;
        const stat = fs.statSync(file);
        if (stat && stat.isDirectory()) {
            results = results.concat(walk(file));
        } else if (file.endsWith('.js') || file.endsWith('.vue')) {
            results.push(file);
        }
    });
    return results;
}

const files = walk(dir);

for (const file of files) {
    let content = fs.readFileSync(file, 'utf-8');
    let codeToParse = content;
    
    if (file.endsWith('.vue')) {
        const { descriptor } = compilerSfc.parse(content);
        codeToParse = '';
        if (descriptor.scriptSetup) {
            codeToParse += descriptor.scriptSetup.content + '\n';
        }
        if (descriptor.script) {
            codeToParse += descriptor.script.content + '\n';
        }
    }

    try {
        parse(codeToParse, {
            sourceType: 'module',
            plugins: ['typescript']
        });
    } catch (e) {
        console.error(`Error in ${file}:`);
        console.error(e.message);
    }
}
console.log('Done checking syntax.');
