const fs = require('fs');
let contents = fs.readFileSync('Frontend/src/components/TaskDetailModal.vue', 'utf8');
contents = contents.replace(/<el-dropdown :teleported="false"-menu/g, '<el-dropdown-menu');
contents = contents.replace(/<el-dropdown :teleported="false"-item/g, '<el-dropdown-item');
fs.writeFileSync('Frontend/src/components/TaskDetailModal.vue', contents);
console.log('Done replacing tags!');
