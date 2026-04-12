<script setup>
import { ref, computed, watch } from 'vue'
import draggable from 'vuedraggable'
import axiosClient from '@/api/axiosClient'

const props = defineProps({
  tasks: { type: Array, default: () => [] },
  statuses: { type: Array, default: () => [] },
  projectId: { type: String, required: true },
  loading: { type: Boolean, default: false }
})

const emit = defineEmits(['task-click', 'status-changed', 'task-reordered', 'refresh'])

// Group tasks by their normalized status
const statusColumns = computed(() => {
  const defaultColumns = [
    { key: 'TO DO', label: 'To Do', color: '#94a3b8', icon: '○' },
    { key: 'IN PROGRESS', label: 'In Progress', color: '#3b82f6', icon: '◑' },
    { key: 'IN REVIEW', label: 'In Review', color: '#f59e0b', icon: '◕' },
    { key: 'DONE', label: 'Done', color: '#22c55e', icon: '●' }
  ]

  return defaultColumns.map(col => ({
    ...col,
    tasks: props.tasks
      .filter(t => (t.statusName || 'TO DO').toUpperCase() === col.key)
      .sort((a, b) => (a.sortOrder || 0) - (b.sortOrder || 0))
  }))
})

const dragEnabled = ref(true)

function onDragEnd(evt, targetStatus) {
  const movedTask = evt.item?.__draggable_context?.element
  if (!movedTask) return

  const oldStatus = movedTask.statusName
  const newStatus = targetStatus

  if (oldStatus !== newStatus) {
    emit('status-changed', {
      issueId: movedTask.id,
      newStatusName: newStatus,
      taskStatusId: movedTask.taskStatusId,
      rowVersion: movedTask.rowVersion
    })
  }

  // Calculate new SortOrder (LexoRank mid-point)
  const column = statusColumns.value.find(c => c.key === newStatus)
  if (column) {
    const newIndex = evt.newIndex
    const tasks = column.tasks
    let newSort

    if (tasks.length <= 1) {
      newSort = 65536
    } else if (newIndex === 0) {
      newSort = (tasks[1]?.sortOrder || 65536) / 2
    } else if (newIndex >= tasks.length - 1) {
      newSort = (tasks[tasks.length - 2]?.sortOrder || 0) + 65536
    } else {
      const before = tasks[newIndex - 1]?.sortOrder || 0
      const after = tasks[newIndex + 1]?.sortOrder || before + 131072
      newSort = (before + after) / 2
    }

    emit('task-reordered', {
      issueId: movedTask.id,
      sortOrder: newSort,
      newStatusName: newStatus
    })
  }
}

function getPriorityInfo(priority) {
  const map = {
    0: { label: 'None', color: '#94a3b8', icon: '—' },
    1: { label: 'Low', color: '#22c55e', icon: '↓' },
    2: { label: 'Medium', color: '#f59e0b', icon: '→' },
    3: { label: 'High', color: '#ef4444', icon: '↑' },
    4: { label: 'Urgent', color: '#dc2626', icon: '⚡' }
  }
  return map[priority] || map[0]
}

function getInitials(name) {
  if (!name) return '?'
  return name.split(' ').map(w => w[0]).join('').substring(0, 2).toUpperCase()
}

function getAvatarColor(name) {
  if (!name) return '#6b7280'
  let hash = 0
  for (let i = 0; i < name.length; i++) {
    hash = name.charCodeAt(i) + ((hash << 5) - hash)
  }
  const hue = hash % 360
  return `hsl(${Math.abs(hue)}, 55%, 50%)`
}

function formatDate(date) {
  if (!date) return ''
  const d = new Date(date)
  return d.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit' })
}
</script>

