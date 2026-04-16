<script setup>
import { ref } from 'vue'

const views = ref([
  { id: 1, name: 'Project Urgent Tasks', hasGlobal: true }
])

const showCreateModal = ref(false)
const showFilters = ref(false)
const viewType = ref('list') // 'list' or 'display'

const resetModal = () => {
   showCreateModal.value = false
   showFilters.value = false
   viewType.value = 'list'
}

</script>

<template>
  <div class="plane-views-wrapper">
    <!-- Header Controls -->
    <div class="views-header border-b border-[#1E2025]">
       <div class="vh-left flex items-center gap-2 text-[13px] font-medium text-gray-400">
           <i class="fa-solid fa-certificate text-orange-400"></i> CYBWF
           <i class="fa-solid fa-chevron-right text-[9px] mx-1"></i>
           <i class="fa-solid fa-layer-group text-gray-300"></i> <span class="text-gray-200">Views</span>
       </div>
       <div class="vh-right flex items-center gap-3">
          <button class="icon-action hover-white text-gray-400"><i class="fa-solid fa-magnifying-glass"></i></button>
          
          <!-- Dropdown Name Filter -->
          <el-dropdown trigger="click" popper-class="plane-popover dark !p-0">
              <button class="filter-action outlined"><i class="fa-solid fa-arrow-up-z-a" style="transform: scaleY(-1)"></i> Name</button>
              <template #dropdown>
                 <el-dropdown-menu class="dark-dropdown custom-menu w-48 bg-[#1B1C20] border border-[#2D2F36] py-1 rounded-md shadow-xl text-[13px] text-gray-300">
                    <el-dropdown-item class="hover:bg-gray-800 flex justify-between px-3 py-1.5 cursor-pointer">Name <i class="fa-solid fa-check text-gray-400"></i></el-dropdown-item>
                    <el-dropdown-item class="hover:bg-gray-800 flex px-3 py-1.5 cursor-pointer">Created at</el-dropdown-item>
                    <el-dropdown-item class="hover:bg-gray-800 flex px-3 py-1.5 cursor-pointer">Updated at</el-dropdown-item>
                    <div class="border-b border-gray-700 my-1 mx-2"></div>
                    <el-dropdown-item class="hover:bg-gray-800 flex px-3 py-1.5 cursor-pointer">Ascending</el-dropdown-item>
                    <el-dropdown-item class="hover:bg-gray-800 flex justify-between px-3 py-1.5 cursor-pointer">Descending <i class="fa-solid fa-check text-gray-400"></i></el-dropdown-item>
                 </el-dropdown-menu>
              </template>
          </el-dropdown>
          
          <button class="filter-action outlined"><i class="fa-solid fa-filter"></i> Filters</button>
          <button class="primary-action bg-[#0EA5E9] hover:bg-[#0284C7] text-white px-4 py-1.5 rounded-md font-medium text-[13px]" @click="showCreateModal = true">Add view</button>
       </div>
    </div>

    <!-- Body -->
    <div class="views-body p-6 w-full">
      <div v-if="views.length === 0" class="empty-state text-muted text-sm flex justify-center w-full mt-10">No custom views found.</div>
      
      <div class="views-list flex flex-col w-full">
         <div class="view-item group flex justify-between items-center py-3 border-b border-transparent hover:bg-[#16181D] px-4 rounded-lg cursor-pointer transition-colors" v-for="v in views" :key="v.id">
            <div class="mr-left flex items-center gap-3">
               <i class="fa-solid fa-layer-group text-gray-400 text-lg"></i>
               <div class="m-title text-sm font-medium text-[#E4E4E7]">{{ v.name }}</div>
            </div>
            
            <div class="mr-right flex items-center gap-4 text-gray-400">
               <i v-if="v.hasGlobal" class="fa-solid fa-earth-americas text-sm"></i>
               <div class="avatar-xxs bg-teal-700 rounded-full w-6 h-6 flex justify-center items-center text-white text-xs font-semibold">P</div>
               <button class="icon-action hover-white opacity-0 group-hover:opacity-100 transition-opacity"><i class="fa-regular fa-star"></i></button>
               <button class="icon-action hover-white p-1 rounded bg-[#16181D] group-hover:bg-[#27272A] border border-gray-800 transition-colors w-7 h-7 flex-center text-xs"><i class="fa-solid fa-ellipsis"></i></button>
            </div>
         </div>
      </div>
    </div>
    
    <!-- Create View Modal Overlay -->
    <div class="modal-overlay" v-if="showCreateModal" @click.self="resetModal">
       <div class="create-view-modal">
          <div class="cm-header">
             <h2 class="cm-title">Create View</h2>
          </div>
          
          <div class="cm-body">
             <!-- Icon & Title Row -->
             <div class="flex items-center bg-[#18191B] border border-[#27272A] rounded-md focus-within:border-blue-400 transition-colors">
                <div class="px-3 border-r border-[#27272A] flex justify-center items-center h-full bg-[#1B1C20] rounded-l-md">
                   <i class="fa-solid fa-layer-group text-gray-400"></i>
                </div>
                <input type="text" class="cm-input border-none flex-1 bg-transparent px-3 py-2 text-sm text-white focus:outline-none placeholder-gray-500 font-medium" placeholder="Title" autofocus />
             </div>
             
             <!-- Description text area -->
             <textarea class="cm-textarea mt-4 bg-[#18191B] border border-[#27272A] rounded-md px-3 py-2 text-sm text-white focus:outline-none focus:border-blue-400 transition-colors placeholder-gray-500 w-full resize-none" placeholder="Description" rows="4"></textarea>
             
             <!-- List / Display Toggles -->
             <div class="mt-4 flex bg-[#16181D] w-max rounded px-1 py-1 border border-[#27272A]">
                 <button class="toggle-btn px-4 py-1 text-xs rounded" :class="viewType === 'list' ? 'bg-[#27272A] text-white border border-gray-700 font-medium shadow-sm' : 'text-gray-400 hover:text-gray-200 border border-transparent'" @click="viewType = 'list'"><i class="fa-solid fa-bars mr-1"></i> List</button>
                 <button class="toggle-btn px-4 py-1 text-xs rounded" :class="viewType === 'display' ? 'bg-[#27272A] text-white border border-gray-700 font-medium shadow-sm' : 'text-gray-400 hover:text-gray-200 border border-transparent'" @click="viewType = 'display'">Display</button>
             </div>
             
             <!-- Filters section -->
             <div class="mt-4 bg-[#141518] rounded-md border border-[#27272A] p-4 min-h-[80px]">
                 <!-- Closed state -->
                 <button v-if="!showFilters" class="cbr-btn outlined w-max text-xs font-medium px-3 py-1.5 focus:outline-none" @click="showFilters = true"><i class="fa-solid fa-filter mr-1.5 text-gray-400 text-[10px]"></i> Filters</button>
                 
                 <!-- Open state -->
                 <div v-else class="filters-open-state w-full relative">
                    <button class="absolute -top-2 right-0 px-3 py-1.5 rounded border border-gray-700 bg-transparent text-xs text-gray-300 font-medium hover:bg-gray-800 transition-colors" @click="showFilters = false">Clear all</button>
                    
                    <div class="flex flex-wrap gap-y-3 gap-x-2 w-10/12">
                       <!-- Filter Chips -->
                       <div class="f-chip">
                          <i class="fa-regular fa-user"></i> Assignees
                          <span>is</span>
                          <span class="text-white"><div class="w-4 h-4 rounded-full bg-teal-700 flex justify-center items-center text-[8px] text-white mr-1 font-bold">D</div> dsa</span>
                          <i class="fa-solid fa-xmark"></i>
                       </div>
                       
                       <div class="f-chip">
                          <i class="fa-solid fa-chart-simple"></i> Priority
                          <span>is</span>
                          <span class="text-white"><i class="fa-solid fa-arrow-down text-blue-500 mr-1"></i> Low</span>
                          <i class="fa-solid fa-xmark"></i>
                       </div>
                       
                       <div class="f-chip">
                          <i class="fa-solid fa-cube text-gray-400"></i> Module
                          <span>is</span>
                          <span class="text-gray-500 font-bold tracking-widest pl-2">--</span>
                          <i class="fa-solid fa-xmark"></i>
                       </div>
                       
                       <div class="f-chip"><i class="fa-solid fa-tag"></i> Label <span>is</span> <span class="text-gray-500 font-bold tracking-widest pl-2">--</span> <i class="fa-solid fa-xmark"></i></div>
                       <div class="f-chip"><i class="fa-solid fa-circle-half-stroke"></i> Cycle <span>is</span> <span class="text-gray-500 font-bold tracking-widest pl-2">--</span> <i class="fa-solid fa-xmark"></i></div>
                       <div class="f-chip"><i class="fa-regular fa-calendar"></i> Created at <span>is</span> <span class="text-gray-500 font-bold tracking-widest pl-2">--</span> <i class="fa-solid fa-xmark"></i></div>
                       
                       <button class="cbr-btn outlined w-8 h-8 flex justify-center items-center text-gray-400"><i class="fa-solid fa-filter"></i><i class="fa-solid fa-plus text-[6px] absolute mb-2 mr-2"></i></button>
                    </div>
                 </div>
             </div>
          </div>
          
          <div class="cm-footer">
             <button class="cm-btn-cancel" @click="resetModal">Cancel</button>
             <button class="cm-btn-create" @click="resetModal">Create View</button>
          </div>
       </div>
    </div>
    
  </div>
