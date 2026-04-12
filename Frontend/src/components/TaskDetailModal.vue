<script setup>
import { ref, computed, watch } from 'vue'
import axiosClient from '@/api/axiosClient'

const props = defineProps({
  visible: { type: Boolean, default: false },
  task: { type: Object, default: null },
  projectId: { type: String, required: true },
  members: { type: Array, default: () => [] },
  statuses: { type: Array, default: () => [] },
  taskTypes: { type: Array, default: () => [] }
})

const emit = defineEmits(['close', 'updated', 'deleted'])

const editForm = ref({})
const isEditing = ref(false)
const saving = ref(false)
const newComment = ref('')
const comments = ref([])
const loadingComments = ref(false)

watch(() => props.task, (val) => {
  if (val) {
    editForm.value = { ...val }
    loadComments()
  }
}, { immediate: true, deep: true })

async function loadComments() {
  if (!props.task?.id) return
  loadingComments.value = true
  try {
    const res = await axiosClient.get(`/${props.task.id}/comments`)
    comments.value = res.data?.data || []
  } catch (e) {
    console.error('Failed to load comments', e)
  } finally {
    loadingComments.value = false
  }
}

async function saveTask() {
  saving.value = true
  try {
    await axiosClient.put(`/projects/${props.projectId}/WorkTasks/${props.task.id}`, {
      title: editForm.value.title,
      description: editForm.value.description,
      priority: editForm.value.priority,
      storyPoints: editForm.value.storyPoints,
      assignedUserId: editForm.value.assignedUserId,
      dueDate: editForm.value.dueDate,
      plannedStartDate: editForm.value.plannedStartDate,
      plannedEndDate: editForm.value.plannedEndDate,
      taskTypeId: editForm.value.taskTypeId,
      rowVersion: editForm.value.rowVersion
    })
    isEditing.value = false
    emit('updated')
  } catch (e) {
    if (e.response?.status === 409) {
      alert('Dữ liệu đã bị người khác sửa. Vui lòng tải lại!')
    } else {
      alert(e.response?.data?.message || 'Lỗi khi cập nhật')
    }
  } finally {
    saving.value = false
  }
}

async function addComment() {
  if (!newComment.value.trim()) return
  try {
    await axiosClient.post(`/projects/${props.projectId}/WorkTasks/${props.task.id}/comments`, {
      content: newComment.value
    })
    newComment.value = ''
    loadComments()
  } catch (e) {
    console.error('Failed to add comment', e)
  }
}

function getPriorityOptions() {
  return [
    { value: 0, label: 'Không', color: '#94a3b8' },
    { value: 1, label: 'Thấp', color: '#22c55e' },
    { value: 2, label: 'Trung bình', color: '#f59e0b' },
    { value: 3, label: 'Cao', color: '#ef4444' },
    { value: 4, label: 'Khẩn cấp', color: '#dc2626' }
  ]
}

function formatDateTime(date) {
  if (!date) return '—'
  return new Date(date).toLocaleString('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit'
  })
}

function getInitials(name) {
  if (!name) return '?'
  return name.split(' ').map(w => w[0]).join('').substring(0, 2).toUpperCase()
}
</script>

