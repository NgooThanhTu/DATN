<script setup>
import { ref, computed, onMounted, nextTick } from 'vue'
import { useRouter } from 'vue-router'
import axiosClient from '@/api/axiosClient'
import { ElMessageBox, ElMessage } from 'element-plus'

const props = defineProps({
  projectId: { type: String, required: true }
})
const projectId = props.projectId
const router = useRouter()

const modules = ref([])
const projectMembers = ref([])

// Status config for display
const statusConfig = {
  'backlog': { label: 'Backlog', icon: 'fa-solid fa-expand', color: '#71717A', bg: 'rgba(113,113,122,0.15)' },
  'planned': { label: 'Planned', icon: 'fa-regular fa-circle', color: '#60A5FA', bg: 'rgba(96,165,250,0.15)' },
  'in progress': { label: 'In Progress', icon: 'fa-solid fa-circle-notch', color: '#FBBF24', bg: 'rgba(251,191,36,0.15)' },
  'paused': { label: 'Paused', icon: 'fa-solid fa-pause', color: '#A1A1AA', bg: 'rgba(161,161,170,0.15)' },
  'completed': { label: 'Completed', icon: 'fa-regular fa-circle-check', color: '#4ADE80', bg: 'rgba(74,222,128,0.15)' },
  'cancelled': { label: 'Cancelled', icon: 'fa-regular fa-circle-xmark', color: '#F87171', bg: 'rgba(248,113,113,0.15)' },
}

const getStatusKey = (raw) => {
  if (!raw) return 'backlog'
  const s = raw.toLowerCase().trim()
  if (s.includes('progress') || s === 'active') return 'in progress'
  if (s.includes('complete') || s === 'done') return 'completed'
  if (s.includes('cancel')) return 'cancelled'
  if (s.includes('plan')) return 'planned'
  if (s.includes('pause')) return 'paused'
  return 'backlog'
}

const formatDateRange = (startDate, targetDate) => {
  const fmt = (d) => {
    if (!d) return null
    const dt = new Date(d)
    const months = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec']
    return `${months[dt.getMonth()]} ${dt.getDate()}`
  }
  const s = fmt(startDate)
  const e = fmt(targetDate)
  if (s && e) return `${s} - ${e}, ${new Date(targetDate).getFullYear()}`
  if (s) return `${s}, ${new Date(startDate).getFullYear()}`
  return 'Start date → End date'
}

const fetchMembers = async () => {
  if (!projectId) return
  try {
    const res = await axiosClient.get(`/projects/${projectId}/members`)
    const raw = res.data?.data || res.data || []
    projectMembers.value = (Array.isArray(raw) ? raw : []).map(m => ({
      id: m.userId || m.id,
      name: m.fullName || m.name || m.userName || m.email || 'Unknown',
      avatar: (m.fullName || m.name || m.userName || 'U').substring(0, 1).toUpperCase()
    }))
  } catch (e) {
    console.error('[ModulesTab] Failed to fetch members', e)
  }
}

const fetchModules = async () => {
  if (!projectId) return
  try {
    const res = await axiosClient.get(`/projects/${projectId}/modules`)
    modules.value = (res.data?.data || []).map(m => ({
      id: m.id,
      name: m.name,
      description: m.description || '',
      statusKey: getStatusKey(m.status),
      statusRaw: m.status || 'Backlog',
      leadId: m.leadId || null,
      startDate: m.startDate || null,
      targetDate: m.targetDate || null,
      dateRange: formatDateRange(m.startDate, m.targetDate),
      isFavorite: m.isFavorite || false,
      progress: m.progressPercent ?? m.progress ?? 0
    }))
  } catch (e) {
    console.error('[ModulesTab] Failed to fetch modules', e)
  }
}

onMounted(() => {
  fetchModules()
  fetchMembers()
})

// ===== Active Module (double-click → task table view) =====
const activeModule = ref(null)
const moduleTasks = ref([])
const loadingTasks = ref(false)

const openModuleTaskView = async (mod) => {
  router.push({
    name: 'SpaceSummary',
    params: { id: projectId },
    query: {
      tab: 'spreadsheet',
      moduleId: mod.id,
      moduleName: mod.name
    }
  })
  return

  activeModule.value = mod
  loadingTasks.value = true
  try {
    const res = await axiosClient.get(`/projects/${projectId}/WorkTasks`)
    const allTasks = res.data?.data || []
    // Filter tasks that belong to this module
    moduleTasks.value = allTasks.filter(t => t.moduleId === mod.id).map(t => ({
      id: t.id,
      identifier: t.identifier || `CYBWF-${t.sequenceNumber || '?'}`,
      title: t.title,
      status: t.statusName || t.status || 'Backlog',
      priority: t.priority || 'None',
      assigneeName: Array.isArray(t.assignees) && t.assignees.length
        ? t.assignees.map(a => a.fullName || a.email).filter(Boolean).join(', ')
        : (t.assigneeName || null),
      dueDate: t.dueDate ? new Date(t.dueDate).toLocaleDateString() : null
    }))
  } catch (e) {
    console.error('[ModulesTab] Failed to fetch module tasks', e)
    moduleTasks.value = []
  }
  loadingTasks.value = false
}

