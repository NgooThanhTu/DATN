<script setup>
import { computed } from 'vue'

const props = defineProps({
  tasks: { type: Array, default: () => [] },
  loading: { type: Boolean, default: false }
})

const emit = defineEmits(['task-click'])

function getPriorityInfo(priority) {
  const map = {
    0: { label: 'Không', color: '#94a3b8', icon: '—' },
    1: { label: 'Thấp', color: '#22c55e', icon: '↓' },
    2: { label: 'Trung bình', color: '#f59e0b', icon: '→' },
    3: { label: 'Cao', color: '#ef4444', icon: '↑' },
    4: { label: 'Khẩn cấp', color: '#dc2626', icon: '⚡' }
  }
  return map[priority] || map[0]
}

function getStatusClass(statusName) {
  const s = (statusName || '').toUpperCase().replace(/\s/g, '-')
  return 'status-' + s.toLowerCase()
}

function formatDate(date) {
  if (!date) return '—'
  return new Date(date).toLocaleDateString('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric'
  })
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
  return `hsl(${Math.abs(hash % 360)}, 55%, 50%)`
}
</script>

<template>
  <div class="list-view-container">
    <el-table
      :data="tasks"
      v-loading="loading"
      stripe
      class="task-table"
      @row-click="(row) => emit('task-click', row)"
      empty-text="Chưa có công việc nào"
      row-class-name="task-row"
    >
      <el-table-column label="ID" width="100" prop="sequenceId">
        <template #default="{ row }">
          <span class="seq-id">{{ row.sequenceId || row.id?.substring(0, 8) }}</span>
        </template>
      </el-table-column>

      <el-table-column label="Tiêu đề" min-width="250">
        <template #default="{ row }">
          <div class="task-title-cell">
            <span class="task-title-text">{{ row.title }}</span>
          </div>
        </template>
      </el-table-column>

      <el-table-column label="Trạng thái" width="130">
        <template #default="{ row }">
          <span class="status-badge" :class="getStatusClass(row.statusName)">
            {{ row.statusName || 'TO DO' }}
          </span>
        </template>
      </el-table-column>

      <el-table-column label="Ưu tiên" width="110">
        <template #default="{ row }">
          <span class="priority-cell" :style="{ color: getPriorityInfo(row.priority).color }">
            {{ getPriorityInfo(row.priority).icon }} {{ getPriorityInfo(row.priority).label }}
          </span>
        </template>
      </el-table-column>

      <el-table-column label="Phụ trách" width="160">
        <template #default="{ row }">
          <div class="assignee-cell" v-if="row.assigneeName">
            <div class="mini-avatar" :style="{ backgroundColor: getAvatarColor(row.assigneeName) }">
              {{ getInitials(row.assigneeName) }}
            </div>
            <span>{{ row.assigneeName }}</span>
          </div>
          <span v-else class="text-muted">Chưa gán</span>
        </template>
      </el-table-column>

      <el-table-column label="SP" width="60" align="center" prop="storyPoints" />

      <el-table-column label="Hạn chót" width="110">
        <template #default="{ row }">
          <span :class="{ 'overdue-text': row.dueDate && new Date(row.dueDate) < new Date() }">
            {{ formatDate(row.dueDate) }}
          </span>
        </template>
      </el-table-column>

      <el-table-column label="Cập nhật" width="110">
        <template #default="{ row }">
          {{ formatDate(row.updatedAt) }}
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<style scoped>
.list-view-container {
  border-radius: 8px;
  overflow: hidden;
}

.task-table {
  width: 100%;
}

.task-row {
  cursor: pointer;
}

.task-row:hover {
  background-color: var(--active-bg, #eff6ff) !important;
}

.seq-id {
  font-size: 11px;
  font-weight: 600;
  color: var(--text-muted);
  font-family: 'JetBrains Mono', monospace;
}

.task-title-cell {
  display: flex;
  align-items: center;
  gap: 8px;
}

.task-title-text {
  font-weight: 500;
  font-size: 13px;
  color: var(--text-primary);
}

.status-badge {
  font-size: 11px;
  font-weight: 600;
  padding: 3px 10px;
  border-radius: 12px;
  text-transform: uppercase;
  letter-spacing: 0.3px;
  white-space: nowrap;
}

.status-to-do { background: #f1f5f9; color: #64748b; }
.status-in-progress { background: #dbeafe; color: #2563eb; }
.status-in-review { background: #fef3c7; color: #d97706; }
.status-done { background: #dcfce7; color: #16a34a; }

.priority-cell {
  font-size: 12px;
  font-weight: 600;
}

.assignee-cell {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
}

.mini-avatar {
  width: 22px;
  height: 22px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 9px;
  font-weight: 700;
  color: white;
  flex-shrink: 0;
}

.text-muted {
  color: var(--text-muted);
  font-size: 12px;
}

.overdue-text {
  color: #ef4444;
  font-weight: 600;
}
</style>