<template>
  <el-drawer
    :model-value="visible"
    @close="emit('close')"
    size="600px"
    direction="rtl"
    :show-close="true"
    class="task-drawer"
    :with-header="false"
  >
    <div v-if="task" class="task-detail">
      <!-- Header -->
      <div class="detail-header">
        <div class="detail-header-left">
          <span class="detail-seq">{{ task.sequenceId || task.id?.substring(0, 8) }}</span>
          <span class="detail-status-badge" :class="'status-' + (task.statusName || 'todo').toLowerCase().replace(/\s/g, '-')">
            {{ task.statusName || 'TO DO' }}
          </span>
        </div>
        <div class="detail-actions">
          <el-button v-if="!isEditing" size="small" type="primary" plain @click="isEditing = true">
            ✏️ Chỉnh sửa
          </el-button>
          <template v-else>
            <el-button size="small" @click="isEditing = false">Hủy</el-button>
            <el-button size="small" type="primary" :loading="saving" @click="saveTask">Lưu</el-button>
          </template>
        </div>
      </div>

      <!-- Title -->
      <div class="detail-title-section">
        <input
          v-if="isEditing"
          v-model="editForm.title"
          class="detail-title-input"
          placeholder="Tiêu đề công việc..."
        />
        <h2 v-else class="detail-title">{{ task.title }}</h2>
      </div>

      <!-- Properties Grid -->
      <div class="detail-props">
        <div class="prop-row">
          <span class="prop-label">Người phụ trách</span>
          <el-select
            v-if="isEditing"
            v-model="editForm.assignedUserId"
            placeholder="Chọn người"
            clearable
            size="small"
            class="prop-value-select"
          >
            <el-option
              v-for="m in members"
              :key="m.userId"
              :value="m.userId"
              :label="m.fullName"
            />
          </el-select>
          <span v-else class="prop-value">{{ task.assigneeName || 'Chưa gán' }}</span>
        </div>

        <div class="prop-row">
          <span class="prop-label">Độ ưu tiên</span>
          <el-select
            v-if="isEditing"
            v-model="editForm.priority"
            size="small"
            class="prop-value-select"
          >
            <el-option
              v-for="p in getPriorityOptions()"
              :key="p.value"
              :value="p.value"
              :label="p.label"
            />
          </el-select>
          <span v-else class="prop-value priority-badge" :style="{ color: getPriorityOptions()[task.priority]?.color }">
            {{ getPriorityOptions()[task.priority]?.label || 'Không' }}
          </span>
        </div>

        <div class="prop-row">
          <span class="prop-label">Story Points</span>
          <el-input-number
            v-if="isEditing"
            v-model="editForm.storyPoints"
            :min="0"
            :max="100"
            size="small"
          />
          <span v-else class="prop-value">{{ task.storyPoints || 0 }}</span>
        </div>

        <div class="prop-row">
          <span class="prop-label">Hạn chót</span>
          <el-date-picker
            v-if="isEditing"
            v-model="editForm.dueDate"
            type="date"
            size="small"
            format="DD/MM/YYYY"
          />
          <span v-else class="prop-value">{{ formatDateTime(task.dueDate) }}</span>
        </div>

        <div class="prop-row">
          <span class="prop-label">Người báo cáo</span>
          <span class="prop-value">{{ task.reporterName || '—' }}</span>
        </div>

        <div class="prop-row">
          <span class="prop-label">Tạo lúc</span>
          <span class="prop-value">{{ formatDateTime(task.createdAt) }}</span>
        </div>

        <div class="prop-row">
          <span class="prop-label">Cập nhật lúc</span>
          <span class="prop-value">{{ formatDateTime(task.updatedAt) }}</span>
        </div>
      </div>

      <!-- Description -->
      <div class="detail-section">
        <h3 class="section-title">Mô tả</h3>
        <textarea
          v-if="isEditing"
          v-model="editForm.description"
          class="detail-description-input"
          rows="4"
          placeholder="Thêm mô tả..."
        ></textarea>
        <p v-else class="detail-description">{{ task.description || 'Chưa có mô tả.' }}</p>
      </div>

      <!-- Comments -->
      <div class="detail-section">
        <h3 class="section-title">Bình luận ({{ comments.length }})</h3>
        <div class="comments-list">
          <div v-for="c in comments" :key="c.id" class="comment-item">
            <div class="comment-avatar" :style="{ backgroundColor: '#6366f1' }">
              {{ c.avatar || getInitials(c.fullName) }}
            </div>
            <div class="comment-body">
              <div class="comment-header">
                <span class="comment-author">{{ c.fullName }}</span>
                <span class="comment-time">{{ formatDateTime(c.createdAt) }}</span>
              </div>
              <div class="comment-content">{{ c.content }}</div>
            </div>
          </div>
        </div>

        <div class="comment-input-area">
          <textarea
            v-model="newComment"
            placeholder="Viết bình luận..."
            rows="2"
            class="comment-textarea"
            @keydown.enter.ctrl="addComment"
          ></textarea>
          <el-button type="primary" size="small" @click="addComment" :disabled="!newComment.trim()">
            Gửi
          </el-button>
        </div>
      </div>
    </div>
  </el-drawer>
