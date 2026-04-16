<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import axiosClient from '@/api/axiosClient'

const props = defineProps({
  projectId: { type: String, required: true }
})
const projectId = props.projectId

const modules = ref([])

const mockUsers = ref([])

const fetchMembers = async () => {
  if (!projectId) return;
  try {
    console.log('[ModulesTab] Fetching members for project:', projectId);
    const res = await axiosClient.get(`/projects/${projectId}/members`);
    console.log('[ModulesTab] Raw members response:', JSON.stringify(res.data));
    const rawMembers = res.data?.data || res.data || [];
    mockUsers.value = (Array.isArray(rawMembers) ? rawMembers : []).map(m => ({
      id: m.userId || m.id,
      name: m.fullName || m.name || m.userName || m.email || 'Unknown',
      avatar: (m.fullName || m.name || m.userName || 'U').substring(0, 1).toUpperCase()
    }));
    console.log('[ModulesTab] Mapped members:', JSON.stringify(mockUsers.value));
  } catch (error) {
    console.error('[ModulesTab] Failed to fetch members', error);
  }
}

// Mock Tasks per module to calculate progress
// e.g. module 1 has 3 tasks, 2 are completed (100%), 1 is 0%. Progress = 66%
const mockTasksByModule = {
  '1': [ { id: 't1', progress: 100 }, { id: 't2', progress: 0 }, { id: 't3', progress: 0 } ], // 33.33%
  '2': [ { id: 't4', progress: 100 }, { id: 't5', progress: 100 } ], // 100%
}

const fetchModules = async () => {
  if (!projectId) return;
  try {
    const res = await axiosClient.get(`/projects/${projectId}/modules`);
    modules.value = res.data.data.map(m => {
      const tasks = mockTasksByModule[m.id] || []
      const totalProgress = tasks.length ? tasks.reduce((sum, t) => sum + t.progress, 0) / tasks.length : 0
      
      return {
        id: m.id,
        name: m.name,
        progress: totalProgress,
        status: m.status?.toLowerCase() === 'completed' ? 'completed' : (m.status?.toLowerCase() === 'active' ? 'active' : 'backlog'),
        statusText: m.status?.toLowerCase() === 'completed' ? 'Done' : (m.status?.toLowerCase() === 'active' ? 'In Progress' : 'Backlog'),
        lead: m.leadName ? m.leadName.substring(0, 1).toUpperCase() : 'L',
        dateRange: m.targetDate ? `Apr 14 - 28, 2026` : 'Apr 12 - 26, 2026'
      }
    });
  } catch (error) {
    console.error('Failed to fetch modules', error);
  }
}

onMounted(() => {
  fetchModules();
  fetchMembers();
})

// === Create/Edit Module logic ===
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

const monthNames = ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"]
const dayNames = ["CN", "T2", "T3", "T4", "T5", "T6", "T7"]

const daysInMonth = computed(() => {
   const days = []
   const date = new Date(currentYear.value, currentMonth.value, 1)
   const firstDay = date.getDay()
   const lastDate = new Date(currentYear.value, currentMonth.value + 1, 0).getDate()
   const prevLastDate = new Date(currentYear.value, currentMonth.value, 0).getDate()

   for (let i = firstDay - 1; i >= 0; i--) {
      days.push({ day: prevLastDate - i, isCurrent: false, date: null })
   }
   for (let i = 1; i <= lastDate; i++) {
      days.push({ day: i, isCurrent: true, date: new Date(currentYear.value, currentMonth.value, i) })
   }
   const rem = days.length % 7
   if (rem !== 0) {
      for (let i = 1; i <= 7 - rem; i++) {
         days.push({ day: i, isCurrent: false, date: null })
      }
   }
   return days
})

const moveMonth = (dir) => {
   currentMonth.value += dir
   if (currentMonth.value > 11) { currentMonth.value = 0; currentYear.value++ }
   if (currentMonth.value < 0) { currentMonth.value = 11; currentYear.value-- }
}

const isSameDate = (d1, d2) => d1 && d2 && d1.getFullYear() === d2.getFullYear() && d1.getMonth() === d2.getMonth() && d1.getDate() === d2.getDate()

