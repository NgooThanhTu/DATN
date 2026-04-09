<template>
  <AdminLayout>
    <div class="admin-page-header">
      <h1>Audit Log</h1>
      <div class="header-actions">
        <el-radio-group v-model="timeFilter" class="custom-radio-group">
          <el-radio-button label="All Time" />
          <el-radio-button label="24h" />
          <el-radio-button label="30d" />
        </el-radio-group>
      </div>
    </div>

    <div class="admin-card">
      <el-table :data="logs" style="width: 100%" class="admin-table" :show-header="true">
        <el-table-column prop="timestamp" label="TIMESTAMP" min-width="150" />
        <el-table-column prop="user" label="USER" min-width="200" />
        <el-table-column prop="action" label="ACTION" min-width="150" />
        <el-table-column prop="resource" label="RESOURCE" min-width="220" />
        
        <el-table-column prop="status" label="STATUS" min-width="120">
          <template #default="scope">
            <div class="status-cell">
              <span class="status-dot" :class="scope.row.status.toLowerCase()"></span>
              <span :class="scope.row.status.toLowerCase() + '-text'">{{ scope.row.status }}</span>
            </div>
          </template>
        </el-table-column>
        
        <el-table-column prop="ip" label="IP ADDRESS" min-width="150" />
      </el-table>
    </div>
  </AdminLayout>
</template>

<script setup>
import { ref } from 'vue'
import AdminLayout from '@/components/layout/AdminLayout.vue'

const timeFilter = ref('All Time')

const logs = ref([
  { timestamp: '2026-04-03\n14:32:15', user: 'admin@nexus.io', action: 'User Created', resource: 'john.doe@company.com', status: 'Success', ip: '192.168.1.100' },
  { timestamp: '2026-04-03\n14:18:42', user: 'manager@nexus.io', action: 'Role Updated', resource: 'jane.smith@company.com', status: 'Success', ip: '192.168.1.101' },
  { timestamp: '2026-04-03\n13:55:30', user: 'admin@nexus.io', action: 'Login Failed', resource: 'unknown@company.com', status: 'Warning', ip: '203.0.113.45' },
  { timestamp: '2026-04-03\n13:42:18', user: 'editor@nexus.io', action: 'Document Edited', resource: 'project-specs.pdf', status: 'Success', ip: '192.168.1.102' },
  { timestamp: '2026-04-03\n13:15:05', user: 'admin@nexus.io', action: 'Settings Changed', resource: 'Security Policy', status: 'Success', ip: '192.168.1.100' },
  { timestamp: '2026-04-03\n12:58:33', user: 'user@nexus.io', action: 'Access Denied', resource: '/admin/settings', status: 'Warning', ip: '198.51.100.23' },
  { timestamp: '2026-04-03\n12:30:12', user: 'admin@nexus.io', action: 'User Deleted', resource: 'old.user@company.com', status: 'Success', ip: '192.168.1.100' }
])
</script>

<style scoped>
.admin-page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.admin-page-header h1 {
  font-size: 24px;
  font-weight: 500;
  color: #1e293b;
  margin: 0;
}

.admin-card {
  background: #ffffff;
  border-radius: 12px;
  padding: 24px;
  /* Light Neumorphism Box Shadow */
  box-shadow: 8px 8px 16px rgba(0,0,0,0.05), -8px -8px 16px rgba(255,255,255,0.8);
}

:deep(.admin-table th.el-table__cell) {
  background-color: transparent !important;
  color: #0d9488 !important; /* Primary Teal */
  font-weight: 700;
  font-size: 12px;
  text-transform: uppercase;
  border-bottom: 2px solid #f1f5f9;
}

:deep(.admin-table td.el-table__cell) {
  padding: 16px 0;
  color: #475569;
  font-size: 14px;
  border-bottom: 1px solid #f8fafc;
  white-space: pre-line;
}

.status-cell {
  display: flex;
  align-items: center;
  gap: 8px;
}

.status-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
}

.status-dot.success { background-color: #10b981; }
.success-text { color: #10b981; font-weight: 500; }

.status-dot.warning { background-color: #f59e0b; }
.warning-text { color: #f59e0b; font-weight: 500; }

.status-dot.error { background-color: #ef4444; }
.error-text { color: #ef4444; font-weight: 500; }

/* Custom Radio Group matching image */
:deep(.custom-radio-group .el-radio-button__inner) {
  border: none !important;
  background-color: #ffffff;
  box-shadow: 2px 2px 5px rgba(0,0,0,0.05), -2px -2px 5px rgba(255,255,255,0.8);
  color: #64748b;
  border-radius: 6px !important;
  margin-left: 8px;
  font-weight: 500;
}

:deep(.custom-radio-group .el-radio-button:first-child .el-radio-button__inner) {
  margin-left: 0;
}

:deep(.custom-radio-group .el-radio-button__original-radio:checked + .el-radio-button__inner) {
  background-color: #f8fafc;
  color: #0d9488;
  box-shadow: inset 2px 2px 5px rgba(0,0,0,0.05), inset -2px -2px 5px rgba(255,255,255,0.8);
}
</style>
