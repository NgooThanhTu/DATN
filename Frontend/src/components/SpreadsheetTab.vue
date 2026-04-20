<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import axiosClient from '@/api/axiosClient'

const props = defineProps({
  tasks: { type: Array, default: () => [] },
  projectId: { type: String, default: '' },
  projectMembers: { type: Array, default: () => [] }
})

const emit = defineEmits(['task-click', 'update-task', 'create-task'])

const modules = ref([])
const cycles = ref([])
const assigneeSearch = ref('')

const fetchOptions = async () => {
  if (!props.projectId) return
  const [modulesRes, cyclesRes] = await Promise.allSettled([
    axiosClient.get(`/projects/${props.projectId}/modules`),
    axiosClient.get(`/projects/${props.projectId}/sprints`)
  ])

  if (modulesRes.status === 'fulfilled') modules.value = modulesRes.value.data?.data || []
  if (cyclesRes.status === 'fulfilled') cycles.value = cyclesRes.value.data?.data || []
}

const formatDate = (dateString) => {
  if (!dateString) return '—'
  const d = new Date(dateString)
  return d.toLocaleString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
}

const toInputDate = (value) => {
  if (!value) return ''
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return ''
  const year = date.getFullYear()
  const month = `${date.getMonth() + 1}`.padStart(2, '0')
  const day = `${date.getDate()}`.padStart(2, '0')
  return `${year}-${month}-${day}`
}

const toApiDate = (value) => (value ? `${value}T00:00:00` : null)

const getPrioIcon = (prio) => {
  if (prio === 1) return { class: 'fa-solid fa-angles-up text-red', label: 'Urgent' }
  if (prio === 2) return { class: 'fa-solid fa-chevron-up text-orange', label: 'High' }
  if (prio === 3) return { class: 'fa-solid fa-minus text-blue', label: 'Normal' }
  if (prio === 4) return { class: 'fa-solid fa-chevron-down text-muted', label: 'Low' }
  return { class: 'fa-solid fa-ban text-muted', label: 'None' }
}

const getStatusDisplay = (statusName) => {
  const s = statusName?.toUpperCase() || ''
  if (s === 'DONE') return { class: 'fa-solid fa-circle-check text-green', label: 'Done' }
  if (s === 'IN PROGRESS' || s === 'INPROGRESS') return { class: 'fa-solid fa-circle-half-stroke text-orange', label: 'In Progress' }
  if (s === 'IN REVIEW' || s === 'REVIEW') return { class: 'fa-solid fa-eye text-orange', label: 'In Review' }
  if (s === 'TO DO' || s === 'TODO') return { class: 'fa-regular fa-circle text-muted', label: 'Todo' }
  return { class: 'fa-regular fa-circle-dashed text-muted', label: 'Backlog' }
}

const memberId = (member) => member.userId || member.id
const memberName = (member) => member.fullName || member.name || member.email || 'Unknown'

const filteredMembers = computed(() => {
  const keyword = assigneeSearch.value.trim().toLowerCase()
  if (!keyword) return props.projectMembers
  return props.projectMembers.filter(member => memberName(member).toLowerCase().includes(keyword))
})

const getTaskAssigneeIds = (task) => {
  if (Array.isArray(task.assigneeIds) && task.assigneeIds.length) return task.assigneeIds
  if (Array.isArray(task.assignees) && task.assignees.length) return task.assignees.map(item => item.userId || item.id).filter(Boolean)
  if (task.assignedUserId) return [task.assignedUserId]
  return []
}

const assigneeLabel = (task) => {
  const ids = getTaskAssigneeIds(task)
  if (!ids.length) return 'Assignees'
  if (ids.length > 1) return `${ids.length} assignees`
  const member = props.projectMembers.find(item => memberId(item) === ids[0])
  return member ? memberName(member) : task.assigneeName || 'Assignee'
}

const createdByLabel = (task) => {
  const creatorId = task.createdById || task.createdBy || task.reporterId
  const member = props.projectMembers.find(item => memberId(item) === creatorId)
  return member ? memberName(member) : task.createdByName || task.reporterName || '—'
}

const moduleLabel = (task) => {
  const moduleId = task.moduleId || task.moduleIds?.[0] || task.modules?.[0]?.id || task.modules?.[0]?.moduleId
  return modules.value.find(item => item.id === moduleId)?.name || task.moduleName || 'Modules'
}

const cycleLabel = (task) => cycles.value.find(item => item.id === task.sprintId)?.name || task.sprintName || 'Cycle'

const parentLabel = (task) => task.parentSequenceId || task.parentTitle || task.parentId || 'Parent'
const labelsLabel = (task) => {
  const labels = task.labels || task.labelNames || []
  return Array.isArray(labels) && labels.length ? labels.map(item => item.name || item).join(', ') : 'Labels'
}

const updateTaskTitle = (task, event) => {
  const newTitle = event.target.innerText.trim()
  if (!newTitle) {
    event.target.innerText = task.title
    return
  }
  if (newTitle !== task.title) emit('update-task', task, 'title', newTitle)
}

