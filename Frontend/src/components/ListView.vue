<template>
  <div class="plane-list-view">
    <!-- Header Controls omitted here as they are managed by SpaceSummary.vue -->

    <div v-for="(group, key) in groupedTasks" :key="key" class="list-group">
      <!-- Group Header -->
      <div class="group-header" @click="toggleGroup(key)">
        <div class="gh-left">
           <i class="gh-chevron fa-solid" :class="collapsedGroups[key] ? 'fa-chevron-right' : 'fa-chevron-down'"></i>
           <i class="status-icon" :class="group.iconClass" :style="{ color: group.color }"></i>
           <span class="group-name">{{ group.name }}</span>
           <span class="group-count">{{ group.tasks.length }}</span>
        </div>
        <div class="gh-right">
           <i class="fa-solid fa-plus add-icon"></i>
        </div>
      </div>

      <!-- Group Tasks -->
      <div class="group-content" v-show="!collapsedGroups[key]">
        <div class="task-row" v-for="task in group.tasks" :key="task.id" @click="emit('task-click', task)">
          <div class="tr-left">
            <span class="task-id">{{ task.sequenceId || task.id.substring(0,8).toUpperCase() }}</span>
            <span class="task-title" :style="group.name === 'Done' ? { textDecoration: 'line-through', color: '#71717A' } : {}">
               {{ task.title }}
               <span v-if="task.description" style="margin-left: 6px; font-size: 13px;">{{ task.description.includes('đ') ? '🐶' : '📝' }}</span>
            </span>
          </div>
          <div class="tr-right">
            <!-- Properties pills -->
            <div class="pill-group">
              <el-dropdown trigger="click" @command="(val) => updateTaskProperty(task, 'statusName', val)">
                <div class="pill pill-status cursor-pointer hover:bg-[#1E2025]">
                   <i class="status-icon-sm" :class="group.iconClass" :style="{ color: group.color }"></i>
                   {{ task.statusName || group.name }}
                </div>
                <template #dropdown>
                  <el-dropdown-menu class="plane-dropdown">
                    <el-dropdown-item command="BACKLOG">Backlog</el-dropdown-item>
                    <el-dropdown-item command="TO DO">To Do</el-dropdown-item>
                    <el-dropdown-item command="IN PROGRESS">In Progress</el-dropdown-item>
                    <el-dropdown-item command="IN REVIEW">In Review</el-dropdown-item>
                    <el-dropdown-item command="DONE">Done</el-dropdown-item>
                  </el-dropdown-menu>
                </template>
              </el-dropdown>

              <el-dropdown trigger="click" @command="(val) => updateTaskProperty(task, 'priority', val)">
                <div class="pill pill-priority cursor-pointer hover:bg-[#1E2025]">
                   <i class="fa-solid fa-angles-up text-red-500" v-if="task.priority === 1"></i>
                   <i class="fa-solid fa-chevron-up text-orange-500" v-else-if="task.priority === 2"></i>
                   <i class="fa-solid fa-minus text-blue-500" v-else-if="task.priority === 3"></i>
                   <i class="fa-solid fa-chevron-down text-gray-400" v-else></i>
                </div>
                <template #dropdown>
                  <el-dropdown-menu class="plane-dropdown">
                    <el-dropdown-item :command="1"><i class="fa-solid fa-angles-up text-red-500"></i> Urgent</el-dropdown-item>
                    <el-dropdown-item :command="2"><i class="fa-solid fa-chevron-up text-orange-500"></i> High</el-dropdown-item>
                    <el-dropdown-item :command="3"><i class="fa-solid fa-minus text-blue-500"></i> Normal</el-dropdown-item>
                    <el-dropdown-item :command="4"><i class="fa-solid fa-chevron-down text-gray-400"></i> Low</el-dropdown-item>
                  </el-dropdown-menu>
                </template>
              </el-dropdown>
              
              <!-- Assignee omitted for brevity as it needs dynamic user list, wait, we can just show generic user icon if we don't pass projectMembers -->
              <div class="pill pill-user cursor-not-allowed">
                 <div class="avatar-xxs">
                    <i class="fa-regular fa-user" v-if="!task.assigneeName"></i>
                    <span v-else>{{ task.assigneeName.substring(0,1).toUpperCase() }}</span>
                 </div>
              </div>
            </div>
            <div class="row-action">
              <i class="fa-solid fa-ellipsis"></i>
            </div>
          </div>
        </div>

        <div class="add-row-placeholder" v-if="inlineCreateGroup !== key" @click="openInlineCreate(key)">
          <i class="fa-solid fa-plus"></i> New work item
        </div>
        <div class="inline-create-box" v-if="inlineCreateGroup === key">
           <input type="text" class="ic-input" v-model="inlineTaskTitle" placeholder="Work item title" 
