<script setup>
import { ref, onMounted, computed, provide } from 'vue'
import { useSprintStore } from '@/store/useSprintStore'
import axiosClient from '@/api/axiosClient'

import { use } from 'echarts/core';
import { CanvasRenderer } from 'echarts/renderers';
import { LineChart } from 'echarts/charts';
import { TitleComponent, TooltipComponent, LegendComponent, GridComponent } from 'echarts/components';
import VChart, { THEME_KEY } from 'vue-echarts';

use([CanvasRenderer, LineChart, TitleComponent, TooltipComponent, LegendComponent, GridComponent]);

const props = defineProps({
  projectId: { type: String, required: true }
})

provide(THEME_KEY, 'dark')

const sprintStore = useSprintStore()
const showCreateModal = ref(false)

const expandedTabs = ref({
  active: true,
  upcoming: true,
  completed: true
});

const toggleTab = (tab) => {
  expandedTabs.value[tab] = !expandedTabs.value[tab]
}

// Data binding
onMounted(async () => {
  await sprintStore.fetchSprints(props.projectId)
  fetchBurndowns()
})

const allSprints = computed(() => sprintStore.sprints || [])

// ========== CYCLE CLASSIFICATION ==========
// A newly created cycle is ALWAYS Active (startDate <= today <= endDate) or Upcoming (startDate > today).
// We use pure date-range logic. The backend "Status" field (true/false) is only used for the
// manual Start/Close sprint flow — we ignore it for display classification.
const normalizeDate = (dateStrOrObj) => {
   if (!dateStrOrObj) return 0;
   const d = new Date(dateStrOrObj);
   // Strip time component, compare dates only
   return new Date(d.getFullYear(), d.getMonth(), d.getDate()).getTime();
}

const todayMs = computed(() => normalizeDate(new Date()))

const activeSprints = computed(() => {
   return allSprints.value.filter(s => {
      const start = normalizeDate(s.startDate);
      const end = normalizeDate(s.endDate);
      return start <= todayMs.value && todayMs.value <= end;
   })
})

const upcomingSprints = computed(() => {
   return allSprints.value.filter(s => {
      const start = normalizeDate(s.startDate);
      return start > todayMs.value;
   })
})

const completedSprints = computed(() => {
   return allSprints.value.filter(s => {
      const end = normalizeDate(s.endDate);
      return end < todayMs.value;
   })
})

const formatDateCompact = (d) => {
  if (!d) return ''
  const date = new Date(d)
  const month = date.toLocaleString('en-US', { month: 'short' })
  const day = date.getDate()
  const year = date.getFullYear()
  return `${month} ${day}, ${year}`
}

const burndownCharts = ref({})

const fetchBurndowns = async () => {
   for (const sprint of activeSprints.value) {
      try {
         const res = await axiosClient.get(`/projects/${props.projectId}/sprints/${sprint.id}/burndown`)
         const bData = res.data?.data || []
         
         const xData = bData.map(b => formatDateCompact(b.date))
         const idealData = bData.map(b => b.idealRemaining ?? b.idealPoints ?? 0)
         const currentData = bData.map(b => b.actualRemaining ?? b.remainingPoints ?? 0)

         burndownCharts.value[sprint.id] = {
            backgroundColor: 'transparent',
            tooltip: { trigger: 'axis' },
            legend: { 
               data: ['Current work items', 'Ideal work items'],
               bottom: 0,
               textStyle: { color: '#A1A1AA', fontSize: 10 }
            },
            grid: { top: 10, left: 30, right: 10, bottom: 40 },
            xAxis: {
               type: 'category',
               data: xData,
               axisLine: { show: false },
               axisTick: { show: false },
               axisLabel: { color: '#71717A', fontSize: 9 }
            },
            yAxis: {
               type: 'value',
               splitLine: { lineStyle: { color: 'rgba(255,255,255,0.05)' } },
               axisLabel: { color: '#71717A', fontSize: 10 }
            },
            series: [
               {
                  name: 'Current work items',
                  type: 'line',
                  data: currentData,
                  itemStyle: { color: '#3B82F6' },
                  areaStyle: { color: 'rgba(59,130,246,0.15)' }
               },
               {
                  name: 'Ideal work items',
                  type: 'line',
                  data: idealData,
                  itemStyle: { color: '#A1A1AA' },
                  lineStyle: { type: 'dashed' }
               }
            ]
         }
      } catch(e) {}
   }
}

