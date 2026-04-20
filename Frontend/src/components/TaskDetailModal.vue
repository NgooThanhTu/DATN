<template>
  <transition name="fade">
    <div class="task-modal-overlay" v-if="showTaskModal" @click.self="showTaskModal = false">
      
      <!-- MODE: CREATE NEW WORK ITEM (Image 1) -->
      <div class="create-centered-modal" v-if="selectedTask?.isNew">
        <h3 class="cm-title">Create new work item</h3>
        
        <div class="cm-badge-row">
           <div class="cm-badge">
             <i class="fa-solid fa-bell" style="color: #F59E0B"></i> CYBWF
           </div>
        </div>

        <div class="cm-form-group">
          <input type="text" class="cm-inputbox" placeholder="Title" v-model="selectedTask.title" />
          <textarea class="cm-textareabox" placeholder="Click to add description" v-model="selectedTask.description"></textarea>
        </div>
        
        <div class="cm-toolbar-row">
           <!-- STATUS -->
           <el-dropdown  trigger="click" @command="(cmd) => selectedTask.statusName = cmd">
             <div class="t-btn"><i :class="getStatusIcon(selectedTask?.statusName)"></i> {{ selectedTask?.statusName || 'Todo' }}</div>
             <template #dropdown>
               <el-dropdown-menu class="dark-dropdown">
                 <el-dropdown-item command="Backlog"><i class="fa-solid fa-circle-dashed text-gray-500 mr-2"></i> Backlog</el-dropdown-item>
                 <el-dropdown-item command="Todo"><i class="fa-regular fa-circle text-gray-400 mr-2"></i> Todo</el-dropdown-item>
                 <el-dropdown-item command="In Progress"><i class="fa-solid fa-circle-half-stroke text-yellow-500 mr-2"></i> In Progress</el-dropdown-item>
                 <el-dropdown-item command="In Review"><i class="fa-regular fa-circle-play text-blue-500 mr-2"></i> In Review</el-dropdown-item>
                 <el-dropdown-item command="Done"><i class="fa-regular fa-circle-check text-green-500 mr-2"></i> Done</el-dropdown-item>
               </el-dropdown-menu>
             </template>
           </el-dropdown>

           <!-- PRIORITY -->
           <el-dropdown  trigger="click" @command="(cmd) => selectedTask.priority = cmd">
             <div class="t-btn"><i :class="getPrioIcon(selectedTask?.priority)"></i> {{ getPrioLabel(selectedTask?.priority) }}</div>
             <template #dropdown>
               <el-dropdown-menu class="dark-dropdown">
                 <el-dropdown-item :command="1"><i class="fa-solid fa-angles-up mr-2" style="color: #ef4444"></i> Urgent</el-dropdown-item>
                 <el-dropdown-item :command="2"><i class="fa-solid fa-chevron-up mr-2" style="color: #f59e0b"></i> High</el-dropdown-item>
                 <el-dropdown-item :command="3"><i class="fa-solid fa-minus mr-2" style="color: #3b82f6"></i> Medium</el-dropdown-item>
                 <el-dropdown-item :command="4"><i class="fa-solid fa-arrow-down mr-2" style="color: #9ca3af"></i> Low</el-dropdown-item>
                 <el-dropdown-item :command="0"><i class="fa-solid fa-ban mr-2 text-gray-500"></i> None</el-dropdown-item>
               </el-dropdown-menu>
             </template>
           </el-dropdown>

           <!-- ASSIGNEES -->
           <el-popover  placement="bottom-start" trigger="click" popper-class="plane-popover" :width="220" @show="assigneeSearch = ''">
             <template #reference>
               <div class="t-btn"><i class="fa-regular fa-user"></i> {{ getAssigneeLabel(selectedTask?.assigneeId) }}</div>
             </template>
             <div class="popover-content">
               <input type="text" v-model="assigneeSearch" class="popover-search" placeholder="Type to search..." />
               <div class="popover-list">
                 <div class="popover-item" v-for="user in filteredMembers" :key="user.id" @click="selectedTask.assigneeId = user.id">
                   <div class="avatar-xxs bg-gray-600 rounded-full w-5 h-5 flex-center text-white text-xs mr-2">{{ user.name?.charAt(0).toUpperCase() }}</div>
                   <span>{{ user.name }}</span>
                 </div>
                 <div v-if="!filteredMembers.length" class="text-xs text-center text-gray-500 py-2">No assignees found.</div>
               </div>
             </div>
           </el-popover>

           <!-- LABELS -->
           <el-popover  placement="bottom-start" trigger="click" popper-class="plane-popover" :width="220" @show="labelSearch = ''">
             <template #reference>
               <div class="t-btn"><i class="fa-solid fa-tag"></i> {{ selectedTask?.labelIds?.length ? selectedTask.labelIds.length + ' Labels' : 'Labels' }}</div>
             </template>
             <div class="popover-content">
               <input type="text" v-model="labelSearch" class="popover-search" placeholder="Search" />
               <div class="popover-list">
                 <div class="popover-item" v-for="l in filteredLabels" :key="l.id" @click="toggleSelectedLabel(l.id)">
                   <div class="popover-c-circle mr-2 w-3 h-3 rounded-full" :style="{ backgroundColor: l.color || '#3b82f6' }"></div>
                   <span>{{ l.name }}</span>
                   <i v-if="selectedTask?.labelIds?.includes(l.id)" class="fa-solid fa-check ms-auto"></i>
                 </div>
                 <div class="popover-item pointer hover-bg-dark-1" v-if="filteredLabels.length === 0 && labelSearch" @click="createLabel(labelSearch)">
                   <span>Add "{{ labelSearch }}"</span>
                 </div>
               </div>
             </div>
           </el-popover>

           <!-- DATES -->
           <el-date-picker v-model="selectedTask.plannedStartDate" type="date" placeholder="Start date" class="t-btn-date" format="MMM DD" value-format="YYYY-MM-DD" style="width:130px; height:28px" />
           <el-date-picker v-model="selectedTask.dueDate" type="date" placeholder="Due date" class="t-btn-date" format="MMM DD" value-format="YYYY-MM-DD" style="width:125px; height:28px" />

           <!-- CYCLE -->
           <el-popover  placement="bottom-start" trigger="click" popper-class="plane-popover" :width="280" @show="cycleSearch = ''">
             <template #reference>
               <div class="t-btn"><i class="fa-solid fa-circle-half-stroke"></i> {{ getCycleLabel(selectedTask?.sprintId) }}</div>
             </template>
             <div class="popover-content">
               <input type="text" v-model="cycleSearch" class="popover-search" placeholder="Search" />
               <div class="popover-list">
                 <div class="popover-item" @click="selectedTask.sprintId = null">
                   <i class="fa-solid fa-circle-half-stroke mr-2 w-4 text-center"></i> No cycle
                   <i v-if="!selectedTask?.sprintId" class="fa-solid fa-check ms-auto"></i>
                 </div>
                 <div class="popover-item" v-for="c in filteredCycles" :key="c.id" @click="selectedTask.sprintId = c.id">
                   <i class="fa-solid fa-certificate mr-2 w-4 text-center text-blue-500"></i>
                   <span class="truncate flex-1">{{ c.name }}</span>
                   <i v-if="selectedTask?.sprintId === c.id" class="fa-solid fa-check ms-auto"></i>
                 </div>
                 <div v-if="!filteredCycles.length" class="text-xs text-center text-gray-500 py-2">No cycles setup.</div>
               </div>
             </div>
           </el-popover>

           <!-- MODULES -->
           <el-popover  placement="bottom-start" trigger="click" popper-class="plane-popover" :width="280" @show="moduleSearch = ''">
             <template #reference>
               <div class="t-btn"><i class="fa-solid fa-cube"></i> {{ getModuleLabel(selectedTask?.moduleId) }}</div>
             </template>
             <div class="popover-content">
               <input type="text" v-model="moduleSearch" class="popover-search" placeholder="Search" />
               <div class="popover-list">
                 <div class="popover-item" @click="selectedTask.moduleId = null">
                   <i class="fa-solid fa-cube mr-2"></i> No module
                 </div>
                 <div class="popover-item" v-for="m in filteredModules" :key="m.id" @click="selectedTask.moduleId = m.id">
                   <i class="fa-solid fa-box mr-2 text-orange-500"></i>
                   <span class="truncate flex-1">{{ m.name }}</span>
                 </div>
               </div>
             </div>
           </el-popover>

           <!-- PARENT -->
           <el-popover  placement="bottom-start" trigger="click" popper-class="plane-popover dark" :width="350" @show="parentSearch = ''">
             <template #reference>
               <div class="t-btn"><i class="fa-solid fa-arrow-turn-up fa-rotate-90"></i> {{ selectedTask?.parentId ? 'Parent selected' : 'Add parent' }}</div>
             </template>
             <div class="popover-content h-[250px] flex flex-col bg-[#1E2025]">
               <div class="p-2 border-b border-gray-700">
                 <div class="relative flex items-center">
                   <i class="fa-solid fa-magnifying-glass absolute left-2 text-gray-400"></i>
                   <input type="text" v-model="parentSearch" class="w-full bg-transparent border-none text-white pl-8 focus:outline-none" placeholder="Type to search..." />
                 </div>
               </div>
               <div class="flex-1 overflow-y-auto no-scrollbar p-2">
                 <div class="popover-item text-xs text-gray-400 hover:text-white cursor-pointer p-2 rounded hover:bg-gray-700 flex items-center" @click="selectedTask.parentId = null">
                   <i class="fa-solid fa-ban mr-2"></i> Remove parent
                 </div>
                 <div class="popover-item text-xs text-gray-300 hover:text-white cursor-pointer p-2 rounded hover:bg-gray-700 flex items-center" v-for="pt in filteredParents" :key="pt.id" @click="selectedTask.parentId = pt.id">
                   <span class="text-gray-500 mr-3 w-16 truncate font-mono">{{ pt.sequenceId || pt.id.substring(0,8) }}</span>
                   <span class="truncate flex-1">{{ pt.title }}</span>
                   <i v-if="selectedTask?.parentId === pt.id" class="fa-solid fa-check ml-2 text-blue-500"></i>
                 </div>
               </div>
             </div>
           </el-popover>
        </div>
        
        <div class="cm-footer-row">
           <div class="cm-t-more">
              <el-switch v-model="createMore" size="small" style="--el-switch-on-color: #38bdf8;" /> <span>Create more</span>
           </div>
           <button class="btn-discard" @click="discardNewTask">Discard</button>
           <button class="btn-save" @click="submitNewTask">Save</button>
        </div>
      </div>
      
      <!-- MODE: TASK DETAIL SLIDEOUT (Image 2 & 3) -->
      <div class="task-side-panel slide-in-right" v-else>
         <div class="sp-header">
            <div class="sph-left">
               <i class="fa-solid fa-arrow-right icon-btn" @click="showTaskModal = false"></i>
               <i class="fa-solid fa-expand icon-btn"></i>
               <i class="fa-brands fa-markdown icon-btn"></i>
            </div>
            <div class="sph-right">
                <button class="unsub-btn" :class="{ 'subscribed': props.selectedTask?.isSubscribed }" @click="toggleSubscription">
                   <i :class="props.selectedTask?.isSubscribed ? 'fa-solid fa-bell' : 'fa-regular fa-bell-slash'"></i>
                   {{ props.selectedTask?.isSubscribed ? 'Subscribed' : 'Subscribe' }}
                </button>
                <i class="fa-solid fa-link icon-btn"></i>
                <i class="fa-solid fa-link icon-btn"></i>
                <el-dropdown trigger="click" @command="handleHeaderCommand">
                  <i class="fa-solid fa-ellipsis icon-btn"></i>
                  <template #dropdown>
                    <el-dropdown-menu class="dark-dropdown">
                      <el-dropdown-item command="archive">
                        <i class="fa-regular fa-box-archive"></i> Archive
                      </el-dropdown-item>
                      <el-dropdown-item command="archive_soon">
                        <i class="fa-solid fa-hourglass-start"></i> Archive soon
                      </el-dropdown-item>
                    </el-dropdown-menu>
                  </template>
                </el-dropdown>
             </div>
         </div>
         
         <div class="sp-body">
            <!-- Header Title -->
            <div class="sp-breadcrumb">
               {{ selectedTask?.sequenceId || selectedTask?.id.substring(0,8).toUpperCase() }}
            </div>
            
            <h1 class="sp-title" contenteditable @blur="(e) => updateTaskField(selectedTask, 'title', e.target.innerText)">{{ selectedTask?.title }}</h1>
            <p class="sp-desc" contenteditable @blur="(e) => updateTaskField(selectedTask, 'description', e.target.innerText)">{{ selectedTask?.description || 'Click to add description' }}</p>
            
            <div class="sp-sub-actions">
               <i class="fa-regular fa-face-smile icon-btn" style="font-size: 16px;"></i>
               <div class="sp-edit-info">
                  <i class="fa-solid fa-clock-rotate-left"></i> Last edited by <b>dsa</b> about 10 hours ago <i class="fa-solid fa-chevron-down"></i>
               </div>
            </div>

            <!-- Action Chips -->
            <div class="sp-toolbar">
               <el-popover placement="bottom-start" trigger="click" popper-class="plane-popover dark" :width="300">
                 <template #reference>
                   <button class="s-btn"><i class="fa-solid fa-layer-group"></i> Add sub-work item</button>
                 </template>
                 <div class="popover-content h-[200px] flex flex-col bg-[#1E2025]">
                   <div class="p-2 border-b border-gray-700">
                     <div class="relative flex items-center">
                       <i class="fa-solid fa-magnifying-glass absolute left-2 text-gray-400"></i>
                       <input type="text" class="w-full bg-transparent border-none text-white pl-8 focus:outline-none" placeholder="Search tasks..." />
                     </div>
                   </div>
                   <div class="flex-1 flex-center justify-center py-4 text-gray-500 text-xs">
                     Coming soon
                   </div>
                 </div>
               </el-popover>
               
               <el-popover placement="bottom-start" trigger="click" popper-class="plane-popover dark" :width="300">
                 <template #reference>
                   <button class="s-btn"><i class="fa-solid fa-code-fork"></i> Add relation</button>
                 </template>
                 <div class="popover-content h-[200px] flex flex-col bg-[#1E2025]">
                   <div class="p-2 border-b border-gray-700">
                     <div class="relative flex items-center">
                       <i class="fa-solid fa-magnifying-glass absolute left-2 text-gray-400"></i>
                       <input type="text" class="w-full bg-transparent border-none text-white pl-8 focus:outline-none" placeholder="Search to relate..." />
                     </div>
                   </div>
                   <div class="flex-1 flex-center justify-center py-4 text-gray-500 text-xs">
                     No related items
                   </div>
                 </div>
               </el-popover>
               
               <button class="s-btn"><i class="fa-solid fa-link"></i> Add link</button>
               <button class="s-btn" @click="triggerFileUpload"><i class="fa-solid fa-paperclip"></i> Attach</button>
               <input type="file" ref="commentFileInput" style="display: none" multiple @change="handleFileChange" />
            </div>

            <h3 class="sp-section-title">Properties</h3>
            <div class="props-grid">
               <div class="p-row">
                 <div class="p-label"><i class="fa-regular fa-circle-dot"></i> State</div>
                 <div class="p-val">
                   <el-dropdown  trigger="click" @command="(val) => updateTaskField(selectedTask, 'statusName', val)">
                     <div class="cursor-pointer hover:text-white transition-colors flex items-center gap-2"><i :class="getStatusIcon(selectedTask?.statusName)"></i> {{ selectedTask?.statusName || 'Todo' }}</div>
                     <template #dropdown>
                       <el-dropdown-menu class="dark-dropdown">
                         <el-dropdown-item command="Backlog"><i class="fa-solid fa-circle-dashed text-gray-500 mr-2"></i> Backlog</el-dropdown-item>
                         <el-dropdown-item command="Todo"><i class="fa-regular fa-circle text-gray-400 mr-2"></i> Todo</el-dropdown-item>
                         <el-dropdown-item command="In Progress"><i class="fa-solid fa-circle-half-stroke text-yellow-500 mr-2"></i> In Progress</el-dropdown-item>
                         <el-dropdown-item command="In Review"><i class="fa-regular fa-circle-play text-blue-500 mr-2"></i> In Review</el-dropdown-item>
                         <el-dropdown-item command="Done"><i class="fa-regular fa-circle-check text-green-500 mr-2"></i> Done</el-dropdown-item>
                       </el-dropdown-menu>
                     </template>
                   </el-dropdown>
                 </div>
               </div>
               <div class="p-row">
                 <div class="p-label"><i class="fa-regular fa-user"></i> Assignees</div>
                 <div class="p-val">
                   <el-popover  placement="bottom-start" trigger="click" popper-class="plane-popover" :width="220" @show="assigneeSearch = ''">
                     <template #reference>
                       <div class="cursor-pointer hover:text-white transition-colors" :class="{ 'muted-val': !selectedTask?.assigneeId }">
                         {{ getAssigneeLabel(selectedTask?.assigneeId) }}
                       </div>
                     </template>
                     <div class="popover-content">
                       <input type="text" v-model="assigneeSearch" class="popover-search" placeholder="Search assignees..." />
                       <div class="popover-list">
                         <div class="popover-item" v-for="user in filteredMembers" :key="user.id" @click="updateTaskField(selectedTask, 'assigneeId', user.id)">
                           <div class="avatar-xxs bg-gray-600 rounded-full w-5 h-5 flex-center text-white text-xs mr-2">{{ user.name?.charAt(0).toUpperCase() }}</div>
                           <span>{{ user.name }}</span>
                         </div>
                         <div v-if="!filteredMembers.length" class="text-xs text-center text-gray-500 py-2">No assignees found.</div>
                       </div>
                     </div>
                   </el-popover>
                 </div>
               </div>
               <div class="p-row">
                 <div class="p-label"><i class="fa-solid fa-chart-simple"></i> Priority</div>
                 <div class="p-val">
                   <el-dropdown  trigger="click" @command="(cmd) => updateTaskField(selectedTask, 'priority', cmd)">
                     <div class="cursor-pointer hover:text-white transition-colors flex items-center gap-2" :class="{ 'muted-val': !selectedTask?.priority }"><i :class="getPrioIcon(selectedTask?.priority)"></i> {{ getPrioLabel(selectedTask?.priority) }}</div>
                     <template #dropdown>
                       <el-dropdown-menu class="dark-dropdown">
                         <el-dropdown-item :command="1"><i class="fa-solid fa-angles-up mr-2" style="color: #ef4444"></i> Urgent</el-dropdown-item>
                         <el-dropdown-item :command="2"><i class="fa-solid fa-chevron-up mr-2" style="color: #f59e0b"></i> High</el-dropdown-item>
                         <el-dropdown-item :command="3"><i class="fa-solid fa-minus mr-2" style="color: #3b82f6"></i> Medium</el-dropdown-item>
                         <el-dropdown-item :command="4"><i class="fa-solid fa-arrow-down mr-2" style="color: #9ca3af"></i> Low</el-dropdown-item>
                         <el-dropdown-item :command="0"><i class="fa-solid fa-ban mr-2 text-gray-500"></i> None</el-dropdown-item>
                       </el-dropdown-menu>
                     </template>
                   </el-dropdown>
                 </div>
               </div>
               <div class="p-row">
                 <div class="p-label"><i class="fa-regular fa-circle-user"></i> Created by</div>
                 <div class="p-val flex items-center gap-2">
                   <div class="avatar-xxs bg-green-700 rounded-full w-5 h-5 flex-center text-white text-[10px]">{{ (props.currentUser?.fullName || 'A')[0].toUpperCase() }}</div>
                   <span class="text-[13px] font-medium">{{ props.currentUser?.fullName || 'Creator' }}</span>
                 </div>
               </div>
               <div class="p-row">
                 <div class="p-label"><i class="fa-regular fa-calendar"></i> Start date</div>
                 <div class="p-val border border-transparent hover:border-gray-700 rounded px-1 -ml-1 transition-colors">
                   <el-date-picker v-model="selectedTask.plannedStartDate" type="date" placeholder="Add start date" class="t-btn-date border-none h-6 p-0! bg-transparent! outline-none" format="MMM DD YYYY" value-format="YYYY-MM-DD" style="width:130px;" @change="val => updateTaskField(selectedTask, 'plannedStartDate', val)" />
                 </div>
               </div>
               <div class="p-row">
                 <div class="p-label"><i class="fa-regular fa-calendar-check"></i> Due date</div>
                 <div class="p-val border border-transparent hover:border-gray-700 rounded px-1 -ml-1 transition-colors">
                   <el-date-picker v-model="selectedTask.dueDate" type="date" placeholder="Add due date" class="t-btn-date border-none h-6 p-0! bg-transparent! outline-none" format="MMM DD YYYY" value-format="YYYY-MM-DD" style="width:130px;" @change="val => updateTaskField(selectedTask, 'dueDate', val)" />
                 </div>
               </div>
               <div class="p-row">
                 <div class="p-label"><i class="fa-solid fa-cube"></i> Modules</div>
                 <div class="p-val">
                   <el-popover  placement="bottom-start" trigger="click" popper-class="plane-popover" :width="280" @show="moduleSearch = ''">
                     <template #reference>
                       <div class="cursor-pointer hover:text-white transition-colors flex items-center gap-2" :class="{ 'muted-val': !selectedTask?.moduleId }">{{ getModuleLabel(selectedTask?.moduleId) }}</div>
                     </template>
                     <div class="popover-content">
                       <input type="text" v-model="moduleSearch" class="popover-search" placeholder="Search modules..." />
                       <div class="popover-list">
                         <div class="popover-item" @click="updateTaskField(selectedTask, 'moduleId', null)">
                           <i class="fa-solid fa-cube mr-2"></i> No module
                         </div>
                         <div class="popover-item" v-for="m in filteredModules" :key="m.id" @click="updateTaskField(selectedTask, 'moduleId', m.id)">
                           <i class="fa-solid fa-box mr-2 text-orange-500"></i>
                           <span class="truncate flex-1">{{ m.name }}</span>
                         </div>
                       </div>
                     </div>
                   </el-popover>
                 </div>
               </div>
               <div class="p-row">
                 <div class="p-label"><i class="fa-solid fa-circle-half-stroke"></i> Cycle</div>
                 <div class="p-val">
                   <el-popover  placement="bottom-start" trigger="click" popper-class="plane-popover" :width="280" @show="cycleSearch = ''">
                     <template #reference>
                       <div class="cursor-pointer hover:text-white transition-colors flex items-center gap-2" :class="{ 'muted-val': !selectedTask?.sprintId }">{{ getCycleLabel(selectedTask?.sprintId) }}</div>
                     </template>
                     <div class="popover-content">
                       <input type="text" v-model="cycleSearch" class="popover-search" placeholder="Search cycle..." />
                       <div class="popover-list">
                         <div class="popover-item" @click="updateTaskField(selectedTask, 'sprintId', null)">
                           <i class="fa-solid fa-circle-half-stroke mr-2 w-4 text-center"></i> No cycle
                           <i v-if="!selectedTask?.sprintId" class="fa-solid fa-check ms-auto"></i>
                         </div>
                         <div class="popover-item" v-for="c in filteredCycles" :key="c.id" @click="updateTaskField(selectedTask, 'sprintId', c.id)">
                           <i class="fa-solid fa-certificate mr-2 w-4 text-center text-blue-500"></i>
                           <span class="truncate flex-1">{{ c.name }}</span>
                           <i v-if="selectedTask?.sprintId === c.id" class="fa-solid fa-check ms-auto"></i>
                         </div>
                         <div v-if="!filteredCycles.length" class="text-xs text-center text-gray-500 py-2">No cycles setup.</div>
                       </div>
                     </div>
                   </el-popover>
                 </div>
               </div>
               <div class="p-row">
                 <div class="p-label"><i class="fa-solid fa-arrow-turn-up fa-rotate-90"></i> Parent</div>
                 <div class="p-val">
                   <el-popover  placement="bottom-start" trigger="click" popper-class="plane-popover dark" :width="350" @show="parentSearch = ''">
                     <template #reference>
                       <div class="cursor-pointer hover:text-white transition-colors flex items-center gap-2" :class="{ 'muted-val': !selectedTask?.parentId }">{{ selectedTask?.parentId ? 'Parent picked' : 'Add parent work item' }}</div>
                     </template>
                     <div class="popover-content h-[250px] flex flex-col bg-[#1E2025]">
                       <div class="p-2 border-b border-gray-700">
                         <div class="relative flex items-center">
                           <i class="fa-solid fa-magnifying-glass absolute left-2 text-gray-400"></i>
                           <input type="text" v-model="parentSearch" class="w-full bg-transparent border-none text-white pl-8 focus:outline-none" placeholder="Search work items..." />
                         </div>
                       </div>
                       <div class="flex-1 overflow-y-auto no-scrollbar p-2">
                         <div class="popover-item text-xs text-gray-400 hover:text-white cursor-pointer p-2 rounded hover:bg-gray-700 flex items-center" @click="updateTaskField(selectedTask, 'parentId', null)">
                           <i class="fa-solid fa-ban mr-2"></i> Remove parent
                         </div>
                         <div class="popover-item text-xs text-gray-300 hover:text-white cursor-pointer p-2 rounded hover:bg-gray-700 flex items-center" v-for="pt in filteredParents" :key="pt.id" @click="updateTaskField(selectedTask, 'parentId', pt.id)">
                           <span class="text-gray-500 mr-3 w-16 truncate font-mono">{{ pt.sequenceId || pt.id.substring(0,8) }}</span>
                           <span class="truncate flex-1">{{ pt.title }}</span>
                           <i v-if="selectedTask?.parentId === pt.id" class="fa-solid fa-check ml-2 text-blue-500"></i>
                         </div>
                       </div>
                     </div>
                   </el-popover>
                 </div>
               </div>
               <div class="p-row">
                 <div class="p-label"><i class="fa-solid fa-tags"></i> Labels</div>
                 <div class="p-val flex flex-wrap gap-2 items-center">
                    <div v-for="lid in (selectedTask?.labelIds || [])" :key="lid" class="flex items-center gap-1.5 bg-[#16181D] border border-gray-700 rounded px-2 py-0.5 text-xs text-gray-300">
                       <span class="w-2 h-2 rounded-full" :style="{ backgroundColor: projectLabels.find(l=>l.id===lid)?.color || '#3b82f6' }"></span>
                       {{ projectLabels.find(l=>l.id===lid)?.name || lid }}
                       <i class="fa-solid fa-xmark ml-1 cursor-pointer hover:text-red-400" @click="toggleSelectedLabelDetail(lid)"></i>
                    </div>
                    <el-popover  placement="bottom-start" trigger="click" popper-class="plane-popover" :width="220" @show="labelSearch = ''">
                      <template #reference>
                        <button class="btn-add-label"><i class="fa-solid fa-plus"></i> Add labels</button>
                      </template>
                      <div class="popover-content">
                        <input type="text" v-model="labelSearch" class="popover-search" placeholder="Search" />
                        <div class="popover-list">
                          <div class="popover-item" v-for="l in filteredLabels" :key="l.id" @click="toggleSelectedLabelDetail(l.id)">
                            <div class="popover-c-circle mr-2 w-3 h-3 rounded-full" :style="{ backgroundColor: l.color || '#3b82f6' }"></div>
                            <span>{{ l.name }}</span>
                            <i v-if="selectedTask?.labelIds?.includes(l.id)" class="fa-solid fa-check ms-auto"></i>
                          </div>
                          <div class="popover-item pointer hover-bg-dark-1" v-if="filteredLabels.length === 0 && labelSearch" @click="createLabelDetail(labelSearch)">
                            <span>Add "{{ labelSearch }}"</span>
                          </div>
                        </div>
                      </div>
                    </el-popover>
                 </div>
               </div>
            </div>

            <div class="flex-between mb-6" style="margin-top: 56px;">
               <h3 class="sp-section-title mb-0">Activity</h3>
               <div class="flex-center gap-2">
                  <button class="icon-filter-btn"><i class="fa-solid fa-arrow-down-short-wide"></i></button>
                  <button class="icon-filter-btn"><i class="fa-solid fa-bars-staggered"></i></button>
               </div>
            </div>

            <div class="activity-feed">
               <div class="feed-item">
                  <div class="feed-icon"><i class="fa-solid fa-clone"></i></div>
                  <div class="feed-text"><b>{{ props.currentUser?.fullName || 'Plane' }}</b> created the work item. <span class="muted-val">about 10 hours ago</span></div>
               </div>
               
               <div v-for="c in topLevelComments" :key="c.id" class="feed-item group">
                  <div class="feed-avatar">{{ c.fullName?.[0] || 'U' }}</div>
                  <div class="feed-content w-full relative">
                    <div class="flex items-center justify-between">
                       <div>
                          <span class="font-bold text-white text-[13px]">{{ c.fullName || 'User' }}</span> 
                          <span class="text-gray-500 text-xs ml-2">commented {{ formatDate(c.createdAt) }} <span v-if="c.isEdited" class="italic">(edited)</span></span>
                       </div>
                       
                       <!-- Hover Actions -->
                       <div class="hidden group-hover:flex items-center gap-1 bg-[#16181D] border border-[#27272A] rounded p-0.5 shadow-lg absolute right-0 -top-2">
                          <el-popover  placement="bottom-end" trigger="click" popper-class="plane-popover dark !p-0" :width="320">
                             <template #reference>
                                <i class="fa-regular fa-face-smile text-gray-400 hover:text-white cursor-pointer px-1.5 py-1 rounded hover:bg-gray-700"></i>
                             </template>
                             <div class="popover-content bg-[#1E2025]">
                                <div class="p-2 border-b border-gray-700 relative">
                                  <i class="fa-solid fa-magnifying-glass absolute left-4 top-1/2 transform -translate-y-1/2 text-gray-400 text-xs"></i>
                                  <input type="text" v-model="emojiSearch" class="w-full bg-[#111] border border-gray-700 rounded text-white py-1.5 pl-8 pr-2 text-xs focus:outline-none focus:border-blue-500" placeholder="Search..." />
                                </div>
                                <div class="p-2 text-xs font-semibold text-gray-400">Smileys & emotion</div>
                                <div class="grid grid-cols-8 gap-1 p-2 max-h-[160px] overflow-y-auto no-scrollbar">
                                  <div v-for="emoji in filteredEmojis" :key="emoji" @click="addReaction(c, emoji)" class="cursor-pointer text-lg text-center hover:bg-gray-700 rounded p-1">{{ emoji }}</div>
                                </div>
                             </div>
                          </el-popover>
                          
                          <el-dropdown  trigger="click" placement="bottom-end">
                             <i class="fa-solid fa-ellipsis text-gray-400 hover:text-white cursor-pointer px-1.5 py-1 rounded hover:bg-gray-700"></i>
                             <template #dropdown>
                               <el-dropdown-menu class="dark-dropdown" style="width: 150px;">
                                 <el-dropdown-item @click="startEditingComment(c)"><i class="fa-solid fa-pen mr-2"></i> Edit</el-dropdown-item>
                                 <el-dropdown-item @click="copyCommentLink(c.id)"><i class="fa-solid fa-link mr-2"></i> Copy link</el-dropdown-item>
                                 <el-dropdown-item @click="deleteComment(c.id)" style="color: #f87171 !important;"><i class="fa-regular fa-trash-can mr-2"></i> Delete</el-dropdown-item>
                               </el-dropdown-menu>
                             </template>
                          </el-dropdown>
                       </div>
                    </div>
                    
                    <!-- Editable vs Normal -->
                    <div v-if="editingCommentId === c.id" class="mt-2">
                       <div class="editor-wrap !bg-[#1E2025]">
                          <textarea class="c-input bg-transparent border-none !h-[60px]" v-model="editingContent" autofocus></textarea>
                          <div class="c-toolbar flex justify-end gap-2 p-2">
                             <button class="px-3 py-1.5 text-xs rounded border border-gray-600 text-gray-300 hover:bg-gray-700 transition" @click="cancelEditingComment">Cancel</button>
                             <button class="px-3 py-1.5 text-xs rounded bg-blue-600 text-white hover:bg-blue-700 transition" @click="saveEditedComment(c.id, c)">Update</button>
                          </div>
                       </div>
                    </div>
                    <div v-else>
                       <p class="mt-1 text-[14px] text-gray-300 whitespace-pre-wrap">{{ c.content }}</p>
                       
                       <!-- Reactions -->
                       <div class="flex flex-wrap gap-2 mt-2" v-if="c.reactions && Object.keys(c.reactions).length > 0">
                          <div v-for="(count, emoji) in c.reactions" :key="emoji" class="flex items-center gap-1.5 bg-[#1E2025] border border-[#27272A] rounded-full px-2.5 py-0.5 cursor-pointer hover:bg-gray-700 transition-colors" @click="addReaction(c, emoji)">
                             <span class="text-sm mt-px">{{ emoji }}</span> <span class="text-xs text-blue-400 font-medium">{{ count }}</span>
                          </div>
                       </div>
                    </div>
                  </div>
               </div>
            </div>

            <div class="comment-box">
               <p class="text-[13px] font-semibold mb-2 text-gray-400">Add comment</p>
               <div class="editor-wrap">
                  <textarea class="c-input" rows="2" v-model="newComment" placeholder="Click to add comment..."></textarea>
                  <div class="c-toolbar">
                     <div class="ct-left">
                       <i class="fa-solid fa-bold icon-hover"></i> <i class="fa-solid fa-italic icon-hover"></i> <i class="fa-solid fa-underline icon-hover"></i> <i class="fa-solid fa-strikethrough icon-hover"></i>
                       <div class="w-[1px] h-4 bg-gray-700 mx-1"></div>
                       <i class="fa-solid fa-list-ul icon-hover"></i> <i class="fa-solid fa-list-ol icon-hover"></i> <i class="fa-solid fa-check-double icon-hover"></i>
                       <div class="w-[1px] h-4 bg-gray-700 mx-1"></div>
                       <i class="fa-regular fa-image icon-hover" @click="triggerFileUpload"></i> <i class="fa-solid fa-paperclip icon-hover" @click="triggerFileUpload"></i>
                     </div>
                     <button class="c-submit" :class="{'bg-[#3b82f6] text-white cursor-pointer border-transparent': newComment.trim().length > 0}" :disabled="!newComment.trim() && pendingAttachments.length === 0" @click="submitComment">Comment</button>
                  </div>
               </div>
            </div>
         </div>
      </div>
      
    </div>
  </transition>