const closeModuleTaskView = () => {
  activeModule.value = null
  moduleTasks.value = []
}

// ===== Status update (inline on row) =====
const updateModuleStatus = async (mod, newStatusKey) => {
  const cfg = statusConfig[newStatusKey]
  if (!cfg) return
  try {
    await axiosClient.put(`/projects/${projectId}/modules/${mod.id}`, {
      name: mod.name,
      status: cfg.label
    })
    mod.statusKey = newStatusKey
    mod.statusRaw = cfg.label
    ElMessage.success(`Status updated to ${cfg.label}`)
  } catch (e) {
    console.error('[ModulesTab] Failed to update status', e)
    ElMessage.error('Failed to update status')
  }
}

// ===== Delete =====
const deleteModule = async (mod) => {
  try {
    await ElMessageBox.confirm(
      `Are you sure you want to delete "${mod.name}"? This action cannot be undone.`,
      'Delete Module',
      { confirmButtonText: 'Delete', cancelButtonText: 'Cancel', type: 'warning', customClass: 'dark-confirm-box' }
    )
    await axiosClient.delete(`/projects/${projectId}/modules/${mod.id}`)
    modules.value = modules.value.filter(m => m.id !== mod.id)
    ElMessage.success('Module deleted')
  } catch (e) {
    if (e !== 'cancel') {
      console.error('[ModulesTab] Failed to delete module', e)
      ElMessage.error('Failed to delete module')
    }
  }
}

// ===== Favorite / Star =====
const toggleFavorite = (mod) => {
  mod.isFavorite = !mod.isFavorite
  // Note: Backend doesn't have favorite API for modules yet, so this is local only
  ElMessage.info(mod.isFavorite ? `"${mod.name}" added to favorites` : `"${mod.name}" removed from favorites`)
}

// ===== Row Date picker =====
const rowCalendarModId = ref(null)
const rowCalMonth = ref(new Date().getMonth())
const rowCalYear = ref(new Date().getFullYear())
const rowDateStep = ref(0)
const rowTempStart = ref(null)
const rowTempEnd = ref(null)

const toggleRowCalendar = (mod) => {
  if (rowCalendarModId.value === mod.id) {
    rowCalendarModId.value = null
    return
  }
  rowCalendarModId.value = mod.id
  rowCalMonth.value = new Date().getMonth()
  rowCalYear.value = new Date().getFullYear()
  rowDateStep.value = 0
  rowTempStart.value = mod.startDate ? new Date(mod.startDate) : null
  rowTempEnd.value = mod.targetDate ? new Date(mod.targetDate) : null
}

const monthNames = ["January","February","March","April","May","June","July","August","September","October","November","December"]
const dayNames = ["SU","MO","TU","WE","TH","FR","SA"]

const rowDaysInMonth = computed(() => {
  const days = []
  const date = new Date(rowCalYear.value, rowCalMonth.value, 1)
  const firstDay = date.getDay()
  const lastDate = new Date(rowCalYear.value, rowCalMonth.value + 1, 0).getDate()
  const prevLastDate = new Date(rowCalYear.value, rowCalMonth.value, 0).getDate()
  for (let i = firstDay - 1; i >= 0; i--) days.push({ day: prevLastDate - i, isCurrent: false, date: null })
  for (let i = 1; i <= lastDate; i++) days.push({ day: i, isCurrent: true, date: new Date(rowCalYear.value, rowCalMonth.value, i) })
  const rem = days.length % 7
  if (rem !== 0) for (let i = 1; i <= 7 - rem; i++) days.push({ day: i, isCurrent: false, date: null })
  return days
})

const rowMoveMonth = (dir) => {
  rowCalMonth.value += dir
  if (rowCalMonth.value > 11) { rowCalMonth.value = 0; rowCalYear.value++ }
  if (rowCalMonth.value < 0) { rowCalMonth.value = 11; rowCalYear.value-- }
}

const isSameDate = (d1, d2) => d1 && d2 && d1.getFullYear() === d2.getFullYear() && d1.getMonth() === d2.getMonth() && d1.getDate() === d2.getDate()
const isToday = (d) => d && isSameDate(d, new Date())

const rowSelectDate = async (dObj, mod) => {
  if (!dObj.isCurrent) return
  const picked = dObj.date
  if (rowDateStep.value === 0) {
    rowTempStart.value = picked
    rowTempEnd.value = null
    rowDateStep.value = 1
  } else {
    if (picked < rowTempStart.value) {
      rowTempStart.value = picked
      rowTempEnd.value = null
    } else {
      rowTempEnd.value = picked
      rowDateStep.value = 0
      rowCalendarModId.value = null
      // Save to backend
      try {
        await axiosClient.put(`/projects/${projectId}/modules/${mod.id}`, {
          name: mod.name,
          status: mod.statusRaw,
          startDate: rowTempStart.value.toISOString(),
          targetDate: rowTempEnd.value.toISOString()
        })
        mod.startDate = rowTempStart.value.toISOString()
        mod.targetDate = rowTempEnd.value.toISOString()
        mod.dateRange = formatDateRange(mod.startDate, mod.targetDate)
        ElMessage.success('Date range updated')
      } catch (e) {
        console.error('[ModulesTab] Failed to update dates', e)
      }
    }
  }
}