</template>

<style scoped>
.task-detail {
  padding: 20px;
  height: 100%;
  overflow-y: auto;
}

.detail-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.detail-header-left {
  display: flex;
  align-items: center;
  gap: 10px;
}

.detail-seq {
  font-size: 12px;
  font-weight: 600;
  color: var(--text-muted);
  font-family: 'JetBrains Mono', monospace;
}

.detail-status-badge {
  font-size: 11px;
  font-weight: 600;
  padding: 3px 10px;
  border-radius: 12px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.status-to-do { background: #f1f5f9; color: #64748b; }
.status-in-progress { background: #dbeafe; color: #2563eb; }
.status-in-review { background: #fef3c7; color: #d97706; }
.status-done { background: #dcfce7; color: #16a34a; }

.detail-title {
  font-size: 20px;
  font-weight: 700;
  color: var(--text-primary);
  margin-bottom: 20px;
  line-height: 1.3;
}

.detail-title-input {
  width: 100%;
  font-size: 20px;
  font-weight: 700;
  border: none;
  border-bottom: 2px solid var(--el-color-primary);
  padding: 8px 0;
  margin-bottom: 20px;
  background: transparent;
  color: var(--text-primary);
  outline: none;
}

.detail-props {
  display: grid;
  gap: 1px;
  background: var(--border-color, #e5e7eb);
  border: 1px solid var(--border-color, #e5e7eb);
  border-radius: 8px;
  overflow: hidden;
  margin-bottom: 24px;
}

.prop-row {
  display: flex;
  align-items: center;
  padding: 10px 14px;
  background: var(--bg-card, #fff);
}

.prop-label {
  width: 140px;
  font-size: 12px;
  font-weight: 600;
  color: var(--text-muted);
  flex-shrink: 0;
}

.prop-value {
  font-size: 13px;
  color: var(--text-primary);
}

.prop-value-select {
  flex: 1;
}

.detail-section {
  margin-bottom: 24px;
}

.section-title {
  font-size: 14px;
  font-weight: 700;
  color: var(--text-primary);
  margin-bottom: 12px;
  padding-bottom: 8px;
  border-bottom: 1px solid var(--border-color);
}

.detail-description {
  font-size: 13px;
  color: var(--text-secondary);
  line-height: 1.6;
  white-space: pre-wrap;
}

.detail-description-input {
  width: 100%;
  font-size: 13px;
  border: 1px solid var(--border-color);
  border-radius: 8px;
  padding: 10px;
  background: var(--bg-secondary);
  color: var(--text-primary);
  resize: vertical;
  outline: none;
}

.detail-description-input:focus {
  border-color: var(--el-color-primary);
}

.comments-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: 16px;
}

.comment-item {
  display: flex;
  gap: 10px;
}

.comment-avatar {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 10px;
  font-weight: 700;
  color: white;
  flex-shrink: 0;
}

.comment-body {
  flex: 1;
}

.comment-header {
  display: flex;
  gap: 8px;
  align-items: center;
  margin-bottom: 4px;
}

.comment-author {
  font-size: 12px;
  font-weight: 600;
  color: var(--text-primary);
}

.comment-time {
  font-size: 11px;
  color: var(--text-muted);
}

.comment-content {
  font-size: 13px;
  color: var(--text-secondary);
  line-height: 1.5;
  background: var(--bg-secondary);
  padding: 8px 12px;
  border-radius: 8px;
}

.comment-input-area {
  display: flex;
  gap: 8px;
  align-items: flex-end;
}

.comment-textarea {
  flex: 1;
  border: 1px solid var(--border-color);
  border-radius: 8px;
  padding: 8px 12px;
  font-size: 13px;
  background: var(--bg-secondary);
  color: var(--text-primary);
  resize: none;
  outline: none;
}

.comment-textarea:focus {
  border-color: var(--el-color-primary);
}
</style>