</template>


<script setup>
import { ref, watch, computed } from 'vue';
import { ElMessage, ElNotification, ElMessageBox } from 'element-plus';
import axiosClient from '@/api/axiosClient';
import { useActivityStore } from '@/store/useActivityStore';

const actStore = useActivityStore();

const props = defineProps({
  selectedTask: { type: Object, default: null },
  projectId: { type: [String, Number], required: true },
  projectMembers: { type: Array, default: () => [] },
  currentUser: { type: Object, default: () => ({}) }
});

const emit = defineEmits(['updateTask', 'close', 'open-task', 'create-subtask', 'refresh-tasks']);

const showTaskModal = ref(true);

const discardNewTask = () => {
    if (props.selectedTask) {
        props.selectedTask.title = '';
        props.selectedTask.description = '';
        props.selectedTask.statusName = 'Todo';
        props.selectedTask.priority = 3;
        props.selectedTask.assigneeId = null;
        props.selectedTask.labelIds = [];
        props.selectedTask.plannedStartDate = null;
        props.selectedTask.dueDate = null;
        props.selectedTask.sprintId = null;
        props.selectedTask.moduleId = null;
        props.selectedTask.parentId = null;
    }
};

// ====================== CREATE TASK POPOVER REFS ======================
const createMore = ref(false);
const assigneeSearch = ref('');
const labelSearch = ref('');
const cycleSearch = ref('');
const moduleSearch = ref('');
const parentSearch = ref('');