const isRowSelectedStart = (d) => isSameDate(d, rowTempStart.value)
const isRowSelectedEnd = (d) => isSameDate(d, rowTempEnd.value)
const isRowInRange = (dObj) => {
  const d = dObj.date
  if (!d || !rowTempStart.value || !rowTempEnd.value || !dObj.isCurrent) return false
  return d.getTime() > rowTempStart.value.getTime() && d.getTime() < rowTempEnd.value.getTime()
}

// ===== Create / Edit Module Modal =====
const showCreateModal = ref(false)
const isEditing = ref(false)
const editingModuleId = ref(null)
const newModule = ref({ name: '', description: '', startDate: null, endDate: null, status: 'Backlog', lead: null, members: [] })

const showCalendar = ref(false)
const currentMonth = ref(new Date().getMonth())
const currentYear = ref(new Date().getFullYear())
const dateSelectionStep = ref(0)
const tempStart = ref(null)
const tempEnd = ref(null)

const toggleCalendar = () => {
  showCalendar.value = !showCalendar.value
  if (showCalendar.value) {
    tempStart.value = newModule.value.startDate
    tempEnd.value = newModule.value.endDate
    dateSelectionStep.value = 0
  }
}

const modalDaysInMonth = computed(() => {
  const days = []
  const date = new Date(currentYear.value, currentMonth.value, 1)
  const firstDay = date.getDay()
  const lastDate = new Date(currentYear.value, currentMonth.value + 1, 0).getDate()
  const prevLastDate = new Date(currentYear.value, currentMonth.value, 0).getDate()
  for (let i = firstDay - 1; i >= 0; i--) days.push({ day: prevLastDate - i, isCurrent: false, date: null })
  for (let i = 1; i <= lastDate; i++) days.push({ day: i, isCurrent: true, date: new Date(currentYear.value, currentMonth.value, i) })
  const rem = days.length % 7
  if (rem !== 0) for (let i = 1; i <= 7 - rem; i++) days.push({ day: i, isCurrent: false, date: null })
  return days
})

const modalMoveMonth = (dir) => {
  currentMonth.value += dir
  if (currentMonth.value > 11) { currentMonth.value = 0; currentYear.value++ }
  if (currentMonth.value < 0) { currentMonth.value = 11; currentYear.value-- }
}

const modalSelectDate = (dObj) => {
  if (!dObj.isCurrent) return
  const picked = dObj.date
  if (dateSelectionStep.value === 0) {
    tempStart.value = picked; tempEnd.value = null; dateSelectionStep.value = 1
    newModule.value.startDate = picked; newModule.value.endDate = null
  } else {
    if (picked < tempStart.value) {
      tempStart.value = picked; tempEnd.value = null
      newModule.value.startDate = picked; newModule.value.endDate = null
    } else {
      tempEnd.value = picked; dateSelectionStep.value = 0
      newModule.value.endDate = picked; showCalendar.value = false
    }
  }
}

const isModalSelectedStart = (d) => isSameDate(d, tempStart.value)
const isModalSelectedEnd = (d) => isSameDate(d, tempEnd.value)
const isModalInRange = (dObj) => {
  const d = dObj.date
  if (!d || !tempStart.value || !tempEnd.value || !dObj.isCurrent) return false
  return d.getTime() > tempStart.value.getTime() && d.getTime() < tempEnd.value.getTime()
}

const formatBtnDate = (d) => d ? `${d.getDate()} Thg ${d.getMonth() + 1}, ${d.getFullYear()}` : ''
const btnDateText = computed(() => {
  if (!newModule.value.startDate) return "Start date → End date"
  const s = formatBtnDate(newModule.value.startDate)
  const e = newModule.value.endDate ? formatBtnDate(newModule.value.endDate) : '...'
  return `${s} → ${e}`
})

const openCreateModal = () => {
  isEditing.value = false
  newModule.value = { name: '', description: '', startDate: null, endDate: null, status: 'Backlog', lead: null, members: [] }
  showCreateModal.value = true
}

const editModule = (mod) => {
  isEditing.value = true
  editingModuleId.value = mod.id
  newModule.value = {
    name: mod.name,
    description: mod.description || '',
    status: statusConfig[mod.statusKey]?.label || 'Backlog',
    lead: mod.leadId,
    members: [],
    startDate: mod.startDate ? new Date(mod.startDate) : null,
    endDate: mod.targetDate ? new Date(mod.targetDate) : null
  }
  showCreateModal.value = true
}

const submitCreateModule = async () => {
  if (!newModule.value.name || !projectId) return
  try {
    const payload = {
      name: newModule.value.name,
      description: newModule.value.description,
      status: newModule.value.status,
      leadId: newModule.value.lead,
      memberIds: Array.from(newModule.value.members),
      startDate: newModule.value.startDate?.toISOString?.() || newModule.value.startDate,
      targetDate: newModule.value.endDate?.toISOString?.() || newModule.value.endDate
    }
    if (isEditing.value) {
      await axiosClient.put(`/projects/${projectId}/modules/${editingModuleId.value}`, payload)
    } else {
      await axiosClient.post(`/projects/${projectId}/modules`, payload)
    }
    showCreateModal.value = false
    await fetchModules()
    ElMessage.success(isEditing.value ? 'Module updated' : 'Module created')
  } catch (e) {
    console.error('[ModulesTab] Failed to save module', e)
    ElMessage.error('Failed to save module')
  }
}

