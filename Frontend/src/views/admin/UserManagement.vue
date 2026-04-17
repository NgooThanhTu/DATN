<template>
  <AdminLayout>
    <div class="admin-page-header">
      <div class="header-title-section">
        <div class="breadcrumb">
          <i class="fa-solid fa-users"></i> System / User Directory
        </div>
        <h1 class="page-title">Danh bạ Người dùng (Directory)</h1>
        <p class="page-subtitle">Quản lý vòng đời nhân sự toàn công ty và thiết lập đặc quyền hệ thống.</p>
      </div>
      
      <div class="header-actions">
        <el-input 
          v-model="searchQuery" 
          style="width: 250px; margin-right: 12px" 
          placeholder="Tìm user theo tên, email..." 
          @input="debounceSearch" 
          clearable 
          class="glass-input"
        >
          <template #prefix>
            <i class="fa-solid fa-search"></i>
          </template>
        </el-input>
        <el-button type="primary" class="add-user-btn" @click="openAddModal">
          <i class="fa-solid fa-plus mr-2"></i> Thêm Nhân sự
        </el-button>
      </div>
    </div>

    <!-- ADD USER DIALOG -->
    <el-dialog v-model="showAddModal" title="Thêm Nhân sự mới" width="500px" append-to-body destroy-on-close class="custom-glass-dialog">
      <el-form :model="newUserForm" :rules="rules" ref="addUserFormRef" label-position="top">
        <el-form-item label="Email Nhân sự" prop="email">
          <el-input v-model="newUserForm.email" placeholder="VD: nguyen.a@company.com"></el-input>
        </el-form-item>

        <el-form-item label="Vai trò Hệ thống" prop="role">
          <el-select v-model="newUserForm.role" class="w-full">
            <el-option label="Developer (DEV)" value="Developer"></el-option>
            <el-option label="Admin" value="Admin"></el-option>
          </el-select>
        </el-form-item>

        <el-form-item label="Gán vào Dự án (Tùy chọn)">
          <el-select v-model="newUserForm.projectId" class="w-full" clearable placeholder="Chọn dự án hiện có">
            <el-option v-for="p in projectsList" :key="p.id" :label="p.name" :value="p.id"></el-option>
          </el-select>
        </el-form-item>
      </el-form>

      <template #footer>
        <div class="dialog-footer">
          <el-button @click="showAddModal = false">Hủy</el-button>
          <el-button type="primary" @click="submitAddUser" :loading="isSubmitting">Xác nhận Thêm</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- USERS TABLE - Custom Dark Design -->
    <div class="users-table-card">
      <div v-if="loading" class="table-loading">
        <i class="fa-solid fa-spinner fa-spin"></i> Đang tải dữ liệu...
      </div>
      <div v-else-if="!users || users.length === 0" class="table-empty">
        <i class="fa-solid fa-user-slash"></i>
        <p>Chưa có người dùng nào.</p>
      </div>
      <table v-else class="dark-table">
        <thead>
          <tr>
            <th>Thông tin nhân sự</th>
            <th>Ngày tham gia</th>
            <th>Vai trò Hệ thống</th>
            <th>Trạng thái</th>
            <th style="text-align: center;">Hành động</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users" :key="user.id">
            <td>
              <div class="user-info-cell">
                <div class="user-avatar" :style="{ background: getAvatarColor(user.name) }">
                  {{ getInitials(user.name) }}
                </div>
                <div class="user-meta">
                  <span class="user-name">{{ user.name }}</span>
                  <span class="user-email"><i class="fa-solid fa-envelope"></i> {{ user.email }}</span>
                </div>
              </div>
            </td>
            <td>
              <span class="date-text">{{ new Date(user.createdAt).toLocaleDateString('vi-VN') }}</span>
            </td>
            <td>
              <span
                v-if="user.roles && user.roles.length"
                class="role-badge"
                :class="getRoleBadgeClass(user.roles[0])"
              >
                {{ user.roles.join(', ') }}
              </span>
              <span v-else class="role-badge role-member">Member</span>
            </td>
            <td>
              <span class="status-badge" :class="user.isActive ? 'status-active' : 'status-suspended'">
                <i class="fa-solid" :class="user.isActive ? 'fa-circle-check' : 'fa-circle-xmark'"></i>
                {{ user.isActive ? 'Active' : 'Suspended' }}
              </span>
            </td>
            <td style="text-align: center;">
              <el-tooltip :content="user.isActive ? 'Đình chỉ nhân sự (Kill-Switch)' : 'Đã đình chỉ'" placement="top">
                <el-switch
                  :model-value="user.isActive"
                  active-color="#10b981"
                  inactive-color="#27272a"
                  :disabled="!user.isActive"
                  @change="handleSuspendUser(user)"
                />
              </el-tooltip>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

  </AdminLayout>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import AdminLayout from '@/components/layout/AdminLayout.vue'