const projectCycles = ref([]);
const projectModules = ref([]);

const filteredMembers = computed(() => {
    if (!assigneeSearch.value) return props.projectMembers;
    return props.projectMembers.filter(m => 
        (m.fullName || m.userName || '').toLowerCase().includes(assigneeSearch.value.toLowerCase())
    );
});

const filteredLabels = computed(() => {
    if (!labelSearch.value) return projectLabels.value;
    return projectLabels.value.filter(l => l.name?.toLowerCase().includes(labelSearch.value.toLowerCase()));
});

const filteredCycles = computed(() => {
    if (!cycleSearch.value) return projectCycles.value;
    return projectCycles.value.filter(c => c.name?.toLowerCase().includes(cycleSearch.value.toLowerCase()));
});

const filteredModules = computed(() => {
    if (!moduleSearch.value) return projectModules.value;
    return projectModules.value.filter(m => m.name?.toLowerCase().includes(moduleSearch.value.toLowerCase()));
});

const filteredParents = computed(() => {
    let tasks = cachedProjectTasks.value.filter(t => t.id !== props.selectedTask?.id);
    if (!parentSearch.value) return tasks;
    return tasks.filter(t => t.title?.toLowerCase().includes(parentSearch.value.toLowerCase()) || t.sequenceId?.toLowerCase().includes(parentSearch.value.toLowerCase()));
});

