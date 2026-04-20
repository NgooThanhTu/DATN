import re

with open('D:/A/QuanLyCongViec/Frontend/src/components/layout/NexusSidebar.vue', 'r', encoding='utf-8') as f:
    text = f.read()

text = re.sub(
    r'<router-link\s+:to="`/space/\$\{projectId\}/cycles`"\s+class="nav-link"[^>]*>\s*<i class="fa-solid fa-arrows-spin"></i>\s*<span>Cycles</span>\s*</router-link>',
    '<div class="nav-link" :class="{ \'active\' : route.name === \'CyclesView\' }" @click="router.push(`/space/${projectId}/cycles`)">\n            <i class="fa-solid fa-arrows-spin"></i>\n            <span>Cycles</span>\n          </div>',
    text
)

text = re.sub(
    r'<router-link\s+:to="`/space/\$\{projectId\}/modules`"\s+class="nav-link"[^>]*>\s*<i class="fa-solid fa-table-cells-large"></i>\s*<span>Modules</span>\s*</router-link>',
    '<div class="nav-link" :class="{ \'active\' : route.name === \'ModulesView\' }" @click="router.push(`/space/${projectId}/modules`)">\n            <i class="fa-solid fa-table-cells-large"></i>\n            <span>Modules</span>\n          </div>',
    text
)

if 'useRouter' not in text:
    text = text.replace('const route = useRoute()', 'const route = useRoute()\nconst router = useRouter()')
    text = text.replace('import { useRoute } from \'vue-router\'', 'import { useRoute, useRouter } from \'vue-router\'')

with open('D:/A/QuanLyCongViec/Frontend/src/components/layout/NexusSidebar.vue', 'w', encoding='utf-8') as f:
    f.write(text)
print("Finished!")
