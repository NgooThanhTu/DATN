<template>
  <aside class="admin-sidebar shadow-sm">
    <div class="sidebar-header">
      <img :src="logoImg" alt="SprintA Logo" class="nav-logo" />
      <h2>SprintA<span style="font-size: 14px; font-weight: 500; color: #64748b; margin-left: 6px">Admin</span></h2>
    </div>

    <div class="back-link">
      <a href="#" @click.prevent="goBack" class="flex items-center gap-2">
        <i class="fa-solid fa-arrow-left"></i>
        <span>Back to App</span>
      </a>
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

      <el-sub-menu index="/admin/security">
        <template #title>
          <i class="fa-solid fa-shield-halved menu-icon"></i>
          <span>Security</span>
        </template>
        <el-menu-item index="/admin/security/2fa">Xác thực 2 bước</el-menu-item>
        <el-menu-item index="/admin/security/password">Đổi mật khẩu</el-menu-item>
        <el-menu-item index="/admin/security/ip-whitelist">IP cho phép</el-menu-item>
      </el-sub-menu>
    </el-menu>
  </aside>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import logoImg from '@/assets/logo_QLCV.png'

const route = useRoute()
const router = useRouter()

const goBack = () => {
  if (window.history.state && window.history.state.back) {
    router.back()
  } else {
    router.push('/dashboard')
  }
}

const activeMenu = computed(() => {
  return route.path
})
</script>

<style scoped>
.admin-sidebar {
  width: 250px;
  background-color: var(--bg-card);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  display: flex;
  flex-direction: column;
  border-right: 1px solid var(--border-color);
  z-index: 10;
}

.sidebar-header {
  padding: 24px;
  display: flex;
  align-items: center;
  gap: 10px;
}

.nav-logo {
  height: 28px;
  width: auto;
}

.sidebar-header h2 {
  font-size: 20px;
  font-weight: 700;
  color: var(--text-primary);
  margin: 0;
  display: flex;
  align-items: baseline;
}

.back-link {
  padding: 0 24px 24px 24px;
}

.back-link a {
  color: var(--text-secondary);
  text-decoration: none;
  font-size: 14px;
  font-weight: 500;
}

.back-link a:hover {
  color: var(--text-primary);
}

.admin-menu {
  border-right: none;
  background-color: transparent;
}

.menu-icon {
  width: 24px;
  text-align: center;
  margin-right: 8px;
  font-size: 16px;
  color: var(--text-secondary);
}

:deep(.el-menu) {
  background-color: transparent !important;
}

:deep(.el-menu-item) {
  height: 44px;
  line-height: 44px;
  margin: 4px 12px;
  border-radius: 6px;
  color: var(--text-primary) !important;
  background-color: transparent !important;
}

:deep(.el-sub-menu__title) {
  height: 44px;
  line-height: 44px;
  margin: 4px 12px;
  border-radius: 6px;
  color: var(--text-primary) !important;
  background-color: transparent !important;
}

:deep(.el-menu-item.is-active) {
  background-color: color-mix(in srgb, var(--bg-layout) 15%, var(--text-primary) 85%) !important;
  color: var(--bg-layout) !important;
  font-weight: 700;
}

:deep(.el-menu-item.is-active .menu-icon) {
  color: var(--bg-layout) !important;
}

:deep(.el-menu-item:hover), :deep(.el-sub-menu__title:hover) {
  background-color: var(--bg-hover) !important;
}
</style>
