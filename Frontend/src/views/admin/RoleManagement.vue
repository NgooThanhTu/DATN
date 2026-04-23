<template>
  <AdminLayout>
    <div class="roles-page">
      <header class="page-header">
        <div>
          <div class="breadcrumb">Admin / Role Management</div>
          <h1>Role Management</h1>
          <p>Create custom system roles, keep protected roles intact, and assign roles to users from a focused workspace.</p>
        </div>
        <button class="primary-btn" type="button" @click="saveRoleDraft">
          {{ editingRoleId ? 'Update role' : 'Create role' }}
        </button>
      </header>

      <section class="roles-grid">
        <article class="panel">
          <div class="panel-head">
            <div>
              <h2>Role builder</h2>
              <p>Protected roles like Admin and PM can be assigned, but their structure stays locked.</p>
            </div>
          </div>

          <div class="form-grid">
            <label>
              <span>Role name</span>
              <input v-model="roleDraft.name" type="text" placeholder="Release manager" />
            </label>
            <label>
              <span>Description</span>
              <input v-model="roleDraft.description" type="text" placeholder="What this role is responsible for" />
            </label>
            <label class="wide">
              <span>Permissions</span>
              <el-select
                v-model="roleDraft.permissionIds"
                multiple
                collapse-tags
                collapse-tags-tooltip
                placeholder="Select permissions"
                popper-class="admin-project-dropdown"
              >
                <el-option
                  v-for="permission in permissions"
                  :key="permission.id"
                  :label="`${permission.module} / ${permission.code}`"
                  :value="permission.id"
                />
              </el-select>
            </label>
          </div>

          <div class="action-row">
            <button class="secondary-btn" type="button" @click="resetRoleDraft">Reset</button>
          </div>
        </article>

        <article class="panel">
          <div class="panel-head">
            <div>
              <h2>Assign roles</h2>
              <p>Choose a user, then replace their system roles in one save.</p>
            </div>
          </div>

          <div class="form-grid">
            <label>
              <span>User</span>
              <el-select
                v-model="roleAssignment.userId"
                clearable
                filterable
                placeholder="Select user"
                popper-class="admin-project-dropdown"
              >
                <el-option
                  v-for="user in users"
                  :key="user.id"
                  :label="`${displayName(user)} (${user.email})`"
                  :value="user.id"
                />
              </el-select>
            </label>
            <label class="wide">
              <span>Roles</span>
              <el-select
                v-model="roleAssignment.roleIds"
                multiple
                collapse-tags
                collapse-tags-tooltip
                placeholder="Assign roles"
                popper-class="admin-project-dropdown"
              >
                <el-option v-for="role in roles" :key="role.id" :label="role.name" :value="role.id" />
              </el-select>
            </label>
          </div>

          <div class="action-row">
            <button class="primary-btn" type="button" @click="assignRolesToSelectedUser">Save assignments</button>
          </div>
        </article>
      </section>

      <section class="panel">
        <div class="panel-head">
          <div>
            <h2>Role catalog</h2>
            <p>Custom roles can be edited and deleted. Protected roles stay assignable but cannot be structurally changed here.</p>
          </div>
        </div>

        <div v-if="!roles.length" class="empty-state">No system roles loaded yet.</div>
        <div v-else class="role-list">
          <article v-for="role in roles" :key="role.id" class="role-card">
            <div class="role-copy">
              <strong>{{ role.name }}</strong>
              <p>{{ role.description || 'No description yet.' }}</p>
              <small>{{ role.memberCount }} members · {{ (role.permissions || []).length }} permissions</small>
            </div>
            <div class="role-tags">
              <span v-for="permission in role.permissions || []" :key="`${role.id}-${permission.id}`" class="permission-tag">
                {{ permission.module }} / {{ permission.code }}
              </span>
            </div>
            <div class="action-row">
              <button class="secondary-btn" type="button" @click="editRole(role)">Edit</button>
              <button class="danger-outline-btn" type="button" :disabled="role.isProtected" @click="removeRole(role)">
                {{ role.isProtected ? 'Protected' : 'Delete' }}
              </button>
            </div>
          </article>
        </div>
      </section>
    </div>
  </AdminLayout>
</template>

<script setup>
import { onMounted, ref, watch } from 'vue'
import { storeToRefs } from 'pinia'
import { ElMessage, ElMessageBox } from 'element-plus'
import AdminLayout from '@/components/layout/AdminLayout.vue'
import { useAdminUserStore } from '@/store/useAdminUserStore'

const adminUserStore = useAdminUserStore()
const { users, roles, permissions } = storeToRefs(adminUserStore)

const editingRoleId = ref('')
const roleDraft = ref({
  name: '',
  description: '',
  permissionIds: []
})
const roleAssignment = ref({
  userId: '',
  roleIds: []
})

const displayName = (user) => user?.name || user?.fullName || user?.email || 'User'

const resetRoleDraft = () => {
  editingRoleId.value = ''
  roleDraft.value = {
    name: '',
    description: '',
    permissionIds: []
  }
}

const editRole = (role) => {
  editingRoleId.value = role.id
  roleDraft.value = {
    name: role.name || '',
    description: role.description || '',
    permissionIds: Array.isArray(role.permissionIds) ? [...role.permissionIds] : []
  }
}