const getPrioLabel = (p) => {
    if (p===1) return 'Urgent';
    if (p===2) return 'High';
    if (p===3) return 'Medium';
    if (p===4) return 'Low';
    return 'None';
};

const getPrioIcon = (p) => {
    if (p===1) return 'fa-solid fa-angles-up text-red-500';
    if (p===2) return 'fa-solid fa-chevron-up text-yellow-500';
    if (p===3) return 'fa-solid fa-minus text-blue-500';
    if (p===4) return 'fa-solid fa-arrow-down text-gray-400';
    return 'fa-solid fa-ban text-gray-500';
};

const getStatusIcon = (s) => {
    const st = (s||'').toUpperCase();
    if(st.includes('DONE')) return 'fa-regular fa-circle-check text-green-500';
    if(st.includes('PROGRESS')) return 'fa-solid fa-circle-half-stroke text-yellow-500';
    if(st.includes('REVIEW')) return 'fa-regular fa-circle-play text-blue-500';
    if(st.includes('TODO')) return 'fa-regular fa-circle text-gray-400';
    return 'fa-solid fa-circle-dashed text-gray-500';
};

const getAssigneeLabel = (id) => {
   if (!id) return 'Assignees';
   const user = props.projectMembers.find(m => m.id === id);
   return user ? user.name : 'Assignees';
};