// ========== COMPLETED CYCLE DETAIL ==========
const expandedCompletedId = ref(null)
const completedCycleTasks = ref({})
const loadingCycleTasks = ref({})

const toggleCompletedDetail = async (cycleId) => {
   if (expandedCompletedId.value === cycleId) {
      expandedCompletedId.value = null;
      return;
   }
   expandedCompletedId.value = cycleId;
   if (!completedCycleTasks.value[cycleId]) {
      loadingCycleTasks.value[cycleId] = true;
      try {
         // Fetch tasks that belong to this sprint
         const res = await axiosClient.get(`/projects/${props.projectId}/WorkTasks?sprintId=${cycleId}`);
         const tasks = res.data?.data || res.data || [];
         completedCycleTasks.value[cycleId] = Array.isArray(tasks) ? tasks : [];
      } catch(e) {
         completedCycleTasks.value[cycleId] = [];
      } finally {
         loadingCycleTasks.value[cycleId] = false;
      }
   }
}

const getCycleTaskStats = (cycleId) => {
   const tasks = completedCycleTasks.value[cycleId] || [];
   const total = tasks.length;
   const done = tasks.filter(t => {
      const statusName = (t.statusName || t.taskStatus?.name || '').toUpperCase();
      return statusName.includes('DONE') || statusName.includes('COMPLETE') || statusName.includes('CLOSED');
   }).length;
   return { total, done, percent: total > 0 ? Math.round((done / total) * 100) : 0 };
}

const getTaskStatusColor = (task) => {
   const name = (task.statusName || task.taskStatus?.name || '').toUpperCase();
   if (name.includes('DONE') || name.includes('COMPLETE') || name.includes('CLOSED')) return '#10B981';
   if (name.includes('PROGRESS') || name.includes('STARTED')) return '#F59E0B';
   if (name.includes('TODO') || name.includes('UNSTARTED')) return '#3F3F46';
   return '#71717A';
}

const getPriorityLabel = (p) => {
   const map = { 0: 'None', 1: 'Urgent', 2: 'High', 3: 'Medium', 4: 'Low' };
   return map[p] || 'None';
}

// Create Cycle Modal Logic
const newCycle = ref({ name: '', description: '', startDate: null, endDate: null })
const showCalendar = ref(false)
const currentMonth = ref(new Date().getMonth())
const currentYear = ref(new Date().getFullYear())

const dateSelectionStep = ref(0)
const tempStart = ref(null)
const tempEnd = ref(null)