</template>

<style scoped>
.plane-views-wrapper {
  display: flex;
  flex-direction: column;
  height: 100%;
  color: #E4E4E7;
  font-family: inherit;
  background: #0D0F11;
  min-height: calc(100vh - 120px);
}

.hover-white:hover { color: #E4E4E7; }
.flex-center { display: flex; align-items: center; justify-content: center; }

.filter-action.outlined {
  background: transparent;
  border: 1px solid #27272A;
  color: #E4E4E7;
  padding: 4px 12px;
  border-radius: 6px;
  font-size: 13px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 6px;
  transition: 0.2s;
}
.filter-action.outlined:hover { background: #1E2025; border-color: #3F3F46;}


/* Modal Overlay Create */
.modal-overlay {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
}
.create-view-modal {
  width: 650px;
  background: #1B1C20;
  border: 1px solid #2D2F36;
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0,0,0,0.8);
}
.cm-header { padding: 24px 24px 16px 24px; }
.cm-title { font-size: 18px; font-weight: 600; color: #E4E4E7; margin: 0; }
.cm-body { padding: 0 24px 24px 24px; display: flex; flex-direction: column; }

.cbr-btn.outlined { background: transparent; border: 1px solid #2D2F36; color: #E4E4E7; border-radius: 6px; cursor: pointer; transition: 0.2s; display: flex; align-items: center;}
.cbr-btn.outlined:hover { background: #27272A; }

.cm-footer {
  padding: 16px 24px; border-top: 1px solid #2D2F36; display: flex; justify-content: flex-end; gap: 12px; background: #141518; border-bottom-left-radius: 12px; border-bottom-right-radius: 12px;
}
.cm-btn-cancel { background: transparent; border: 1px solid transparent; border-radius: 6px; padding: 6px 16px; color: #E4E4E7; font-size: 13px; font-weight: 500; cursor: pointer; transition: background 0.2s; }
.cm-btn-cancel:hover { background: #27272A; }
.cm-btn-create { background: #0EA5E9; border: none; border-radius: 6px; padding: 6px 16px; color: white; font-size: 13px; font-weight: 500; cursor: pointer; transition: 0.2s; }
.cm-btn-create:hover { background: #0284C7; }

/* Filter Chips */
.f-chip {
  display: inline-flex;
  align-items: center;
  background: transparent;
  border: 1px solid #2D2F36;
  border-radius: 4px;
  font-size: 11px;
  color: #A1A1AA;
}
.f-chip i:first-child { padding: 6px 6px 6px 8px; color: #A1A1AA; }
.f-chip > span:first-of-type { 
  display: flex; align-items: center; padding-left: 8px; padding-right: 8px; border-left: 1px solid #2D2F36; height: 100%; border-right: 1px solid #2D2F36; 
}
.f-chip > span:last-of-type { 
  display: flex; align-items: center; padding-left: 8px; padding-right: 8px; height: 100%; 
}
.f-chip > i.fa-xmark {
  padding-left: 8px; padding-right: 8px; border-left: 1px solid #2D2F36; cursor: pointer; height: 100%; display: flex; align-items: center; justify-content: center; transition: color 0.2s;
}
.f-chip > i.fa-xmark:hover { color: #E4E4E7; background: #1E2025; }

/* El-dropdown overriding wrapper classes, injected global style locally */
:deep(.dark-dropdown) {
  background-color: #1B1C20 !important;
  border-color: #2D2F36 !important;
  color: #E4E4E7 !important;
}
:deep(.el-dropdown-menu__item) {
  color: #E4E4E7 !important;
}
:deep(.el-dropdown-menu__item:hover) {
  background-color: #27272A !important;
}

</style>
