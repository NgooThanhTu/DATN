<template>
  <aside class="admin-sidebar shadow-sm">
    <div class="sidebar-header">
      <h2>Nexus Admin</h2>
    </div>

    <div class="back-link">
      <router-link to="/" class="flex items-center gap-2">
        <i class="fa-solid fa-arrow-left"></i>
        <span>Back to Projects</span>
      </router-link>
    </div>

    <el-menu
      :default-active="activeMenu"
      class="admin-menu"
      :router="true"
    >
      <el-menu-item index="/admin/audit-log">
        <i class="fa-solid fa-file-lines menu-icon"></i>
        <span>Audit Log</span>
      </el-menu-item>

      <el-menu-item index="/admin/users">
        <i class="fa-solid fa-users menu-icon"></i>
        <span>User Management</span>
      </el-menu-item>

      <el-sub-menu index="/admin/organization">
        <template #title>
          <i class="fa-regular fa-building menu-icon"></i>
          <span>Organization</span>
        </template>
        <el-menu-item index="/admin/organization/profile">Profile</el-menu-item>
        <el-menu-item index="/admin/organization/contact">Contact</el-menu-item>
      </el-sub-menu>

      <el-menu-item index="/admin/configuration">
        <i class="fa-solid fa-gear menu-icon"></i>
        <span>Configuration</span>
      </el-menu-item>

      <el-menu-item index="/admin/customization">
        <i class="fa-solid fa-palette menu-icon"></i>
        <span>Customization</span>
      </el-menu-item>
    </el-menu>
  </aside>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

const activeMenu = computed(() => {
  return route.path
})
</script>

<style scoped>
.admin-sidebar {
  width: 250px;
  background-color: #ffffff;
  display: flex;
  flex-direction: column;
  border-right: 1px solid #eaeaea;
  z-index: 10;
}

.sidebar-header {
  padding: 24px;
}

.sidebar-header h2 {
  font-size: 20px;
  font-weight: 600;
  color: #1e293b;
  margin: 0;
}

.back-link {
  padding: 0 24px 24px 24px;
}

.back-link a {
  color: #475569;
  text-decoration: none;
  font-size: 14px;
  font-weight: 500;
}

.back-link a:hover {
  color: #1e293b;
}

.admin-menu {
  border-right: none;
}

.menu-icon {
  width: 24px;
  text-align: center;
  margin-right: 8px;
  font-size: 16px;
  color: #64748b;
}

:deep(.el-menu-item) {
  height: 44px;
  line-height: 44px;
  margin: 4px 12px;
  border-radius: 6px;
  color: #475569;
}

:deep(.el-sub-menu__title) {
  height: 44px;
  line-height: 44px;
  margin: 4px 12px;
  border-radius: 6px;
  color: #475569;
}

:deep(.el-menu-item.is-active) {
  background-color: #f0fdfa; /* Teal 50 */
  color: #0d9488; /* Teal 600 */
  font-weight: 500;
}

:deep(.el-menu-item.is-active .menu-icon) {
  color: #0d9488;
}

:deep(.el-menu-item:hover), :deep(.el-sub-menu__title:hover) {
  background-color: #f8fafc;
}
</style>