const selectDate = (dObj) => {
   if (!dObj.isCurrent) return
   const picked = dObj.date
   if (dateSelectionStep.value === 0) {
      tempStart.value = picked
      tempEnd.value = null
      dateSelectionStep.value = 1
      newModule.value.startDate = picked
      newModule.value.endDate = null
   } else {
      if (picked < tempStart.value) {
         tempStart.value = picked
         tempEnd.value = null
         newModule.value.startDate = picked
         newModule.value.endDate = null
      } else {
         tempEnd.value = picked
         dateSelectionStep.value = 0
         newModule.value.endDate = picked
         showCalendar.value = false
      }
   }
}

const isSelectedStart = (d) => isSameDate(d, tempStart.value)
const isSelectedEnd = (d) => isSameDate(d, tempEnd.value)
const isInRange = (dObj) => {
   const d = dObj.date
   if(!d || !tempStart.value || !tempEnd.value || !dObj.isCurrent) return false
   const tStart = new Date(tempStart.value.getFullYear(), tempStart.value.getMonth(), tempStart.value.getDate()).getTime()
   const tEnd = new Date(tempEnd.value.getFullYear(), tempEnd.value.getMonth(), tempEnd.value.getDate()).getTime()
   const tD = new Date(d.getFullYear(), d.getMonth(), d.getDate()).getTime()
   return tD > tStart && tD < tEnd
}

const formatBtnDate = (d) => d ? `${d.getDate()} Thg ${d.getMonth() + 1}, ${d.getFullYear()}` : ''

const btnDateText = computed(() => {
   if (!newModule.value.startDate) return "Start date \u2192 End date"
   const start = formatBtnDate(newModule.value.startDate)
   const end = newModule.value.endDate ? formatBtnDate(newModule.value.endDate) : '...'
   return `${start} \u2192 ${end}`
})

const openCreateModal = () => {
    isEditing.value = false;
    newModule.value = { name: '', description: '', startDate: null, endDate: null, status: 'Backlog', lead: null, members: [] }
    showCreateModal.value = true;
}

const editModule = (mod) => {
    isEditing.value = true;
    editingModuleId.value = mod.id;
    newModule.value = { 
        name: mod.name, 
        description: '', 
        status: mod.statusText, 
        lead: null, 
        members: [],
        startDate: null,
        endDate: null
    }
    showCreateModal.value = true;
}

const submitCreateModule = async () => {
  if (!newModule.value.name || !projectId) return;
  try {
    if (isEditing.value) {
       await axiosClient.put(`/projects/${projectId}/modules/${editingModuleId.value}`, {
          name: newModule.value.name,
          description: newModule.value.description,
          status: newModule.value.status,
          leadId: newModule.value.lead,
          memberIds: Array.from(newModule.value.members),
          startDate: newModule.value.startDate,
          targetDate: newModule.value.endDate
       });
    } else {
        await axiosClient.post(`/projects/${projectId}/modules`, {
          name: newModule.value.name,
          description: newModule.value.description,
          status: newModule.value.status,
          leadId: newModule.value.lead,
          memberIds: Array.from(newModule.value.members),
          startDate: newModule.value.startDate,
          targetDate: newModule.value.endDate
        });
    }
    showCreateModal.value = false;
    await fetchModules();
  } catch (error) {
    console.error('Failed to create/update module', error);
  }
}

// === Module Detail Slide Panel ===
const detailedModule = ref(null)

const openModuleDetail = (mod) => {
    detailedModule.value = mod
}

</script>