const getCycleLabel = (id) => {
   if (!id) return 'Cycle';
   const c = projectCycles.value.find(c => c.id === id);
   return c ? c.name : 'Cycle';
};

const getModuleLabel = (id) => {
   if (!id) return 'Modules';
   const m = projectModules.value.find(m => m.id === id);
   return m ? m.name : 'Modules';
};

const toggleSelectedLabel = (lId) => {
    if (!props.selectedTask) return;
    if (!props.selectedTask.labelIds) props.selectedTask.labelIds = [];
    if (props.selectedTask.labelIds.includes(lId)) {
        props.selectedTask.labelIds = props.selectedTask.labelIds.filter(id => id !== lId);
    } else {
        props.selectedTask.labelIds.push(lId);
    }
};
const toggleSelectedLabelDetail = (lId) => {
    if (!props.selectedTask) return;
    if (!props.selectedTask.labelIds) props.selectedTask.labelIds = [];
    let newArr = [...props.selectedTask.labelIds];
    if (newArr.includes(lId)) {
        newArr = newArr.filter(id => id !== lId);
    } else {
        newArr.push(lId);
    }
    emit('updateTask', props.selectedTask, 'labelIds', newArr);
    props.selectedTask.labelIds = newArr;
};

const createLabel = async (name) => {
    try {
        const res = await axiosClient.post(`/projects/${props.projectId}/labels`, { name, color: '#3b82f6' });
        const newL = res.data?.data || res.data;
        if(newL) {
            projectLabels.value.push(newL);
            toggleSelectedLabel(newL.id);
            labelSearch.value = '';
        }
    } catch(e) {}
};
const createLabelDetail = async (name) => {
    try {
        const res = await axiosClient.post(`/projects/${props.projectId}/labels`, { name, color: '#3b82f6' });
        const newL = res.data?.data || res.data;
        if(newL) {
            projectLabels.value.push(newL);
            toggleSelectedLabelDetail(newL.id);
            labelSearch.value = '';
        }
    } catch(e) {}
};

// ====================== COMMENTS EXTENSION REFS ======================
const editingCommentId = ref(null);
const editingContent = ref('');
const emojiSearch = ref('');
const allEmojis = ["😀","😃","😄","😁","😆","😅","😂","🤣","😊","😇","🙂","🙃","😉","😌","😍","🥰","😘","😗","😙","😚","😋","😛","😝","😜","🤪","🤨","🧐","🤓","😎","🤩","🥳","😏","😒","😞","😔","😟","😕","🙁","☹️","😣","😖","😫","😩","🥺","😢","😭","😤","😠","😡","🤬","🤯","😳","🥵","🥶","😱","😨","😰","😥","😓","🤗","🤔","🤭","🤫","🤥","😶","😐","😑","😬","🙄","😯","😦","😧","😮","😲","🥱","😴","🤤","😪","😵","🤐","🥴","🤢","🤮","🤧","😷","🤒","🤕","🤑","🤠","😈","👿","👹","👺","🤡","💩","👻","💀","☠️","👽","👾","🤖","🎃","😺","😸","😹","😻","😼","😽","🙀","😿","😾","👍","👎","👌","✌️","🤞","🤟","🤘","🤙","👈","👉","👆","👇","☝️","✋","🤚","🖐","🖖","👋","👏","🙌","👐","🤲","🤝","🙏","✍️","💅","🤳","💪","🦾","🦵","🦿","🦶","👣","👂","🦻","👃","🧠","🦷","🦴","👀","👁","👅","👄","💋","🩸"];

const filteredEmojis = computed(() => {
    if (!emojiSearch.value) return allEmojis;
    return allEmojis;
});

const startEditingComment = (c) => {
    editingCommentId.value = c.id;
    editingContent.value = c.content;
};
const cancelEditingComment = () => {
    editingCommentId.value = null;
    editingContent.value = '';
};
const saveEditedComment = async (cId, cRef) => {
    try {
        await axiosClient.put(`/projects/${props.projectId}/WorkTasks/${props.selectedTask.id}/comments/${cId}`, {
            content: editingContent.value
        });
        cRef.content = editingContent.value;
        cRef.isEdited = true;
        cancelEditingComment();
        ElMessage.success("Đã cập nhật bình luận");
    } catch(e) { ElMessage.error("Lỗi khi sửa"); }
};
const deleteComment = async (cId) => {
    try {
        await axiosClient.delete(`/projects/${props.projectId}/WorkTasks/${props.selectedTask.id}/comments/${cId}`);
        comments.value = comments.value.filter(cm => cm.id !== cId);
        ElMessage.success("Đã xoá bình luận");
    } catch(e) { ElMessage.error("Lỗi xóa bình luận"); }
};
const copyCommentLink = (cId) => {
    const url = `${window.location.origin}/projects/${props.projectId}/work-tasks/${props.selectedTask.id}?comment=${cId}`;
    navigator.clipboard.writeText(url);
    ElMessage.success("Đã copy link bình luận");
};
const addReaction = (c, emoji) => {
    if(!c.reactions) c.reactions = {};
    if(!c.reactions[emoji]) c.reactions[emoji] = 0;
    c.reactions[emoji]++;
};