import { useAdminUserStore } from '@/store/useAdminUserStore'
import { ElMessage, ElMessageBox } from 'element-plus'
import axiosClient from '@/api/axiosClient'

const adminUserStore = useAdminUserStore()
const { users, loading } = storeToRefs(adminUserStore)

// Modal State
const showAddModal = ref(false)
const isSubmitting = ref(false)
const addUserFormRef = ref(null)
const projectsList = ref([])

const newUserForm = ref({
  email: '',
  role: 'Developer',
  projectId: null
})

const rules = {
  email: [
    { required: true, message: 'Vui lòng nhập email', trigger: 'blur' },
    { type: 'email', message: 'Email không hợp lệ', trigger: 'blur' }
  ],
  role: [{ required: true, message: 'Vui lòng chọn vai trò', trigger: 'change' }]
}

const avatarColors = ['#0ea5e9', '#f59e0b', '#ef4444', '#10b981', '#8b5cf6', '#ec4899', '#06b6d4', '#f97316']

const getAvatarColor = (name) => {
  if (!name) return avatarColors[0]
  let hash = 0
  for (let i = 0; i < name.length; i++) {
    hash = name.charCodeAt(i) + ((hash << 5) - hash)
  }
  return avatarColors[Math.abs(hash) % avatarColors.length]
}

const getInitials = (name) => {
  if (!name) return '?'
  return name.split(' ').map(w => w[0]).join('').substring(0, 2).toUpperCase()
}

const getRoleBadgeClass = (role) => {
  const r = role?.toLowerCase()
  if (r === 'admin' || r === 'system admin') return 'role-admin'
  if (r === 'developer' || r === 'dev') return 'role-dev'
  if (r === 'pm' || r === 'po') return 'role-pm'
  return 'role-member'
}

const openAddModal = async () => {
    showAddModal.value = true
    try {
        const res = await axiosClient.get('/projects')
        if (res.data && res.data.data) {
            projectsList.value = res.data.data
        }
    } catch (e) {
        console.error("Failed to load projects", e)
    }
}

const submitAddUser = async () => {
  if (!addUserFormRef.value) return
  await addUserFormRef.value.validate(async (valid) => {
    if (valid) {
      isSubmitting.value = true
      try {
        // Send POST request via Store
        await adminUserStore.createUser(newUserForm.value)
        ElMessage.success({ message: `Đã gửi lời mời bảo mật đến ${newUserForm.value.email}!`, duration: 4000 })
        
        // Reset and close
        showAddModal.value = false
        newUserForm.value = { email: '', role: 'Developer', projectId: null }
      } catch (err) {
        ElMessage.error(err.response?.data?.message || 'Có lỗi xảy ra khi thêm user.')
      } finally {
        isSubmitting.value = false
      }
    }
  })
}

const searchQuery = ref('')
let searchTimeout = null

const debounceSearch = () => {
    clearTimeout(searchTimeout)
    searchTimeout = setTimeout(() => {
        adminUserStore.fetchUsers(searchQuery.value)
    }, 500)
}

const handleSuspendUser = (user) => {
    if (!user.isActive) return;
    
    ElMessageBox.confirm(
        `Đình chỉ (Suspend) tài khoản của ${user.name}?\nHành động này sẽ ngắt toàn bộ phiên đăng nhập của người này khỏi hệ thống ngay lập tức!`,
        '⚠️ KÍCH HOẠT KILL-SWITCH',
        {
            confirmButtonText: 'Đình chỉ',
            cancelButtonText: 'Hủy',
            type: 'error',
        }
    ).then(async () => {
        try {
            await adminUserStore.suspendUser(user.id);
            ElMessage.success({
              message: `Đã đình chỉ & vô hiệu hóa kết nối của ${user.name}!`,
              duration: 4000
            });
        } catch(e) {
            ElMessage.error('Lỗi phân quyền hệ thống hoặc API!');
        }
    }).catch(() => {})
}