// Get lead avatar letter
const getLeadAvatar = (mod) => {
  if (!mod.leadId) return null
  const member = projectMembers.value.find(m => m.id === mod.leadId)
  return member ? member.avatar : null
}
</script>

<template>
  <div class="plane-modules-wrapper">
    <!-- Header -->
    <div class="modules-view-header">
       <div class="vh-left">
          <div class="flex items-center gap-2 text-[13px] font-medium text-gray-400">
             <i class="fa-solid fa-certificate" style="color: #F59E0B"></i> CYBWF
             <i class="fa-solid fa-chevron-right text-[9px] mx-1"></i>
             <i class="fa-solid fa-cube text-gray-500"></i>
             <span class="text-gray-200 cursor-pointer" @click="closeModuleTaskView">Modules</span>
             <template v-if="activeModule">
                <i class="fa-solid fa-chevron-right text-[9px] mx-1"></i>
                <span class="text-gray-200">{{ activeModule.name }}</span>
             </template>
          </div>
       </div>
       <div class="vh-right">
          <button class="icon-action"><i class="fa-solid fa-magnifying-glass"></i></button>
          <button class="filter-action"><i class="fa-solid fa-arrow-up-z-a" style="transform: scaleY(-1)"></i> Name <i class="fa-solid fa-chevron-down ml-1 text-[10px]"></i></button>
          <button class="filter-action"><i class="fa-solid fa-filter"></i> Filters</button>
          <div class="view-toggles">
             <button class="view-btn active"><i class="fa-solid fa-bars"></i></button>
             <button class="view-btn"><i class="fa-solid fa-border-all"></i></button>
             <button class="view-btn"><i class="fa-solid fa-link"></i></button>
          </div>
          <button class="primary-action" @click="openCreateModal">Add Module</button>
       </div>
    </div>

    <!-- ===== MODULE TASK TABLE VIEW (double-click) ===== -->
    <div class="modules-body" v-if="activeModule">
       <div class="flex items-center gap-3 mb-4">
          <button class="icon-action hover:text-white" @click="closeModuleTaskView"><i class="fa-solid fa-arrow-left"></i></button>
          <h2 class="text-lg font-semibold text-gray-200">{{ activeModule.name }}</h2>
          <span class="text-xs px-2 py-0.5 rounded" :style="{ background: statusConfig[activeModule.statusKey]?.bg, color: statusConfig[activeModule.statusKey]?.color }">
            {{ statusConfig[activeModule.statusKey]?.label }}
          </span>
       </div>

       <div v-if="loadingTasks" class="text-gray-500 text-sm py-8 text-center">Loading tasks...</div>
       <div v-else-if="moduleTasks.length === 0" class="empty-state-wrapper" style="height: 40vh">
          <div class="es-icon"><i class="fa-solid fa-layer-group"></i></div>
          <h3 class="es-title">No work items</h3>
          <p class="es-desc">No tasks have been added to this module yet.</p>
       </div>
       <div v-else class="module-task-table">
          <div class="mtt-header">
             <div class="mtt-col mtt-id">ID</div>
             <div class="mtt-col mtt-title">Title</div>
             <div class="mtt-col mtt-status">Status</div>
             <div class="mtt-col mtt-priority">Priority</div>
             <div class="mtt-col mtt-assignee">Assignee</div>
             <div class="mtt-col mtt-due">Due Date</div>
          </div>
          <div class="mtt-row" v-for="task in moduleTasks" :key="task.id">
             <div class="mtt-col mtt-id text-blue-400">{{ task.identifier }}</div>
             <div class="mtt-col mtt-title">{{ task.title }}</div>
             <div class="mtt-col mtt-status"><span class="task-status-chip">{{ task.status }}</span></div>
             <div class="mtt-col mtt-priority">{{ task.priority }}</div>
             <div class="mtt-col mtt-assignee">{{ task.assigneeName || '—' }}</div>
             <div class="mtt-col mtt-due">{{ task.dueDate || '—' }}</div>
          </div>
       </div>
    </div>

    <!-- ===== MODULE LIST VIEW ===== -->
    <div class="modules-body" v-else>
      <div class="empty-state-wrapper" v-if="modules.length === 0">
         <div class="es-icon"><i class="fa-solid fa-cube"></i></div>
         <h3 class="es-title">No modules found</h3>
         <p class="es-desc">Modules help you group work items together into specific phases or features. Create your first module to get started.</p>
         <button class="primary-action mt-4" @click="openCreateModal"><i class="fa-solid fa-plus mr-2"></i> Create Module</button>
      </div>
      <div class="modules-list" v-else>
         <div class="module-row" v-for="mod in modules" :key="mod.id" @dblclick="openModuleTaskView(mod)">
            <div class="mr-left">
               <div class="m-progress-ring">{{ Math.round(mod.progress) }}%</div>
               <div class="m-title">{{ mod.name }}</div>
            </div>
            
            <div class="mr-right" @click.stop>
               <!-- Date Range Picker -->
               <div class="dp-wrapper-inline">
                  <div class="m-date" @click="toggleRowCalendar(mod)">
                     <i class="fa-regular fa-calendar text-[10px] mr-1 opacity-60"></i>{{ mod.dateRange }}
                  </div>
                  <div class="dp-popover-row" v-if="rowCalendarModId === mod.id" @click.stop>
                     <div class="dp-header">
                        <div class="dp-month-year">
                           <span>{{ monthNames[rowCalMonth] }} <i class="fa-solid fa-chevron-down text-xs"></i></span>
                           <span>{{ rowCalYear }} <i class="fa-solid fa-chevron-down text-xs"></i></span>
                        </div>
                        <div class="dp-nav">
                           <button @click.prevent="rowMoveMonth(-1)"><i class="fa-solid fa-chevron-left"></i></button>
                           <button @click.prevent="rowMoveMonth(1)"><i class="fa-solid fa-chevron-right"></i></button>
                        </div>
                     </div>
                     <div class="dp-grid">
                        <div class="dp-day-num headday" v-for="dn in dayNames" :key="dn">{{ dn }}</div>
                        <div class="dp-day-wrapper" v-for="(dObj, idx) in rowDaysInMonth" :key="idx">
                           <div class="dp-bg-range" v-if="isRowInRange(dObj) || (isRowSelectedStart(dObj.date) && rowTempEnd) || isRowSelectedEnd(dObj.date)"
                                :class="{ 'range-start': isRowSelectedStart(dObj.date) && rowTempEnd, 'range-end': isRowSelectedEnd(dObj.date), 'range-mid': isRowInRange(dObj) }"></div>
                           <div class="dp-day-num" :class="{ 'current-month': dObj.isCurrent, 'selected': isRowSelectedStart(dObj.date) || isRowSelectedEnd(dObj.date), 'today-dot': isToday(dObj.date) }"
                                @click="rowSelectDate(dObj, mod)">{{ dObj.day }}</div>
                        </div>
                     </div>
                  </div>
               </div>

               <!-- Status Dropdown -->
               <el-dropdown trigger="click" popper-class="plane-dropdown dark !p-0" @command="(key) => updateModuleStatus(mod, key)">
                  <div class="m-status-chip cursor-pointer" :style="{ background: statusConfig[mod.statusKey]?.bg, color: statusConfig[mod.statusKey]?.color }">
                    {{ statusConfig[mod.statusKey]?.label }}
                  </div>
                  <template #dropdown>
                     <el-dropdown-menu class="dark-dropdown custom-menu w-44">
                        <el-dropdown-item v-for="(cfg, key) in statusConfig" :key="key" :command="key">
                           <i :class="cfg.icon" class="mr-2" :style="{ color: cfg.color }"></i> {{ cfg.label }}
                        </el-dropdown-item>
                     </el-dropdown-menu>
                  </template>
               </el-dropdown>

               <!-- Lead Avatar (display only) -->
               <div class="m-avatar" :class="{ 'has-lead': getLeadAvatar(mod) }" :title="mod.leadId ? 'Module Lead' : 'No lead assigned'">
                  <span v-if="getLeadAvatar(mod)" class="text-[10px] font-bold text-white">{{ getLeadAvatar(mod) }}</span>
                  <i v-else class="fa-solid fa-user text-xs"></i>
               </div>

               <!-- Star / Favorite -->
               <button class="icon-action m-icon" :class="{ 'is-fav': mod.isFavorite }" @click="toggleFavorite(mod)">
                  <i :class="mod.isFavorite ? 'fa-solid fa-star text-yellow-400' : 'fa-regular fa-star'"></i>
               </button>
               
               <!-- 3-dot Menu -->
               <el-dropdown trigger="click" popper-class="plane-dropdown dark !p-0">
                  <button class="icon-action m-icon cursor-pointer"><i class="fa-solid fa-ellipsis"></i></button>
                  <template #dropdown>
                     <el-dropdown-menu class="dark-dropdown custom-menu w-48 py-1">
                        <el-dropdown-item @click="editModule(mod)"><i class="fa-solid fa-pen mr-2 text-gray-400"></i> Edit</el-dropdown-item>
                        <el-dropdown-item @click="openModuleTaskView(mod)"><i class="fa-solid fa-arrow-up-right-from-square mr-2 text-gray-400"></i> Open</el-dropdown-item>
                        <el-dropdown-item divided class="!text-red-400" @click="deleteModule(mod)"><i class="fa-regular fa-trash-can mr-2"></i> Delete</el-dropdown-item>
                     </el-dropdown-menu>
                  </template>
               </el-dropdown>
            </div>
         </div>
      </div>
    </div>

    <!-- ===== CREATE / EDIT MODULE MODAL ===== -->
    <div class="modal-overlay" v-if="showCreateModal" @click.self="showCreateModal = false; showCalendar = false">
       <div class="create-module-modal">
          <div class="cm-header">
             <div class="cm-badge">
               <i class="fa-solid fa-certificate text-orange-400"></i> CYBWF
             </div>
             <h2 class="cm-title">{{ isEditing ? 'Edit module' : 'Create module' }}</h2>
          </div>
          
          <div class="cm-body">
             <input type="text" class="cm-input border-focus" placeholder="Title" v-model="newModule.name" autofocus />
             <textarea class="cm-textarea mt-3 border-focus" placeholder="Description" rows="4" v-model="newModule.description"></textarea>
             
             <div class="cm-toolbar mt-4">
                <!-- Date picker -->
                <div class="dp-wrapper">
                   <button class="cbr-btn" @click="toggleCalendar">
                      <i class="fa-regular fa-calendar text-gray-400"></i> {{ btnDateText }}
                   </button>
                   <div class="dp-popover" v-if="showCalendar">
                      <div class="dp-header">
                         <div class="dp-month-year">
                            <span>{{ monthNames[currentMonth] }} <i class="fa-solid fa-chevron-down text-xs"></i></span>
                            <span>{{ currentYear }} <i class="fa-solid fa-chevron-down text-xs"></i></span>
                         </div>
                         <div class="dp-nav">
                            <button @click.prevent="modalMoveMonth(-1)"><i class="fa-solid fa-chevron-left"></i></button>
                            <button @click.prevent="modalMoveMonth(1)"><i class="fa-solid fa-chevron-right"></i></button>
                         </div>
                      </div>
                      <div class="dp-grid">
                         <div class="dp-day-num headday" v-for="dn in dayNames" :key="dn">{{ dn }}</div>
                         <div class="dp-day-wrapper" v-for="(dObj, idx) in modalDaysInMonth" :key="idx">
                            <div class="dp-bg-range" v-if="isModalInRange(dObj) || (isModalSelectedStart(dObj.date) && tempEnd) || isModalSelectedEnd(dObj.date)"
                                 :class="{ 'range-start': isModalSelectedStart(dObj.date) && tempEnd, 'range-end': isModalSelectedEnd(dObj.date), 'range-mid': isModalInRange(dObj) }"></div>
                            <div class="dp-day-num" :class="{ 'current-month': dObj.isCurrent, 'selected': isModalSelectedStart(dObj.date) || isModalSelectedEnd(dObj.date) }"
                                 @click="modalSelectDate(dObj)">{{ dObj.day }}</div>
                         </div>
                      </div>
                   </div>
                </div>

                <!-- Status -->
                <el-dropdown trigger="click" popper-class="plane-dropdown dark !p-0" @command="(val) => newModule.status = val">
                   <button class="cbr-btn"><i class="fa-solid fa-expand text-gray-500"></i> {{ newModule.status }}</button>
                   <template #dropdown>
                      <el-dropdown-menu class="dark-dropdown custom-menu w-40">
                         <el-dropdown-item v-for="(cfg, key) in statusConfig" :key="key" :command="cfg.label">
                            <i :class="cfg.icon" class="mr-2" :style="{ color: cfg.color }"></i> {{ cfg.label }}
                         </el-dropdown-item>
                      </el-dropdown-menu>
                   </template>
                </el-dropdown>
                
                <!-- Lead -->
                <el-dropdown trigger="click" popper-class="plane-dropdown dark !p-0" @command="(uid) => newModule.lead = uid">
                   <button class="cbr-btn">
                     <i class="fa-regular fa-user text-gray-400"></i>
                     {{ projectMembers.find(u => u.id === newModule.lead)?.name || 'Lead' }}
                   </button>
                   <template #dropdown>
                      <el-dropdown-menu class="dark-dropdown custom-menu w-48">
                         <el-dropdown-item v-for="u in projectMembers" :key="u.id" :command="u.id">
                            <div class="flex items-center gap-2">
                               <div class="w-5 h-5 rounded-full bg-blue-500 flex items-center justify-center text-white text-[10px]">{{ u.avatar }}</div>
                               {{ u.name }}
                            </div>
                         </el-dropdown-item>
                         <el-dropdown-item v-if="!projectMembers.length" disabled>No members found</el-dropdown-item>
                      </el-dropdown-menu>
                   </template>
                </el-dropdown>

                <!-- Members -->
                <el-popover placement="bottom" trigger="click" popper-class="plane-popover dark !p-2" :width="220">
                  <template #reference>
                     <button class="cbr-btn">
                       <i class="fa-solid fa-user-group text-gray-400"></i>
                       {{ newModule.members.length > 0 ? newModule.members.length + ' Members' : 'Members' }}
                     </button>
                  </template>
                  <div class="flex flex-col gap-1 max-h-48 overflow-y-auto">
                     <label v-for="u in projectMembers" :key="u.id" class="flex items-center gap-3 p-2 hover:bg-[#27272A] rounded cursor-pointer transition">
                        <input type="checkbox" :value="u.id" v-model="newModule.members" class="accent-blue-500 w-3 h-3" />
                        <div class="flex items-center gap-2">
                           <div class="w-5 h-5 rounded-full bg-purple-500 flex items-center justify-center text-white text-[10px]">{{ u.avatar }}</div>
                           <span class="text-xs text-gray-200">{{ u.name }}</span>
                        </div>
                     </label>
                     <div v-if="!projectMembers.length" class="text-xs text-gray-500 text-center py-2">No members found</div>
                  </div>
                </el-popover>
             </div>
          </div>
          
          <div class="cm-footer">
             <button class="cm-btn-cancel" @click="showCreateModal = false">Cancel</button>
             <button class="cm-btn-create" @click="submitCreateModule">{{ isEditing ? 'Update Module' : 'Create Module' }}</button>
          </div>
       </div>
    </div>
  </div>