const fetchAdditionalProjectData = async () => {
    if (!props.projectId) return;
    try {
        const [cyclesRes, modulesRes, labelsRes, tasksRes] = await Promise.all([
             axiosClient.get(`/projects/${props.projectId}/sprints`).catch(()=>({data:{data:[]}})),
             axiosClient.get(`/projects/${props.projectId}/modules`).catch(()=>({data:{data:[]}})),
             axiosClient.get(`/projects/${props.projectId}/labels`).catch(()=>({data:{data:[]}})),
             axiosClient.get(`/projects/${props.projectId}/WorkTasks`).catch(()=>({data:{data:[]}}))
        ]);
        projectCycles.value = cyclesRes.data?.data || [];
        projectModules.value = modulesRes.data?.data || [];
        projectLabels.value = labelsRes.data?.data || [];
        cachedProjectTasks.value = tasksRes.data?.data || [];
    } catch(e) {}
};
// ======================================================================

watch(showTaskModal, (val) => {
  if (!val) emit('close');
});

const formatDate = (dateStr) => {
  if (!dateStr) return '';
  const d = new Date(dateStr);
  return d.toLocaleDateString('vi-VN');
};

const handleHeaderCommand = (command) => {
    if (command === 'archive') {
        archiveTask();
    } else if (command === 'archive_soon') {
        archiveSoonTask();
    }
};

const toggleSubscription = async () => {
    if (!props.selectedTask?.id) return;
    try {
        const res = await axiosClient.post(`/tasks/${props.selectedTask.id}/subscribe`);
        
        // Backend trả về: { statusCode: 200, data: { isSubscribed: true/false } }
        const newState = (res.data?.data && typeof res.data.data.isSubscribed !== 'undefined')
            ? res.data.data.isSubscribed
            : !props.selectedTask.isSubscribed;

        const msg = newState ? 'Đã đăng ký theo dõi' : 'Đã hủy đăng ký theo dõi';
        ElMessage.success(msg);
        
        // Emit để cha cập nhật task, không mutate trực tiếp props
        emit('updateTask', { ...props.selectedTask, isSubscribed: newState });
        
        actStore.logActivity(msg, `Task: ${props.selectedTask.title}`, 'fa-solid fa-bell');
    } catch (e) {
        console.error('Subscription error:', e.response?.data || e.message);
        const errorMsg = e.response?.data?.message || 'Lỗi khi thay đổi trạng thái theo dõi';
        ElMessage.error(errorMsg);
    }
};

const archiveSoonTask = async () => {
    try {
        await ElMessageBox.confirm('Tính năng Archive soon sẽ tự động lưu trữ công việc này sau 24 giờ nếu không có hoạt động mới. Bạn có muốn kích hoạt?', 'Xác nhận Archive Soon', {
            confirmButtonText: 'Kích hoạt',
            cancelButtonText: 'Hủy',
            type: 'info'
        });
        ElMessage.info('Archive soon đang được chuẩn bị - Đã ghi nhận yêu cầu của bạn.');
        actStore.logActivity('Requested Archive Soon', `Task: ${props.selectedTask.title}`, 'fa-solid fa-hourglass-start');
    } catch(e) {}
};

const archiveTask = async () => {
    try {
        await ElMessageBox.confirm('Bạn có chắc muốn lưu trữ công việc này? Nó sẽ bị ẩn khỏi danh sách hiện tại.', 'Xác nhận', {
            confirmButtonText: 'Lưu trữ',
            cancelButtonText: 'Hủy',
            type: 'warning'
        });
        await axiosClient.put(`/projects/${props.projectId}/WorkTasks/${props.selectedTask.id}/archive`);
        ElMessage.success('Đã lưu trữ công việc thành công');
        actStore.logActivity('Archived work item', `Task: ${props.selectedTask.title}`, 'fa-regular fa-box-archive');
        emit('refresh-tasks');
        showTaskModal.value = false;
    } catch (e) {
        if (e !== 'cancel') ElMessage.error('Lỗi khi lưu trữ công việc');
    }
};

const updateTaskField = (task, field, value) => {
  emit('updateTask', task, field, value);
};

const handleDescriptionBlur = () => {
  if (!props.selectedTask?.isNew) {
    updateTaskField(props.selectedTask, 'description', props.selectedTask.description);
  }
};

const openTaskDetail = (task) => emit('open-task', task);
const createSubtask = (task) => emit('create-subtask', task);

const submitNewTask = async () => {
    if(!props.selectedTask?.title) {
        ElMessage.warning('Vui lòng nhập tiêu đề');
        return;
    }
    try {
        await axiosClient.post(`/projects/${props.projectId}/WorkTasks`, {
            title: props.selectedTask.title,
            description: props.selectedTask.description,
            statusName: props.selectedTask.statusName || 'Todo',
            priority: props.selectedTask.priority !== undefined ? props.selectedTask.priority : 0,
            assignedUserId: props.selectedTask.assigneeId,
            plannedStartDate: props.selectedTask.plannedStartDate,
            dueDate: props.selectedTask.dueDate,
            sprintId: props.selectedTask.sprintId,
            moduleId: props.selectedTask.moduleId,
            parentTaskId: props.selectedTask.parentId,
            labelIds: props.selectedTask.labelIds || []
        });
        ElMessage.success('Đã tạo thành công');
        emit('refresh-tasks');
        if (!createMore.value) {
            emit('close');
        } else {
            props.selectedTask.title = '';
            props.selectedTask.description = '';
        }
    } catch(e) {
        ElMessage.error('Lỗi khi tạo công việc');
    }
};

// === Subtasks Logic ===
const subtasksList = ref([]);
const isCreatingSubtask = ref(false);
const newSubtaskTitle = ref('');
import { nextTick } from 'vue';
const subtaskInputRef = ref(null);

async function fetchSubtasks() {
    try {
        const res = await axiosClient.get(`/projects/${props.projectId}/WorkTasks/${props.selectedTask.id}/subtasks`);
        subtasksList.value = res.data?.data || [];
    } catch(e) {}
}

const startCreateSubtask = () => {
    isCreatingSubtask.value = true;
    newSubtaskTitle.value = '';
    nextTick(() => {
        if(subtaskInputRef.value) subtaskInputRef.value.focus();
    });
};

const submitSubtask = async () => {
    if(!newSubtaskTitle.value.trim()) {
        isCreatingSubtask.value = false;
        return;
    }
    try {
        await axiosClient.post(`/projects/${props.projectId}/WorkTasks/${props.selectedTask.id}/subtasks`, {
            title: newSubtaskTitle.value.trim(),
            statusName: 'To Do',
            taskTypeId: props.selectedTask.taskTypeId,
            priority: props.selectedTask.priority
        });
        isCreatingSubtask.value = false;
        newSubtaskTitle.value = '';
        fetchSubtasks();
        emit('refresh-tasks');
    } catch(e) {
        ElMessage.error(e.response?.data?.message || 'Lỗi khi tạo subtask');
    }
};

// === AI Brain Logic ===
const aiPrompt = ref('');
const isBrainTyping = ref(false);

const askBrain = async () => {
    if (!aiPrompt.value.trim() || !props.selectedTask) return;
    isBrainTyping.value = true;
    try {
        const res = await axiosClient.post('/ai/generate-description', { prompt: aiPrompt.value });
        const newDesc = res.data.data;
        props.selectedTask.description = newDesc; // optimistic update
        updateTaskField(props.selectedTask, 'description', newDesc);
        ElMessage.success('Brain đã tạo mô tả thành công!');
        aiPrompt.value = '';
    } catch (e) {
        ElMessage.error('Có lỗi xảy ra khi tạo mô tả bằng AI.');
    } finally {
        isBrainTyping.value = false;
    }
};

// Comments logic
const comments = ref([]);
const replyingToCommentId = ref(null);
const newComment = ref('');
const pendingAttachments = ref([]);
const commentFileInput = ref(null);

async function fetchComments() {
  if (!props.selectedTask || !props.selectedTask.id) return;
  try {
    const res = await axiosClient.get(`/projects/${props.projectId}/WorkTasks/${props.selectedTask.id}/comments`);
    comments.value = res.data?.data || [];
  } catch (err) { }
}


// === Labels Logic ===
const projectLabels = ref([]);
const assignedLabels = ref([]);