const saveRoleDraft = async () => {
  if (!roleDraft.value.name.trim()) {
    ElMessage.warning('Role name is required')
    return
  }

  try {
    const payload = {
      name: roleDraft.value.name.trim(),
      description: roleDraft.value.description?.trim() || null,
      permissionIds: roleDraft.value.permissionIds
    }

    if (editingRoleId.value) {
      await adminUserStore.updateRole(editingRoleId.value, payload)
      ElMessage.success('Role updated')
    } else {
      await adminUserStore.createRole(payload)
      ElMessage.success('Role created')
    }

    resetRoleDraft()
  } catch (error) {
    ElMessage.error(error.response?.data?.message || 'Could not save role')
  }
}

const removeRole = async (role) => {
  try {
    await ElMessageBox.confirm(`Delete role "${role.name}"?`, 'Delete role', { type: 'warning' })
    await adminUserStore.deleteRole(role.id)
    if (editingRoleId.value === role.id) {
      resetRoleDraft()
    }
    ElMessage.success('Role deleted')
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error(error.response?.data?.message || 'Could not delete role')
    }
  }
}

const assignRolesToSelectedUser = async () => {
  if (!roleAssignment.value.userId) {
    ElMessage.warning('Select a user first')
    return
  }

  try {
    await adminUserStore.assignUserRoles(roleAssignment.value.userId, roleAssignment.value.roleIds)
    ElMessage.success('User roles updated')
  } catch (error) {
    ElMessage.error(error.response?.data?.message || 'Could not update user roles')
  }
}

watch(() => roleAssignment.value.userId, (userId) => {
  const selectedUser = users.value.find(user => user.id === userId)
  if (!selectedUser) {
    roleAssignment.value.roleIds = []
    return
  }

  const normalized = (selectedUser.roles || [])
    .map(role => String(role).trim().toLowerCase())

  roleAssignment.value.roleIds = roles.value
    .filter(role => normalized.includes(String(role.name).trim().toLowerCase()))
    .map(role => role.id)
})

onMounted(async () => {
  await Promise.all([
    adminUserStore.fetchUsers(),
    adminUserStore.fetchRoles()
  ])
})
</script>

<style scoped>
.roles-page {
  min-height: calc(100vh - 56px);
  padding: 28px;
  color: #e4e4e7;
}

.page-header,
.roles-grid,
.panel-head,
.action-row {
  display: flex;
}

.page-header,
.panel-head,
.action-row {
  align-items: center;
  justify-content: space-between;
}

.page-header {
  gap: 24px;
  margin-bottom: 24px;
}

.breadcrumb {
  color: #60a5fa;
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  margin-bottom: 8px;
}

.page-header h1,
.panel-head h2 {
  margin: 0;
}

.page-header p,
.panel-head p,
.role-copy p,
.role-copy small {
  margin: 6px 0 0;
  color: #a1a1aa;
}

.roles-grid {
  gap: 16px;
  margin-bottom: 16px;
}

.panel {
  flex: 1;
  border: 1px solid #27272a;
  border-radius: 14px;
  background: #16181d;
  padding: 20px;
}

.form-grid,
.role-list,
.role-tags {
  display: grid;
  gap: 14px;
}

.form-grid {
  grid-template-columns: repeat(2, minmax(0, 1fr));
  margin-top: 18px;
}

label {
  display: grid;
  gap: 8px;
}

label span {
  color: #a1a1aa;
  font-size: 13px;
  font-weight: 600;
}

label.wide {
  grid-column: 1 / -1;
}

input {
  width: 100%;
  border: 1px solid #27272a;
  background: #0f1115;
  color: #fff;
  border-radius: 10px;
  padding: 10px 12px;
  font: inherit;
}

.action-row {
  gap: 12px;
  margin-top: 18px;
}

.primary-btn,
.secondary-btn,
.danger-outline-btn {
  min-height: 42px;
  padding: 0 14px;
  border-radius: 10px;
  cursor: pointer;
  font-weight: 600;
  font: inherit;
}

.primary-btn {
  background: #0ea5e9;
  color: #fff;
  border: 1px solid #0284c7;
}

.secondary-btn,
.danger-outline-btn {
  background: transparent;
  color: #e4e4e7;
  border: 1px solid #27272a;
}

.danger-outline-btn {
  color: #fca5a5;
  border-color: rgba(239, 68, 68, 0.4);
}

.role-list {
  margin-top: 18px;
}

.role-card {
  border: 1px solid #1f232a;
  border-radius: 12px;
  padding: 16px;
  background: #111317;
}

.role-copy strong {
  display: block;
}

.role-tags {
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
  margin-top: 12px;
}

.permission-tag {
  display: inline-flex;
  align-items: center;
  min-height: 32px;
  padding: 0 10px;
  border-radius: 999px;
  background: rgba(59, 130, 246, 0.12);
  color: #93c5fd;
  font-size: 12px;
}

.empty-state {
  padding: 18px;
  border-radius: 10px;
  border: 1px dashed #334155;
  color: #94a3b8;
  margin-top: 18px;
}

::v-deep(.el-select__wrapper) {
  min-height: 44px;
  border-radius: 10px;
  background: #0f1115;
  box-shadow: 0 0 0 1px #27272a inset;
}

@media (max-width: 980px) {
  .roles-grid,
  .page-header {
    flex-direction: column;
    align-items: stretch;
  }

  .form-grid {
    grid-template-columns: 1fr;
  }
}
</style>