const toggleCalendar = () => {
    showCalendar.value = !showCalendar.value
    if (showCalendar.value) {
       tempStart.value = newCycle.value.startDate
       tempEnd.value = newCycle.value.endDate
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
      newCycle.value.startDate = picked
      newCycle.value.endDate = null
   } else {
      if (picked < tempStart.value) {
         tempStart.value = picked
         tempEnd.value = null
         newCycle.value.startDate = picked
         newCycle.value.endDate = null
      } else {
         tempEnd.value = picked
         dateSelectionStep.value = 0
         newCycle.value.endDate = picked
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
   if (!newCycle.value.startDate) return "Chọn khoảng thời gian"
   const start = formatBtnDate(newCycle.value.startDate)
   const end = newCycle.value.endDate ? formatBtnDate(newCycle.value.endDate) : '...'
   return `${start} -> ${end}`
})

const createNewCycle = async () => {
    if(!newCycle.value.name) {
        alert("Vui lòng nhập tên Cycle hợp lệ!");
        return;
    }
    if(!newCycle.value.startDate) {
        alert("Vui lòng chọn ngày bắt đầu (và nhấp thêm lần nữa vào ngày kết thúc)!");
        return;
    }
    
    let finalEndDate = newCycle.value.endDate;
    if (!finalEndDate) {
        finalEndDate = new Date(newCycle.value.startDate);
        finalEndDate.setDate(finalEndDate.getDate() + 14);
    }

    try {
        const createRes = await axiosClient.post(`/projects/${props.projectId}/sprints`, {
            name: newCycle.value.name,
            description: newCycle.value.description,
            startDate: newCycle.value.startDate,
            endDate: finalEndDate
        });
        showCreateModal.value = false;
        newCycle.value = { name: '', description: '', startDate: null, endDate: null };
        await sprintStore.fetchSprints(props.projectId);
        fetchBurndowns();
    } catch(e) {
        const msg = e.response?.data?.message || e.response?.data?.title || 'Không thể tạo Cycle!';
        alert(msg);
    }
}
</script>

<template>
  <div class="plane-cycles-wrapper">
    <!-- Header Controls -->
    <!-- The user specifies top-right search/filters in Space header, 
         but we will put it in cycles view to match the image precisely -->
    <div class="cycles-view-header">
       <div class="vh-left">
          <!-- Handled by SpaceSummary header usually, keeping empty to stay true to image inside wrapper -->
       </div>
       <div class="vh-right">
          <button class="icon-action"><i class="fa-solid fa-magnifying-glass"></i></button>
          <button class="filter-action"><i class="fa-solid fa-filter"></i> Filters</button>
          <button class="primary-action" @click="showCreateModal = true">Add cycle</button>
       </div>
    </div>

    <div class="cycles-body">
      
      <!-- Active Cycle -->
      <div class="cycle-section">
        <div class="cs-header" @click="toggleTab('active')">
          <i class="chevron fa-solid" :class="expandedTabs.active ? 'fa-chevron-down' : 'fa-chevron-right'"></i>
          <i class="fa-solid fa-circle-half-stroke text-orange"></i>
          <span class="cs-title">Active cycle</span>
        </div>
        
        <div class="cs-content" v-show="expandedTabs.active">
           <div class="empty-state text-muted" v-if="activeSprints.length === 0">No active cycles.</div>
           <div class="cycle-card expanded" v-for="activeSprint in activeSprints" :key="activeSprint.id" style="margin-bottom: 24px;">
             <!-- Card Header -->
             <div class="cc-top">
                <div class="cct-left">
                   <div class="progress-ring text-green">25%</div>
                   <span class="cycle-name">{{ activeSprint?.name || 'Cycle' }}</span>
                </div>
                <div class="cct-right">
                   <span class="detail-link"><i class="fa-solid fa-info-circle"></i> More details</span>
                   <span class="date-range">
                     <i class="fa-regular fa-calendar"></i>
                     {{ formatDateCompact(activeSprint.startDate) }} - 
                     {{ formatDateCompact(activeSprint.endDate) }}
                   </span>
                   <div class="avatar-xxs bg-green">P</div>
                   <button class="icon-btn" @click="sprintStore.toggleFavorite(props.projectId, activeSprint.id)">
                      <i class="fa-solid fa-star text-orange-400" v-if="activeSprint.isFavorite"></i>
                      <i class="fa-regular fa-star" v-else></i>
                   </button>
                   <button class="icon-btn"><i class="fa-solid fa-ellipsis"></i></button>
                </div>
             </div>
             
             <!-- Card Body Split in 3 -->
             <div class="cc-grid">
                <!-- Progress -->
                <div class="grid-panel panel-progress">
                   <div class="gp-header">
                      <span>Progress</span>
                      <span class="sub">Work items</span>
                   </div>
                   <div class="progress-bar-container">
                      <div class="pb-segment bg-green" style="width: 25%"></div>
                      <div class="pb-segment bg-orange" style="width: 25%"></div>
                      <div class="pb-segment bg-darkgray" style="width: 25%"></div>
                      <div class="pb-segment bg-lightgray" style="width: 25%"></div>
                   </div>
                   <div class="legend-list">
                      <div class="legend-item"><span class="dot bg-green"></span> Completed <span class="val">--</span></div>
                      <div class="legend-item"><span class="dot bg-orange"></span> Started <span class="val">--</span></div>
                      <div class="legend-item"><span class="dot bg-darkgray"></span> Unstarted <span class="val">--</span></div>
                      <div class="legend-item"><span class="dot bg-lightgray"></span> Backlog <span class="val">--</span></div>
                   </div>
                </div>

                <!-- Burndown Chart -->
                <div class="grid-panel panel-chart">
                   <div class="gp-header">
                      <span>Work item burndown</span>
                      <span class="sub text-right">Progress</span>
                   </div>
                   <div class="chart-mockup" style="height: 140px;">
                      <v-chart v-if="burndownCharts[activeSprint.id]" :option="burndownCharts[activeSprint.id]" autoresize />
                      <div v-else class="text-muted text-xs text-center pt-8">No burndown data yet.</div>
                   </div>
                </div>

                <!-- Tabs panel -->
                <div class="grid-panel panel-tabs">
                   <div class="tabs-header">
                      <div class="tab-h active">Priority work items</div>
                      <div class="tab-h">Assignees</div>
                      <div class="tab-h">Labels</div>
                   </div>
                   <div class="tabs-body">
                      <div class="tab-row">
                         <div class="tr-user">
                            <i class="fa-regular fa-user avatar-icon"></i> No assignee
                         </div>
                         <div class="tr-stat text-muted">0% of 0</div>
                      </div>
                   </div>
                </div>
             </div>
           </div>
        </div>
      </div>

      <!-- Upcoming Cycle -->
      <div class="cycle-section">
        <div class="cs-header" @click="toggleTab('upcoming')">
          <i class="chevron fa-solid" :class="expandedTabs.upcoming ? 'fa-chevron-down' : 'fa-chevron-right'"></i>
          <i class="fa-regular fa-circle-dashed text-blue"></i>
          <span class="cs-title">Upcoming cycle</span>
          <span class="cs-count">{{ upcomingSprints.length }}</span>
        </div>
        
        <div class="cs-content" v-show="expandedTabs.upcoming">
           <div class="empty-state text-muted" v-if="upcomingSprints.length === 0">No upcoming cycles.</div>
           <div class="cycle-card collapsed cursor-pointer" v-for="sc in upcomingSprints" :key="sc.id" style="margin-bottom:8px">
             <div class="cct-left">
                <div class="progress-ring text-muted">0%</div>
                <span class="cycle-name">{{ sc.name }}</span>
             </div>
             <div class="cct-right">
                <span class="date-range mr-4">
                  <i class="fa-regular fa-calendar"></i>
                  {{ formatDateCompact(sc.startDate) }} - 
                  {{ formatDateCompact(sc.endDate) }}
                </span>
                <button class="icon-btn" @click.stop="sprintStore.toggleFavorite(props.projectId, sc.id)">
                   <i class="fa-solid fa-star text-orange-400" v-if="sc.isFavorite"></i>
                   <i class="fa-regular fa-star" v-else></i>
                </button>
                <button class="icon-btn"><i class="fa-solid fa-ellipsis"></i></button>
             </div>
           </div>
        </div>
      </div>

      <!-- Completed Cycle -->
      <div class="cycle-section">
        <div class="cs-header" @click="toggleTab('completed')">
          <i class="chevron fa-solid" :class="expandedTabs.completed ? 'fa-chevron-down' : 'fa-chevron-right'"></i>
          <i class="fa-solid fa-circle-check text-green"></i>
          <span class="cs-title">Completed cycle</span>
          <span class="cs-count">{{ completedSprints.length }}</span>
        </div>
        <div class="cs-content" v-show="expandedTabs.completed">
           <div class="empty-state text-muted" v-if="completedSprints.length === 0">No completed cycles yet.</div>
           
           <div v-for="cc in completedSprints" :key="cc.id" class="completed-cycle-wrapper" style="margin-bottom: 8px;">
             <!-- Collapsed completed card -->
             <div class="cycle-card collapsed cursor-pointer" @click="toggleCompletedDetail(cc.id)">
               <div class="cct-left">
                  <div class="progress-ring text-green">100%</div>
                  <span class="cycle-name">{{ cc.name }}</span>
               </div>
               <div class="cct-right">
                  <span class="completed-badge">Completed</span>
                  <span class="date-range">
                    <i class="fa-regular fa-calendar"></i>
                    {{ formatDateCompact(cc.startDate) }} - {{ formatDateCompact(cc.endDate) }}
                  </span>
                  <span class="task-count-badge" v-if="cc.taskCount">
                     <i class="fa-solid fa-layer-group"></i> {{ cc.taskCount }}
                  </span>
                  <button class="icon-btn" @click.stop="sprintStore.toggleFavorite(props.projectId, cc.id)">
                     <i class="fa-solid fa-star text-orange-400" v-if="cc.isFavorite"></i>
                     <i class="fa-regular fa-star" v-else></i>
                  </button>
                  <i class="fa-solid fa-chevron-down expand-chevron" :class="{ rotated: expandedCompletedId === cc.id }"></i>
               </div>
             </div>
             
             <!-- Expanded Detail Report -->
             <div class="completed-detail" v-if="expandedCompletedId === cc.id">
                <div class="cd-loading" v-if="loadingCycleTasks[cc.id]">Loading tasks...</div>
                <div v-else-if="!completedCycleTasks[cc.id] || completedCycleTasks[cc.id].length === 0" class="cd-empty">
                   No work items were in this cycle.
                </div>
                <div v-else class="cd-report">
                   <!-- Summary bar -->
                   <div class="cd-summary">
                      <div class="cd-stat">
                         <span class="cd-stat-num text-green">{{ getCycleTaskStats(cc.id).done }}</span>
                         <span class="cd-stat-label">Done</span>
                      </div>
                      <div class="cd-stat">
                         <span class="cd-stat-num text-orange">{{ getCycleTaskStats(cc.id).total - getCycleTaskStats(cc.id).done }}</span>
                         <span class="cd-stat-label">Not Done</span>
                      </div>
                      <div class="cd-stat">
                         <span class="cd-stat-num" style="color:#E4E4E7;">{{ getCycleTaskStats(cc.id).total }}</span>
                         <span class="cd-stat-label">Total</span>
                      </div>
                      <div class="cd-progress-bar-wrap">
                         <div class="cd-progress-bar">
                            <div class="cd-progress-fill" :style="{ width: getCycleTaskStats(cc.id).percent + '%' }"></div>
                         </div>
                         <span class="cd-percent">{{ getCycleTaskStats(cc.id).percent }}%</span>
                      </div>
                   </div>
                   <!-- Task list -->
                   <div class="cd-task-list">
                      <div class="cd-task-row" v-for="task in completedCycleTasks[cc.id]" :key="task.id">
                         <div class="cd-task-status">
                            <span class="cd-status-dot" :style="{ backgroundColor: getTaskStatusColor(task) }"></span>
                         </div>
                         <span class="cd-task-id">{{ task.sequenceId || '--' }}</span>
                         <span class="cd-task-title">{{ task.title }}</span>
                         <span class="cd-task-priority" :class="'priority-' + (task.priority || 0)">{{ getPriorityLabel(task.priority) }}</span>
                         <span class="cd-task-status-name">{{ task.statusName || task.taskStatus?.name || '—' }}</span>
                      </div>
                   </div>
                </div>
             </div>
           </div>
        </div>
      </div>

    </div>
    
    <!-- Create Cycle Modal (Matching Image) -->
    <div class="modal-overlay" v-if="showCreateModal" @click.self="showCreateModal = false; showCalendar = false">
       <div class="create-cycle-modal">
          <div class="cm-header">
             <div class="cm-badge"><i class="fa-solid fa-certificate text-orange"></i> CYBWF</div>
             <h2 class="cm-title">Create cycle</h2>
          </div>
          
          <div class="cm-body">
             <input type="text" class="cm-input" placeholder="Title" v-model="newCycle.name" />
             <textarea class="cm-textarea" placeholder="Description" rows="4" v-model="newCycle.description"></textarea>
             
             <!-- Date Picker Section -->
             <div class="dp-wrapper">
                <button class="dp-btn" @click="toggleCalendar">
                   <i class="fa-regular fa-calendar"></i> {{ btnDateText }}
                </button>
                
                <div class="dp-popover" v-if="showCalendar">
                   <div class="dp-header">
                      <div class="dp-month-year">
                         <span>{{ monthNames[currentMonth] }} <i class="fa-solid fa-chevron-down text-xs"></i></span>
                         <span>{{ currentYear }} <i class="fa-solid fa-chevron-down text-xs"></i></span>
                      </div>
                      <div class="dp-nav">
                         <button @click="moveMonth(-1)"><i class="fa-solid fa-chevron-left"></i></button>
                         <button @click="moveMonth(1)"><i class="fa-solid fa-chevron-right"></i></button>
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
          </div>
          
          <div class="cm-footer">
             <button class="cm-btn-cancel" @click="showCreateModal = false">Cancel</button>
             <button class="cm-btn-create" @click="createNewCycle">Create cycle</button>
          </div>
       </div>
    </div>
  </div>
</template>

<style scoped>
.plane-cycles-wrapper {
  display: flex;
  flex-direction: column;
  height: 100%;
  color: #E4E4E7;
  font-family: 'Inter', sans-serif;
}

/* Header */
.cycles-view-header {
  display: flex;
  justify-content: space-between;
  padding: 16px 24px;
  background-color: #0D0F11;
}
.vh-right {
  display: flex;
  gap: 12px;
  margin-left: auto;
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
}
.filter-action:hover { background: #16181D; }
.primary-action {
  background: #0EA5E9;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 6px 12px;
  font-size: 13px;
  cursor: pointer;
  font-weight: 500;
}
.primary-action:hover { background: #0284C7; }

/* Body Area */
.cycles-body {
  padding: 0 24px 24px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.cycle-section {
  display: flex;
  flex-direction: column;
}

.cs-header {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px 0;
  cursor: pointer;
  border-bottom: 1px solid #16181D;
  margin-bottom: 8px;
}
.chevron { font-size: 10px; color: #71717A; width: 14px; text-align: center; }
.cs-title { font-size: 14px; font-weight: 600; color: #E4E4E7; }
.cs-count { font-size: 12px; color: #71717A; }

.text-orange { color: #F59E0B; }
.text-blue { color: #3B82F6; }
.text-green { color: #10B981; }
.text-muted { color: #A1A1AA; }
.bg-orange { background-color: #F59E0B; }
.bg-green { background-color: #10B981; }
.bg-darkgray { background-color: #3F3F46; }
.bg-lightgray { background-color: #71717A; }
.bg-blue { background-color: #3B82F6; }
.bg-gray { background-color: #A1A1AA; }

/* Cards */
.cycle-card {
  background: #111315;
  border: 1px solid #1E2025;
  border-radius: 8px;
  padding: 0;
  overflow: hidden;
}
.cycle-card.collapsed {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 14px;
}

.cc-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  border-bottom: 1px solid #1E2025;
}

.cct-left { display: flex; align-items: center; gap: 12px; }
.progress-ring {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  border: 2px solid currentColor; /* Simplified ring */
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 10px;
  font-weight: 600;
}
.cycle-name { font-size: 14px; font-weight: 500; }

.cct-right { display: flex; align-items: center; gap: 16px; }
.detail-link { font-size: 12px; color: #38BDF8; cursor: pointer; display: flex; align-items: center; gap: 6px; }
.date-range { font-size: 12px; color: #A1A1AA; display: flex; align-items: center; gap: 6px; }

.avatar-xxs {
  width: 18px;
  height: 18px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 9px;
  font-weight: 600;
  color: white;
}

.icon-btn { background: transparent; border: none; color: #71717A; cursor: pointer; font-size: 13px; padding: 4px; }
.icon-btn:hover { color: #E4E4E7; }

/* 3 Pane Grid */
.cc-grid {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  min-height: 250px;
}
.grid-panel {
  padding: 20px;
  border-right: 1px solid #1E2025;
}
.grid-panel:last-child { border-right: none; }

.gp-header {
  display: flex;
  justify-content: space-between;
  font-size: 13px;
  font-weight: 500;
  color: #E4E4E7;
  margin-bottom: 24px;
}
.gp-header .sub { font-size: 12px; color: #71717A; font-weight: 400; }

/* Progress Panel */
.progress-bar-container {
  display: flex;
  height: 8px;
  border-radius: 4px;
  overflow: hidden;
  gap: 2px;
  margin-bottom: 16px;
}
.pb-segment { height: 100%; border-radius: 2px; }
.legend-list { display: flex; flex-direction: column; gap: 12px; }
.legend-item { display: flex; align-items: center; font-size: 12px; color: #A1A1AA; position: relative; }
.legend-item .val { margin-left: auto; color: #E4E4E7; }
.legend-item .dot { width: 8px; height: 8px; border-radius: 50%; margin-right: 8px; }

/* Chart Panel */
.panel-chart { position: relative; display: flex; flex-direction: column; }
.chart-mockup {
  position: relative;
  flex: 1;
  margin-bottom: 16px;
  overflow: hidden;
}
.grid-line {
  position: absolute;
  left: 0;
  right: 0;
  height: 1px;
  background: rgba(255,255,255,0.05);
}
.grid-line span {
  position: absolute;
  left: 0;
  top: -6px;
  font-size: 10px;
  color: #71717A;
}
.x-axis {
  position: absolute;
  bottom: 24px;
  left: 20px;
  right: 0;
  display: flex;
  justify-content: space-between;
  font-size: 9px;
  color: #71717A;
  text-transform: uppercase;
}
.chart-legend {
  position: absolute;
  bottom: 0px;
  left: 20px;
  right: 0;
  display: flex;
  justify-content: center;
  gap: 16px;
  font-size: 10px;
  color: #A1A1AA;
}
.leg-item { display: flex; align-items: center; gap: 4px; }
.leg-item .box { width: 6px; height: 6px; }

/* Tabs Panel */
.panel-tabs { display: flex; flex-direction: column; padding: 0; }
.tabs-header {
  display: flex;
  background: #1E2025;
}
.tab-h {
  flex: 1;
  text-align: center;
  padding: 8px 0;
  font-size: 12px;
  color: #71717A;
  background: #111315;
  border-bottom: 1px solid #1E2025;
  border-right: 1px solid #1E2025;
  cursor: pointer;
}
.tab-h.active {
  background: #1E2025;
  color: #E4E4E7;
  border-bottom: none;
}
.tabs-body { padding: 16px 20px; }
.tab-row { display: flex; justify-content: space-between; align-items: center; font-size: 12px; }
.tr-user { display: flex; align-items: center; gap: 8px; color: #A1A1AA; }
.avatar-icon { background: #27272A; width: 24px; height: 24px; display: flex; align-items: center; justify-content: center; border-radius: 50%; font-size: 10px; color: #71717A;}

.empty-state {
  padding: 16px 30px;
  font-size: 13px;
}

/* Modal Overlay */
.modal-overlay {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
}
.create-cycle-modal {
  width: 700px;
  background: #111111;
  border: 1px solid #27272A;
  border-radius: 8px;
  box-shadow: 0 10px 40px rgba(0,0,0,0.8);
  font-family: inherit;
}
.cm-header {
  padding: 24px 24px 16px 24px;
}
.cm-badge {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  background: #16181D;
  border: 1px solid #27272A;
  padding: 4px 10px;
  border-radius: 4px;
  font-size: 12px;
  color: #E4E4E7;
  font-weight: 500;
  margin-bottom: 12px;
}
.cm-title {
  font-size: 20px;
  font-weight: 600;
  color: #E4E4E7;
  margin: 0;
}
.cm-body {
  padding: 0 24px 24px 24px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.cm-input, .cm-textarea {
  width: 100%;
  background: #18191B;
  border: 1px solid #27272A;
  border-radius: 6px;
  padding: 12px 16px;
  color: #E4E4E7;
  font-size: 14px;
  outline: none;
  font-family: inherit;
}
.cm-textarea { resize: none; }

/* Date Picker */
.dp-wrapper { position: relative; }
.dp-btn {
  background: transparent;
  border: 1px solid #27272A;
  color: #E4E4E7;
  border-radius: 6px;
  padding: 6px 12px;
  font-size: 13px;
  font-weight: 500;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}
.dp-popover {
  position: absolute;
  top: 100%;
  left: 0;
  margin-top: 8px;
  background: #111111;
  border: 1px solid #27272A;
  border-radius: 8px;
  width: 280px;
  padding: 16px;
  box-shadow: 0 10px 30px rgba(0,0,0,0.8);
  z-index: 100;
}
.dp-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}
.dp-month-year {
  display: flex;
  gap: 12px;
  font-size: 15px;
  font-weight: 600;
  color: #E4E4E7;
}
.dp-month-year span { cursor: pointer; }
.dp-nav { display: flex; gap: 8px; }
.dp-nav button {
  background: transparent;
  border: none;
  color: #71717A;
  cursor: pointer;
  padding: 4px;
}
.dp-nav button:hover { color: #E4E4E7; }

.dp-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 0px 0px; 
  /* No horizontal gap so background can connect seamlessly */
  row-gap: 8px;
}
.headday {
  font-size: 10px;
  font-weight: 700;
  color: #E4E4E7;
  margin-bottom: 8px;
  pointer-events: none;
}
.dp-day-wrapper {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  height: 32px;
}
.dp-bg-range {
  position: absolute;
  top: 0; bottom: 0; left: 0; right: 0;
  background: #1D435E; /* Dark blue/teal for range */
  z-index: 1;
}
.range-start { border-top-left-radius: 16px; border-bottom-left-radius: 16px; width: 100%; left: 0;}
.range-end { border-top-right-radius: 16px; border-bottom-right-radius: 16px; width: 100%; right: 0;}

.dp-day-num {
  position: relative;
  z-index: 2;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  font-size: 12px;
  color: #71717A;
  cursor: pointer;
  transition: 0.2s;
}
.dp-day-num.current-month { color: #A1A1AA; }
.dp-day-num:hover:not(.headday) { background: #27272A; color: white; }
.dp-day-num.selected {
  background: #0EA5E9;
  color: white;
  border: 1px solid #38BDF8;
}

.cm-footer {
  padding: 16px 24px;
  border-top: 1px solid #27272A;
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
.cm-btn-cancel {
  background: transparent;
  border: 1px solid #3F3F46;
  border-radius: 6px;
  padding: 8px 16px;
  color: #E4E4E7;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
}
.cm-btn-cancel:hover { background: #1E2025; }
.cm-btn-create {
  background: #0EA5E9;
  border: none;
  border-radius: 6px;
  padding: 8px 16px;
  color: white;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
}
.cm-btn-create:hover { background: #0284C7; }

/* Completed Cycle Cards */
.completed-badge {
  background: rgba(16,185,129,0.15);
  color: #10B981;
  font-size: 11px;
  padding: 2px 8px;
  border-radius: 4px;
  font-weight: 500;
}
.task-count-badge {
  font-size: 12px;
  color: #A1A1AA;
  display: flex;
  align-items: center;
  gap: 4px;
}
.expand-chevron {
  font-size: 10px;
  color: #71717A;
  transition: transform 0.2s;
}
.expand-chevron.rotated {
  transform: rotate(180deg);
}
.cursor-pointer { cursor: pointer; }

/* Completed Detail Panel */
.completed-detail {
  background: #111315;
  border: 1px solid #1E2025;
  border-top: none;
  border-radius: 0 0 8px 8px;
  padding: 20px;
}
.cd-loading, .cd-empty {
  color: #71717A;
  font-size: 13px;
  text-align: center;
  padding: 16px;
}
.cd-summary {
  display: flex;
  align-items: center;
  gap: 24px;
  padding-bottom: 16px;
  border-bottom: 1px solid #1E2025;
  margin-bottom: 16px;
}
.cd-stat {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 2px;
}
.cd-stat-num {
  font-size: 20px;
  font-weight: 700;
}
.cd-stat-label {
  font-size: 11px;
  color: #71717A;
  text-transform: uppercase;
}
.cd-progress-bar-wrap {
  flex: 1;
  display: flex;
  align-items: center;
  gap: 12px;
  margin-left: auto;
}
.cd-progress-bar {
  flex: 1;
  height: 8px;
  background: #27272A;
  border-radius: 4px;
  overflow: hidden;
}
.cd-progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #10B981, #34D399);
  border-radius: 4px;
  transition: width 0.4s ease;
}
.cd-percent {
  font-size: 13px;
  color: #10B981;
  font-weight: 600;
  min-width: 40px;
}

/* Task List */
.cd-task-list {
  display: flex;
  flex-direction: column;
  gap: 0;
}
.cd-task-row {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 10px 8px;
  border-bottom: 1px solid rgba(255,255,255,0.04);
  font-size: 13px;
  transition: background 0.15s;
}
.cd-task-row:hover {
  background: rgba(255,255,255,0.03);
}
.cd-task-row:last-child { border-bottom: none; }
.cd-status-dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  display: inline-block;
}
.cd-task-id {
  color: #71717A;
  font-size: 12px;
  min-width: 60px;
}
.cd-task-title {
  flex: 1;
  color: #E4E4E7;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.cd-task-priority {
  font-size: 11px;
  padding: 2px 6px;
  border-radius: 3px;
  background: #27272A;
  color: #A1A1AA;
}
.cd-task-priority.priority-1 { background: rgba(239,68,68,0.15); color: #EF4444; }
.cd-task-priority.priority-2 { background: rgba(249,115,22,0.15); color: #F97316; }
.cd-task-priority.priority-3 { background: rgba(234,179,8,0.15); color: #EAB308; }
.cd-task-priority.priority-4 { background: rgba(59,130,246,0.15); color: #3B82F6; }
.cd-task-status-name {
  font-size: 12px;
  color: #A1A1AA;
  min-width: 80px;
  text-align: right;
}
</style>
