<script setup>
import { ref, onMounted } from 'vue'
import axiosClient from '@/api/axiosClient'

const props = defineProps({
  projectId: { type: String, required: true }
})

const labels = ref([])
const loading = ref(false)
const showCreate = ref(false)
const newLabel = ref({ name: '', colorCode: '#3b82f6', description: '' })

const presetColors = [
  '#ef4444', '#f97316', '#f59e0b', '#eab308', '#84cc16',
  '#22c55e', '#14b8a6', '#06b6d4', '#3b82f6', '#6366f1',
  '#8b5cf6', '#a855f7', '#d946ef', '#ec4899', '#f43f5e',
  '#64748b'
]

onMounted(() => loadLabels())

async function loadLabels() {
  loading.value = true
  try {
    const res = await axiosClient.get(`/projects/${props.projectId}/labels`)
    labels.value = res.data?.data || []
  } catch (e) {
    console.error('Failed to load labels', e)
  } finally {
    loading.value = false
  }
}

async function createLabel() {
  if (!newLabel.value.name.trim()) return
  try {
    await axiosClient.post(`/projects/${props.projectId}/labels`, newLabel.value)
    newLabel.value = { name: '', colorCode: '#3b82f6', description: '' }
    showCreate.value = false
    loadLabels()
  } catch (e) {
    alert(e.response?.data?.message || 'Lỗi khi tạo nhãn')
  }
}

async function deleteLabel(labelId) {
  if (!confirm('Xóa nhãn này?')) return
  try {
    await axiosClient.delete(`/projects/${props.projectId}/labels/${labelId}`)
    loadLabels()
  } catch (e) {
    alert('Lỗi khi xóa nhãn')
  }
}
</script>

<template>
  <div class="label-manager">
    <div class="label-header">
      <h3>🏷️ Nhãn</h3>
      <el-button type="primary" size="small" @click="showCreate = true">+ Thêm nhãn</el-button>
    </div>

    <div v-loading="loading" class="label-list">
      <div v-for="label in labels" :key="label.id" class="label-item">
        <div class="label-info">
          <span class="label-dot" :style="{ backgroundColor: label.colorCode }"></span>
          <span class="label-name">{{ label.name }}</span>
          <span class="label-count">{{ label.issueCount || 0 }}</span>
        </div>
        <div class="label-actions">
          <el-button size="small" text type="danger" @click="deleteLabel(label.id)">Xóa</el-button>
        </div>
      </div>

      <div v-if="!loading && labels.length === 0" class="label-empty">
        <p>Chưa có nhãn nào.</p>
      </div>
    </div>

    <!-- Create Label Popover -->
    <el-dialog v-model="showCreate" title="Tạo nhãn mới" width="400px">
      <el-form label-position="top">
        <el-form-item label="Tên nhãn">
          <el-input v-model="newLabel.name" placeholder="VD: Bug, Feature, Urgent" />
        </el-form-item>
        <el-form-item label="Màu sắc">
          <div class="color-grid">
            <div
              v-for="c in presetColors"
              :key="c"
              class="color-swatch"
              :class="{ selected: newLabel.colorCode === c }"
              :style="{ backgroundColor: c }"
              @click="newLabel.colorCode = c"
            ></div>
          </div>
        </el-form-item>
        <el-form-item label="Mô tả (tùy chọn)">
          <el-input v-model="newLabel.description" placeholder="Mô tả ngắn..." />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showCreate = false">Hủy</el-button>
        <el-button type="primary" @click="createLabel" :disabled="!newLabel.name.trim()">Tạo</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped>
.label-manager {
  padding: 4px 0;
}

.label-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.label-header h3 {
  font-size: 16px;
  font-weight: 700;
  color: var(--text-primary);
}

.label-list {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.label-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 14px;
  border-radius: 8px;
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  transition: all 0.15s;
}

.label-item:hover {
  background: var(--hover-bg);
}

.label-info {
  display: flex;
  align-items: center;
  gap: 10px;
}

.label-dot {
  width: 12px;
  height: 12px;
  border-radius: 3px;
  flex-shrink: 0;
}

.label-name {
  font-size: 13px;
  font-weight: 500;
  color: var(--text-primary);
}

.label-count {
  font-size: 11px;
  color: var(--text-muted);
  background: var(--hover-bg);
  padding: 1px 6px;
  border-radius: 8px;
}

.label-empty {
  text-align: center;
  padding: 30px;
  color: var(--text-muted);
  font-size: 13px;
}

.color-grid {
  display: grid;
  grid-template-columns: repeat(8, 1fr);
  gap: 6px;
}

.color-swatch {
  width: 28px;
  height: 28px;
  border-radius: 6px;
  cursor: pointer;
  transition: transform 0.15s;
  border: 2px solid transparent;
}

.color-swatch:hover {
  transform: scale(1.15);
}

.color-swatch.selected {
  border-color: var(--text-primary);
  transform: scale(1.15);
}
</style>
