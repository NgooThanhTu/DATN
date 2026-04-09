<template>
  <AdminLayout>
    <div class="admin-page-header">
      <div v-if="viewMode === 'projects'" class="header-title-section">
        <div class="breadcrumb">
          <i class="fa-solid fa-users"></i> System / User Management
        </div>
        <h1 class="page-title">Quản lý Người dùng (User Management)</h1>
      </div>
      <div v-else class="header-title-section" style="display: flex; align-items: center; gap: 12px">
        <el-button @click="backToProjects" plain circle><i class="fa-solid fa-arrow-left"></i></el-button>
        <h1 class="page-title" style="margin-bottom: 0">{{ selectedProject?.name }} - Members</h1>
      </div>
      
      <div class="header-actions" v-if="viewMode === 'users'">
        <el-input v-model="searchQuery" style="width: 220px; margin-right: 12px" placeholder="Search user..." @input="debounceSearch" clearable />
        <el-button type="primary" class="add-user-btn">
          <i class="fa-solid fa-plus mr-2"></i> Add User
        </el-button>
      </div>
    </div>

    <!-- VIEW 1: PROJECTS GRID -->
    <div class="projects-grid" v-if="viewMode === 'projects'" v-loading="loading">
      <div class="project-card" v-for="p in projects" :key="p.id" @click="openProject(p)">
        <div class="project-icon">
          <i class="fa-solid fa-layer-group"></i>
        </div>
        <div class="project-content">
          <div class="project-name">{{ p.name }}</div>
          <div class="project-desc">{{ p.description || 'No description' }}</div>
        </div>
        <i class="fa-solid fa-chevron-right arrow-icon"></i>
      </div>
      <div v-if="!loading && projects.length === 0" style="text-align: center; color: #94a3b8; width: 100%; padding: 40px;">
        Chưa có dự án nào.
      </div>
    </div>

    <!-- VIEW 2: USERS LIST -->
    <div class="users-list-card" v-else v-loading="loading">
      <div class="user-row" v-for="user in computedUsers" :key="user.id + '-' + user.projectId">
        <div class="user-info-section">
          <el-avatar :size="48" :src="user.avatar" />
          <div class="user-details">
            <div class="user-name">{{ user.name }}</div>
            <div class="user-email">
              <i class="fa-solid fa-envelope" style="margin-right: 4px;"></i> {{ user.email }}
            </div>
            <div style="font-size: 11px; color: #94a3b8; font-weight: 600; margin-top: 4px;">PROJECT: {{ user.projectName }}</div>
          </div>
        </div>

        <div class="user-phone">
          <i class="fa-solid fa-phone" style="margin-right: 4px;"></i> {{ user.phone || 'N/A' }}
        </div>

        <div class="user-role-section">
          <el-dropdown trigger="click" @command="(val) => updateRole(user, val)">
             <span class="role-pill" :class="'role-' + user.role.toLowerCase()" style="cursor: pointer;">
                 {{ user.role }} <i class="fa-solid fa-chevron-down" style="font-size:10px; margin-left: 4px;"></i>
             </span>
             <template #dropdown>
               <el-dropdown-menu>
                 <el-dropdown-item command="Admin">Admin</el-dropdown-item>
                 <el-dropdown-item command="PM">PM (Quản lý)</el-dropdown-item>
                 <el-dropdown-item command="PO">PO (Product Owner)</el-dropdown-item>
                 <el-dropdown-item command="SM">SM (Scrum Master)</el-dropdown-item>
                 <el-dropdown-item command="DEV">DEV (Lập trình viên)</el-dropdown-item>
                 <el-dropdown-item command="QA">QA (Kiểm thử viên)</el-dropdown-item>
               </el-dropdown-menu>
             </template>
          </el-dropdown>
        </div>

        <div class="user-status-section">
          <div class="status-cell">
            <span class="status-dot active"></span>
            <span class="active-text">Active</span>
          </div>
        </div>

        <div class="user-actions">
          <el-button class="action-btn" circle plain @click="removeUser(user)">
            <i class="fa-regular fa-trash-can"></i>
          </el-button>
        </div>
      </div>
      
      <div v-if="!loading && users.length === 0" style="text-align: center; color: #94a3b8; padding: 30px;">
        Không tìm thấy người dùng phù hợp.
      </div>
    </div>

  </AdminLayout>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import AdminLayout from '@/components/layout/AdminLayout.vue'