<template>
  <div class="kanban-board">
    <div v-for="column in statusColumns" :key="column.key" class="kanban-column">
      <!-- Column Header -->
      <div class="column-header">
        <div class="column-header-left">
          <span class="column-status-dot" :style="{ color: column.color }">{{ column.icon }}</span>
          <span class="column-title">{{ column.label }}</span>
          <span class="column-count">{{ column.tasks.length }}</span>
        </div>
      </div>

      <!-- Draggable Task List -->
      <draggable
        :list="column.tasks"
        group="kanban"
        item-key="id"
        ghost-class="task-ghost"
        drag-class="task-drag"
        :animation="200"
        :disabled="!dragEnabled"
        @end="(evt) => onDragEnd(evt, column.key)"
        class="column-body"
      >
        <template #item="{ element }">
          <div
            class="kanban-card"
            @click="emit('task-click', element)"
          >
            <!-- Card Header: Sequence ID + Priority -->
            <div class="card-top">
              <span class="card-seq" v-if="element.sequenceId">{{ element.sequenceId }}</span>
              <span class="card-seq" v-else>{{ element.id?.substring(0, 8) }}</span>
              <span
                class="card-priority"
                :style="{ color: getPriorityInfo(element.priority).color }"
                :title="getPriorityInfo(element.priority).label"
              >
                {{ getPriorityInfo(element.priority).icon }}
              </span>
            </div>

            <!-- Card Title -->
            <div class="card-title">{{ element.title }}</div>

            <!-- Card Meta -->
            <div class="card-meta">
              <div class="card-meta-left">
                <span v-if="element.storyPoints" class="card-sp" title="Story Points">
                  {{ element.storyPoints }} SP
                </span>
                <span v-if="element.dueDate" class="card-due" :class="{ overdue: new Date(element.dueDate) < new Date() }">
                  📅 {{ formatDate(element.dueDate) }}
                </span>
              </div>
              <div
                v-if="element.assigneeName"
                class="card-avatar"
                :style="{ backgroundColor: getAvatarColor(element.assigneeName) }"
                :title="element.assigneeName"
              >
                {{ getInitials(element.assigneeName) }}
              </div>
            </div>
          </div>
        </template>

        <!-- Empty State -->
        <template #footer>
          <div v-if="column.tasks.length === 0" class="column-empty">
            <span class="empty-icon">📋</span>
            <span>Không có công việc</span>
          </div>
        </template>
      </draggable>
    </div>
  </div>
</template>

<style scoped>
.kanban-board {
  display: flex;
  gap: 16px;
  overflow-x: auto;
  padding: 8px 0;
  min-height: 60vh;
}

.kanban-column {
  flex: 0 0 300px;
  min-width: 300px;
  display: flex;
  flex-direction: column;
  background: var(--bg-secondary, #f8f9fa);
  border-radius: 12px;
  overflow: hidden;
}

.column-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 14px 16px;
  border-bottom: 1px solid var(--border-color, #e5e7eb);
}

.column-header-left {
  display: flex;
  align-items: center;
  gap: 8px;
}

.column-status-dot {
  font-size: 14px;
}

.column-title {
  font-weight: 600;
  font-size: 13px;
  color: var(--text-primary, #1a1a2e);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.column-count {
  background: var(--hover-bg, #e5e7eb);
  color: var(--text-secondary, #6b7280);
  font-size: 11px;
  font-weight: 700;
  padding: 2px 8px;
  border-radius: 10px;
}

.column-body {
  flex: 1;
  padding: 8px;
  display: flex;
  flex-direction: column;
  gap: 8px;
  min-height: 100px;
  overflow-y: auto;
}

.kanban-card {
  background: var(--bg-card, #ffffff);
  border: 1px solid var(--border-color, #e5e7eb);
  border-radius: 8px;
  padding: 12px;
  cursor: pointer;
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
  user-select: none;
}

.kanban-card:hover {
  border-color: var(--el-color-primary, #3b82f6);
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.15);
  transform: translateY(-1px);
}

.card-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 6px;
}

.card-seq {
  font-size: 11px;
  font-weight: 600;
  color: var(--text-muted, #9ca3af);
  font-family: 'JetBrains Mono', 'Courier New', monospace;
}

.card-priority {
  font-size: 14px;
  font-weight: 700;
}

.card-title {
  font-size: 13px;
  font-weight: 500;
  color: var(--text-primary, #1a1a2e);
  line-height: 1.4;
  margin-bottom: 8px;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.card-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-meta-left {
  display: flex;
  gap: 8px;
  align-items: center;
}

.card-sp {
  font-size: 11px;
  font-weight: 600;
  color: var(--text-muted, #9ca3af);
  background: var(--hover-bg, #f3f4f6);
  padding: 2px 6px;
  border-radius: 4px;
}

.card-due {
  font-size: 11px;
  color: var(--text-secondary, #6b7280);
}

.card-due.overdue {
  color: #ef4444;
  font-weight: 600;
}

.card-avatar {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 10px;
  font-weight: 700;
  color: white;
  flex-shrink: 0;
}

.column-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 24px;
  color: var(--text-muted, #9ca3af);
  font-size: 13px;
  gap: 8px;
}

.empty-icon {
  font-size: 24px;
  opacity: 0.5;
}

/* Drag & Drop Effects */
.task-ghost {
  opacity: 0.4;
  border: 2px dashed var(--el-color-primary, #3b82f6) !important;
  background: var(--active-bg, #eff6ff) !important;
}

.task-drag {
  transform: rotate(2deg);
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.15) !important;
  z-index: 100;
}

/* Scrollbar for columns */
.column-body::-webkit-scrollbar { width: 4px; }
.column-body::-webkit-scrollbar-track { background: transparent; }
.column-body::-webkit-scrollbar-thumb { background: var(--scrollbar-thumb, #ddd); border-radius: 2px; }
</style>