<template>
  <div class="plane-modules-wrapper">
    <!-- Header Controls -->
    <div class="modules-view-header">
       <div class="vh-left">
          <div class="flex items-center gap-2 text-[13px] font-medium text-gray-400">
             <i class="fa-solid fa-certificate" style="color: #F59E0B"></i> CYBWF
             <i class="fa-solid fa-chevron-right text-[9px] mx-1"></i>
             <i class="fa-solid fa-cube text-gray-500"></i> <span class="text-gray-200">Modules</span>
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

    <!-- Body / List -->
    <div class="modules-body">
      <div class="empty-state-wrapper" v-if="modules.length === 0">
         <div class="es-icon"><i class="fa-solid fa-cube"></i></div>
         <h3 class="es-title">No modules found</h3>
         <p class="es-desc">Modules help you group work items together into specific phases or features. Create your first module to get started.</p>
         <button class="primary-action mt-4" @click="openCreateModal"><i class="fa-solid fa-plus mr-2"></i> Create Module</button>
      </div>
      <div class="modules-list" v-else>
         <div class="module-row" v-for="mod in modules" :key="mod.id" @click="openModuleDetail(mod)">
             <div class="mr-left">
               <div class="m-progress-ring">{{ Math.round(mod.progress) }}%</div>
               <div class="m-title">{{ mod.name }}</div>
               <i class="fa-solid fa-circle-info m-info" @click.stop="openModuleDetail(mod)"></i>
            </div>
            
            <div class="mr-right" @click.stop>
               <div class="m-date cursor-pointer hover:bg-[#1E2025] px-2 py-1 transition rounded">{{ mod.dateRange }}</div>
               
               <el-dropdown trigger="click" popper-class="plane-dropdown dark !p-0">
                  <div class="m-status-chip cursor-pointer" :class="mod.status">{{ mod.statusText }}</div>
                  <template #dropdown>
                     <el-dropdown-menu class="dark-dropdown custom-menu w-40 bg-[#1B1C20]">
                       <el-dropdown-item><i class="fa-solid fa-expand text-gray-500 mr-2"></i> Backlog</el-dropdown-item>
                       <el-dropdown-item><i class="fa-regular fa-circle text-blue-500 mr-2"></i> Planned</el-dropdown-item>
                       <el-dropdown-item><i class="fa-solid fa-circle-notch text-orange-500 mr-2"></i> In Progress</el-dropdown-item>
                       <el-dropdown-item><i class="fa-solid fa-pause text-gray-400 mr-2"></i> Paused</el-dropdown-item>
                       <el-dropdown-item><i class="fa-regular fa-circle-check text-green-500 mr-2"></i> Completed</el-dropdown-item>
                       <el-dropdown-item><i class="fa-regular fa-circle-xmark text-red-500 mr-2"></i> Cancelled</el-dropdown-item>
                     </el-dropdown-menu>
                  </template>
               </el-dropdown>

               <div class="m-avatar cursor-pointer hover:bg-[#27272A]"><i class="fa-solid fa-user text-xs"></i></div>
               <button class="icon-action m-icon hover:text-yellow-500"><i class="fa-regular fa-star"></i></button>
               
               <el-dropdown trigger="click" popper-class="plane-dropdown dark !p-0">
                  <button class="icon-action m-icon cursor-pointer"><i class="fa-solid fa-ellipsis"></i></button>
                  <template #dropdown>
                     <el-dropdown-menu class="dark-dropdown custom-menu w-48 bg-[#1B1C20] py-1">
                        <el-dropdown-item @click="editModule(mod)"><i class="fa-solid fa-pen mr-2"></i> Edit</el-dropdown-item>
                        <el-dropdown-item><i class="fa-solid fa-arrow-up-right-from-square mr-2"></i> Open in new tab</el-dropdown-item>
                        <el-dropdown-item><i class="fa-solid fa-link mr-2"></i> Copy link</el-dropdown-item>
                        <el-dropdown-item divided class="!text-gray-400 block h-auto">
                           <div class="flex flex-col py-1">
                              <span><i class="fa-solid fa-box-archive mr-2"></i> Archive</span>
                              <span class="text-[10px] text-gray-500 break-words whitespace-normal mt-1 leading-tight">Only completed or cancelled modules can be archived</span>
                           </div>
                        </el-dropdown-item>
                        <el-dropdown-item class="!text-red-400 hover:!bg-red-900/20"><i class="fa-regular fa-trash-can mr-2"></i> Delete</el-dropdown-item>
                     </el-dropdown-menu>
                  </template>
               </el-dropdown>
            </div>
         </div>
      </div>
    </div>
    
    <!-- Module Detail Side Panel -->
    <div class="m-panel-overlay" v-if="detailedModule" @click.self="detailedModule = null"></div>
    <div class="m-side-panel" :class="{ 'open': detailedModule }">
       <div class="msp-header">
          <button class="icon-action hover-circle" @click="detailedModule = null"><i class="fa-solid fa-chevron-right text-xs"></i></button>
          <div class="ms-status-chip">Backlog</div>
          <button class="icon-action hover-circle ml-auto"><i class="fa-solid fa-ellipsis"></i></button>
       </div>
       
       <div class="msp-body no-scrollbar">
          <h2 class="msp-title">{{ detailedModule?.name }}</h2>
          <p class="msp-desc">Everything about getting started - creating a project, inviting teammates.</p>
          
          <div class="msp-props">
             <div class="p-row">
                <div class="p-label"><i class="fa-regular fa-calendar"></i> Date range</div>
                <div class="p-val-badge">Apr 14, 2026 <i class="fa-solid fa-arrow-right text-[10px] mx-1"></i> Apr 28, 2026</div>
             </div>
             <div class="p-row mt-3">
                <div class="p-label"><i class="fa-regular fa-user"></i> Lead</div>
                <div class="p-val-badge gap-1.5 px-2 py-1"><i class="fa-regular fa-user text-gray-500"></i> Lead</div>
             </div>
             <div class="p-row mt-3">
                <div class="p-label"><i class="fa-solid fa-user-group"></i> Members</div>
                <div class="p-val-badge gap-1.5 px-2 py-1"><i class="fa-solid fa-user-group text-gray-500"></i> Members</div>
             </div>
             <div class="p-row mt-4">
                <div class="p-label"><i class="fa-solid fa-layer-group"></i> Work items</div>
                <div class="p-val pl-1">0/5</div>
             </div>
          </div>
          
          <div class="collapsible-section mt-8">
             <div class="cs-head">
                <span class="font-medium">progress</span>
                <i class="fa-solid fa-chevron-up"></i>
             </div>
             <div class="cs-body pt-4">
                 <div class="chart-mockup-side relative h-48 w-full border-b border-l border-gray-800 ml-4">
                    <div class="absolute left-[-15px] bottom-1/4 text-gray-600 text-[10px] transform -rotate-90 origin-left tracking-widest whitespace-nowrap">COMPLETION</div>
                    <div class="absolute left-1/2 -bottom-6 text-gray-600 text-[10px] tracking-widest">DATE</div>
                    
                    <div class="grid-line" style="top: 0%"><span>9</span></div>
                    <div class="grid-line" style="top: 25%"><span>7</span></div>
                    <div class="grid-line" style="top: 50%"><span>5</span></div>
                    <div class="grid-line" style="top: 75%"><span>2</span></div>
                    <div class="grid-line" style="top: 100%"><span>0</span></div>
                    
                    <svg viewBox="0 0 100 100" preserveAspectRatio="none" style="position:absolute; width:100%; height:100%; top:0; left:10px;">
                       <line x1="10" y1="50" x2="90" y2="100" stroke="#71717A" stroke-width="1.5" stroke-dasharray="3,3"/>
                       <circle cx="10" cy="50" r="2.5" fill="#A1A1AA"/>
                       <circle cx="90" cy="100" r="2.5" fill="#A1A1AA"/>
                       <circle cx="20" cy="55" r="2" fill="#fff"/>
                       
                       <path d="M 10 50 L 15 50 L 20 100 L 90 100" fill="none" stroke="#3B82F6" stroke-width="1.5"/>
                       <path d="M 10 50 L 15 50 L 20 100 L 90 100 L 90 100 L 10 100 Z" fill="rgba(37, 99, 235, 0.2)"/>
                       <circle cx="10" cy="50" r="2.5" fill="#3B82F6"/>
                       <circle cx="15" cy="50" r="2" fill="#3B82F6" stroke="#111" stroke-width="1"/>
                       <circle cx="20" cy="100" r="2" fill="#3B82F6" stroke="#111" stroke-width="1"/>
                       <!-- Tooltip Mock -->
                       <g transform="translate(18, 30)">
                          <rect width="90" height="40" rx="4" fill="#16181D" stroke="#27272A"/>
                          <text x="5" y="12" fill="#fff" font-size="6" font-weight="bold">Apr 15</text>
                          <text x="12" y="22" fill="#555" font-size="5">Current work items: 5</text>
                          <rect x="5" y="19" width="4" height="4" fill="#1E40AF" rx="1"/>
                          <text x="12" y="32" fill="#bbb" font-size="5">Ideal ... </text>
                          <text x="35" y="32" fill="#fff" font-size="5" font-weight="bold">4.64285...</text>
                          <rect x="5" y="29" width="4" height="4" fill="#E5E7EB" rx="1"/>
                       </g>
                    </svg>
                    
                    <div class="absolute -bottom-4 right-0 text-[9px] text-gray-500 w-full flex justify-between px-2 pl-4">
                       <span>Apr 16</span><span>Apr 22</span><span>Apr 28</span>
                    </div>
                 </div>
                 
                 <div class="flex-center gap-4 mt-10 text-[11px] text-gray-400 font-medium">
                    <span class="flex items-center gap-1.5"><div class="w-2 h-2 rounded bg-blue-500"></div> Current work items</span>
                    <span class="flex items-center gap-1.5"><div class="w-2 h-2 rounded bg-gray-300"></div> Ideal work items</span>
                 </div>
             </div>
          </div>
          
          <div class="msp-tabs mt-8">
             <div class="msp-tab">States</div>
             <div class="msp-tab active">Assignees</div>
             <div class="msp-tab">Labels</div>
          </div>
          
          <div class="msp-tab-content mt-4 border-t border-[#27272A] pt-4">
             <div class="flex justify-between items-center text-[13px] mb-3">
                <div class="flex items-center gap-2 text-gray-300">
                   <div class="w-3 h-3 rounded-full bg-purple-600"></div>
                   concepts
                </div>
                <div class="text-gray-400">0% of 2</div>
             </div>
             <div class="flex justify-between items-center text-[13px]">
                <div class="flex items-center gap-2 text-gray-400 pl-5">
                   No labels yet
                </div>
                <div class="text-gray-400">0% of 3</div>
             </div>
          </div>
       </div>
    </div>

    <!-- Create Module Modal Overlay -->
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
             
             <!-- Toolbars -->
             <div class="cm-toolbar mt-4">
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
                            <button @click.prevent="moveMonth(-1)"><i class="fa-solid fa-chevron-left"></i></button>
                            <button @click.prevent="moveMonth(1)"><i class="fa-solid fa-chevron-right"></i></button>
                         </div>
                      </div>
                      
                      <div class="dp-grid">
                         <div class="dp-day-num headday" v-for="dn in dayNames" :key="dn">{{ dn }}</div>
                         <div class="dp-day-wrapper" v-for="(dObj, idx) in daysInMonth" :key="idx">
                            <div 
                              class="dp-bg-range" 
                              v-if="isInRange(dObj) || (isSelectedStart(dObj.date) && tempEnd) || (isSelectedEnd(dObj.date))"
                              :class="{ 'range-start': isSelectedStart(dObj.date) && tempEnd, 'range-end': isSelectedEnd(dObj.date), 'range-mid': isInRange(dObj) }"
                            ></div>
                            <div 
                              class="dp-day-num" 
                              :class="{ 'current-month': dObj.isCurrent, 'selected': isSelectedStart(dObj.date) || isSelectedEnd(dObj.date) }"
                              @click="selectDate(dObj)"
                            >
                              {{ dObj.day }}
                            </div>
                         </div>
                      </div>
                   </div>
                </div>
                
                <el-dropdown trigger="click" popper-class="plane-dropdown dark !p-0" @command="(val) => newModule.status = val">
                   <button class="cbr-btn"><i class="fa-solid fa-expand text-gray-500"></i> {{ newModule.status }}</button>
                   <template #dropdown>
                       <el-dropdown-menu class="dark-dropdown custom-menu w-40 bg-[#1B1C20] border border-[#2D2F36]">
                         <el-dropdown-item command="Backlog"><i class="fa-solid fa-expand text-gray-500 mr-2"></i> Backlog</el-dropdown-item>
                         <el-dropdown-item command="Planned"><i class="fa-regular fa-circle text-blue-500 mr-2"></i> Planned</el-dropdown-item>
                         <el-dropdown-item command="In Progress"><i class="fa-solid fa-circle-notch text-orange-500 mr-2"></i> In Progress</el-dropdown-item>
                         <el-dropdown-item command="Paused"><i class="fa-solid fa-pause text-gray-400 mr-2"></i> Paused</el-dropdown-item>
                         <el-dropdown-item command="Completed"><i class="fa-regular fa-circle-check text-green-500 mr-2"></i> Completed</el-dropdown-item>
                         <el-dropdown-item command="Cancelled"><i class="fa-regular fa-circle-xmark text-red-500 mr-2"></i> Cancelled</el-dropdown-item>
                       </el-dropdown-menu>
                   </template>
                </el-dropdown>
                
                <el-dropdown trigger="click" popper-class="plane-dropdown dark !p-0" @command="(uid) => newModule.lead = uid">
                   <button class="cbr-btn">
                     <i class="fa-regular fa-user text-gray-400"></i> 
                     {{ mockUsers.find(u => u.id === newModule.lead)?.name || 'Lead' }}
                   </button>
                   <template #dropdown>
                       <el-dropdown-menu class="dark-dropdown custom-menu w-48 bg-[#1B1C20] border border-[#2D2F36]">
                          <el-dropdown-item v-for="u in mockUsers" :key="u.id" :command="u.id">
                              <div class="flex items-center gap-2">
                                <div class="w-5 h-5 rounded-full bg-blue-500 flex items-center justify-center text-white text-[10px]">{{ u.avatar }}</div>
                                {{ u.name }}
                              </div>
                          </el-dropdown-item>
                          <el-dropdown-item v-if="!mockUsers.length" disabled class="text-xs text-gray-500 text-center py-2 relative pointer-events-none">No assignees found</el-dropdown-item>
                       </el-dropdown-menu>
                   </template>
                </el-dropdown>

                <el-popover placement="bottom" trigger="click" popper-class="plane-popover dark !p-2 bg-[#1B1C20] border-[#2D2F36]" :width="200">
                  <template #reference>
                     <button class="cbr-btn">
                       <i class="fa-solid fa-user-group text-gray-400"></i> 
                       {{ newModule.members.length > 0 ? newModule.members.length + ' Members' : 'Members' }}
                     </button>
                  </template>
                  <div class="flex flex-col gap-1 max-h-40 overflow-y-auto w-full">
                     <label v-for="u in mockUsers" :key="u.id" class="flex items-center gap-3 p-2 hover:bg-[#27272A] rounded cursor-pointer transition">
                        <input type="checkbox" :value="u.id" v-model="newModule.members" class="accent-blue-500 w-3 h-3 bg-[#16181D] border-[#3F3F46] rounded" />
                        <div class="flex items-center gap-2">
                           <div class="w-5 h-5 rounded-full bg-purple-500 flex items-center justify-center text-white text-[10px]">{{ u.avatar }}</div>
                           <span class="text-xs text-gray-200">{{ u.name }}</span>
                        </div>
                     </label>
                     <div v-if="!mockUsers.length" class="text-xs text-gray-500 text-center py-2">No assignees found</div>
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

