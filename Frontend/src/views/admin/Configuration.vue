<template>
  <AdminLayout>
    <div class="admin-page-header">
      <h1>General Configuration</h1>
    </div>

    <div class="config-grid">
      <!-- System Settings Card -->
      <div class="config-card">
        <div class="card-header">
          <div class="icon-box icon-teal">
            <i class="fa-solid fa-server"></i>
          </div>
          <h2>System Settings</h2>
        </div>
        
        <div class="form-group">
          <label>System Name</label>
          <el-input v-model="systemName" class="neumorphic-input" />
        </div>
        
        <div class="form-group">
          <label>Time Zone</label>
          <el-input v-model="timeZone" class="neumorphic-input" />
        </div>
        
        <div class="form-group">
          <label>Date Format</label>
          <el-input v-model="dateFormat" class="neumorphic-input" />
        </div>
      </div>

      <!-- Security Card -->
      <div class="config-card">
        <div class="card-header">
          <div class="icon-box icon-purple">
            <i class="fa-solid fa-shield-halved"></i>
          </div>
          <h2>Security</h2>
        </div>
        
        <div class="form-group">
          <label>Session Timeout (minutes)</label>
          <el-input v-model="sessionTimeout" class="neumorphic-input" />
        </div>
        
        <div class="form-group">
          <label>Password Policy</label>
          <el-input v-model="passwordPolicy" class="neumorphic-input" />
        </div>
        
        <div class="form-group mt-4">
          <el-checkbox v-model="require2FA" label="Require Two-Factor Authentication" />
        </div>
      </div>

      <!-- Performance Metrics Card -->
      <div class="config-card">
        <div class="card-header">
          <div class="icon-box icon-blue">
            <i class="fa-solid fa-chart-line"></i>
          </div>
          <h2>Performance Metrics</h2>
        </div>
        
        <div class="metrics-content">
          <div class="metric-header">
            <span class="metric-label">API Response Time</span>
            <span class="metric-value">127ms</span>
          </div>
          <div class="chart-bars">
            <!-- Mock bars -->
            <div class="bar" style="height: 30%"></div>
            <div class="bar" style="height: 40%"></div>
            <div class="bar" style="height: 35%"></div>
            <div class="bar" style="height: 45%"></div>
            <div class="bar" style="height: 50%"></div>
            <div class="bar" style="height: 40%"></div>
            <div class="bar" style="height: 55%"></div>
            <div class="bar" style="height: 60%"></div>
            <div class="bar" style="height: 50%"></div>
            <div class="bar" style="height: 40%"></div>
          </div>
        </div>
      </div>

      <!-- Recent Audit Logs Card -->
      <div class="config-card">
        <div class="card-header">
          <div class="icon-box icon-orange">
            <i class="fa-regular fa-file-lines"></i>
          </div>
          <h2>Recent Audit Logs</h2>
        </div>
        
        <div class="recent-logs">
          <div class="log-item">
            <div class="log-icon" style="color: #0d9488; background: #f0fdfa;">
              <i class="fa-solid fa-shield-halved"></i>
            </div>
            <div class="log-details">
              <span class="log-action">Security settings updated</span>
              <span class="log-time">2 minutes ago</span>
            </div>
          </div>
          
          <div class="log-item">
            <div class="log-icon" style="color: #3b82f6; background: #eff6ff;">
              <i class="fa-solid fa-server"></i>
            </div>
            <div class="log-details">
              <span class="log-action">Database backup completed</span>
              <span class="log-time">15 minutes ago</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </AdminLayout>
</template>

<script setup>
import { ref } from 'vue'
import AdminLayout from '@/components/layout/AdminLayout.vue'

const systemName = ref('Nexus Platform')
const timeZone = ref('EST (GMT-5)')
const dateFormat = ref('YYYY-MM-DD')
const sessionTimeout = ref('30')
const passwordPolicy = ref('Strong')
const require2FA = ref(true)
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

.config-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 24px;
  max-width: 1200px;
}

.config-card {
  background: #ffffff;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 8px 8px 16px rgba(0,0,0,0.05), -8px -8px 16px rgba(255,255,255,0.8);
}

.card-header {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 24px;
}

.card-header h2 {
  font-size: 18px;
  font-weight: 600;
  color: #1e293b;
  margin: 0;
}

.icon-box {
  width: 40px;
  height: 40px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 18px;
}

.icon-teal { background-color: #f0fdfa; color: #0d9488; }
.icon-purple { background-color: #f5f3ff; color: #8b5cf6; }
.icon-blue { background-color: #eff6ff; color: #3b82f6; }
.icon-orange { background-color: #fff7ed; color: #ea580c; }

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  font-size: 14px;
  font-weight: 500;
  color: #1e293b;
  margin-bottom: 8px;
}

:deep(.neumorphic-input .el-input__wrapper) {
  background-color: #ffffff;
  box-shadow: inset 2px 2px 5px rgba(0,0,0,0.05), inset -2px -2px 5px rgba(255,255,255,0.8) !important;
  border-radius: 8px;
  padding: 8px 16px;
}

:deep(.neumorphic-input .el-input__inner) {
  color: #475569;
  height: 24px;
  line-height: 24px;
}

/* Performance Metrics */
.metrics-content {
  margin-top: 16px;
}

.metric-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.metric-label {
  font-size: 14px;
  font-weight: 500;
  color: #475569;
}

.metric-value {
  font-size: 16px;
  font-weight: 600;
  color: #10b981;
}

.chart-bars {
  display: flex;
  align-items: flex-end;
  height: 80px;
  gap: 8px;
}

.bar {
  flex: 1;
  background-color: #10b981;
  border-radius: 4px 4px 0 0;
}

/* Recent Audit Logs */
.recent-logs {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.log-item {
  display: flex;
  align-items: flex-start;
  gap: 16px;
  padding: 16px;
  border-radius: 8px;
  border: 1px solid #f1f5f9;
}

.log-icon {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
}

.log-details {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.log-action {
  font-size: 14px;
  color: #1e293b;
  font-weight: 500;
}

.log-time {
  font-size: 12px;
  color: #64748b;
}

@media (max-width: 1024px) {
  .config-grid {
    grid-template-columns: 1fr;
  }
}
</style>
