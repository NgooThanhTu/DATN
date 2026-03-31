const fs = require('fs');

function inspectFile(file, words) {
    let out = file + '\n';
    const lines = fs.readFileSync(file, 'utf8').split('\n');
    for (let i = 0; i < lines.length; i++) {
        const l = lines[i];
        for (let word of words) {
            if (l.includes(word)) {
                out += `${i + 1}: ${l.trim()}\n`;
                break;
            }
        }
    }
    return out + '\n';
}

let result = '';
result += inspectFile('src/views/SpaceSummary.vue', ['const payload', 'const data', 'const s', 'let s']);
result += inspectFile('src/views/Register.vue', ['const response', 'let response']);
result += inspectFile('src/views/Login.vue', ['const accessToken', 'const fullName', 'const email', 'const systemRoles', 'const id', 'const redirect', 'const errorMsg']);
result += inspectFile('src/components/CustomizeSidebarModal.vue', ['const saved', 'const parsed']);

fs.writeFileSync('lines.txt', result, 'utf8');