const updateField = (task, field, value) => {
  emit('update-task', task, field, value, task[field])
}

const updateDateField = (task, field, event) => {
  const value = toApiDate(event.target.value)
  updateField(task, field, value)
}

const toggleTaskAssignee = (task, id) => {
  const currentIds = getTaskAssigneeIds(task)
  const nextIds = currentIds.includes(id) ? currentIds.filter(item => item !== id) : [...currentIds, id]
  emit('update-task', task, 'assigneeIds', nextIds, currentIds)
}

onMounted(fetchOptions)
watch(() => props.projectId, fetchOptions)
</script>

<template>
  <div class="spreadsheet-container">
    <table class="plane-table">
      <thead>
        <tr>
          <th class="sticky-work-item">Work items</th>
          <th><i class="fa-regular fa-circle-dot"></i> State</th>
          <th><i class="fa-solid fa-signal"></i> Priority</th>
          <th><i class="fa-solid fa-user-group"></i> Assignees</th>
          <th><i class="fa-regular fa-user"></i> Created by</th>
          <th><i class="fa-regular fa-calendar"></i> Start date</th>
          <th><i class="fa-solid fa-calendar-day"></i> Due date</th>
          <th><i class="fa-solid fa-table-cells-large"></i> Modules</th>
          <th><i class="fa-solid fa-arrows-spin"></i> Cycle</th>
          <th><i class="fa-solid fa-diagram-project"></i> Parent</th>
          <th><i class="fa-solid fa-tag"></i> Labels</th>
          <th>Created on</th>
          <th>Updated on</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(task, index) in tasks" :key="task.id || index">
          <td class="sticky-work-item">
            <div class="wi-cell" @click="emit('task-click', task)">
              <span class="wi-id">{{ task.sequenceId || `CUN-${index + 1}` }}</span>
              <span
                class="wi-title"
                contenteditable="true"
                @click.stop
                @blur="updateTaskTitle(task, $event)"
                @keydown.enter.prevent="$event.target.blur()"
              >{{ task.title }}</span>
            </div>
          </td>
          <td>
            <el-dropdown trigger="click" @command="(val) => updateField(task, 'statusName', val)">
              <button class="cell-btn">
                <i :class="getStatusDisplay(task.statusName).class"></i>
                <span>{{ getStatusDisplay(task.statusName).label }}</span>
              </button>
              <template #dropdown>
                <el-dropdown-menu class="plane-dropdown">
                  <el-dropdown-item command="BACKLOG">Backlog</el-dropdown-item>
                  <el-dropdown-item command="TO DO">Todo</el-dropdown-item>
                  <el-dropdown-item command="IN PROGRESS">In Progress</el-dropdown-item>
                  <el-dropdown-item command="IN REVIEW">In Review</el-dropdown-item>
                  <el-dropdown-item command="DONE">Done</el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </td>
          <td>
            <el-dropdown trigger="click" @command="(val) => updateField(task, 'priority', val)">
              <button class="cell-btn">
                <i :class="getPrioIcon(task.priority).class"></i>
                <span>{{ getPrioIcon(task.priority).label }}</span>
              </button>
              <template #dropdown>
                <el-dropdown-menu class="plane-dropdown">
                  <el-dropdown-item :command="1">Urgent</el-dropdown-item>
                  <el-dropdown-item :command="2">High</el-dropdown-item>
                  <el-dropdown-item :command="3">Normal</el-dropdown-item>
                  <el-dropdown-item :command="4">Low</el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </td>
          <td>
            <el-popover placement="bottom" trigger="click" width="260" popper-class="plane-popover">
              <template #reference>
                <button class="cell-btn"><i class="fa-regular fa-user"></i>{{ assigneeLabel(task) }}</button>
              </template>
              <div class="popover-content">
                <input v-model="assigneeSearch" type="text" class="plane-search-input" placeholder="Search members" />
                <label v-for="member in filteredMembers" :key="memberId(member)" class="member-option" @click.stop="toggleTaskAssignee(task, memberId(member))">
                  <input type="checkbox" :checked="getTaskAssigneeIds(task).includes(memberId(member))" />
                  {{ memberName(member) }}
                </label>
              </div>
            </el-popover>
          </td>
          <td><span class="muted-text">{{ createdByLabel(task) }}</span></td>
          <td><input class="date-input" type="date" :value="toInputDate(task.plannedStartDate || task.startDate)" @change="updateDateField(task, 'plannedStartDate', $event)" /></td>
          <td><input class="date-input" type="date" :value="toInputDate(task.dueDate)" @change="updateDateField(task, 'dueDate', $event)" /></td>
          <td>
            <el-dropdown trigger="click" @command="(val) => updateField(task, 'moduleId', val)">
              <button class="cell-btn">{{ moduleLabel(task) }}</button>
              <template #dropdown>
                <el-dropdown-menu class="plane-dropdown">
                  <el-dropdown-item :command="null">No module</el-dropdown-item>
                  <el-dropdown-item v-for="module in modules" :key="module.id" :command="module.id">{{ module.name }}</el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </td>
          <td>
            <el-dropdown trigger="click" @command="(val) => updateField(task, 'sprintId', val)">
              <button class="cell-btn">{{ cycleLabel(task) }}</button>
              <template #dropdown>
                <el-dropdown-menu class="plane-dropdown">
                  <el-dropdown-item :command="null">No cycle</el-dropdown-item>
                  <el-dropdown-item v-for="cycle in cycles" :key="cycle.id" :command="cycle.id">{{ cycle.name }}</el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </td>
          <td><span class="muted-text">{{ parentLabel(task) }}</span></td>
          <td><span class="muted-text">{{ labelsLabel(task) }}</span></td>
          <td><span class="date-text">{{ formatDate(task.createdDate || task.createdAt) }}</span></td>
          <td><span class="date-text">{{ formatDate(task.updatedDate || task.updatedAt) }}</span></td>
        </tr>
        <tr v-if="tasks.length === 0">
          <td colspan="13" class="empty-cell">No work items found.</td>
        </tr>
      </tbody>
    </table>

    <div class="table-footer">
      <button class="add-btn" @click="emit('create-task', { statusName: 'TO DO' })"><i class="fa-solid fa-plus"></i> Add work item</button>
    </div>
  </div>
