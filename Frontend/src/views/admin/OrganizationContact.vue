<template>
  <AdminLayout>
    <div class="admin-page-container">
    <div class="page-header">
      <div class="breadcrumb">
        <i class="fa-solid fa-address-book"></i> Organization / Contact Discovery
      </div>
      <h1 class="page-title">Cấu hình Liên hệ Tổ chức</h1>
      <p class="page-subtitle">Quản lý cách tổ chức của bạn được tìm thấy và liên hệ từ bên ngoài hệ thống.</p>
    </div>

    <div class="form-container" v-loading="isLoading">
      <div class="settings-card">
        <h2 class="card-title">Khả năng khám phá</h2>
        
        <div class="setting-row">
          <div class="setting-info">
            <span class="setting-label">Cho phép Liên hệ (Enable Contact)</span>
            <span class="setting-desc">Cho phép mọi người gửi tin nhắn hoặc yêu cầu hỗ trợ qua email cho tổ chức.</span>
          </div>
          <el-switch v-model="settings.enableContact" />
        </div>

        <div class="divider"></div>

        <div class="setting-row">
          <div class="setting-info">
            <span class="setting-label">Hồ sơ Tổ chức Công khai (Public Org Profile)</span>
            <span class="setting-desc">Làm cho tổ chức của bạn hiển thị trong danh bạ công khai hệ thống đối tác.</span>
          </div>
          <el-switch v-model="settings.publicOrgProfile" />
        </div>
      </div>

      <div class="settings-card mt-24">
        <h2 class="card-title">Thông báo & Hỗ trợ</h2>

        <div class="setting-row input-row">
          <div class="setting-info">
            <span class="setting-label">Email nhận thông báo</span>
            <span class="setting-desc">Địa chỉ email để nhận các yêu cầu liên hệ hoặc hỗ trợ mới.</span>
          </div>
          <el-input v-model="settings.notificationEmail" placeholder="support@yourorg.com" style="width: 300px" />
        </div>

        <div class="divider"></div>

        <div class="setting-row checkbox-row">
          <div class="setting-info">
            <span class="setting-label">Chủ đề liên hệ được phép</span>
            <span class="setting-desc">Bật/tắt các loại yêu cầu có thể gửi cho tổ chức của bạn.</span>
          </div>
            <el-checkbox-group v-model="settings.allowedTypes" class="types-group">
            <el-checkbox label="Support">Hỗ trợ kỹ thuật</el-checkbox>
            <el-checkbox label="Sales">Kinh doanh & Bán hàng</el-checkbox>
            <el-checkbox label="Partnerships">Hợp tác đối tác</el-checkbox>
          </el-checkbox-group>
        </div>
      </div>

      <div class="action-footer">
        <el-button @click="fetchSettings">Hủy</el-button>
        <el-button type="primary" :loading="isSaving" @click="saveSettings">Lưu cấu hình</el-button>
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
  enableContact: false,
  publicOrgProfile: false,
  notificationEmail: '',
  allowedTypes: ['Support', 'Sales']
})

onMounted(async () => {
  await fetchSettings()
})

const fetchSettings = async () => {
  isLoading.value = true
  try {
    const res = await axiosClient.get('/settings/ContactDiscovery')
    const data = res.data.data || {}
    
    settings.value.enableContact = data.enableContact === 'true'
    settings.value.publicOrgProfile = data.publicOrgProfile === 'true'
    settings.value.notificationEmail = data.notificationEmail || ''
    
    if (data.allowedTypes) {
      try {
        settings.value.allowedTypes = JSON.parse(data.allowedTypes)
      } catch (e) {
        settings.value.allowedTypes = ['Support', 'Sales']
      }
    }
  } catch (err) {
    console.error(err)
    ElMessage.error('Không thể tải cấu hình Contact Discovery.')
  } finally {
    isLoading.value = false
  }
}

const saveSettings = async () => {
  isSaving.value = true
  try {
    const payload = {
      Settings: {
        enableContact: settings.value.enableContact ? 'true' : 'false',
        publicOrgProfile: settings.value.publicOrgProfile ? 'true' : 'false',
        notificationEmail: settings.value.notificationEmail,
        allowedTypes: JSON.stringify(settings.value.allowedTypes)
      }
    }
    await axiosClient.put('/settings/ContactDiscovery', payload)
    ElMessage.success('Đã lưu cấu hình Contact Discovery.')
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

.input-row, .checkbox-row {
  align-items: flex-start;
}

.types-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.action-footer {
  margin-top: 32px;
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style>