import axiosClient from '@/api/axiosClient'
import { ElMessage, ElMessageBox } from 'element-plus'

const viewMode = ref('projects')
const users = ref([])
const projects = ref([])
const loading = ref(false)
const selectedProjectId = ref(null)
const selectedProject = ref(null)
const searchQuery = ref('')
let searchTimeout = null

const computedUsers = computed(() => {
  return [...users.value].sort((a, b) => {
    const roleWeight = (role) => {
       if (!role) return 99;
       const r = role.toLowerCase();
       if (r === 'admin' || r === 'pm' || r === 'project_manager') return 1;
       if (r === 'po' || r === 'sm') return 2;
       return 3;
    }
    return roleWeight(a.role) - roleWeight(b.role);
  })
})

const openProject = (project) => {
  selectedProject.value = project;
  selectedProjectId.value = project.id;
  viewMode.value = 'users';
  fetchUsers();
}

const backToProjects = () => {
  selectedProject.value = null;
  selectedProjectId.value = null;
  viewMode.value = 'projects';
  users.value = [];
  searchQuery.value = '';
}

const debounceSearch = () => {
    clearTimeout(searchTimeout)
    searchTimeout = setTimeout(() => {
        fetchUsers()
    }, 500)
}

const fetchProjects = async () => {
    loading.value = true;
    try {
        const res = await axiosClient.get('/projects')
        projects.value = res.data.data || []
    } catch(e) { console.error(e) }
    loading.value = false;
}

const fetchUsers = async () => {
    loading.value = true
    try {
        const params = {}
        if (selectedProjectId.value) params.projectId = selectedProjectId.value
        if (searchQuery.value) params.search = searchQuery.value

        const res = await axiosClient.get('/admin/users', { params })
        users.value = res.data.data || []
    } catch(e) {
        console.error(e)
    } finally {
        loading.value = false
    }
}

const updateRole = async (user, newRole) => {
    try {
        await axiosClient.put(`/projects/${user.projectId}/members/${user.id}/role`, { role: newRole })
        ElMessage.success('Cập nhật quyền thành công!')
        user.role = newRole
    } catch(e) {
        ElMessage.error(e.response?.data?.message || 'Lỗi khi cập nhật quyền')
    }
}

const removeUser = async (user) => {
    ElMessageBox.confirm(`Xoá ${user.name} khỏi dự án ${user.projectName}?`, 'Xác nhận', {
        type: 'warning'
    }).then(async () => {
        try {
            await axiosClient.delete(`/projects/${user.projectId}/members/${user.id}`)
            ElMessage.success('Đã xoá người dùng khỏi dự án')
            fetchUsers()
        } catch(e) {
            ElMessage.error(e.response?.data?.message || 'Lỗi xoá người dùng')
        }
    }).catch(() => {})
}

onMounted(() => {
    fetchProjects()
})
</script>

<style scoped>
.admin-page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
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
  color: #1e293b;
  margin: 0;
}

.header-actions {
  display: flex;
  align-items: center;
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

.role-admin, .role-pm, .role-po { background-color: #f3e8ff; color: #9333ea; }
.role-editor, .role-dev, .role-qa, .role-sm { background-color: #e0f2fe; color: #0284c7; }
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

.projects-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 20px;
}

.project-card {
  background: #ffffff;
  border-radius: 12px;
  padding: 24px;
  display: flex;
  align-items: center;
  gap: 16px;
  cursor: pointer;
  box-shadow: 8px 8px 16px rgba(0,0,0,0.05), -8px -8px 16px rgba(255,255,255,0.8);
  border: 1px solid #f8fafc;
  transition: all 0.2s ease;
}

.project-card:hover {
  transform: translateY(-2px);
  box-shadow: 10px 10px 20px rgba(0,0,0,0.08), -10px -10px 20px rgba(255,255,255,0.9);
  border-color: #cbd5e1;
}

.project-icon {
  width: 52px;
  height: 52px;
  border-radius: 14px;
  background-color: #f0fdfa;
  color: #0d9488;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 22px;
}

.project-content {
  flex: 1;
}

.project-name {
  font-weight: 600;
  color: #1e293b;
  font-size: 16px;
  margin-bottom: 4px;
}

.project-desc {
  color: #64748b;
  font-size: 13px;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.arrow-icon {
  color: #cbd5e1;
  font-size: 14px;
}
.project-card:hover .arrow-icon {
  color: #0d9488;
}
</style>