</template>

<style scoped>
.spreadsheet-container {
  flex: 1;
  background: #0D0F11;
  color: #E4E4E7;
  font-family: 'Inter', sans-serif;
  overflow: auto;
  border-top: 1px solid #1E2025;
}

.plane-table {
  min-width: 1900px;
  border-collapse: separate;
  border-spacing: 0;
  text-align: left;
  font-size: 13px;
}

.plane-table th {
  padding: 12px 16px;
  font-weight: 500;
  color: #A1A1AA;
  border-bottom: 2px solid #1E2025;
  border-right: 1px solid #1E2025;
  background: #0D0F11;
  position: sticky;
  top: 0;
  z-index: 10;
  white-space: nowrap;
}

.plane-table th:not(.sticky-work-item),
.plane-table td:not(.sticky-work-item) {
  min-width: 150px;
}

.plane-table th i { margin-right: 6px; font-size: 12px; }

.plane-table td {
  padding: 8px 12px;
  border-bottom: 1px solid #1E2025;
  border-right: 1px solid #1E2025;
  white-space: nowrap;
  background: #0D0F11;
}

.plane-table tr:hover td { background: #16181D; }

.sticky-work-item {
  position: sticky;
  left: 0;
  z-index: 20;
  min-width: 420px;
  max-width: 420px;
  box-shadow: 12px 0 18px rgba(0, 0, 0, 0.32);
}

th.sticky-work-item { z-index: 30; }

.wi-cell { display: flex; align-items: center; gap: 16px; min-width: 0; cursor: pointer; }
.wi-id { color: #A1A1AA; font-size: 12px; min-width: 70px; flex-shrink: 0; }
.wi-title { color: #E4E4E7; font-weight: 500; overflow: hidden; text-overflow: ellipsis; outline: none; }
.wi-title:focus { color: #fff; }

.cell-btn {
  width: 100%;
  min-height: 28px;
  display: flex;
  align-items: center;
  gap: 8px;
  background: transparent;
  border: 1px solid transparent;
  border-radius: 6px;
  color: #E4E4E7;
  padding: 4px 8px;
  text-align: left;
  cursor: pointer;
}
.cell-btn:hover { background: #1E2025; border-color: #27272A; }

.date-input {
  width: 135px;
  background: transparent;
  border: 1px solid #27272A;
  border-radius: 6px;
  color: #D4D4D8;
  padding: 5px 8px;
}

.member-option {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 6px 8px;
  border-radius: 6px;
  color: #D4D4D8;
  cursor: pointer;
}
.member-option:hover { background: #27272A; }

.plane-search-input {
  width: 100%;
  background: #111315;
  border: 1px solid #27272A;
  border-radius: 6px;
  color: #E4E4E7;
  padding: 6px 8px;
  margin-bottom: 8px;
  outline: none;
}

.text-green { color: #10B981; }
.text-orange { color: #F59E0B; }
.text-red { color: #EF4444; }
.text-blue { color: #3B82F6; }
.text-muted { color: #71717A; }
.muted-text, .date-text { color: #A1A1AA; font-size: 12px; }

.empty-cell { text-align: center; color: #71717A; padding: 40px; }
.table-footer { position: sticky; left: 0; padding: 12px 16px; }
.add-btn { background: transparent; color: #A1A1AA; border: none; font-size: 13px; cursor: pointer; display: flex; align-items: center; gap: 8px; }
.add-btn:hover { color: #E4E4E7; }
</style>