.vh-right {
  display: flex;
  align-items: center;
  gap: 12px;
}
.icon-action {
  background: transparent;
  border: none;
  color: #A1A1AA;
  cursor: pointer;
  font-size: 14px;
}
.icon-action:hover { color: #E4E4E7; }
.filter-action {
  background: transparent;
  border: 1px solid #27272A;
  color: #E4E4E7;
  padding: 6px 12px;
  border-radius: 6px;
  font-size: 13px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 6px;
}
.filter-action:hover { background: #1E2025; }

.view-toggles { display: flex; gap: 2px; background: #16181D; padding: 2px; border-radius: 6px; }
.view-btn { border: none; background: transparent; color: #71717A; padding: 4px 8px; border-radius: 4px; cursor: pointer; }
.view-btn.active { background: #27272A; color: #E4E4E7; }

.primary-action {
  background: #0EA5E9;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 6px 16px;
  font-size: 13px;
  cursor: pointer;
  font-weight: 500;
}
.primary-action:hover { background: #0284C7; }

/* Body / List View */
.modules-body {
  padding: 24px;
  flex: 1;
}
.modules-list { display: flex; flex-direction: column; gap: 4px; }
.module-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  border-radius: 8px;
  background: transparent;
  transition: background 0.2s;
  cursor: pointer;
}
.module-row:hover { background: #16181D; }

.mr-left { display: flex; align-items: center; gap: 16px; flex: 1; }
.m-progress-ring {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  border: 4px solid #27272A;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 10px;
  color: #71717A;
}
.m-title { font-size: 14px; font-weight: 500; color: #E4E4E7; }
.m-info { font-size: 14px; color: #71717A; border: 1px solid #3F3F46; border-radius: 50%; width:16px; height: 16px; display:flex; align-items:center; justify-content:center; padding: 1px; font-size: 10px; }
.m-info:hover { color: #E4E4E7; border-color: #E4E4E7; }

.mr-right { display: flex; align-items: center; gap: 16px; }
.m-date { font-size: 12px; color: #A1A1AA; border: 1px solid #27272A; padding: 4px 8px; border-radius: 4px;}
.m-status-chip {
  padding: 4px 12px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
  background: #1E2025;
  color: #A1A1AA;
}
.m-status-chip.planned { background: rgba(37, 99, 235, 0.1); color: #60A5FA; }
.m-status-chip.active { background: rgba(245, 158, 11, 0.1); color: #FBBF24; }
.m-avatar { width: 24px; height: 24px; border-radius: 50%; border: 1px dashed #71717A; display: flex; align-items: center; justify-content: center; color: #71717A; }
.m-icon { color: #71717A; }
.m-icon:hover { color: #E4E4E7; }

/* Empty State */
.empty-state-wrapper {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 60vh;
  text-align: center;
}
.es-icon { font-size: 48px; color: #3F3F46; margin-bottom: 24px; }
.es-title { font-size: 20px; font-weight: 600; color: #E4E4E7; margin-bottom: 8px; }
.es-desc { font-size: 14px; color: #A1A1AA; max-width: 400px; line-height: 1.5; }

/* Side Panel */
.m-panel-overlay { position: fixed; top: 0; left: 0; right: 0; bottom: 0; z-index: 900; background: rgba(0,0,0,0.2) }
.m-side-panel {
  position: fixed;
  top: 0;
  bottom: 0;
  right: -500px;
  width: 450px;
  background: #16181D;
  border-left: 1px solid #27272A;
  z-index: 1000;
  box-shadow: -10px 0 30px rgba(0,0,0,0.5);
  transition: right 0.3s ease;
  display: flex;
  flex-direction: column;
}
.m-side-panel.open { right: 0; }

.msp-header {
  padding: 16px 20px;
  display: flex;
  align-items: center;
  gap: 12px;
  border-bottom: 1px solid transparent;
}
.hover-circle { width: 28px; height: 28px; border-radius: 50%; display: flex; align-items: center; justify-content: center; background: #27272A; }
.ms-status-chip { background: #27272A; color: #A1A1AA; padding: 4px 12px; border-radius: 4px; font-size: 12px; font-weight: 500;}

.msp-body { padding: 20px 24px; overflow-y: auto; flex: 1; }
.msp-title { font-size: 18px; font-weight: 600; color: #E4E4E7; margin: 0 0 8px 0; }
.msp-desc { font-size: 14px; color: #A1A1AA; line-height: 1.5; margin: 0 0 24px 0; }

.msp-props { display: flex; flex-direction: column; }
.p-row { display: flex; align-items: center; font-size: 13px; }
.p-label { width: 130px; color: #A1A1AA; display: flex; align-items: center; gap: 8px; }
.p-val-badge { background: #27272A; padding: 4px 12px; border-radius: 4px; color: #E4E4E7; display: flex; align-items: center; font-weight: 500; font-size: 12px; }

.collapsible-section .cs-head { display: flex; justify-content: space-between; font-size: 13px; font-weight: 500; color: #E4E4E7; text-transform: uppercase; cursor: pointer; }

.grid-line { position: absolute; left: 0; right: 0; height: 1px; background: rgba(255,255,255,0.05); }
.grid-line span { position: absolute; left: -14px; top: -6px; font-size: 10px; color: #71717A; }

.msp-tabs { display: flex; background: #1E2025; padding: 4px; border-radius: 6px; gap: 4px; }
.msp-tab { flex: 1; text-align: center; font-size: 12px; color: #A1A1AA; padding: 6px 0; border-radius: 4px; cursor: pointer; }
.msp-tab.active { background: #27272A; color: #E4E4E7; font-weight: 500; }


/* Modal Overlay Create */
.modal-overlay {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}
.create-module-modal {
  width: 650px;
  background: #1B1C20;
  border: 1px solid #2D2F36;
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0,0,0,0.8);
  overflow: visible;
}
.cm-header {
  padding: 24px 24px 16px 24px;
}
.cm-badge {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  background: transparent;
  border: 1px solid #3F3F46;
  padding: 4px 10px;
  border-radius: 6px;
  font-size: 12px;
  color: #E4E4E7;
  font-weight: 500;
  margin-bottom: 16px;
}
.cm-title { font-size: 20px; font-weight: 600; color: #E4E4E7; margin: 0; }
.cm-body { padding: 0 24px 24px 24px; display: flex; flex-direction: column; overflow: visible; }
.cm-input {
  width: 100%; background: #141518; border: 1px solid #2D2F36; border-radius: 6px; padding: 12px 16px; color: #E4E4E7; font-size: 15px; outline: none; font-family: inherit; font-weight: 500;
}
.cm-textarea {
  width: 100%; background: #141518; border: 1px solid #2D2F36; border-radius: 6px; padding: 12px 16px; color: #E4E4E7; font-size: 14px; outline: none; font-family: inherit; resize: none;
}
.border-focus:focus { border-color: #38BDF8; background: #1B1C20;}

/* Toolbar Items */
.cm-toolbar { display: flex; gap: 12px; align-items: center; position: relative; overflow: visible; }
.cbr-btn { background: transparent; border: 1px solid #2D2F36; color: #E4E4E7; padding: 6px 12px; border-radius: 6px; font-size: 13px; font-weight: 600; display: flex; align-items: center; gap: 8px; cursor: pointer; transition: 0.2s;}
.cbr-btn:hover { background: #27272A; }

.cm-footer {
  padding: 16px 24px; border-top: 1px solid #2D2F36; display: flex; justify-content: flex-end; gap: 12px; background: #141518; border-bottom-left-radius: 12px; border-bottom-right-radius: 12px;
}
.cm-btn-cancel { background: transparent; border: 1px solid #3F3F46; border-radius: 6px; padding: 8px 16px; color: #E4E4E7; font-size: 13px; font-weight: 600; cursor: pointer; }
.cm-btn-cancel:hover { background: #27272A; }
.cm-btn-create { background: #38BDF8; border: none; border-radius: 6px; padding: 8px 16px; color: white; font-size: 13px; font-weight: 600; cursor: pointer; }
.cm-btn-create:hover { background: #0284C7; }

/* Calendar Dropdown */
.dp-wrapper { position: relative; }
.dp-popover { position: absolute; top: 100%; left: 0; margin-top: 8px; background: #141518; border: 1px solid #2D2F36; border-radius: 8px; width: 280px; padding: 16px; box-shadow: 0 10px 30px rgba(0,0,0,0.8); z-index: 100; }
.dp-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.dp-month-year { display: flex; gap: 12px; font-size: 15px; font-weight: 600; color: #E4E4E7; }
.dp-nav { display: flex; gap: 8px; }
.dp-nav button { background: transparent; border: none; color: #71717A; cursor: pointer; padding: 4px; }
.dp-nav button:hover { color: #E4E4E7; }
.dp-grid { display: grid; grid-template-columns: repeat(7, 1fr); row-gap: 8px; }
.headday { font-size: 10px; font-weight: 700; color: #E4E4E7; margin-bottom: 8px; text-align: center; pointer-events: none; }
.dp-day-wrapper { position: relative; display: flex; align-items: center; justify-content: center; height: 32px; }
.dp-bg-range { position: absolute; top: 0; bottom: 0; left: 0; right: 0; background: #1D435E; z-index: 1; }
.range-start { border-top-left-radius: 16px; border-bottom-left-radius: 16px; }
.range-end { border-top-right-radius: 16px; border-bottom-right-radius: 16px; }
.dp-day-num { position: relative; z-index: 2; width: 32px; height: 32px; display: flex; align-items: center; justify-content: center; border-radius: 50%; font-size: 12px; color: #71717A; cursor: pointer; transition: 0.2s;}
.dp-day-num.current-month { color: #A1A1AA; }
.dp-day-num:hover:not(.headday) { background: #27272A; color: white; }
.dp-day-num.selected { background: #0EA5E9; color: white; border: 1px solid #38BDF8; }

.no-scrollbar::-webkit-scrollbar { display: none; }
</style>
