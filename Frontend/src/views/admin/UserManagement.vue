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

    <!-- USERS GLOAL DIRECTORY -->
    <div class="glass-card table-wrapper">
      <el-table :data="users" style="width: 100%" v-loading="loading" class="custom-glass-table">
        <el-table-column label="Thông tin nhân sự" min-width="250">
          <template #default="scope">
            <div class="user-info-section">
              <el-avatar :size="40" :src="scope.row.avatar || `https://ui-avatars.com/api/?name=${scope.row.name}&background=random`" />
              <div class="user-details">
                <div class="user-name">{{ scope.row.name }}</div>
                <div class="user-email">
                  <i class="fa-solid fa-envelope"></i> {{ scope.row.email }}
                </div>
              </div>
            </div>
          </template>
        </el-table-column>

        <el-table-column prop="createdAt" label="Ngày tham gia" width="180">
          <template #default="scope">
            {{ new Date(scope.row.createdAt).toLocaleDateString('vi-VN') }}
          </template>
        </el-table-column>

        <el-table-column label="Vai trò Hệ thống" width="150">
          <template #default="scope">
            <!-- This shows global roles like Admin, SuperAdmin -->
            <el-tag v-if="scope.row.roles && scope.row.roles.length" type="warning" effect="dark" size="small">
              {{ scope.row.roles.join(', ') }}
            </el-tag>
            <el-tag v-else type="info" size="small">Member</el-tag>
          </template>
        </el-table-column>

        <el-table-column label="Trạng thái" width="150" align="center">
          <template #default="scope">
            <el-tag :type="scope.row.isActive ? 'success' : 'danger'" effect="plain">
              {{ scope.row.isActive ? 'Active' : 'Suspended' }}
            </el-tag>
          </template>
        </el-table-column>

        <el-table-column label="Hành động" width="120" fixed="right" align="center">
          <template #default="scope">
            <div class="action-buttons">
              <el-tooltip :content="scope.row.isActive ? 'Đình chỉ nhân sự (Kill-Switch)' : 'Đã đình chỉ'" placement="top">
                <el-switch
                  :model-value="scope.row.isActive"
                  active-color="#dc2626"
                  inactive-color="#374151"
                  :disabled="!scope.row.isActive"
                  @change="handleSuspendUser(scope.row)"
                />
              </el-tooltip>
            </div>
          </template>
        </el-table-column>
      </el-table>
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
  color: var(--text-muted);
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
.page-subtitle {
  font-size: 14px;
  color: var(--text-muted);
  margin-top: 4px;
  margin-bottom: 0;
}
.header-actions {
  display: flex;
  align-items: center;
}
.glass-card {
  background: var(--bg-card);
  backdrop-filter: blur(var(--glass-blur));
  -webkit-backdrop-filter: blur(var(--glass-blur));
  border: 1px solid var(--border-color);
  border-radius: var(--border-radius-xl, 16px);
  padding: 24px;
  box-shadow: 0 8px 32px rgba(0,0,0,0.2);
}
.table-wrapper {
  padding: 12px;
}
.add-user-btn {
  background-color: var(--el-color-primary) !important;
  border-color: var(--el-color-primary) !important;
  border-radius: 8px;
  font-weight: 500;
  padding: 10px 20px;
}
.user-info-section {
  display: flex;
  align-items: center;
  gap: 12px;
}
.user-name {
  font-weight: 600;
  color: var(--text-primary);
  font-size: 15px;
}
.user-email {
  color: var(--text-secondary);
  font-size: 13px;
  display: flex;
  align-items: center;
  gap: 4px;
}
.action-buttons {
  display: flex;
  justify-content: center;
  align-items: center;
}
</style>
