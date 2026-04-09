<template>
  <AdminLayout>
    <div class="admin-page-header">
      <h1>Organization Profile</h1>
    </div>

    <div class="admin-form-card">
      <div class="form-group">
        <label>Organization Name</label>
        <el-input v-model="form.name" class="neumorphic-input" />
      </div>

      <div class="form-group">
        <label>Organization ID</label>
        <el-input v-model="form.id" class="neumorphic-input" disabled />
        <div class="helper-text">This is your unique organization identifier and cannot be changed.</div>
      </div>

      <div class="form-group">
        <label>Primary Contact Email</label>
        <el-input v-model="form.email" class="neumorphic-input" />
      </div>

      <div class="form-group">
        <label>Website</label>
        <el-input v-model="form.website" class="neumorphic-input" />
      </div>

      <div class="form-group">
        <label>Company Size</label>
        <el-input v-model="form.companySize" class="neumorphic-input" />
      </div>

      <div class="warning-alert">
        <div class="alert-header">
          <i class="fa-solid fa-triangle-exclamation"></i>
          <span>Transferring Applications</span>
        </div>
        <div class="alert-body">
          Changing the organization owner will transfer all applications and settings to the new owner. This action requires verification from both parties.
        </div>
      </div>

      <div class="form-actions">
        <el-button type="primary" class="save-btn">
          <i class="fa-regular fa-floppy-disk mr-2"></i> Save Changes
        </el-button>
      </div>
    </div>
  </AdminLayout>
</template>

<script setup>
import { ref } from 'vue'
import AdminLayout from '@/components/layout/AdminLayout.vue'

const form = ref({
  name: 'Nexus Corporation',
  id: 'org_8f3kd92jd0s',
  email: 'admin@nexuscorp.com',
  website: 'https://nexuscorp.com',
  companySize: '51-200 employees'
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
  margin: 0;
}

.admin-form-card {
  background: #ffffff;
  border-radius: 12px;
  padding: 32px;
  box-shadow: 8px 8px 16px rgba(0,0,0,0.05), -8px -8px 16px rgba(255,255,255,0.8);
  max-width: 800px;
}

.form-group {
  margin-bottom: 24px;
}

.form-group label {
  display: block;
  font-size: 14px;
  font-weight: 500;
  color: #1e293b;
  margin-bottom: 8px;
}

.helper-text {
  font-size: 12px;
  color: #0d9488; /* Cyan-teal color like image */
  margin-top: 8px;
}

:deep(.neumorphic-input .el-input__wrapper) {
  background-color: #ffffff;
  box-shadow: inset 2px 2px 5px rgba(0,0,0,0.05), inset -2px -2px 5px rgba(255,255,255,0.8) !important;
  border-radius: 8px;
  padding: 8px 16px;
}

:deep(.neumorphic-input.is-disabled .el-input__wrapper) {
  background-color: #f8fafc;
}

:deep(.neumorphic-input .el-input__inner) {
  color: #475569;
  height: 24px;
  line-height: 24px;
}

.warning-alert {
  background-color: #fffbeb;
  border: 1px solid #fde68a;
  border-radius: 8px;
  padding: 16px;
  margin-top: 32px;
  margin-bottom: 32px;
}

.alert-header {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #b45309;
  font-weight: 600;
  font-size: 14px;
  margin-bottom: 4px;
}

.alert-body {
  color: #b45309;
  font-size: 13px;
  line-height: 1.5;
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
  box-shadow: 4px 4px 10px rgba(13, 148, 136, 0.3), -2px -2px 6px rgba(255,255,255,0.7);
}

.save-btn:hover {
  background-color: #0f766e !important;
}
</style>
