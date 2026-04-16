<template>
  <NexusLayout>
    <div class="drafts-wrapper">
      <!-- Header -->
      <header class="dr-header">
        <div class="dr-left">
          <i class="fa-solid fa-pen-nib text-muted"></i>
          <span class="dr-title">Drafts</span>
          <span class="dr-badge">{{ drafts.length }}</span>
        </div>
        <div class="dr-right">
          <button class="plane-primary-btn" @click="openModal">Draft a work item</button>
        </div>
      </header>

      <!-- Body / List -->
      <div class="dr-body">
         <div class="list-row" v-for="draft in drafts" :key="draft.id" @dblclick="openEditModal(draft)">
            <div class="lr-left">
               <span class="text-muted fw-500 text-xs" style="min-width: 60px;">{{ draft.sequenceId || 'DRAFT' }}</span>
               <span class="lr-title">{{ draft.title || 'Untitled Draft' }}</span>
            </div>
            <div class="lr-right" @dblclick.stop>
               <div class="dm-toolbar">
                 <!-- Status Dropdown -->
                 <el-dropdown trigger="click" @command="(val) => updateDraftProperty(draft, 'statusName', val)">
                    <button class="dm-tool-btn"><i :class="getStatusIcon(draft.statusName)" :style="{ color: getStatusColor(draft.statusName) }"></i></button>
                    <template #dropdown>
                      <el-dropdown-menu class="plane-dropdown">
                        <el-dropdown-item command="BACKLOG"><i class="fa-regular fa-circle-dot" style="color:#71717A"></i> Backlog</el-dropdown-item>
                        <el-dropdown-item command="TO DO"><i class="fa-regular fa-circle" style="color:#D4D4D8"></i> To Do</el-dropdown-item>
                        <el-dropdown-item command="IN PROGRESS"><i class="fa-solid fa-circle-half-stroke" style="color:#3B82F6"></i> In Progress</el-dropdown-item>
                        <el-dropdown-item command="DONE"><i class="fa-solid fa-circle-check" style="color:#10B981"></i> Done</el-dropdown-item>
                      </el-dropdown-menu>
                    </template>
                 </el-dropdown>
                 
                 <!-- Priority Dropdown -->
                 <el-dropdown trigger="click" @command="(val) => updateDraftProperty(draft, 'priority', val)">
                    <button class="dm-tool-btn">
                       <i :class="getPriorityIcon(draft.priority)" :style="{ color: getPriorityColor(draft.priority) }"></i>
                    </button>
                    <template #dropdown>
                      <el-dropdown-menu class="plane-dropdown">
                        <el-dropdown-item :command="1"><i class="fa-solid fa-angles-up" style="color:#EF4444"></i> Urgent</el-dropdown-item>
                        <el-dropdown-item :command="2"><i class="fa-solid fa-chevron-up" style="color:#F97316"></i> High</el-dropdown-item>
                        <el-dropdown-item :command="3"><i class="fa-solid fa-minus" style="color:#3B82F6"></i> Normal</el-dropdown-item>
                        <el-dropdown-item :command="4"><i class="fa-solid fa-chevron-down" style="color:#94A3B8"></i> Low</el-dropdown-item>
                      </el-dropdown-menu>
                    </template>
                 </el-dropdown>

                 <!-- Label (placeholder) -->
                 <button class="dm-tool-btn" title="Labels"><i class="fa-solid fa-tag text-muted"></i></button>
                 <!-- Start date (placeholder) -->
                 <button class="dm-tool-btn" title="Start date"><i class="fa-regular fa-calendar text-muted"></i></button>
                 <!-- Due date (placeholder) -->
                 <button class="dm-tool-btn" title="Due date"><i class="fa-solid fa-calendar-day text-muted"></i></button>
                 <!-- Members (placeholder) -->
                 <button class="dm-tool-btn" title="Members"><i class="fa-solid fa-user-group text-muted"></i></button>
                 <!-- Modules (placeholder) -->
                 <button class="dm-tool-btn" title="Modules"><i class="fa-solid fa-table-cells-large text-muted"></i></button>
                 <!-- Cycles (placeholder) -->
                 <button class="dm-tool-btn" title="Cycles"><i class="fa-solid fa-eye text-muted"></i></button>

                 <!-- Ellipsis Actions -->
                 <el-dropdown trigger="click" @command="(cmd) => handleEllipsisAction(cmd, draft)">
                    <button class="dm-tool-btn"><i class="fa-solid fa-ellipsis text-muted"></i></button>
                    <template #dropdown>
                      <el-dropdown-menu class="plane-dropdown">
                        <el-dropdown-item command="edit"><i class="fa-solid fa-pen" style="color:#A1A1AA"></i> Edit</el-dropdown-item>
                        <el-dropdown-item command="copy"><i class="fa-regular fa-copy" style="color:#A1A1AA"></i> Make a copy</el-dropdown-item>
                        <el-dropdown-item command="move"><i class="fa-solid fa-arrow-right-from-bracket" style="color:#A1A1AA"></i> Move to project</el-dropdown-item>
                        <el-dropdown-item command="delete" divided><i class="fa-solid fa-trash" style="color:#EF4444"></i> Delete</el-dropdown-item>
                      </el-dropdown-menu>
                    </template>
                 </el-dropdown>
               </div>
            </div>
         </div>
         <div v-if="drafts.length === 0" style="padding: 24px; color: #71717A;">
            No drafts found.
         </div>
      </div>
    </div>

    <!-- Create / Edit Draft Modal -->
    <div class="modal-overlay" v-if="showModal" @click.self="showModal = false">
       <div class="draft-modal">
          <h3 class="modal-title">{{ editMode ? 'Edit draft' : 'Create a draft' }}</h3>
          
          <div class="proj-badge mt-4" v-if="projects.length > 0">
             <i class="fa-solid fa-bell" style="color: #F59E0B"></i>
             <span>{{ projects[0]?.name || 'Project' }}</span>
          </div>
          
          <input type="text" class="dm-title-input mt-4" placeholder="Title" v-model="form.title" />
          <textarea class="dm-desc-input mt-2" placeholder="Click to add description" v-model="form.description"></textarea>
          
          <div class="dm-toolbar mt-2">
             <!-- Status Dropdown -->
             <el-dropdown trigger="click" @command="(val) => form.statusName = val">
                <button class="dm-tool-btn"><i :class="getStatusIcon(form.statusName)" :style="{ color: getStatusColor(form.statusName) }"></i> {{ getStatusLabel(form.statusName) }}</button>
                <template #dropdown>
                  <el-dropdown-menu class="plane-dropdown">
                    <el-dropdown-item command="BACKLOG"><i class="fa-regular fa-circle-dot" style="color:#71717A"></i> Backlog</el-dropdown-item>
                    <el-dropdown-item command="TO DO"><i class="fa-regular fa-circle" style="color:#D4D4D8"></i> To Do</el-dropdown-item>
                    <el-dropdown-item command="IN PROGRESS"><i class="fa-solid fa-circle-half-stroke" style="color:#3B82F6"></i> In Progress</el-dropdown-item>
                    <el-dropdown-item command="DONE"><i class="fa-solid fa-circle-check" style="color:#10B981"></i> Done</el-dropdown-item>
                  </el-dropdown-menu>
                </template>
             </el-dropdown>
             
             <!-- Priority Dropdown -->
             <el-dropdown trigger="click" @command="(val) => form.priority = val">
                <button class="dm-tool-btn">
                   <i :class="getPriorityIcon(form.priority)" :style="{ color: getPriorityColor(form.priority) }"></i>
                   {{ getPriorityLabel(form.priority) }}
                </button>
                <template #dropdown>
                  <el-dropdown-menu class="plane-dropdown">
                    <el-dropdown-item :command="1"><i class="fa-solid fa-angles-up" style="color:#EF4444"></i> Urgent</el-dropdown-item>
                    <el-dropdown-item :command="2"><i class="fa-solid fa-chevron-up" style="color:#F97316"></i> High</el-dropdown-item>
                    <el-dropdown-item :command="3"><i class="fa-solid fa-minus" style="color:#3B82F6"></i> Normal</el-dropdown-item>
                    <el-dropdown-item :command="4"><i class="fa-solid fa-chevron-down" style="color:#94A3B8"></i> Low</el-dropdown-item>
                  </el-dropdown-menu>
                </template>
             </el-dropdown>

             <!-- Assignees Dropdown (placeholder) -->
             <button class="dm-tool-btn"><i class="fa-solid fa-user-group text-muted"></i> Assignees</button>
             <button class="dm-tool-btn"><i class="fa-solid fa-tag text-muted"></i> Labels</button>
             <button class="dm-tool-btn"><i class="fa-regular fa-calendar text-muted"></i> Start date</button>
             <button class="dm-tool-btn"><i class="fa-solid fa-calendar-day text-muted"></i> Due date</button>
             <button class="dm-tool-btn"><i class="fa-solid fa-arrows-spin text-muted"></i> Cycle</button>
             <button class="dm-tool-btn"><i class="fa-solid fa-table-cells-large text-muted"></i> Modules</button>
          </div>
          
          <div class="dm-footer mt-4">
             <div class="dm-footer-left">
                <label class="toggle-wrap" v-if="!editMode">
                   <div class="toggle-bg"><div class="toggle-knob"></div></div>
                   Create more
                </label>
             </div>
             <div class="dm-footer-right">
                <button class="discard-btn" @click="showModal = false">Discard</button>
                <button class="save-btn" @click="saveDraft">{{ editMode ? 'Update Draft' : 'Save as Draft' }}</button>
             </div>
          </div>
       </div>
    </div>

    <!-- Move to Project Modal -->
    <div class="modal-overlay" v-if="showMoveModal" @click.self="showMoveModal = false">
       <div class="draft-modal" style="width: 420px;">
          <h3 class="modal-title">Move to project</h3>
          <p class="text-muted text-xs mt-2" style="margin-bottom: 16px;">Select a project to move this draft into as a work item.</p>
          <div class="project-list">
            <div 
              class="project-item" 
              v-for="p in projects" 
              :key="p.id" 
              @click="moveToProject(p.id)"
            >
              <i class="fa-solid fa-bell" style="color: #F59E0B; font-size: 14px;"></i>
              <span>{{ p.name }}</span>
            </div>
          </div>
          <div class="dm-footer mt-4">
            <div></div>
            <button class="discard-btn" @click="showMoveModal = false">Cancel</button>
          </div>
       </div>
    </div>
  </NexusLayout>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import NexusLayout from '@/components/layout/NexusLayout.vue'
