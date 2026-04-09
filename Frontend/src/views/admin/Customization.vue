<template>
  <AdminLayout>
    <div class="admin-page-container">
    <div class="page-header">
      <div class="breadcrumb">
        <i class="fa-solid fa-palette"></i> System / Customization
      </div>
      <h1 class="page-title">Giao diện (Customization)</h1>
      <p class="page-subtitle">Tùy biến màu sắc và trải nghiệm giao diện cho toàn bộ tổ chức.</p>
    </div>

    <div class="form-container" v-loading="isLoading">
      <div class="settings-card">
        <h2 class="card-title">Màu sắc Hệ thống (Color Palette)</h2>
        <p class="section-desc">Thay đổi thông số màu này sẽ áp dụng ngay lập tức cho tất cả người dùng.</p>

        <div class="theme-presets mt-24" style="margin-bottom: 32px;">
          <h3 style="font-size: 15px; margin-bottom: 12px; color: var(--text-primary)">Sử dụng mẫu hệ màu (Templates)</h3>
          <div class="preset-buttons" style="display: flex; gap: 12px; flex-wrap: wrap;">
             <el-button type="default" plain @click="applyTemplate('default')">Dark Default</el-button>
             <el-button type="default" plain @click="applyTemplate('light')">Light Classic</el-button>
             <el-button type="default" plain @click="applyTemplate('oceanic')">Oceanic Blue</el-button>
             <el-button type="default" plain @click="applyTemplate('monochrome')">Monochrome</el-button>
          </div>
        </div>

        
        <div class="color-grid mt-24">
          <div class="color-setting" v-for="(color, key) in themeColors" :key="key">
            <span class="color-label">{{ color.label }}</span>
            <div class="color-picker-wrapper">
              <el-color-picker v-model="color.value" :predefine="predefineColors" @change="previewColor(key, color.value)" />
              <span class="color-hex">{{ color.value }}</span>
            </div>
            <span class="color-variable">{{ color.variable }}</span>
          </div>
        </div>
      </div>

      <div class="action-footer">
        <el-button @click="resetToDefault">Khôi phục mặc định</el-button>
        <el-button type="primary" :loading="isSaving" @click="saveTheme">Lưu hệ màu</el-button>
      </div>
    </div>
  </div>
  </AdminLayout>
</template>

<script setup>
import AdminLayout from '@/components/layout/AdminLayout.vue'
import { ref, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import axiosClient from '@/api/axiosClient'

const isLoading = ref(false)
const isSaving = ref(false)

const predefineColors = [
  '#0f172a', // dark blue
  '#1e293b',
  '#1d2125', // nexus default
  '#22272b',
  '#2c3338',
  '#8b949e',
  '#3b82f6',
  '#10b981',
  '#f59e0b',
  '#ef4444'
]

// Define theme structure
// Variables must match index.css root variables
const themeColors = ref({
  bgLayout: { label: 'Nền Màn hình (Background)', variable: '--bg-layout', value: '#1d2125', default: '#1d2125' },
  bgCard: { label: 'Màu Khối (Card/Panel)', variable: '--bg-card', value: '#22272b', default: '#22272b' },
  bgHover: { label: 'Màu nền di chuột (Hover)', variable: '--bg-hover', value: '#2c3338', default: '#2c3338' },
  borderColor: { label: 'Màu viền (Border)', variable: '--border-color', value: '#38414a', default: '#38414a' },
  textPrimary: { label: 'Màu chữ chính', variable: '--text-primary', value: '#e2e8f0', default: '#e2e8f0' }
})

const applyTemplate = (type) => {
  const templates = {
    default: { bgLayout: '#1d2125', bgCard: '#22272b', bgHover: '#2c3338', borderColor: '#38414a', textPrimary: '#e2e8f0' },
    light: { bgLayout: '#f8fafc', bgCard: '#ffffff', bgHover: '#f1f5f9', borderColor: '#e2e8f0', textPrimary: '#0f172a' },
    oceanic: { bgLayout: '#0f172a', bgCard: '#1e293b', bgHover: '#334155', borderColor: '#475569', textPrimary: '#f8fafc' },
    monochrome: { bgLayout: '#000000', bgCard: '#111111', bgHover: '#222222', borderColor: '#333333', textPrimary: '#ffffff' }
  };
  
  if (templates[type]) {
    for (const key in templates[type]) {
      if (themeColors.value[key]) {
        themeColors.value[key].value = templates[type][key];
        previewColor(key, templates[type][key]);
      }
    }
  }
}

onMounted(async () => {
  await fetchTheme()
})

const fetchTheme = async () => {
  isLoading.value = true
  try {
    const res = await axiosClient.get('/settings/ThemeSettings')
    const data = res.data.data || {}
    
    // Load from DB if exists
    for (const key in themeColors.value) {
      if (data[key]) {
        themeColors.value[key].value = data[key]
      }
    }
    
    applyThemeToDocument()
  } catch (err) {
    console.error(err)
    ElMessage.error('Không thể tải cấu hình Hệ màu.')
  } finally {
    isLoading.value = false
  }
}

const previewColor = (key, val) => {
  const variable = themeColors.value[key].variable
  document.documentElement.style.setProperty(variable, val)
}

const applyThemeToDocument = () => {
  for (const key in themeColors.value) {
    document.documentElement.style.setProperty(themeColors.value[key].variable, themeColors.value[key].value)
  }
}

const resetToDefault = () => {
  for (const key in themeColors.value) {
    themeColors.value[key].value = themeColors.value[key].default
    previewColor(key, themeColors.value[key].default)
  }
}

const saveTheme = async () => {
  isSaving.value = true
  try {
    const payload = { Settings: {} }
    for (const key in themeColors.value) {
      payload.Settings[key] = themeColors.value[key].value
    }
    
    await axiosClient.put('/settings/ThemeSettings', payload)
    ElMessage.success('Đã lưu và cập nhật Theme Toàn Hệ Thống.')
  } catch (err) {
    console.error(err)
    ElMessage.error('Có lỗi xảy ra khi lưu Theme.')
  } finally {
    isSaving.value = false
  }
}
</script>

<style scoped>


.page-header {
  margin-bottom: 24px;
}

.breadcrumb {
  font-size: 13px;
  color: #8b949e;
  margin-bottom: 8px;
  display: flex;
  align-items: center;
  gap: 8px;
}

.page-title {
  font-size: 24px;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 4px;
}

.page-subtitle {
  font-size: 14px;
  color: #8b949e;
}

.settings-card {
  background-color: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: 12px;
  padding: 24px;
}

.card-title {
  font-size: 18px;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 8px;
}

.section-desc {
  font-size: 13px;
  color: #8b949e;
  margin-bottom: 20px;
}

.mt-24 {
  margin-top: 24px;
}

.color-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 20px;
}

.color-setting {
  background-color: var(--bg-layout);
  border: 1px solid var(--border-color);
  border-radius: 8px;
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.color-label {
  font-weight: 600;
  color: var(--text-primary);
  font-size: 14px;
}

.color-picker-wrapper {
  display: flex;
  align-items: center;
  gap: 12px;
}

.color-hex {
  font-family: monospace;
  background-color: var(--bg-hover);
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 13px;
  color: var(--text-primary);
  border: 1px solid var(--border-color);
}

.color-variable {
  font-size: 12px;
  color: #8b949e;
  font-family: monospace;
}

.action-footer {
  margin-top: 32px;
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style>