</template>

<style scoped>
.plane-modules-wrapper {
  display: flex;
  flex-direction: column;
  height: 100%;
  color: #E4E4E7;
  font-family: inherit;
  background: #0D0F11;
  min-height: calc(100vh - 120px);
}

/* Header */
.modules-view-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 24px;
  border-bottom: 1px solid #1E2025;
}
.vh-right { display: flex; align-items: center; gap: 12px; }
.icon-action { background: transparent; border: none; color: #A1A1AA; cursor: pointer; font-size: 14px; }
.icon-action:hover { color: #E4E4E7; }
.filter-action { background: transparent; border: 1px solid #27272A; color: #E4E4E7; padding: 6px 12px; border-radius: 6px; font-size: 13px; cursor: pointer; display: flex; align-items: center; gap: 6px; }
.filter-action:hover { background: #1E2025; }
.view-toggles { display: flex; gap: 2px; background: #16181D; padding: 2px; border-radius: 6px; }
.view-btn { border: none; background: transparent; color: #71717A; padding: 4px 8px; border-radius: 4px; cursor: pointer; }
.view-btn.active { background: #27272A; color: #E4E4E7; }
.primary-action { background: #0EA5E9; color: white; border: none; border-radius: 6px; padding: 6px 16px; font-size: 13px; cursor: pointer; font-weight: 500; }
.primary-action:hover { background: #0284C7; }

/* Module List */
.modules-body { padding: 24px; flex: 1; }
.modules-list { display: flex; flex-direction: column; gap: 2px; }
.module-row {
  display: flex; justify-content: space-between; align-items: center;
  padding: 14px 20px; border-radius: 8px; background: transparent;
  transition: background 0.2s; cursor: pointer;
}
.module-row:hover { background: #16181D; }
.mr-left { display: flex; align-items: center; gap: 16px; flex: 1; }
.m-progress-ring {
  width: 32px; height: 32px; border-radius: 50%; border: 3px solid #27272A;
  display: flex; align-items: center; justify-content: center;
  font-size: 9px; color: #71717A; font-weight: 600; flex-shrink: 0;
}
.m-title { font-size: 14px; font-weight: 500; color: #E4E4E7; }
.mr-right { display: flex; align-items: center; gap: 12px; }
.m-date {
  font-size: 12px; color: #A1A1AA; border: 1px solid #27272A;
  padding: 4px 10px; border-radius: 6px; cursor: pointer;
  transition: background 0.2s; white-space: nowrap; display: flex; align-items: center;
}
.m-date:hover { background: #1E2025; }
.m-status-chip {
  padding: 4px 12px; border-radius: 6px; font-size: 12px;
  font-weight: 600; cursor: pointer; white-space: nowrap;
  transition: filter 0.2s;
}
.m-status-chip:hover { filter: brightness(1.2); }
.m-avatar {
  width: 26px; height: 26px; border-radius: 50%;
  border: 1px dashed #52525B; display: flex; align-items: center;
  justify-content: center; color: #71717A; flex-shrink: 0;
}
.m-avatar.has-lead {
  border: none; background: #3B82F6;
}
.m-icon { color: #52525B; font-size: 14px; }
.m-icon:hover { color: #E4E4E7; }
.m-icon.is-fav { color: #FBBF24; }

/* Empty State */
.empty-state-wrapper { display: flex; flex-direction: column; align-items: center; justify-content: center; height: 60vh; text-align: center; }
.es-icon { font-size: 48px; color: #3F3F46; margin-bottom: 24px; }
.es-title { font-size: 20px; font-weight: 600; color: #E4E4E7; margin-bottom: 8px; }
.es-desc { font-size: 14px; color: #A1A1AA; max-width: 400px; line-height: 1.5; }

/* Module Task Table */
.module-task-table { border: 1px solid #1E2025; border-radius: 8px; overflow: hidden; }
.mtt-header { display: flex; background: #16181D; border-bottom: 1px solid #1E2025; padding: 10px 16px; font-size: 12px; font-weight: 600; color: #71717A; text-transform: uppercase; letter-spacing: 0.5px; }
.mtt-row { display: flex; padding: 12px 16px; border-bottom: 1px solid #1E2025; font-size: 13px; transition: background 0.15s; }
.mtt-row:hover { background: #16181D; }
.mtt-row:last-child { border-bottom: none; }
.mtt-col { display: flex; align-items: center; }
.mtt-id { width: 120px; flex-shrink: 0; }
.mtt-title { flex: 1; color: #E4E4E7; font-weight: 500; }
.mtt-status { width: 110px; flex-shrink: 0; }
.mtt-priority { width: 90px; flex-shrink: 0; color: #A1A1AA; }
.mtt-assignee { width: 120px; flex-shrink: 0; color: #A1A1AA; }
.mtt-due { width: 110px; flex-shrink: 0; color: #A1A1AA; }
.task-status-chip { padding: 2px 8px; background: rgba(113,113,122,0.15); color: #A1A1AA; border-radius: 4px; font-size: 11px; font-weight: 500; }

/* Row Date Picker */
.dp-wrapper-inline { position: relative; }
.dp-popover-row {
  position: absolute; top: 100%; right: 0; margin-top: 8px;
  background: #141518; border: 1px solid #2D2F36; border-radius: 8px;
  width: 290px; padding: 16px; box-shadow: 0 10px 30px rgba(0,0,0,0.8);
  z-index: 2000;
}

/* Modal */
.modal-overlay {
  position: fixed; top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.5); display: flex; align-items: center;
  justify-content: center; z-index: 1000;
}
.create-module-modal {
  width: 650px; background: #1B1C20; border: 1px solid #2D2F36;
  border-radius: 12px; box-shadow: 0 10px 40px rgba(0,0,0,0.8);
  overflow: visible;
}
.cm-header { padding: 24px 24px 16px 24px; }
.cm-badge { display: inline-flex; align-items: center; gap: 8px; background: transparent; border: 1px solid #3F3F46; padding: 4px 10px; border-radius: 6px; font-size: 12px; color: #E4E4E7; font-weight: 500; margin-bottom: 16px; }
.cm-title { font-size: 20px; font-weight: 600; color: #E4E4E7; margin: 0; }
.cm-body { padding: 0 24px 24px 24px; display: flex; flex-direction: column; overflow: visible; }
.cm-input { width: 100%; background: #141518; border: 1px solid #2D2F36; border-radius: 6px; padding: 12px 16px; color: #E4E4E7; font-size: 15px; outline: none; font-family: inherit; font-weight: 500; }
.cm-textarea { width: 100%; background: #141518; border: 1px solid #2D2F36; border-radius: 6px; padding: 12px 16px; color: #E4E4E7; font-size: 14px; outline: none; font-family: inherit; resize: none; }
.border-focus:focus { border-color: #38BDF8; background: #1B1C20; }
.cm-toolbar { display: flex; gap: 12px; align-items: center; position: relative; overflow: visible; flex-wrap: wrap; }
.cbr-btn { background: transparent; border: 1px solid #2D2F36; color: #E4E4E7; padding: 6px 12px; border-radius: 6px; font-size: 13px; font-weight: 600; display: flex; align-items: center; gap: 8px; cursor: pointer; transition: 0.2s; }
.cbr-btn:hover { background: #27272A; }
.cm-footer { padding: 16px 24px; border-top: 1px solid #2D2F36; display: flex; justify-content: flex-end; gap: 12px; background: #141518; border-bottom-left-radius: 12px; border-bottom-right-radius: 12px; }
.cm-btn-cancel { background: transparent; border: 1px solid #3F3F46; border-radius: 6px; padding: 8px 16px; color: #E4E4E7; font-size: 13px; font-weight: 600; cursor: pointer; }
.cm-btn-cancel:hover { background: #27272A; }
.cm-btn-create { background: #38BDF8; border: none; border-radius: 6px; padding: 8px 16px; color: white; font-size: 13px; font-weight: 600; cursor: pointer; }
.cm-btn-create:hover { background: #0284C7; }

/* Calendar shared styles */
.dp-wrapper { position: relative; }
.dp-popover { position: absolute; top: 100%; left: 0; margin-top: 8px; background: #141518; border: 1px solid #2D2F36; border-radius: 8px; width: 290px; padding: 16px; box-shadow: 0 10px 30px rgba(0,0,0,0.8); z-index: 2000; }
.dp-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.dp-month-year { display: flex; gap: 12px; font-size: 15px; font-weight: 600; color: #E4E4E7; }
.dp-nav { display: flex; gap: 8px; }
.dp-nav button { background: transparent; border: none; color: #71717A; cursor: pointer; padding: 4px; }
.dp-nav button:hover { color: #E4E4E7; }
.dp-grid { display: grid; grid-template-columns: repeat(7, 1fr); row-gap: 6px; }
.headday { font-size: 10px; font-weight: 700; color: #A1A1AA; margin-bottom: 8px; text-align: center; pointer-events: none; }
.dp-day-wrapper { position: relative; display: flex; align-items: center; justify-content: center; height: 32px; }
.dp-bg-range { position: absolute; top: 0; bottom: 0; left: 0; right: 0; background: #1D435E; z-index: 1; }
.range-start { border-top-left-radius: 16px; border-bottom-left-radius: 16px; }
.range-end { border-top-right-radius: 16px; border-bottom-right-radius: 16px; }
.dp-day-num { position: relative; z-index: 2; width: 32px; height: 32px; display: flex; align-items: center; justify-content: center; border-radius: 50%; font-size: 12px; color: #52525B; cursor: pointer; transition: 0.2s; }
.dp-day-num.current-month { color: #A1A1AA; }
.dp-day-num:hover:not(.headday) { background: #27272A; color: white; }
.dp-day-num.selected { background: #0EA5E9; color: white; border: 1px solid #38BDF8; }
.dp-day-num.today-dot { position: relative; }
.dp-day-num.today-dot::after { content: ''; position: absolute; bottom: 2px; width: 4px; height: 4px; border-radius: 50%; background: #0EA5E9; }

.no-scrollbar::-webkit-scrollbar { display: none; }
</style>