import axiosClient from '@/api/axiosClient'
import { ElMessage } from 'element-plus'

const showModal = ref(false)
const showMoveModal = ref(false)
const editMode = ref(false)
const drafts = ref([])
const projects = ref([])
const movingDraft = ref(null)

const form = ref({
  id: null,
  title: '',
  description: '',
  statusName: 'BACKLOG',
  priority: 3
})

// ============ HELPERS ============

const getStatusIcon = (status) => {
  const s = (status || 'BACKLOG').toUpperCase()
  if (s === 'DONE') return 'fa-solid fa-circle-check'
  if (s === 'IN PROGRESS') return 'fa-solid fa-circle-half-stroke'
  if (s === 'TO DO') return 'fa-regular fa-circle'
  return 'fa-regular fa-circle-dot' // BACKLOG
}

const getStatusColor = (status) => {
  const s = (status || 'BACKLOG').toUpperCase()
  if (s === 'DONE') return '#10B981'
  if (s === 'IN PROGRESS') return '#3B82F6'
  if (s === 'TO DO') return '#D4D4D8'
  return '#71717A' // BACKLOG
}

const getStatusLabel = (status) => {
  const s = (status || 'BACKLOG').toUpperCase()
  if (s === 'DONE') return 'Done'
  if (s === 'IN PROGRESS') return 'In Progress'
  if (s === 'TO DO') return 'To Do'
  return 'Backlog'
}

