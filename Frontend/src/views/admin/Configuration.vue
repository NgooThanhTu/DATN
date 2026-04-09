<template>
  <AdminLayout>
    <div class="admin-page-container">
    <div class="page-header">
      <div class="breadcrumb">
        <i class="fa-solid fa-gear"></i> System / Configuration
      </div>
      <h1 class="page-title">Cấu hình Hệ thống (General Configuration)</h1>
      <p class="page-subtitle">Thiết lập các thông số cơ bản cho hệ thống và quy tắc bảo mật.</p>
    </div>

    <div class="form-container" v-loading="isLoading">
      <!-- System Settings -->
      <div class="settings-card">
        <h2 class="card-title">Hệ thống & Hiển thị</h2>
        
        <div class="setting-row input-row">
          <div class="setting-info">
            <span class="setting-label">Tên Hệ Thống (System Name)</span>
            <span class="setting-desc">Tên hiển thị trên tiêu đề của trình duyệt.</span>
          </div>
          <el-input v-model="settings.systemName" placeholder="Quantum Nexus" style="width: 300px" />
        </div>

        <div class="divider"></div>

        <div class="setting-row input-row">
          <div class="setting-info">
            <span class="setting-label">Múi giờ mặc định (Timezone)</span>
            <span class="setting-desc">Múi giờ chuẩn dùng để tính toán thời gian cho dự án.</span>
          </div>
          <el-select v-model="settings.timezone" style="width: 300px">
            <el-option label="Asia/Ho_Chi_Minh (GMT+7)" value="Asia/Ho_Chi_Minh"></el-option>
            <el-option label="UTC" value="UTC"></el-option>
            <el-option label="America/New_York (EST)" value="America/New_York"></el-option>
          </el-select>
        </div>

        <div class="divider"></div>

        <div class="setting-row input-row">
          <div class="setting-info">
            <span class="setting-label">Định dạng ngày (Date Format)</span>
          </div>
          <el-select v-model="settings.dateFormat" style="width: 300px">
            <el-option label="DD/MM/YYYY" value="DD/MM/YYYY"></el-option>
            <el-option label="MM/DD/YYYY" value="MM/DD/YYYY"></el-option>
            <el-option label="YYYY-MM-DD" value="YYYY-MM-DD"></el-option>
          </el-select>
        </div>
      </div>

      <!-- Security Settings -->
      <div class="settings-card mt-24">
        <h2 class="card-title">Bảo mật (Security)</h2>

        <div class="setting-row input-row">
          <div class="setting-info">
            <span class="setting-label">Thời gian hết hạn phiên (Session Timeout)</span>
            <span class="setting-desc">Số phút trước khi tự động đăng xuất nếu không có tương tác.</span>
          </div>
          <el-input-number v-model="settings.sessionTimeoutMin" :min="15" :max="1440" style="width: 150px" />
        </div>

        <div class="divider"></div>

        <div class="setting-row">
          <div class="setting-info">
            <span class="setting-label">Chính sách Mật khẩu (Password Policy)</span>
            <span class="setting-desc">Bắt buộc mật khẩu bao gồm chữ hoa, số và ký tự đặc biệt.</span>
          </div>
          <el-switch v-model="settings.strictPassword" />
        </div>

        <div class="divider"></div>

        <div class="setting-row">
          <div class="setting-info">
            <span class="setting-label">Bắt buộc xác thực 2 lớp (Force 2FA)</span>
            <span class="setting-desc">Bắt buộc mọi người dùng kích hoạt 2FA.</span>
          </div>
          <el-switch v-model="settings.force2FA" active-color="#f87171" inactive-color="#4b5563" />
        </div>
      </div>

      <div class="action-footer">
        <el-button @click="fetchSettings">Hủy</el-button>
        <el-button type="primary" :loading="isSaving" @click="saveSettings">Lưu cài đặt</el-button>
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

const settings = ref({
  systemName: 'Quantum Nexus',
  timezone: 'Asia/Ho_Chi_Minh',
  dateFormat: 'DD/MM/YYYY',
  sessionTimeoutMin: 60,
  strictPassword: true,
  force2FA: false
})

onMounted(async () => {
  await fetchSettings()
})

const fetchSettings = async () => {
  isLoading.value = true
  try {
    const res = await axiosClient.get('/settings/GeneralConfig')
    const data = res.data.data || {}
    
    if (data.systemName) settings.value.systemName = data.systemName
    if (data.timezone) settings.value.timezone = data.timezone
    if (data.dateFormat) settings.value.dateFormat = data.dateFormat
    if (data.sessionTimeoutMin) settings.value.sessionTimeoutMin = parseInt(data.sessionTimeoutMin) || 60
    if (data.strictPassword) settings.value.strictPassword = data.strictPassword === 'true'
    if (data.force2FA) settings.value.force2FA = data.force2FA === 'true'
  } catch (err) {
    console.error(err)
    ElMessage.error('Không thể tải cấu hình Hệ thống.')
  } finally {
    isLoading.value = false
  }
}

const saveSettings = async () => {
  isSaving.value = true
  try {
    const payload = {
      Settings: {
        systemName: settings.value.systemName,
        timezone: settings.value.timezone,
        dateFormat: settings.value.dateFormat,
        sessionTimeoutMin: settings.value.sessionTimeoutMin.toString(),
        strictPassword: settings.value.strictPassword ? 'true' : 'false',
        force2FA: settings.value.force2FA ? 'true' : 'false'
      }
    }
    await axiosClient.put('/settings/GeneralConfig', payload)
    ElMessage.success('Đã lưu cấu hình Hệ thống thành công.')
  } catch (err) {
    console.error(err)
    ElMessage.error('Có lỗi xảy ra khi lưu cấu hình.')
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

.mt-24 {
  margin-top: 24px;
}

.card-title {
  font-size: 18px;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 20px;
}

.setting-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.setting-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.setting-label {
  font-weight: 600;
  color: var(--text-primary);
  font-size: 14px;
}

.setting-desc {
  font-size: 13px;
  color: #8b949e;
}

.divider {
  height: 1px;
  background-color: var(--border-color);
  margin: 20px 0;
}

.input-row {
  align-items: center;
}

.action-footer {
  margin-top: 32px;
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style>