@keyup.enter="submitInlineTask(group)" @keyup.esc="inlineCreateGroup = null" ref="inlineInputs" />
           <div class="dm-toolbar mt-2">
             <!-- Status Dropdown -->
             <el-dropdown trigger="click" @command="(val) => inlineTaskStatus = val">
                <button class="dm-tool-btn"><i class="fa-regular fa-circle text-muted"></i> {{ inlineTaskStatus }}</button>
                <template #dropdown>
                  <el-dropdown-menu class="plane-dropdown">
                    <el-dropdown-item command="BACKLOG">Backlog</el-dropdown-item>
                    <el-dropdown-item command="TO DO">To Do</el-dropdown-item>
                    <el-dropdown-item command="IN PROGRESS">In Progress</el-dropdown-item>
                    <el-dropdown-item command="DONE">Done</el-dropdown-item>
                  </el-dropdown-menu>
                </template>
             </el-dropdown>
             
             <!-- Priority Dropdown -->
             <el-dropdown trigger="click" @command="(val) => inlineTaskPriority = val">
                <button class="dm-tool-btn">
                   <i class="fa-solid fa-signal text-muted" v-if="inlineTaskPriority === 3"></i>
                   <i class="fa-solid fa-angles-up text-red-500" v-else-if="inlineTaskPriority === 1"></i>
                   <i class="fa-solid fa-chevron-up text-orange-500" v-else-if="inlineTaskPriority === 2"></i>
                   <i class="fa-solid fa-chevron-down text-gray-400" v-else></i>
                   {{ inlineTaskPriority === 1 ? 'Urgent' : (inlineTaskPriority === 2 ? 'High' : (inlineTaskPriority === 3 ? 'Normal' : 'Low')) }}
                </button>
                <template #dropdown>
                  <el-dropdown-menu class="plane-dropdown">
                    <el-dropdown-item :command="1"><i class="fa-solid fa-angles-up text-red-500"></i> Urgent</el-dropdown-item>
                    <el-dropdown-item :command="2"><i class="fa-solid fa-chevron-up text-orange-500"></i> High</el-dropdown-item>
                    <el-dropdown-item :command="3"><i class="fa-solid fa-minus text-blue-500"></i> Normal</el-dropdown-item>
                    <el-dropdown-item :command="4"><i class="fa-solid fa-chevron-down text-gray-400"></i> Low</el-dropdown-item>
                  </el-dropdown-menu>
                </template>
             </el-dropdown>

             <!-- Fake Dropdowns for remaining UI parity -->
             <el-dropdown trigger="click">
                <button class="dm-tool-btn"><i class="fa-solid fa-user-group text-muted"></i> Assignees</button>
                <template #dropdown><el-dropdown-menu class="plane-dropdown"><el-dropdown-item>Unassigned</el-dropdown-item></el-dropdown-menu></template>
             </el-dropdown>
             <button class="dm-tool-btn cursor-not-allowed"><i class="fa-solid fa-tag text-muted"></i> Labels</button>
             <button class="dm-tool-btn cursor-not-allowed"><i class="fa-regular fa-calendar text-muted"></i> Start date</button>
             <button class="dm-tool-btn cursor-not-allowed"><i class="fa-solid fa-calendar-day text-muted"></i> Due date</button>
             <button class="dm-tool-btn cursor-not-allowed"><i class="fa-solid fa-arrows-spin text-muted"></i> Cycle</button>
             <button class="dm-tool-btn cursor-not-allowed"><i class="fa-solid fa-table-cells-large text-muted"></i> Modules</button>
           </div>
           <div class="ic-hint mt-2">Press 'Enter' to add another work item</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, ref, watch } from 'vue'

const props = defineProps({
  tasks: { type: Array, default: () => [] }
})

const emit = defineEmits(['task-click', 'task-created'])

const collapsedGroups = ref({})
const inlineCreateGroup = ref(null)
const inlineTaskTitle = ref('')
const inlineTaskPriority = ref(3)
const inlineTaskStatus = ref('TO DO')
const inlineInputs = ref(null)
import { nextTick } from 'vue'

const openInlineCreate = (key) => {
    inlineCreateGroup.value = key
    inlineTaskTitle.value = ''
    inlineTaskPriority.value = 3
    
    // Default to the group name if clicked in a column/group, otherwise BACKLOG
    const groupNameMap = { 'backlog': 'BACKLOG', 'todo': 'TO DO', 'inprogress': 'IN PROGRESS', 'done': 'DONE' }
    inlineTaskStatus.value = groupNameMap[key] || 'BACKLOG'

    nextTick(() => {
        if(inlineInputs.value) {
            if (Array.isArray(inlineInputs.value)) {
                inlineInputs.value[0]?.focus();
            } else {
                inlineInputs.value.focus();
            }
        }
    })
}

const submitInlineTask = (group) => {
   if(!inlineTaskTitle.value.trim()) {
      inlineCreateGroup.value = null;
      return;
   }
   emit('task-created', {
       title: inlineTaskTitle.value.trim(),
       statusName: inlineTaskStatus.value,
       priority: inlineTaskPriority.value
   })
   inlineTaskTitle.value = ''
}

const toggleGroup = (key) => {
  collapsedGroups.value[key] = !collapsedGroups.value[key]
}