async function fetchLabels() {
    try {
        const res = await axiosClient.get(`/projects/${props.projectId}/labels`);
        projectLabels.value = res.data?.data || [];
    } catch {}
}

async function fetchAssignedLabels() {
    if(props.selectedTask?.issueLabels) {
        assignedLabels.value = props.selectedTask.issueLabels; 
    } else {
        assignedLabels.value = props.selectedTask?.labels || [];
    }
}

const assignLabel = async (labelId) => {
    try {
        await axiosClient.post(`/projects/${props.projectId}/WorkTasks/${props.selectedTask.id}/labels`, { labelId });
        const lbl = projectLabels.value.find(l => l.id === labelId);
        if (lbl && !assignedLabels.value.find(a => a.labelId === labelId)) {
            assignedLabels.value.push({ labelId: lbl.id, name: lbl.name, colorCode: lbl.colorCode });
        }
        ElMessage.success("Gắn nhãn thành công");
    } catch (e) {
        ElMessage.error(e.response?.data?.message || "Lỗi khi gắn nhãn");
    }
};

const removeLabel = async (labelId) => {
    try {
        await axiosClient.delete(`/projects/${props.projectId}/WorkTasks/${props.selectedTask.id}/labels/${labelId}`);
        assignedLabels.value = assignedLabels.value.filter(a => a.labelId !== labelId);
    } catch (e) {
        ElMessage.error(e.response?.data?.message || "Lỗi khi gỡ nhãn");
    }
};

// === Dependencies Logic ===
const taskDependencies = ref([]);
const cachedProjectTasks = ref([]);

async function fetchDependencies() {
    try {
        const res = await axiosClient.get(`/projects/${props.projectId}/WorkTasks/${props.selectedTask.id}/dependencies`);
        const items = res.data?.data || [];
        taskDependencies.value = items.map(d => {
            const isPredecessor = d.predecessorTaskId === props.selectedTask.id;
            let relType = "";
            if(d.dependencyType === 1) { relType = isPredecessor ? "blocks" : "blocked_by"; }
            else if(d.dependencyType === 2) { relType = "relates_to"; }
            else if(d.dependencyType === 3) { relType = "duplicate"; }
            
            return {
                targetId: isPredecessor ? d.successorTaskId : d.predecessorTaskId,
                targetSequenceId: isPredecessor ? d.successorSequenceId : d.predecessorSequenceId,
                targetTitle: isPredecessor ? d.successorTitle : d.predecessorTitle,
                relationType: relType
            };
        });
    } catch {}
}

const fetchProjectTasks = async () => {
    try {
        const res = await axiosClient.get(`/projects/${props.projectId}/WorkTasks`);
        cachedProjectTasks.value = res.data?.data || [];
    } catch {}
};

const depsDropdownTasks = computed(() => {
    return cachedProjectTasks.value.filter(t => t.id !== props.selectedTask.id);
});

const getRelationText = (type) => {
    if(type === 'blocks') return 'Blocks (chặn)';
    if(type === 'blocked_by') return 'Bị chặn bởi';
    if(type === 'relates_to') return 'Liên quan đến';
    if(type === 'duplicate') return 'Trùng lặp';
    return type;
};

const addDependency = async (command) => {
    try {
        await axiosClient.post(`/projects/${props.projectId}/WorkTasks/${props.selectedTask.id}/dependencies`, {
            relatedTaskId: command.relatedId,
            relationType: command.type
        });
        await fetchDependencies();
        ElMessage.success("Đã thêm quan hệ!");
    } catch (e) {
        ElMessage.error(e.response?.data?.message || "Lỗi khi thêm quan hệ");
    }
};

const removeDependency = async (targetId) => {
  try {
     await axiosClient.delete(`/projects/${props.projectId}/WorkTasks/${props.selectedTask.id}/dependencies/${targetId}`);
     await fetchDependencies();
  } catch (e) {
     ElMessage.error("Lỗi khi xóa quan hệ");
  }
};

const topLevelComments = computed(() => {
  if (!comments.value) return [];
  const map = {};
  comments.value.forEach(c => { c.childComments = []; map[c.id] = c; });
  const roots = [];
  comments.value.forEach(c => {
    if (c.parentCommentId && map[c.parentCommentId]) {
      map[c.parentCommentId].childComments.push(c);
    } else {
      roots.push(c);
    }
  });
  return roots;
});

const handleFileChange = (e) => {
    if (e.target.files) {
        pendingAttachments.value = [...pendingAttachments.value, ...Array.from(e.target.files)];
    }
};

const triggerFileUpload = () => { if (commentFileInput.value) commentFileInput.value.click(); };
const startReply = (c) => { replyingToCommentId.value = c.id; newComment.value = ''; pendingAttachments.value = []; };
const cancelReply = () => { replyingToCommentId.value = null; newComment.value = ''; pendingAttachments.value = []; };

const submitComment = async () => {
    if (!newComment.value.trim() && pendingAttachments.value.length === 0) return;
    try {
        const formData = new FormData();
        formData.append('content', newComment.value.trim() || '');
        if (replyingToCommentId.value) {
            formData.append('parentCommentId', replyingToCommentId.value);
        }
        pendingAttachments.value.forEach(file => {
            formData.append('files', file);
        });

        await axiosClient.post(`/projects/${props.projectId}/WorkTasks/${props.selectedTask.id}/comments`, formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        });
        
        newComment.value = '';
        pendingAttachments.value = [];
        replyingToCommentId.value = null;
        if (commentFileInput.value) commentFileInput.value.value = '';
        fetchComments();
    } catch(e) {
        ElMessage.error(e.response?.data?.message || "Lỗi khi gửi bình luận");
    }
};

watch(() => props.selectedTask, (newTask) => {
  if (newTask) {
    // Only fetch data for EXISTING tasks (have an id)
    // New tasks (isNew: true) have no id, so API calls would crash
    fetchAdditionalProjectData();

    if (newTask.id && !newTask.isNew) {
      fetchComments();
      fetchDependencies();
      fetchAssignedLabels();
      fetchSubtasks();
    } else {
      // Reset data for new tasks
      comments.value = [];
      taskDependencies.value = [];
      assignedLabels.value = [];
      subtasksList.value = [];
    }
    replyingToCommentId.value = null;
    newComment.value = '';
    pendingAttachments.value = [];
    showTaskModal.value = true;
  }
}, { immediate: true });
</script>

<style scoped>
/* GENERAL PANEL STYLING */
.task-modal-overlay {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0, 0, 0, 0.6);
  z-index: 1500;
  display: flex;
  justify-content: center;
  align-items: center;
  font-family: 'Inter', sans-serif;
  color: #E5E7EB;
}

