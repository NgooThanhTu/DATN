<template>
  <AdminLayout>
    <div class="admin-page-header">
      <div class="header-title-section">
        <div class="breadcrumb">
          <i class="fa-solid fa-file-lines"></i> System / Audit Log
        </div>
        <h1 class="page-title">Nhật ký Hệ thống (Audit Log)</h1>
      </div>
      <div class="header-actions">
        <el-input v-model="searchQuery" placeholder="Search logs..." style="width: 220px; margin-right: 12px" @input="debounceSearch" clearable />
        
        <el-select v-model="selectedProjectId" placeholder="All Projects" style="width: 180px; margin-right: 12px" @change="fetchLogs" clearable>
           <el-option v-for="p in projects" :key="p.id" :label="p.name" :value="p.id" />
        </el-select>

        <el-radio-group v-model="timeFilter" class="custom-radio-group" @change="fetchLogs">
          <el-radio-button label="All Time" value="all" />
          <el-radio-button label="24h" value="24h" />
          <el-radio-button label="30d" value="30d" />
        </el-radio-group>
      </div>
    </div>

    <div class="admin-card" v-loading="loading">
      <el-table :data="logs" style="width: 100%" class="admin-table" :show-header="true">
        <el-table-column prop="timestamp" label="TIMESTAMP" min-width="150" />
        <el-table-column prop="user" label="USER" min-width="180" />
        <el-table-column prop="action" label="ACTION" min-width="120" />
        <el-table-column prop="resource" label="RESOURCE" min-width="260">
           <template #default="scope">
             <div style="font-weight: 500; font-size: 13px;">{{ scope.row.resource }}</div>
             <div style="color: #64748b; font-size: 12px;">{{ scope.row.targetId }}</div>
           </template>
        </el-table-column>
        
        <el-table-column prop="status" label="STATUS" min-width="120">
          <template #default="scope">
            <div class="status-cell">
              <span class="status-dot" :class="scope.row.status.toLowerCase()"></span>
              <span :class="scope.row.status.toLowerCase() + '-text'">{{ scope.row.status }}</span>
            </div>
          </template>
        </el-table-column>
      </el-table>

      <div class="pagination-container" v-if="total > 0">
        <el-pagination
          v-model:current-page="currentPage"
          layout="prev, pager, next"
          :total="total"
          :page-size="20"
          @current-change="handlePageChange"
          class="custom-pagination"
        />
      </div>
    </div>
  </AdminLayout>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import AdminLayout from '@/components/layout/AdminLayout.vue'
import axiosClient from '@/api/axiosClient'

const timeFilter = ref('all')
const selectedProjectId = ref(null)
const searchQuery = ref('')
const loading = ref(false)
const currentPage = ref(1)
const total = ref(0)
const logs = ref([])
const projects = ref([])

let searchTimeout = null
const debounceSearch = () => {
    clearTimeout(searchTimeout)
    searchTimeout = setTimeout(() => {
        currentPage.value = 1
        fetchLogs()
    }, 500)
}

const handlePageChange = (page) => {
    currentPage.value = page
    fetchLogs()
}

const fetchProjects = async () => {
    try {
        const res = await axiosClient.get('/projects')
        projects.value = res.data.data || []
    } catch(e) {
        console.error(e)
    }
}

const fetchLogs = async () => {
    loading.value = true
    try {
        const params = {
            page: currentPage.value,
            limit: 20
        }
        if (timeFilter.value !== 'all') params.timeFilter = timeFilter.value
        if (selectedProjectId.value) params.projectId = selectedProjectId.value
        if (searchQuery.value) params.search = searchQuery.value

        const res = await axiosClient.get('/auditlogs', { params })
        logs.value = res.data.data.items
        total.value = res.data.data.total
    } catch(e) {
        console.error(e)
    } finally {
        loading.value = false
    }
}

onMounted(() => {
    fetchProjects()
    fetchLogs()
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

.admin-card {
  background: #ffffff;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 8px 8px 16px rgba(0,0,0,0.05), -8px -8px 16px rgba(255,255,255,0.8);
}

:deep(.admin-table th.el-table__cell) {
  background-color: transparent !important;
  color: #0d9488 !important;
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

.pagination-container {
  display: flex;
  justify-content: center;
  margin-top: 24px;
}
</style>