onMounted(() => {
    adminUserStore.fetchUsers()
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
  color: #71717a;
  margin-bottom: 8px;
  display: flex;
  align-items: center;
  gap: 8px;
}
.page-title {
  font-size: 24px;
  font-weight: 600;
  color: #e4e4e7;
  margin: 0;
}
.page-subtitle {
  font-size: 14px;
  color: #71717a;
  margin-top: 4px;
  margin-bottom: 0;
}
.header-actions {
  display: flex;
  align-items: center;
}
.add-user-btn {
  background-color: #0ea5e9 !important;
  border-color: #0ea5e9 !important;
  border-radius: 8px;
  font-weight: 500;
  padding: 10px 20px;
}

/* Table Card */
.users-table-card {
  background: #16181d;
  border: 1px solid #1e2025;
  border-radius: 12px;
  overflow: hidden;
}

.table-loading,
.table-empty {
  text-align: center;
  padding: 60px 0;
  color: #71717a;
  font-size: 14px;
}

.table-empty i {
  font-size: 36px;
  margin-bottom: 12px;
  display: block;
  color: #27272a;
}

/* Custom Dark Table */
.dark-table {
  width: 100%;
  border-collapse: collapse;
}

.dark-table thead {
  border-bottom: 1px solid #1e2025;
}

.dark-table th {
  padding: 14px 20px;
  text-align: left;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  color: #71717a;
  background: transparent;
}

.dark-table tbody tr {
  border-bottom: 1px solid rgba(30, 32, 37, 0.6);
  transition: background 0.15s ease;
}

.dark-table tbody tr:last-child {
  border-bottom: none;
}

.dark-table tbody tr:hover {
  background: rgba(255, 255, 255, 0.02);
}

.dark-table td {
  padding: 16px 20px;
  font-size: 14px;
  color: #a1a1aa;
  vertical-align: middle;
}

/* User Info Cell */
.user-info-cell {
  display: flex;
  align-items: center;
  gap: 14px;
}

.user-avatar {
  width: 40px;
  height: 40px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  font-weight: 700;
  color: white;
  flex-shrink: 0;
}

.user-meta {
  display: flex;
  flex-direction: column;
  gap: 3px;
}

.user-name {
  font-weight: 600;
  color: #e4e4e7;
  font-size: 14px;
}

.user-email {
  color: #71717a;
  font-size: 12px;
  display: flex;
  align-items: center;
  gap: 5px;
}

.user-email i {
  font-size: 10px;
}

.date-text {
  color: #a1a1aa;
  font-size: 13px;
}

/* Role Badges */
.role-badge {
  display: inline-flex;
  align-items: center;
  padding: 4px 12px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 600;
  letter-spacing: 0.3px;
}

.role-admin {
  background: rgba(239, 68, 68, 0.12);
  color: #f87171;
  border: 1px solid rgba(239, 68, 68, 0.2);
}

.role-dev {
  background: rgba(99, 102, 241, 0.12);
  color: #818cf8;
  border: 1px solid rgba(99, 102, 241, 0.2);
}

.role-pm {
  background: rgba(245, 158, 11, 0.12);
  color: #fbbf24;
  border: 1px solid rgba(245, 158, 11, 0.2);
}

.role-member {
  background: rgba(113, 113, 122, 0.12);
  color: #a1a1aa;
  border: 1px solid rgba(113, 113, 122, 0.2);
}

/* Status Badges */
.status-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 12px;
  border-radius: 6px;
  font-size: 12px;
  font-weight: 600;
}

.status-active {
  background: rgba(16, 185, 129, 0.1);
  color: #34d399;
  border: 1px solid rgba(16, 185, 129, 0.2);
}

.status-suspended {
  background: rgba(239, 68, 68, 0.1);
  color: #f87171;
  border: 1px solid rgba(239, 68, 68, 0.2);
}

.status-badge i {
  font-size: 11px;
}
</style>