const groupedTasks = computed(() => {
  const groups = {
    backlog: { name: 'Backlog', iconClass: 'fa-regular fa-circle-dashed', color: '#71717A', tasks: [] },
    todo: { name: 'Todo', iconClass: 'fa-regular fa-circle', color: '#A1A1AA', tasks: [] },
    inprogress: { name: 'In Progress', iconClass: 'fa-solid fa-circle-half-stroke', color: '#F59E0B', tasks: [] },
    done: { name: 'Done', iconClass: 'fa-solid fa-circle-check', color: '#10B981', tasks: [] }
  }

  props.tasks.forEach(task => {
    const s = (task.statusName || '').toUpperCase().trim();
    if (s === 'IN PROGRESS' || s === 'INPROGRESS') groups.inprogress.tasks.push(task)
    else if (s === 'DONE') groups.done.tasks.push(task)
    else if (s === 'BACKLOG' || s === '') groups.backlog.tasks.push(task)
    else groups.todo.tasks.push(task)
  })

  // Filter out empty groups if you want, but often they are kept. Let's keep them.
  return groups
})

const updateTaskProperty = (task, field, value) => {
   emit('update-task', task, field, value);
}
</script>

<style scoped>
.plane-list-view {
  display: flex;
  flex-direction: column;
  color: #E4E4E7;
  font-family: 'Inter', sans-serif;
}

.list-group {
  margin-bottom: 24px;
}

.group-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 0;
  cursor: pointer;
  border-bottom: 1px solid #1E2025;
  margin-bottom: 8px;
}

.group-header:hover .add-icon { opacity: 1; }

.gh-left {
  display: flex;
  align-items: center;
  gap: 10px;
}

.gh-chevron {
  font-size: 10px;
  color: #71717A;
  width: 14px;
  text-align: center;
}

.status-icon {
  font-size: 14px;
}

.group-name {
  font-size: 14px;
  font-weight: 600;
  color: #E4E4E7;
}

.group-count {
  font-size: 12px;
  font-weight: 500;
  color: #71717A;
  margin-left: 4px;
}

.gh-right {
  display: flex;
  align-items: center;
}

.add-icon {
  color: #71717A;
  font-size: 14px;
  opacity: 0;
  transition: opacity 0.2s;
  padding: 4px;
}

.group-content {
  display: flex;
  flex-direction: column;
}

.task-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 0 10px 24px;
  border-bottom: 1px solid #1E2025;
  cursor: pointer;
}
.task-row:hover {
  background-color: #16181D;
}

.tr-left {
  display: flex;
  align-items: center;
  gap: 16px;
}

.task-id {
  font-size: 12px;
  font-weight: 500;
  color: #71717A;
  width: 50px;
}

.task-title {
  font-size: 14px;
  font-weight: 500;
  color: #D4D4D8;
}

.tr-right {
  display: flex;
  align-items: center;
  gap: 12px;
}

.pill-group {
  display: flex;
  align-items: center;
  gap: 8px;
  opacity: 0;
  transition: opacity 0.2s;
}
.task-row:hover .pill-group { opacity: 1; }

.pill {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 4px 8px;
  border: 1px solid #27272A;
  border-radius: 4px;
  font-size: 12px;
  color: #A1A1AA;
}
.pill i { font-size: 12px; }

.status-icon-sm { font-size: 12px; }

.avatar-xxs {
  width: 16px;
  height: 16px;
  border-radius: 50%;
  border: 1px dashed #3F3F46;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 9px;
  font-weight: 600;
}

.row-action {
  color: #71717A;
  padding: 4px 8px;
  opacity: 0;
  transition: opacity 0.2s;
}
.row-action:hover { color: #E4E4E7; }
.task-row:hover .row-action { opacity: 1; }

.add-row-placeholder {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 0 12px 24px;
  font-size: 13px;
  font-weight: 500;
  color: #71717A;
  cursor: pointer;
  border-bottom: 1px solid transparent;
}
.add-row-placeholder:hover {
  color: #E4E4E7;
}

.inline-create-box { background: #16181D; border: 1px solid #38BDF8; border-radius: 8px; padding: 12px; margin: 8px 16px; box-shadow: 0 4px 12px rgba(0,0,0,0.5); }
.ic-input { width: 100%; background: transparent; border: none; color: #E5E7EB; outline: none; font-size: 14px; }
.ic-input::placeholder { color: #71717A; }
.ic-hint { font-size: 11px; color: #71717A; font-style: italic; }

.dm-toolbar { display: flex; flex-wrap: wrap; gap: 8px; }
.dm-tool-btn { background: transparent; border: 1px solid #27272A; color: #A1A1AA; font-size: 11px; display: flex; align-items: center; gap: 6px; padding: 4px 8px; border-radius: 4px; cursor: pointer; }
.dm-tool-btn:hover { background: #1E2025; color: #E4E4E7; }
.mt-2 { margin-top: 8px; }
.text-muted { color: #A1A1AA; }
</style>