/* UTILITIES */
.flex-wrapper { display: flex; align-items: center; }
.flex-center { display: flex; align-items: center; }
.flex-between { display: flex; justify-content: space-between; align-items: center; }
.gap-2 { gap: 8px; } .gap-3 { gap: 12px; } .gap-4 { gap: 16px; } .gap-5 { gap: 20px; } .gap-8 { gap: 32px; }
.text-muted { color: #A1A1AA; }
.text-primary { color: #38BDF8; }
.bg-dark { background: #16181D; }
.bg-dark-2 { background: #111111; }
.border-gray { border-color: #27272A; }
.icon-btn { cursor: pointer; transition: color 0.2s; } .icon-btn:hover { color: #E5E7EB; }
.icon-hover { cursor: pointer; padding: 4px; border-radius: 4px; } .icon-hover:hover { background: #27272A; }

/* CENTRERED CREATION MODAL */
.create-centered-modal {
  width: 780px;
  background: #101010;
  border: 1px solid #27272A;
  border-radius: 8px;
  padding: 24px;
  display: flex;
  flex-direction: column;
  box-shadow: 0 24px 48px rgba(0,0,0,0.8);
  font-family: inherit;
}

.cm-title {
  font-size: 18px;
  font-weight: 600;
  color: #E4E4E7;
  margin: 0 0 16px 0;
}

.cm-badge-row {
  margin-bottom: 20px;
}

.cm-badge {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 4px 10px;
  border: 1px solid #27272A;
  border-radius: 4px;
  font-size: 13px;
  font-weight: 500;
  color: #E4E4E7;
  background-color: transparent;
}

.cm-form-group {
  display: flex;
  flex-direction: column;
  gap: 16px;
  margin-bottom: 20px;
}

.cm-inputbox {
  background: #18191B;
  border: 1px solid #27272A;
  border-radius: 6px;
  padding: 12px 16px;
  font-size: 15px;
  color: #E4E4E7;
  outline: none;
  transition: border-color 0.2s;
  width: 100%;
}
.cm-inputbox::placeholder { color: #52525B; font-weight: 500; }
.cm-inputbox:focus { border-color: #38BDF8; }

.cm-textareabox {
  background: #18191B;
  border: 1px solid #27272A;
  border-radius: 6px;
  padding: 16px;
  font-size: 15px;
  color: #E4E4E7;
  outline: none;
  resize: none;
  height: 180px;
  width: 100%;
  transition: border-color 0.2s;
}
.cm-textareabox::placeholder { color: #52525B; font-weight: 500; }
.cm-textareabox:focus { border-color: #38BDF8; }

.cm-toolbar-row {
  border-top: 1px solid #27272A;
  padding-top: 16px;
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  margin-bottom: 24px;
}

.cm-toolbar-row .t-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  background: transparent;
  border: 1px solid #27272A;
  color: #E4E4E7;
  border-radius: 4px;
  padding: 6px 12px;
  font-size: 12px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}
.cm-toolbar-row .t-btn:hover { background: #27272A; border-color: #3F3F46; }

.cm-footer-row {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 16px;
}

.cm-t-more {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-right: 8px;
}
.cm-t-more span {
  font-size: 13px;
  color: #A1A1AA;
}

.btn-discard { 
  background: transparent; 
  border: 1px solid #27272A; 
  padding: 8px 16px; 
  border-radius: 6px; 
  font-size: 14px;
  font-weight: 500;
  color: #E4E4E7; 
  cursor: pointer; 
  transition: background 0.2s;
}
.btn-discard:hover { background: #1E1F21; }

.btn-save { 
  background: #0EA5E9; 
  border: none; 
  padding: 8px 24px; 
  border-radius: 6px; 
  color: white; 
  font-size: 14px;
  font-weight: 500; 
  cursor: pointer; 
  transition: background 0.2s; 
}
.btn-save:hover { background: #0284C7; }

/* SLIDE IN SIDE PANEL (TASK DETAIL) */
.task-side-panel {
  position: absolute;
  top: 0; right: 0; bottom: 0;
  width: 900px;
  max-width: 95vw;
  background: #151515;
  border-left: 1px solid #27272A;
  display: flex;
  flex-direction: column;
  box-shadow: -10px 0 30px rgba(0,0,0,0.5);
  animation: slideRight 0.3s ease-out;
  font-family: inherit;
}
@keyframes slideRight { from { transform: translateX(100%); } to { transform: translateX(0); } }

.sp-header {
  height: 56px;
  border-bottom: 1px solid #27272A;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 24px;
  flex-shrink: 0;
}
.sph-left, .sph-right {
  display: flex;
  align-items: center;
  gap: 16px;
  color: #A1A1AA;
}
.unsub-btn {
  background: transparent;
  border: 1px solid #3F3F46;
  border-radius: 4px;
  padding: 6px 12px;
  color: #E4E4E7;
  font-size: 13px;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}
.unsub-btn:hover { background: #27272A; }

.sp-body {
  flex: 1;
  overflow-y: auto;
  padding: 32px 32px 80px 32px;
}
.sp-body::-webkit-scrollbar { width: 6px; }
.sp-body::-webkit-scrollbar-thumb { background: #3F3F46; border-radius: 4px; }

.sp-breadcrumb {
  font-size: 14px;
  font-weight: 500;
  color: #A1A1AA;
  margin-bottom: 12px;
}

.sp-title {
  font-size: 28px;
  font-weight: 700;
  color: #E4E4E7;
  margin: 0 0 12px 0;
  outline: none;
}
.sp-title:focus { border-bottom: 1px dashed #38BDF8; }

.sp-desc {
  font-size: 16px;
  color: #A1A1AA;
  margin: 0 0 40px 0;
  outline: none;
}

.sp-sub-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 32px;
  color: #A1A1AA;
}
.sp-edit-info {
  font-size: 13px;
  display: flex;
  align-items: center;
  gap: 6px;
}

.sp-toolbar {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
  margin-bottom: 40px;
}
.sp-toolbar .s-btn {
  background: transparent;
  border: 1px solid #27272A;
  border-radius: 6px;
  padding: 8px 14px;
  color: #E4E4E7;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
  transition: background 0.2s;
}
.sp-toolbar .s-btn:hover { background: #1E1F21; border-color: #3F3F46; }

.sp-section-title {
  font-size: 16px;
  font-weight: 600;
  color: #E4E4E7;
  margin: 0 0 24px 0;
}

.props-grid {
  display: flex;
  flex-direction: column;
  gap: 20px;
}
.p-row {
  display: flex;
  align-items: center;
  font-size: 14px;
}
.p-label {
  width: 140px;
  color: #A1A1AA;
  display: flex;
  align-items: center;
  gap: 10px;
}
.p-val {
  color: #E4E4E7;
  flex: 1;
}
.muted-val {
  color: #71717A;
}

.btn-add-label {
  background: #27272A;
  border: none;
  border-radius: 4px;
  padding: 4px 10px;
  color: #A1A1AA;
  font-size: 12px;
  cursor: pointer;
}

.icon-filter-btn {
  background: #27272A;
  border: none;
  color: #A1A1AA;
  padding: 6px 10px;
  border-radius: 4px;
  cursor: pointer;
}

.activity-feed {
  display: flex;
  flex-direction: column;
  gap: 24px;
  margin-bottom: 24px;
  position: relative;
  padding-left: 14px;
  border-left: 2px solid #27272A;
  margin-left: 10px;
}
.feed-item {
  position: relative;
  display: flex;
  gap: 16px;
}
.feed-icon {
  background: #27272A;
  width: 24px;
  height: 24px;
  border-radius: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  color: #A1A1AA;
  position: absolute;
  left: -27px;
  top: 0;
}
.feed-avatar {
  background: #27272A;
  width: 28px;
  height: 28px;
  border-radius: 50%;
  border: 2px solid #151515;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  color: #A1A1AA;
  position: absolute;
  left: -29px;
  top: -2px;
}
.feed-text {
  font-size: 14px;
  color: #E4E4E7;
}

.comment-box {
  background: transparent;
  margin-top: 24px;
}
.editor-wrap {
  border: 1px solid #27272A;
  border-radius: 6px;
  overflow: hidden;
  background: #111111;
}
.c-input {
  width: 100%;
  background: transparent;
  border: none;
  padding: 16px;
  color: #E4E4E7;
  outline: none;
  resize: none;
  font-family: inherit;
  font-size: 14px;
  height: 100px;
}
.c-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 16px;
  background: transparent;
  border-top: 1px solid #27272A;
}
.ct-left {
  display: flex;
  gap: 12px;
  color: #71717A;
  font-size: 13px;
}
.icon-hover:hover { color: #A1A1AA; cursor: pointer; }
.c-submit {
  background: #27272A;
  color: #A1A1AA;
  border: none;
  padding: 6px 16px;
  border-radius: 4px;
  font-size: 13px;
  font-weight: 500;
  cursor: not-allowed;
}

.dark-dropdown { background: #1E2025 !important; border: 1px solid #333 !important; }
.dark-dropdown .el-dropdown-menu__item { color: #E4E4E7 !important; }
.dark-dropdown .el-dropdown-menu__item:hover { background: #27272A !important; }

/* Plane-like custom popover styles */
:global(.plane-popover) {
  background: #18191B !important;
  border: 1px solid #27272A !important;
  padding: 0 !important;
  border-radius: 8px !important;
  box-shadow: 0 10px 25px rgba(0,0,0,0.5) !important;
}

:global(.plane-popover .popover-content) {
  display: flex;
  flex-direction: column;
}

:global(.plane-popover .popover-search) {
  background: transparent;
  border: none;
  border-bottom: 1px solid #27272A;
  color: #E4E4E7;
  padding: 12px 14px;
  font-size: 13px;
  outline: none;
  width: 100%;
}
:global(.plane-popover .popover-search::placeholder) {
  color: #71717A;
}

:global(.plane-popover .popover-list) {
  max-height: 250px;
  overflow-y: auto;
  padding: 6px;
}
:global(.plane-popover .popover-list::-webkit-scrollbar) {
  width: 6px;
}
:global(.plane-popover .popover-list::-webkit-scrollbar-thumb) {
  background: #27272A;
  border-radius: 4px;
}

:global(.plane-popover .popover-item) {
  display: flex;
  align-items: center;
  padding: 8px 10px;
  border-radius: 4px;
  cursor: pointer;
  color: #D4D4D8;
  font-size: 13px;
  transition: background 0.15s;
}
:global(.plane-popover .popover-item:hover) {
  background: #27272A;
  color: #FFFFFF;
}

.t-btn-date:deep(.el-input__wrapper) {
  background-color: transparent !important;
  box-shadow: none !important;
  padding: 0 !important;
}

.t-btn-date:deep(.el-input__inner) {
  color: #D4D4D8 !important;
  font-size: 12px;
  font-weight: 500;
  cursor: pointer;
}
</style>
