<template>
  <AdminLayout>
    <div class="admin-page-header">
      <h1>User Management</h1>
      <div class="header-actions">
        <el-button type="primary" class="add-user-btn">
          <i class="fa-solid fa-plus mr-2"></i> Add User
        </el-button>
      </div>
    </div>

    <div class="users-list-card">
      <div class="user-row" v-for="user in users" :key="user.email">
        <div class="user-info-section">
          <el-avatar :size="48" :src="user.avatar" />
          <div class="user-details">
            <div class="user-name">{{ user.name }}</div>
            <div class="user-email">
              <i class="fa-solid fa-envelope mr-1"></i> {{ user.email }}
            </div>
          </div>
        </div>

        <div class="user-phone">
          <i class="fa-solid fa-phone mr-1"></i> {{ user.phone }}
        </div>

        <div class="user-role-section">
          <span class="role-pill" :class="'role-' + user.role.toLowerCase()">{{ user.role }}</span>
        </div>

        <div class="user-status-section">
          <div class="status-cell">
            <span class="status-dot" :class="user.status.toLowerCase()"></span>
            <span :class="user.status.toLowerCase() + '-text'">{{ user.status }}</span>
          </div>
        </div>

        <div class="user-actions">
          <el-button class="action-btn" circle plain>
            <i class="fa-solid fa-pen"></i>
          </el-button>
          <el-button class="action-btn" circle plain>
            <i class="fa-regular fa-trash-can"></i>
          </el-button>
        </div>
      </div>
    </div>

    <div class="pagination-container">
      <el-pagination
        v-model:current-page="currentPage"
        :page-size="10"
        layout="pager"
        :total="50"
        class="custom-pagination"
      />
    </div>
  </AdminLayout>
</template>

<script setup>
import { ref } from 'vue'
import AdminLayout from '@/components/layout/AdminLayout.vue'

const currentPage = ref(1)

const users = ref([
  { name: 'Sarah Johnson', email: 'sarah.johnson@company.com', phone: '+1 (555) 123-4567', role: 'Admin', status: 'Active', avatar: 'https://i.pravatar.cc/150?u=sarah' },
  { name: 'Michael Chen', email: 'michael.chen@company.com', phone: '+1 (555) 234-5678', role: 'Editor', status: 'Active', avatar: 'https://i.pravatar.cc/150?u=michael' },
  { name: 'Emily Rodriguez', email: 'emily.rodriguez@company.com', phone: '+1 (555) 345-6789', role: 'Editor', status: 'Active', avatar: 'https://i.pravatar.cc/150?u=emily' },
  { name: 'David Kim', email: 'david.kim@company.com', phone: '+1 (555) 456-7890', role: 'Viewer', status: 'Active', avatar: 'https://i.pravatar.cc/150?u=david' }
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

.add-user-btn {
  background-color: #0d9488 !important;
  border-color: #0d9488 !important;
  border-radius: 8px;
  font-weight: 500;
  padding: 10px 20px;
  box-shadow: 4px 4px 10px rgba(13, 148, 136, 0.3), -2px -2px 6px rgba(255,255,255,0.7);
}

.add-user-btn:hover {
  background-color: #0f766e !important;
}

.users-list-card {
  background: #ffffff;
  border-radius: 12px;
  padding: 12px 24px;
  box-shadow: 8px 8px 16px rgba(0,0,0,0.05), -8px -8px 16px rgba(255,255,255,0.8);
}

.user-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 20px 0;
  border-bottom: 1px solid #f1f5f9;
}

.user-row:last-child {
  border-bottom: none;
}

.user-info-section {
  display: flex;
  align-items: center;
  gap: 16px;
  width: 300px;
}

.user-name {
  font-weight: 600;
  color: #1e293b;
  font-size: 15px;
  margin-bottom: 4px;
}

.user-email {
  color: #64748b;
  font-size: 13px;
  display: flex;
  align-items: center;
}

.user-phone {
  color: #dc2626; /* Match image reference */
  font-size: 14px;
  font-weight: 500;
  display: flex;
  align-items: center;
  width: 200px;
}

.user-role-section {
  width: 100px;
  text-align: center;
}

.role-pill {
  padding: 6px 16px;
  border-radius: 20px;
  font-size: 13px;
  font-weight: 500;
}

.role-admin { background-color: #f3e8ff; color: #9333ea; }
.role-editor { background-color: #e0f2fe; color: #0284c7; }
.role-viewer { background-color: #f1f5f9; color: #475569; }

.user-status-section { width: 100px; }

.status-cell { display: flex; align-items: center; gap: 8px; }
.status-dot { width: 8px; height: 8px; border-radius: 50%; }
.status-dot.active { background-color: #10b981; }
.active-text { color: #10b981; font-weight: 500; font-size: 14px; }

.user-actions { display: flex; gap: 8px; }

.action-btn {
  border: none;
  background-color: #ffffff;
  box-shadow: inset 2px 2px 5px rgba(0,0,0,0.05), inset -2px -2px 5px rgba(255,255,255,0.7);
  color: #64748b;
}

.action-btn:hover {
  color: #0d9488;
  box-shadow: 2px 2px 5px rgba(0,0,0,0.1), -2px -2px 5px rgba(255,255,255,0.8);
}

.pagination-container {
  display: flex;
  justify-content: center;
  margin-top: 24px;
}

:deep(.custom-pagination .el-pager li) {
  background: #ffffff !important;
  border-radius: 8px;
  margin: 0 4px;
  box-shadow: 2px 2px 5px rgba(0,0,0,0.05), -2px -2px 5px rgba(255,255,255,0.8);
  font-weight: 500;
  color: #64748b;
  min-width: 36px;
  height: 36px;
  line-height: 36px;
}

:deep(.custom-pagination .el-pager li.is-active) {
  background: #0d9488 !important;
  color: #ffffff !important;
  box-shadow: inset 2px 2px 5px rgba(0,0,0,0.2) !important;
}
</style>
