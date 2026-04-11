<template>
  <AdminLayout>
    <div class="admin-page-header">
      <div class="header-title-section">
        <div class="breadcrumb">
          <i class="fa-regular fa-user"></i> Admin / Profile
        </div>
        <h1 class="page-title">Hồ sơ cá nhân (Admin Profile)</h1>
      </div>
    </div>

    <div class="admin-form-card" v-loading="isLoading">
      <div class="form-group">
        <label>Họ tên (Full Name)</label>
        <el-input v-model="form.fullName" class="neumorphic-input" />
      </div>

      <div class="form-group">
        <label>Tên công khai (Public Name)</label>
        <el-input v-model="form.publicName" class="neumorphic-input" />
      </div>

      <div class="form-group">
        <label>Chức danh (Job Title)</label>
        <el-input v-model="form.jobTitle" class="neumorphic-input" />
      </div>

      <div class="form-group">
        <label>Phòng ban (Department)</label>
        <el-input v-model="form.departmentName" class="neumorphic-input" />
      </div>

      <div class="form-group">
        <label>Tổ chức (Organization)</label>
        <el-input v-model="form.organizationName" class="neumorphic-input" />
      </div>

      <div class="form-group">
        <label>Cộng tác với bạn (Collaboration Rules)</label>
        <el-input v-model="form.collaborationRules" type="textarea" :rows="3" class="neumorphic-input" placeholder="Giúp người khác nắm được thời điểm và cách thức cộng tác với bạn" />
      </div>

      <div class="form-group">
        <label>Địa chỉ email</label>
        <el-input v-model="form.email" class="neumorphic-input" disabled />
        <div class="helper-text">Email không thể thay đổi trực tiếp tại đây. Nó chỉ được xem bởi bạn và quản trị viên.</div>
      </div>

      <div class="form-actions" style="margin-top: 32px">
        <el-button type="primary" class="save-btn" :loading="isSaving" @click="saveProfile">
          <i class="fa-regular fa-floppy-disk mr-2"></i> Lưu thay đổi
        </el-button>
      </div>
    </div>
  </AdminLayout>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import AdminLayout from '@/components/layout/AdminLayout.vue'
import axiosClient from '@/api/axiosClient'
import { ElMessage } from 'element-plus'

const isLoading = ref(false)
const isSaving = ref(false)

const form = ref({
  fullName: '',
  publicName: '',
  jobTitle: '',
  departmentName: '',
  organizationName: '',
  email: '',
  collaborationRules: ''
})

onMounted(async () => {
  await fetchProfile()
})

const fetchProfile = async () => {
  try {
    isLoading.value = true
    const response = await axiosClient.get('/users/me')
    const data = response.data.data || response.data
    form.value = {
      fullName: data.fullName || '',
      publicName: data.publicName || '',
      jobTitle: data.jobTitle || '',
      departmentName: data.departmentName || '',
      organizationName: data.organizationName || '',
      email: data.email || '',
      collaborationRules: data.collaborationRules || ''
    }
  } catch (err) {
    console.error('Lỗi khi tải profile', err)
    ElMessage.error('Không thể tải thông tin cá nhân.')
  } finally {
    isLoading.value = false
  }
}

const saveProfile = async () => {
  try {
    isSaving.value = true
    await axiosClient.put('/users/profile', {
      fullName: form.value.fullName,
      publicName: form.value.publicName,
      jobTitle: form.value.jobTitle,
      departmentName: form.value.departmentName,
      organizationName: form.value.organizationName,
      collaborationRules: form.value.collaborationRules
    })
    ElMessage.success('Đã lưu thông tin hồ sơ.')
  } catch (err) {
    console.error('Lỗi khi lưu profile', err)
    ElMessage.error('Lưu thông tin thất bại.')
  } finally {
    isSaving.value = false
  }
}
</script>

<style scoped>
.admin-page-header {
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
  margin: 0;
}

.admin-form-card {
  background: var(--bg-card);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border: 1px solid var(--border-color);
  border-radius: 12px;
  padding: 32px;
  box-shadow: 0 8px 32px rgba(0,0,0,0.05);
  max-width: 800px;
}

.form-group {
  margin-bottom: 24px;
}

.form-group label {
  display: block;
  font-size: 14px;
  font-weight: 500;
  color: var(--text-primary);
  margin-bottom: 8px;
}

.helper-text {
  font-size: 12px;
  color: #0d9488;
  margin-top: 8px;
}

:deep(.neumorphic-input .el-input__wrapper),
:deep(.neumorphic-input .el-textarea__inner) {
  background-color: var(--bg-hover) !important;
  box-shadow: none !important;
  border-radius: 8px;
  border: 1px solid var(--border-color);
}

:deep(.neumorphic-input .el-input__wrapper) {
  padding: 8px 16px;
}

:deep(.neumorphic-input .el-textarea__inner) {
  padding: 12px 16px;
  font-family: inherit;
  color: var(--text-primary) !important;
}

:deep(.neumorphic-input.is-disabled .el-input__wrapper) {
  background-color: var(--bg-layout) !important;
  opacity: 0.7;
}

:deep(.neumorphic-input .el-input__inner) {
  color: var(--text-primary) !important;
  height: 24px;
  line-height: 24px;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
}

.save-btn {
  background-color: #0d9488 !important;
  border-color: #0d9488 !important;
  border-radius: 8px;
  font-weight: 500;
  padding: 20px 24px;
  font-size: 15px;
  color: white;
}

.save-btn:hover {
  background-color: #0f766e !important;
}
</style>