const getPriorityIcon = (p) => {
  if (p === 1) return 'fa-solid fa-angles-up'
  if (p === 2) return 'fa-solid fa-chevron-up'
  if (p === 4) return 'fa-solid fa-chevron-down'
  return 'fa-solid fa-minus' // Normal (3 or default)
}

const getPriorityColor = (p) => {
  if (p === 1) return '#EF4444'
  if (p === 2) return '#F97316'
  if (p === 4) return '#94A3B8'
  return '#3B82F6' // Normal
}

const getPriorityLabel = (p) => {
  if (p === 1) return 'Urgent'
  if (p === 2) return 'High'
  if (p === 4) return 'Low'
  return 'Normal'
}

// ============ MODALS ============

const openModal = () => {
  form.value = { id: null, title: '', description: '', statusName: 'BACKLOG', priority: 3 }
  editMode.value = false
  showModal.value = true
}

const openEditModal = (draft) => {
  form.value = { 
     id: draft.id, 
     title: draft.title, 
     description: draft.description,
     statusName: draft.statusName || 'BACKLOG',
     priority: draft.priority || 3
  }
  editMode.value = true
  showModal.value = true
}

// ============ DATA ============

const fetchDrafts = async () => {
  try {
    const res = await axiosClient.get('/drafts')
    drafts.value = res.data?.data || []
  } catch(e) {
    console.error(e)
  }
}

