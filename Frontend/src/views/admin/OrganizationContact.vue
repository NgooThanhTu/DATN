<template>
  <AdminLayout>
    <div class="admin-page-header">
      <h1>Contact Discovery</h1>
      <p class="subtitle">Manage how your organization can be discovered and contacted by others.</p>
    </div>

    <div class="contact-settings-card">
      <!-- Setting 1 -->
      <div class="setting-row">
        <div class="setting-content">
          <div class="icon-box icon-teal">
            <i class="fa-regular fa-envelope"></i>
          </div>
          <div class="setting-text">
            <h3>Enable Contact Discovery</h3>
            <p>Allow other organizations and users to find and contact your organization through the directory. Your primary contact email will be visible to verified users.</p>
          </div>
        </div>
        <el-switch v-model="settings.enableDiscovery" class="custom-switch" />
      </div>

      <el-divider class="setting-divider" />

      <!-- Setting 2 -->
      <div class="setting-row">
        <div class="setting-content">
          <div class="icon-box icon-purple">
            <i class="fa-solid fa-globe"></i>
          </div>
          <div class="setting-text">
            <h3>Public Organization Profile</h3>
            <p>Make your organization profile publicly visible. This includes your organization name, description, and website. Contact information remains private unless contact discovery is enabled.</p>
          </div>
        </div>
        <el-switch v-model="settings.publicProfile" class="custom-switch" />
      </div>

      <el-divider class="setting-divider" />

      <!-- Setting 3 -->
      <div class="setting-row">
        <div class="setting-content">
          <div class="icon-box icon-blue">
            <i class="fa-solid fa-shield-halved"></i>
          </div>
          <div class="setting-text">
            <h3>Email Notifications for Contact Requests</h3>
            <p>Receive email notifications when someone attempts to contact your organization through the platform. Administrators will be notified of all incoming requests.</p>
          </div>
        </div>
        <el-switch v-model="settings.emailNotifications" class="custom-switch" />
      </div>
    </div>

    <!-- Contact Preferences Card -->
    <div class="preferences-card">
      <h3>Contact Preferences</h3>
      <div class="preferences-grid">
        <el-checkbox v-model="preferences.partnership" label="Partnership Inquiries" size="large" />
        <el-checkbox v-model="preferences.support" label="Support Requests" size="large" />
        <el-checkbox v-model="preferences.sales" label="Sales Inquiries" size="large" />
        <el-checkbox v-model="preferences.media" label="Media & Press" size="large" />
      </div>
    </div>
  </AdminLayout>
</template>

<script setup>
import { reactive } from 'vue'
import AdminLayout from '@/components/layout/AdminLayout.vue'

const settings = reactive({
  enableDiscovery: true,
  publicProfile: false,
  emailNotifications: true
})

const preferences = reactive({
  partnership: true,
  support: true,
  sales: false,
  media: false
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

.contact-settings-card, .preferences-card {
  background: #ffffff;
  border-radius: 12px;
  padding: 32px;
  box-shadow: 8px 8px 16px rgba(0,0,0,0.05), -8px -8px 16px rgba(255,255,255,0.8);
  margin-bottom: 24px;
  max-width: 900px;
}

.setting-row {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.setting-content {
  display: flex;
  gap: 20px;
  max-width: 80%;
}

.icon-box {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
  flex-shrink: 0;
}

.icon-teal {
  background-color: #f0fdfa;
  color: #0d9488;
}

.icon-purple {
  background-color: #f5f3ff;
  color: #8b5cf6;
}

.icon-blue {
  background-color: #eff6ff;
  color: #3b82f6;
}

.setting-text h3 {
  margin: 0 0 8px 0;
  font-size: 16px;
  font-weight: 500;
  color: #1e293b;
}

.setting-text p {
  margin: 0;
  color: #64748b;
  font-size: 13px;
  line-height: 1.5;
}

.setting-divider {
  margin: 24px 0;
  border-color: #f1f5f9;
}

/* Custom switch matching Figma (Teal) */
:deep(.custom-switch.is-checked .el-switch__core) {
  background-color: #0d9488;
  border-color: #0d9488;
}

.preferences-card h3 {
  margin: 0 0 20px 0;
  font-size: 16px;
  font-weight: 600;
  color: #1e293b;
}

.preferences-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}

:deep(.el-checkbox__label) {
  color: #475569;
  font-weight: 500;
}

:deep(.el-checkbox.is-checked .el-checkbox__label) {
  color: #1e293b;
}

:deep(.el-checkbox__input.is-checked .el-checkbox__inner) {
  background-color: #3b82f6;
  border-color: #3b82f6;
}
</style>
