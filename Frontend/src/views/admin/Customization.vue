<template>
  <AdminLayout>
    <div class="admin-page-header">
      <h1>Customization</h1>
      <p class="subtitle">Customize your workspace appearance with custom colors or choose from pre-built templates</p>
    </div>

    <div class="tabs-container">
      <div class="tab-buttons">
        <button class="tab-btn active">
          <i class="fa-solid fa-palette mr-2"></i> Custom Colors
        </button>
        <button class="tab-btn">
          <i class="fa-solid fa-wand-magic-sparkles mr-2"></i> Templates
        </button>
      </div>
    </div>

    <div class="customization-card">
      <h3 class="card-title">Custom Color Settings</h3>
      
      <div class="colors-grid">
        <!-- Row 1 -->
        <div class="color-setting">
          <label>Background Color</label>
          <div class="color-picker-wrapper">
            <el-color-picker v-model="colors.background" />
            <span class="color-hex">{{ colors.background }}</span>
          </div>
        </div>
        <div class="color-setting">
          <label>Text Color</label>
          <div class="color-picker-wrapper">
            <el-color-picker v-model="colors.text" />
            <span class="color-hex">{{ colors.text }}</span>
          </div>
        </div>

        <!-- Row 2 -->
        <div class="color-setting">
          <label>Card Background</label>
          <div class="color-picker-wrapper">
            <el-color-picker v-model="colors.cardBackground" />
            <span class="color-hex">{{ colors.cardBackground }}</span>
          </div>
        </div>
        <div class="color-setting">
          <label>Border Color</label>
          <div class="color-picker-wrapper">
            <el-color-picker v-model="colors.border" />
            <span class="color-hex">{{ colors.border }}</span>
          </div>
        </div>

        <!-- Row 3 -->
        <div class="color-setting">
          <label>Header Background</label>
          <div class="color-picker-wrapper">
            <el-color-picker v-model="colors.headerBackground" />
            <span class="color-hex">{{ colors.headerBackground }}</span>
          </div>
        </div>
        <div class="color-setting">
          <label>Sidebar Background</label>
          <div class="color-picker-wrapper">
            <el-color-picker v-model="colors.sidebarBackground" />
            <span class="color-hex">{{ colors.sidebarBackground }}</span>
          </div>
        </div>

        <!-- Row 4 -->
        <div class="color-setting">
          <label>Sprint Container Background</label>
          <div class="color-picker-wrapper">
            <el-color-picker v-model="colors.sprintBackground" />
            <span class="color-hex">{{ colors.sprintBackground }}</span>
          </div>
        </div>
      </div>

      <div class="action-buttons">
        <el-button type="primary" class="apply-btn">Apply Custom Colors</el-button>
        <el-button class="reset-btn">Reset to Default</el-button>
      </div>
    </div>
  </AdminLayout>
</template>

<script setup>
import { reactive } from 'vue'
import AdminLayout from '@/components/layout/AdminLayout.vue'

const colors = reactive({
  background: '#e8eaed',
  text: '#1f2937',
  cardBackground: '#ffffff',
  border: '#e5e7eb',
  headerBackground: '#ffffff',
  sidebarBackground: '#ffffff',
  sprintBackground: '#f3f4f6'
})
</script>

<style scoped>
.admin-page-header {
  margin-bottom: 24px;
}

.admin-page-header h1 {
  font-size: 24px;
  font-weight: 500;
  color: #1e293b;
  margin: 0 0 8px 0;
}

.subtitle {
  color: #64748b;
  margin: 0;
  font-size: 14px;
}

.tabs-container {
  margin-bottom: 24px;
}

.tab-buttons {
  display: flex;
  gap: 8px;
}

.tab-btn {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  padding: 10px 20px;
  border-radius: 8px;
  color: #475569;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  box-shadow: 2px 2px 5px rgba(0,0,0,0.05);
}

.tab-btn:hover {
  background: #f8fafc;
}

.tab-btn.active {
  background: #0d9488;
  color: #ffffff;
  border-color: #0d9488;
  box-shadow: inset 2px 2px 5px rgba(0,0,0,0.1);
}

.customization-card {
  background: #ffffff;
  border-radius: 12px;
  padding: 32px;
  box-shadow: 8px 8px 16px rgba(0,0,0,0.05), -8px -8px 16px rgba(255,255,255,0.8);
  max-width: 900px;
}

.card-title {
  font-size: 18px;
  font-weight: 500;
  color: #1e293b;
  margin: 0 0 24px 0;
}

.colors-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 24px;
  margin-bottom: 32px;
}

.color-setting label {
  display: block;
  font-size: 14px;
  font-weight: 500;
  color: #1e293b;
  margin-bottom: 8px;
}

.color-picker-wrapper {
  display: flex;
  align-items: center;
  gap: 16px;
  background-color: #ffffff;
  border-radius: 8px;
  padding: 8px 16px;
  box-shadow: inset 2px 2px 5px rgba(0,0,0,0.05), inset -2px -2px 5px rgba(255,255,255,0.8);
}

.color-hex {
  font-family: monospace;
  font-size: 14px;
  color: #475569;
}

/* Customizing the el-color-picker visual to match UI reference better */
:deep(.el-color-picker__trigger) {
  border: 1px solid #e5e7eb;
  border-radius: 4px;
  padding: 2px;
}

.action-buttons {
  display: flex;
  gap: 16px;
}

.apply-btn {
  background-color: #0d9488 !important;
  border-color: #0d9488 !important;
  border-radius: 8px;
  font-weight: 500;
  padding: 20px 24px;
  font-size: 15px;
  box-shadow: 4px 4px 10px rgba(13, 148, 136, 0.3), -2px -2px 6px rgba(255,255,255,0.7);
}

.apply-btn:hover {
  background-color: #0f766e !important;
}

.reset-btn {
  background-color: #ffffff !important;
  border-color: #e5e7eb !important;
  color: #475569 !important;
  border-radius: 8px;
  font-weight: 500;
  padding: 20px 24px;
  font-size: 15px;
  box-shadow: 2px 2px 5px rgba(0,0,0,0.05);
}

.reset-btn:hover {
  background-color: #f8fafc !important;
}
</style>