const fetchProjects = async () => {
  try {
    const res = await axiosClient.get('/projects')
    projects.value = res.data?.data || []
  } catch(e) {
    console.error(e)
  }
}

const saveDraft = async () => {
  if (!form.value.title) {
    ElMessage.warning('Title is required')
    return
  }
  try {
    const payload = {
      title: form.value.title,
      description: form.value.description,
      statusName: form.value.statusName,
      priority: form.value.priority
    }
    
    if (editMode.value && form.value.id) {
       await axiosClient.put(`/drafts/${form.value.id}`, payload)
       ElMessage.success('Draft updated')
    } else {
       await axiosClient.post('/drafts', payload)
       ElMessage.success('Draft saved')
    }
    
    showModal.value = false
    fetchDrafts()
  } catch(e) {
    console.error(e)
    ElMessage.error('Failed to save draft')
  }
}

const updateDraftProperty = async (draft, field, value) => {
  try {
    const payload = {
      title: draft.title,
      description: draft.description,
      statusName: draft.statusName,
      priority: draft.priority
    }
    payload[field] = value
    await axiosClient.put(`/drafts/${draft.id}`, payload)
    draft[field] = value
    ElMessage.success('Draft updated')
  } catch(e) {
    console.error(e)
    ElMessage.error('Failed to update draft')
  }
}

const deleteDraft = async (id) => {
  try {
    await axiosClient.delete(`/drafts/${id}`)
    ElMessage.success('Draft deleted')
    fetchDrafts()
  } catch(e) {
    console.error(e)
    ElMessage.error('Failed to delete draft')
  }
}

// ============ ELLIPSIS ACTIONS ============

const handleEllipsisAction = (command, draft) => {
  switch (command) {
    case 'edit':
      openEditModal(draft)
      break
    case 'copy':
      makeCopy(draft)
      break
    case 'move':
      handleMoveToProject(draft)
      break
    case 'delete':
      deleteDraft(draft.id)
      break
  }
}

const makeCopy = async (draft) => {
  try {
    await axiosClient.post('/drafts', {
      title: `${draft.title} (copy)`,
      description: draft.description,
      statusName: draft.statusName || 'BACKLOG',
      priority: draft.priority || 3
    })
    ElMessage.success('Draft duplicated')
    fetchDrafts()
  } catch(e) {
    console.error(e)
    ElMessage.error('Failed to duplicate draft')
  }
}

const handleMoveToProject = (draft) => {
  movingDraft.value = draft
  if (projects.value.length === 1) {
    // Only one project → move directly
    moveToProject(projects.value[0].id)
  } else if (projects.value.length > 1) {
    // Multiple projects → show picker
    showMoveModal.value = true
  } else {
    ElMessage.warning('No projects available')
  }
}

const moveToProject = async (projectId) => {
  if (!movingDraft.value) return
  const draft = movingDraft.value
  
  try {
    // Create as a real work item in the selected project
    await axiosClient.post(`/projects/${projectId}/WorkTasks`, {
      title: draft.title || 'Untitled',
      description: draft.description || '',
      statusName: draft.statusName || 'BACKLOG',
      priority: draft.priority || 3
    })
    
    // Delete the draft after successful move
    await axiosClient.delete(`/drafts/${draft.id}`)
    
    ElMessage.success('Draft moved to project successfully')
    showMoveModal.value = false
    movingDraft.value = null
    fetchDrafts()
  } catch(e) {
    console.error(e)
    ElMessage.error('Failed to move draft to project')
  }
}

onMounted(() => {
  fetchDrafts()
  fetchProjects()
})
</script>

<style scoped>
.drafts-wrapper {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background: #0D0F11;
  color: #E4E4E7;
}

.dr-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 24px;
  border-bottom: 1px solid #1E2025;
}
.dr-left {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  font-weight: 500;
}
.text-muted { color: #A1A1AA; }
.dr-title { color: #E4E4E7; }
.dr-badge {
  background: #1E2025;
  color: #0EA5E9;
  font-size: 11px;
  padding: 2px 6px;
  border-radius: 12px;
  font-weight: 600;
}

.plane-primary-btn {
  background: #0EA5E9;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 6px 12px;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition: background 0.2s;
}
.plane-primary-btn:hover { background: #0284C7; }

/* List Styles */
.dr-body { padding: 0 24px; overflow-y: auto; flex: 1; }
.list-row { display: flex; justify-content: space-between; align-items: center; padding: 10px 16px; border: 1px solid transparent; border-bottom: 1px solid #1E2025; transition: background 0.2s, border 0.2s; cursor: pointer; }
.list-row:hover { background: #16181D; border-radius: 4px; border-color: #27272A; }
.lr-left { display: flex; align-items: center; gap: 12px; flex-shrink: 0; }
.lr-title { font-size: 14px; font-weight: 500; color: #E4E4E7; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; max-width: 300px; }
.lr-right { display: flex; align-items: center; gap: 6px; }
.text-xs { font-size: 12px; }
.fw-500 { font-weight: 500; }

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.6);
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: center;
}
.draft-modal {
  background: #111315;
  border: 1px solid #1E2025;
  width: 700px;
  border-radius: 8px;
  padding: 24px;
  box-shadow: 0 20px 40px rgba(0,0,0,0.5);
}

.modal-title { margin: 0; font-size: 16px; font-weight: 600; color: #E4E4E7; }
.mt-4 { margin-top: 24px; }
.mt-2 { margin-top: 12px; }

.proj-badge { display: inline-flex; align-items: center; gap: 8px; font-size: 12px; font-weight: 500; background: #16181D; padding: 4px 8px; border-radius: 4px; border: 1px solid #27272A; }

.dm-title-input {
  width: 100%;
  background: #16181D;
  border: 1px solid #27272A;
  color: #E4E4E7;
  padding: 12px 16px;
  border-radius: 6px;
  font-size: 14px;
  outline: none;
  box-sizing: border-box;
}
.dm-title-input::placeholder { color: #71717A; }
.dm-title-input:focus { border-color: #38BDF8; }

.dm-desc-input {
  width: 100%;
  background: #16181D;
  border: 1px solid #27272A;
  color: #E4E4E7;
  padding: 12px 16px;
  border-radius: 6px;
  font-size: 14px;
  outline: none;
  height: 120px;
  resize: none;
  font-family: inherit;
  box-sizing: border-box;
}
.dm-desc-input::placeholder { color: #71717A; }
.dm-desc-input:focus { border-color: #38BDF8; }

.dm-toolbar {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
  align-items: center;
}
.dm-tool-btn {
  background: transparent;
  border: 1px solid #27272A;
  color: #A1A1AA;
  font-size: 12px;
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 8px;
  border-radius: 4px;
  cursor: pointer;
  white-space: nowrap;
  transition: background 0.15s, color 0.15s;
}
.dm-tool-btn:hover { background: #1E2025; color: #E4E4E7; }

.dm-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-top: 1px solid #1E2025;
  padding-top: 16px;
}
.toggle-wrap { display: flex; align-items: center; gap: 8px; font-size: 12px; color: #A1A1AA; cursor: pointer; }
.toggle-bg { width: 32px; height: 18px; background: #27272A; border-radius: 9px; position: relative; }
.toggle-knob { width: 14px; height: 14px; background: #71717A; border-radius: 50%; position: absolute; top: 2px; left: 2px; }

.dm-footer-right { display: flex; gap: 12px; }
.discard-btn { background: transparent; border: none; color: #A1A1AA; font-size: 13px; font-weight: 500; cursor: pointer; }
.discard-btn:hover { color: #E4E4E7; }
.save-btn { background: #0EA5E9; color: white; border: none; border-radius: 6px; padding: 6px 16px; font-size: 13px; font-weight: 500; cursor: pointer; }
.save-btn:hover { background: #0284C7; }

/* Move to project modal */
.project-list {
  display: flex;
  flex-direction: column;
  gap: 4px;
  max-height: 300px;
  overflow-y: auto;
}
.project-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 12px;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  color: #E4E4E7;
  transition: background 0.15s;
  border: 1px solid transparent;
}
.project-item:hover {
  background: #16181D;
  border-color: #27272A;
}
</style>
