<script setup>
import { ref, onMounted, watch, onBeforeUnmount } from 'vue'
import axiosClient from '@/api/axiosClient'
import { ElNotification, ElMessageBox } from 'element-plus'
import { useEditor, EditorContent } from '@tiptap/vue-3'
import StarterKit from '@tiptap/starter-kit'
import Image from '@tiptap/extension-image'
import Underline from '@tiptap/extension-underline'
import TextAlign from '@tiptap/extension-text-align'
import { computed } from 'vue'
import dayjs from 'dayjs'

const props = defineProps({
  projectId: { type: String, required: true }
})

const pages = ref([])
const activePage = ref(null)
const loading = ref(false)
const saving = ref(false)
const activeTab = ref('Public')

const sortBy = ref('date_modified')
const sortOrder = ref('desc')
const filterSearch = ref('')
const filterFavorites = ref(false)
const filterDateExpanded = ref(true)
const filterByExpanded = ref(true)

const filteredPages = computed(() => {
   if (activeTab.value === 'Archived') return pages.value.filter(p => p.isArchived)
   if (activeTab.value === 'Private') return pages.value.filter(p => p.isPrivate && !p.isArchived)
   return pages.value.filter(p => !p.isPrivate && !p.isArchived)
})

let saveTimeout = null

onMounted(() => {
  fetchPages()
})

async function fetchPages() {
  loading.value = true
  try {
    const res = await axiosClient.get(`/projects/${props.projectId}/pages`)
    // Note: Backend might need update to return archived pages for the Archived tab
    pages.value = res.data?.data || []
  } catch (error) {
    console.error(error)
  } finally {
    loading.value = false
  }
}

async function createPage() {
  try {
    const res = await axiosClient.post(`/projects/${props.projectId}/pages`, {
      title: 'Trang mới',
      content: ''
    })
    await fetchPages()
    openPage(res.data.data.id)
  } catch (error) {
    ElNotification({ title: 'Lỗi', message: 'Không thể tạo trang', type: 'error' })
  }
}

async function openPage(pageId) {
  loading.value = true
  try {
    const res = await axiosClient.get(`/projects/${props.projectId}/pages/${pageId}`)
    activePage.value = res.data?.data
    if (editor.value) {
      editor.value.commands.setContent(activePage.value.content || '', false)
    }
  } catch (error) {
    ElNotification({ title: 'Lỗi', message: 'Không thể mở trang', type: 'error' })
  } finally {
    loading.value = false
  }
}

// === ACTIONS ===
async function togglePrivacy(page) {
  try {
    // Mocking privacy toggle if backend is not ready
    page.isPrivate = !page.isPrivate
    await axiosClient.put(`/projects/${props.projectId}/pages/${page.id}`, {
        isPrivate: page.isPrivate
    })
    ElNotification({ title: 'Thành công', message: page.isPrivate ? 'Đã chuyển sang riêng tư' : 'Đã chuyển sang công khai', type: 'success' })
  } catch (error) {
    console.error(error)
  }
}

async function archivePage(pageId) {
  try {
    await ElMessageBox.confirm('Bạn có chắc muốn lưu trữ trang này?', 'Xác nhận', { 
        confirmButtonText: 'Lưu trữ',
        cancelButtonText: 'Hủy',
        type: 'warning' 
    })
    await axiosClient.put(`/projects/${props.projectId}/pages/${pageId}/archive`)
    if (activePage.value?.id === pageId) activePage.value = null
    fetchPages()
    ElNotification({ title: 'Thành công', message: 'Đã lưu trữ trang', type: 'success' })
  } catch (e) {
      if (e !== 'cancel') console.error(e)
  }
}

async function restorePage(pageId) {
  try {
    await axiosClient.put(`/projects/${props.projectId}/pages/${pageId}/archive`)
    fetchPages()
    ElNotification({ title: 'Thành công', message: 'Đã khôi phục trang', type: 'success' })
  } catch (error) {
    console.error(error)
  }
}

async function deletePage(pageId) {
  try {
    await ElMessageBox.confirm('Hành động này không thể hoàn tác. Bạn có chắc chắn muốn xóa vĩnh viễn trang này?', 'Cảnh báo', {
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Hủy',
        type: 'error'
    })
    await axiosClient.delete(`/projects/${props.projectId}/pages/${pageId}`)
    fetchPages()
    ElNotification({ title: 'Thành công', message: 'Đã xóa trang', type: 'success' })
  } catch (e) {
      if (e !== 'cancel') console.error(e)
  }
}

function formatDate(date) {
    if (!date) return 'N/A'
    return dayjs(date).format('MMM D, YYYY [at] h:mm A')
}

// === TIPTAP EDITOR ===
const editor = useEditor({
  content: '',
  extensions: [StarterKit, Image, Underline, TextAlign.configure({ types: ['heading', 'paragraph'] })],
  editorProps: {
    attributes: {
      class: 'prose prose-sm sm:prose lg:prose-lg xl:prose-2xl mx-auto focus:outline-none text-gray-300'
    }
  },
  onUpdate: ({ editor }) => {
    if (activePage.value) {
      activePage.value.content = editor.getHTML()
      handleContentInput()
    }
  }
})

onBeforeUnmount(() => {
  if (editor.value) {
    editor.value.destroy()
  }
})

function handleContentInput() {
  if (saveTimeout) clearTimeout(saveTimeout)
  saving.value = true
  saveTimeout = setTimeout(() => {
    savePage()
  }, 1000)
}

async function savePage() {
  if (!activePage.value) return
  try {
    await axiosClient.put(`/projects/${props.projectId}/pages/${activePage.value.id}`, {
      title: activePage.value.title,
      content: activePage.value.content
    })
    const idx = pages.value.findIndex(p => p.id === activePage.value.id)
    if (idx !== -1) pages.value[idx].title = activePage.value.title
  } catch (error) {
    ElNotification({ title: 'Lỗi', message: 'Lỗi khi lưu trang', type: 'error' })
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <div class="plane-pages-wrapper">
    <!-- List View -->
    <div v-if="!activePage" class="pages-list-view">
      
      <!-- Header -->
      <div class="pages-header">
         <div class="ph-left">
            <i class="fa-solid fa-certificate" style="color: #F59E0B"></i>
            <span style="margin-left: 8px">CYBWF</span>
            <i class="fa-solid fa-chevron-right" style="font-size: 9px; margin: 0 8px"></i>
            <i class="fa-regular fa-file-lines" style="color: #A1A1AA"></i>
            <span style="color: #E4E4E7; margin-left: 8px">Pages</span>
         </div>
         <div class="ph-right">
            <button class="primary-action" @click="createPage">Add page</button>
         </div>
      </div>

      <!-- Navigation Tabs -->
      <div class="pages-nav">
         <div class="nav-tab" :class="{ 'active': activeTab === 'Public' }" @click="activeTab = 'Public'">Public</div>
         <div class="nav-tab" :class="{ 'active': activeTab === 'Private' }" @click="activeTab = 'Private'">Private</div>
         <div class="nav-tab" :class="{ 'active': activeTab === 'Archived' }" @click="activeTab = 'Archived'">Archived</div>
      </div>

      <!-- Toolbar -->
      <div class="pages-toolbar">
         <div class="pt-left"></div>
         <div class="pt-right">
            <button class="icon-toggle"><i class="fa-solid fa-magnifying-glass"></i></button>
            
            <el-popover placement="bottom-end" trigger="click" :width="220" popper-class="custom-dark-popover sort-popover" :offset="8" :show-arrow="false">
              <template #reference>
                <button class="filter-btn outlined" style="padding-left: 10px; padding-right: 10px;">
                  <i class="fa-solid fa-arrow-down-short-wide" style="font-size: 12px; margin-right: 2px;"></i> 
                  {{ sortBy === 'name' ? 'Name' : (sortBy === 'date_created' ? 'Date created' : 'Date modified') }}
                </button>
              </template>
              <div class="popover-menu-list">
                <div class="pm-item" @click="sortBy = 'name'">
                  <span class="pm-label">Name</span>
                  <i v-if="sortBy === 'name'" class="fa-solid fa-check pm-check"></i>
                </div>
                <div class="pm-item" @click="sortBy = 'date_created'">
                  <span class="pm-label">Date created</span>
                  <i v-if="sortBy === 'date_created'" class="fa-solid fa-check pm-check"></i>
                </div>
                <div class="pm-item" @click="sortBy = 'date_modified'">
                  <span class="pm-label">Date modified</span>
                  <i v-if="sortBy === 'date_modified'" class="fa-solid fa-check pm-check"></i>
                </div>
                
                <div class="pm-divider"></div>
                
                <div class="pm-item" @click="sortOrder = 'asc'">
                  <span class="pm-label">Ascending</span>
                  <i v-if="sortOrder === 'asc'" class="fa-solid fa-check pm-check"></i>
                </div>
                <div class="pm-item" @click="sortOrder = 'desc'">
                  <span class="pm-label">Descending</span>
                  <i v-if="sortOrder === 'desc'" class="fa-solid fa-check pm-check"></i>
                </div>
              </div>
            </el-popover>

            <el-popover placement="bottom-end" trigger="click" :width="280" popper-class="custom-dark-popover filter-popover" :offset="8" :show-arrow="false">
              <template #reference>
                <button class="filter-btn outlined"><i class="fa-solid fa-bars-staggered" style="font-size: 12px;"></i> Filters</button>
              </template>
              <div class="filter-menu-container">
                <div class="fm-search">
                  <i class="fa-solid fa-magnifying-glass fms-icon"></i>
                  <input v-model="filterSearch" type="text" placeholder="Search" class="fms-input" />
                </div>
                
                <div class="fm-section" style="margin-top: 12px">
                  <label class="fm-checkbox-row">
                    <input type="checkbox" v-model="filterFavorites" class="fm-checkbox" />
                    <span class="fm-checkbox-label">Favorites</span>
                  </label>
                </div>
                
                <div class="pm-divider"></div>
                
                <!-- Created date collapsible -->
                <div class="fm-collapsible">
                  <div class="fm-col-header" @click="filterDateExpanded = !filterDateExpanded">
                    <span class="fm-col-title">Created date</span>
                    <i class="fa-solid fa-chevron-up fm-col-icon" :class="{'rotate-180': !filterDateExpanded}"></i>
                  </div>
                  <div class="fm-col-content" v-show="filterDateExpanded">
                    <label class="fm-checkbox-row">
                      <input type="checkbox" class="fm-checkbox" />
                      <span class="fm-checkbox-label">1 week ago</span>
                    </label>
                    <label class="fm-checkbox-row">
                      <input type="checkbox" class="fm-checkbox" />
                      <span class="fm-checkbox-label">2 weeks ago</span>
                    </label>
                    <label class="fm-checkbox-row">
                      <input type="checkbox" class="fm-checkbox" />
                      <span class="fm-checkbox-label">1 month ago</span>
                    </label>
                    <label class="fm-checkbox-row">
                      <input type="checkbox" class="fm-checkbox" />
                      <span class="fm-checkbox-label">Custom</span>
                    </label>
                  </div>
                </div>
                
                <div class="pm-divider"></div>
                
                <!-- Created by collapsible -->
                <div class="fm-collapsible">
                  <div class="fm-col-header" @click="filterByExpanded = !filterByExpanded">
                    <span class="fm-col-title">Created by</span>
                    <i class="fa-solid fa-chevron-up fm-col-icon" :class="{'rotate-180': !filterByExpanded}"></i>
                  </div>
                  <div class="fm-col-content" v-show="filterByExpanded">
                    <label class="fm-checkbox-row">
                      <input type="checkbox" class="fm-checkbox" />
                      <div class="fm-user">
                        <div class="fm-avatar">D</div>
                        <span>You</span>
                      </div>
                    </label>
                  </div>
                </div>

              </div>
            </el-popover>
         </div>
      </div>

      <!-- List -->
      <div class="pages-list" v-loading="loading">
         <div v-if="filteredPages.length === 0" class="empty-state-full flex flex-col items-center justify-center py-24">
            <div class="relative w-40 h-32 mb-6 opacity-70 flex items-center justify-center">
               <div class="absolute w-24 h-12 border-2 border-[#2D2F36] rounded-[20%] bottom-0 skew-x-[30deg] -rotate-[15deg]"></div>
               <div class="absolute w-24 h-12 border-2 border-[#1E2025] rounded-[20%] bottom-4 skew-x-[30deg] -rotate-[15deg]"></div>
               <div class="absolute w-16 h-20 border-2 border-[#52525B] bg-[#16181D] rounded z-10 skew-x-[15deg] -rotate-[10deg] flex flex-col items-center pt-3 gap-2">
                 <div class="w-8 h-1 bg-[#3F3F46] rounded-full"></div>
                 <div class="w-6 h-1 bg-[#3F3F46] rounded-full self-start ml-3"></div>
               </div>
            </div>
            
            <h3 class="text-[16px] font-medium text-[#E4E4E7] mb-2">
               {{ activeTab === 'Archived' ? 'No archived pages yet' : 'No pages yet' }}
            </h3>
            <p class="text-[13px] text-[#A1A1AA]">
               {{ activeTab === 'Archived' ? 'Archive pages not on your radar. Access them here when needed.' : 'Create your first page to get started.' }}
            </p>
         </div>
         
         <div v-for="page in filteredPages" :key="page.id" class="page-row" @click="openPage(page.id)">
            <div class="pr-left">
               <i class="fa-regular fa-file-lines doc-icon"></i>
               <span class="page-title">{{ page.title || 'Untitled' }}</span>
            </div>
            <div class="pr-right hover-actions-container">
               <div class="avatar-xxs">P</div>
               
               <!-- Exclamation Info - Only shows tooltip on hover -->
               <el-tooltip
                 class="item"
                 effect="dark"
                 :content="'Created on: ' + formatDate(page.createdAt)"
                 placement="top"
               >
                 <button class="icon-btn action-hide" @click.stop="">
                    <i class="fa-solid fa-circle-exclamation"></i>
                 </button>
               </el-tooltip>

               <button class="icon-btn action-hide"><i class="fa-regular fa-star"></i></button>

               <!-- Dropdown Menu -->
               <el-dropdown trigger="click" @click.stop>
                  <button class="icon-btn action-hide btn-box" @click.stop>
                    <i class="fa-solid fa-ellipsis"></i>
                  </button>
                  <template #dropdown>
                    <el-dropdown-menu class="custom-dropdown-menu">
                      <!-- Public Actions -->
                      <template v-if="activeTab === 'Public'">
                        <el-dropdown-item @click="togglePrivacy(page)">
                          <i class="fa-solid fa-lock" style="margin-right: 8px"></i> Make private
                        </el-dropdown-item>
                        <el-dropdown-item @click="archivePage(page.id)">
                          <i class="fa-solid fa-box-archive" style="margin-right: 8px"></i> Archive
                        </el-dropdown-item>
                      </template>

                      <!-- Private Actions -->
                      <template v-else-if="activeTab === 'Private'">
                        <el-dropdown-item @click="togglePrivacy(page)">
                          <i class="fa-solid fa-globe" style="margin-right: 8px"></i> Make public
                        </el-dropdown-item>
                        <el-dropdown-item @click="archivePage(page.id)">
                          <i class="fa-solid fa-box-archive" style="margin-right: 8px"></i> Archive
                        </el-dropdown-item>
                      </template>

                      <!-- Archived Actions -->
                      <template v-else-if="activeTab === 'Archived'">
                        <el-dropdown-item @click="restorePage(page.id)">
                          <i class="fa-solid fa-rotate-left" style="margin-right: 8px"></i> Restore
                        </el-dropdown-item>
                        <el-dropdown-item @click="deletePage(page.id)" style="color: #EF4444">
                          <i class="fa-solid fa-trash" style="margin-right: 8px"></i> Delete
                        </el-dropdown-item>
                      </template>
                    </el-dropdown-menu>
                  </template>
               </el-dropdown>
            </div>
         </div>
      </div>
    </div>
    
    <!-- Word-like Editor View -->
    <div v-else class="page-editor-view">
       <!-- Editor Navigation Header -->
       <div class="editor-header">
          <div class="eh-left">
             <div class="breadcrumb-item">
               <i class="fa-solid fa-certificate" style="color: #F59E0B"></i>
               <span style="margin-left: 8px; color: #E4E4E7; font-weight: 500;">CYBWF</span>
               <i class="fa-solid fa-chevron-down" style="font-size: 10px; margin-left: 6px; color: #71717A"></i>
             </div>
             
             <i class="fa-solid fa-chevron-right" style="font-size: 9px; margin: 0 4px; color: #3F3F46"></i>
             
             <div class="breadcrumb-item" @click="activePage = null">
               <i class="fa-regular fa-file-lines" style="color: #A1A1AA"></i>
               <span style="color: #A1A1AA; margin-left: 8px;">Pages</span>
             </div>
             
             <i class="fa-solid fa-chevron-right" style="font-size: 9px; margin: 0 4px; color: #3F3F46"></i>
             
             <div class="breadcrumb-item active">
               <i class="fa-regular fa-file-lines" style="color: #A1A1AA"></i>
               <span style="color: #A1A1AA; margin-left: 8px;">{{ activePage.title || 'Untitled' }}</span>
               <i class="fa-solid fa-chevron-down" style="font-size: 10px; margin-left: 6px; color: #71717A"></i>
             </div>
             
             <span v-if="saving" class="status-saving">Saving...</span>
             <span v-if="!saving" class="status-saved"><i class="fa-solid fa-check"></i></span>
          </div>
          <div class="eh-right">
             <button class="icon-btn"><i class="fa-solid fa-lock-open" style="font-size: 13px;"></i></button>
             <button class="icon-btn"><i class="fa-regular fa-star" style="font-size: 13px;"></i></button>
             <button class="icon-btn"><i class="fa-solid fa-ellipsis" style="font-size: 13px;"></i></button>
             <div style="width: 1px; height: 16px; background: #27272A; margin: 0 4px;"></div>
             <button class="icon-btn"><i class="fa-solid fa-table-columns" style="font-size: 13px;"></i></button>
          </div>
       </div>

       <!-- WYSIWYG Toolbar -->
       <div class="wysiwyg-toolbar-wrapper">
           <div class="wysiwyg-toolbar">
              <div class="tb-group hover-bg" style="display:flex; align-items:center; gap: 8px">
                  Text <i class="fa-solid fa-chevron-down" style="font-size: 10px; color: #71717A"></i>
              </div>
              <div class="v-divider-tb"></div>
              
              <div class="tb-group hover-bg" style="display:flex; align-items:center; gap: 4px">
                  Color <span style="font-family: serif; font-style: italic; font-weight: bold">Aa</span>
              </div>
              <div class="v-divider-tb"></div>
              
              <div class="tb-group" v-if="editor">
                 <button class="tb-btn" :class="{ 'text-white bg-[#27272A]': editor.isActive('bold') }" @click="editor.chain().focus().toggleBold().run()"><i class="fa-solid fa-bold"></i></button>
                 <button class="tb-btn" :class="{ 'text-white bg-[#27272A]': editor.isActive('italic') }" @click="editor.chain().focus().toggleItalic().run()"><i class="fa-solid fa-italic"></i></button>
                 <button class="tb-btn" :class="{ 'text-white bg-[#27272A]': editor.isActive('underline') }" @click="editor.chain().focus().toggleUnderline().run()"><i class="fa-solid fa-underline"></i></button>
                 <button class="tb-btn" :class="{ 'text-white bg-[#27272A]': editor.isActive('strike') }" @click="editor.chain().focus().toggleStrike().run()"><i class="fa-solid fa-strikethrough"></i></button>
              </div>
              <div class="v-divider-tb"></div>
              
              <div class="tb-group" v-if="editor">
                 <button class="tb-btn" :class="{ 'text-white bg-[#27272A]': editor.isActive({ textAlign: 'left' }) }" @click="editor.chain().focus().setTextAlign('left').run()"><i class="fa-solid fa-align-left"></i></button>
                 <button class="tb-btn" :class="{ 'text-white bg-[#27272A]': editor.isActive({ textAlign: 'center' }) }" @click="editor.chain().focus().setTextAlign('center').run()"><i class="fa-solid fa-align-center"></i></button>
                 <button class="tb-btn" :class="{ 'text-white bg-[#27272A]': editor.isActive({ textAlign: 'right' }) }" @click="editor.chain().focus().setTextAlign('right').run()"><i class="fa-solid fa-align-right"></i></button>
              </div>
              <div class="v-divider-tb"></div>
              
              <div class="tb-group" v-if="editor">
                 <button class="tb-btn" :class="{ 'text-white bg-[#27272A]': editor.isActive('bulletList') }" @click="editor.chain().focus().toggleBulletList().run()"><i class="fa-solid fa-list-ul"></i></button>
                 <button class="tb-btn" :class="{ 'text-white bg-[#27272A]': editor.isActive('orderedList') }" @click="editor.chain().focus().toggleOrderedList().run()"><i class="fa-solid fa-list-ol"></i></button>
                 <button class="tb-btn"><i class="fa-solid fa-list-check"></i></button>
              </div>
              <div class="v-divider-tb"></div>
              
              <div class="tb-group" v-if="editor">
                 <button class="tb-btn" :class="{ 'text-white bg-[#27272A]': editor.isActive('codeBlock') }" @click="editor.chain().focus().toggleCodeBlock().run()"><i class="fa-solid fa-code"></i></button>
                 <button class="tb-btn" @click="() => { const url = prompt('Image URL:'); if (url) editor.chain().focus().setImage({ src: url }).run() }"><i class="fa-solid fa-image"></i></button>
                 <button class="tb-btn"><i class="fa-solid fa-table"></i></button>
              </div>
           </div>
       </div>

       <!-- Editor Canvas -->
       <div class="editor-canvas">
          <button class="add-icon-btn">
             <i class="fa-regular fa-face-smile"></i> Icon
          </button>
          
          <input 
             class="editor-title-input" 
             v-model="activePage.title" 
             @input="handleContentInput"
             placeholder="Untitled" 
          />
          
          <editor-content :editor="editor" class="editor-tiptap-content" />
       </div>
    </div>
  </div>
</template>

<style scoped>
.plane-pages-wrapper {
  display: flex;
  flex-direction: column;
  height: 100%;
  color: #E4E4E7;
  font-family: 'Inter', sans-serif;
  background: #0D0F11;
  min-height: calc(100vh - 120px);
}

/* Base Utility Alignments */
.pages-list-view { display: flex; flex-direction: column; }
.page-editor-view { display: flex; flex-direction: column; height: 100%; flex: 1; }

.pages-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 24px 12px 24px;
  border-bottom: 1px solid #1E2025;
}

.ph-left {
  display: flex;
  align-items: center;
  font-size: 13px;
  font-weight: 500;
  color: #A1A1AA;
}

.primary-action {
  background: #38BDF8;
  color: #0D0F11;
  border: none;
  border-radius: 6px;
  padding: 6px 16px;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}
.primary-action:hover { background: #7DD3FC; transform: translateY(-1px); box-shadow: 0 4px 12px rgba(56, 189, 248, 0.2); }

/* Nav Tabs */
.pages-nav {
  display: flex;
  gap: 24px;
  padding: 12px 24px 0;
  border-bottom: 1px solid #1E2025;
}
.nav-tab {
  font-size: 14px;
  font-weight: 500;
  color: #71717A;
  padding-bottom: 12px;
  cursor: pointer;
  border-bottom: 2px solid transparent;
  margin-bottom: -1px;
  transition: all 0.2s;
}
.nav-tab:hover { color: #E4E4E7; }
.nav-tab.active {
  color: #E4E4E7;
  border-bottom: 2px solid #38BDF8;
}

/* Toolbar */
.pages-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 24px;
}
.pt-right {
  display: flex;
  align-items: center;
  gap: 12px;
}
.icon-toggle {
  background: transparent;
  border: none;
  color: #A1A1AA;
  font-size: 14px;
  cursor: pointer;
}
.icon-toggle:hover { color: #E4E4E7; }

.filter-btn.outlined {
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
  transition: all 0.2s;
}
.filter-btn.outlined:hover { background: #1E2025; border-color: #3F3F46; }

/* List */
.pages-list {
  display: flex;
  flex-direction: column;
  padding: 0 24px 24px;
}

.page-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 16px;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
  margin-bottom: 2px;
}
.page-row:hover { background: #16181D; transform: translateX(4px); }

.pr-left {
  display: flex;
  align-items: center;
  gap: 12px;
}
.doc-icon { color: #52525B; font-size: 16px; }
.page-title { font-size: 14px; font-weight: 400; color: #D4D4D8; }

.pr-right {
  display: flex;
  align-items: center;
  gap: 16px;
}
.avatar-xxs {
  width: 22px;
  height: 22px;
  border-radius: 50%;
  background: #3F3F46;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 10px;
  font-weight: 600;
  color: #E4E4E7;
}

.action-hide { opacity: 0; transition: opacity 0.2s; }
.page-row:hover .action-hide { opacity: 1; }

.icon-btn {
  background: transparent;
  border: none;
  color: #A1A1AA;
  font-size: 14px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
  border-radius: 4px;
  transition: all 0.2s;
}
.icon-btn:hover { color: #E4E4E7; background: #27272A; }

.btn-box {
  background: #0D0F11;
  border: 1px solid #27272A;
}

/* Dropdown styling */
:deep(.custom-dropdown-menu) {
  background-color: #16181D !important;
  border: 1px solid #27272A !important;
  padding: 4px !important;
}

:deep(.el-dropdown-menu__item) {
  color: #A1A1AA !important;
  font-size: 13px !important;
  border-radius: 4px !important;
}

:deep(.el-dropdown-menu__item:hover) {
  background-color: #27272A !important;
  color: #E4E4E7 !important;
}

/* Editor View */
.editor-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 24px;
  background: transparent;
}

.breadcrumb-item {
  display: flex;
  align-items: center;
  cursor: pointer;
  padding: 4px 8px;
  border-radius: 4px;
  transition: 0.2s;
}
.breadcrumb-item:hover {
  background: #1E2025;
}

.eh-left, .eh-right {
  display: flex;
  align-items: center;
}
.eh-left {
  font-size: 13px;
  font-weight: 500;
  color: #A1A1AA;
  gap: 2px;
}
.eh-right {
  gap: 8px;
}

.status-saving { margin-left: 16px; color: #F59E0B; font-size: 12px; font-style: italic; }
.status-saved { margin-left: 16px; color: #10B981; font-size: 12px; opacity: 0.5; }

.v-divider {
  width: 1px;
  height: 16px;
  background: #27272A;
}
.v-divider-tb {
  width: 1px;
  height: 20px;
  background: #2D2F36;
  margin: 0 4px;
}

/* Wysiwyg Toolbar */
.wysiwyg-toolbar-wrapper {
  display: flex;
  justify-content: center;
  padding: 16px 0 32px 0;
}
.wysiwyg-toolbar {
  display: flex;
  align-items: center;
  background: #1B1C20;
  border: 1px solid #27272A;
  border-radius: 8px;
  padding: 4px;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
}
.tb-group {
  display: flex;
  align-items: center;
  padding: 0 4px;
  font-size: 13px;
  color: #D4D4D8;
  cursor: pointer;
}
.hover-bg { padding: 4px 8px; border-radius: 4px; transition: background 0.2s; }
.hover-bg:hover { background: #27272A; }

.tb-btn {
  background: transparent;
  border: none;
  width: 32px;
  height: 32px;
  border-radius: 4px;
  color: #A1A1AA;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: 0.2s;
}
.tb-btn:hover { color: #E4E4E7; background: #27272A; }

/* Canvas */
.editor-canvas {
  max-width: 800px;
  margin: 0 auto;
  width: 100%;
  padding: 0 32px 64px 32px;
  display: flex;
  flex-direction: column;
  flex: 1;
}

.add-icon-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  background: transparent;
  border: none;
  color: #A1A1AA;
  font-size: 14px;
  cursor: pointer;
  width: max-content;
  padding: 4px 8px;
  border-radius: 4px;
  margin-bottom: 16px;
  transition: 0.2s;
}
.add-icon-btn:hover { background: #27272A; color: #E4E4E7; }

.editor-title-input {
  background: transparent;
  border: none;
  font-size: 46px;
  font-weight: 700;
  color: #E4E4E7;
  outline: none;
  margin-bottom: 24px;
  font-family: inherit;
  letter-spacing: -0.02em;
}
.editor-title-input::placeholder { color: #52525B; }

.editor-tiptap-content :deep(.ProseMirror) {
  min-height: 500px;
  outline: none;
  font-size: 16px;
  line-height: 1.7;
  color: #D4D4D8;
}
</style>

<style>
.el-popover.custom-dark-popover {
  background-color: #1B1C20 !important;
  border: 1px solid #27272A !important;
  padding: 8px 12px 12px 12px !important;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.5) !important;
  color: #E4E4E7 !important;
  border-radius: 8px !important;
}

.el-popover.custom-dark-popover .el-popper__arrow {
  display: none !important;
}

/* PM Items */
.popover-menu-list {
  display: flex;
  flex-direction: column;
}
.pm-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 6px 12px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 13px;
  color: #E4E4E7;
  transition: background 0.2s;
  margin-bottom: 2px;
}
.pm-item:hover {
  background-color: #27272A;
}
.pm-check {
  font-size: 12px;
  color: #E4E4E7;
}
.pm-divider {
  height: 1px;
  background-color: #27272A;
  margin: 8px 0;
}

/* Filter Menu */
.filter-menu-container {
  display: flex;
  flex-direction: column;
}
.fm-search {
  position: relative;
  display: flex;
  align-items: center;
  margin-top: 4px;
}
.fms-icon {
  position: absolute;
  left: 10px;
  color: #71717A;
  font-size: 12px;
}
.fms-input {
  width: 100%;
  background-color: #16181D;
  border: 1px solid #27272A;
  border-radius: 4px;
  padding: 6px 10px 6px 30px;
  color: #E4E4E7;
  font-size: 13px;
  outline: none;
  font-family: inherit;
  transition: 0.2s;
}
.fms-input:focus {
  border-color: #3F3F46;
}
.fms-input::placeholder {
  color: #71717A;
}

.fm-checkbox-row {
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
  padding: 6px 8px;
  border-radius: 4px;
  margin-bottom: 2px;
}
.fm-checkbox-row:hover {
  background-color: rgba(255,255,255,0.03);
}
.fm-checkbox {
  appearance: none;
  width: 14px;
  height: 14px;
  border: 1px solid #3F3F46;
  border-radius: 3px;
  background: transparent;
  cursor: pointer;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
}
.fm-checkbox:checked {
  background-color: #E4E4E7;
  border-color: #E4E4E7;
}
.fm-checkbox:checked::after {
  content: '';
  position: absolute;
  width: 3.5px;
  height: 7px;
  border: solid #0D0F11;
  border-width: 0 1.5px 1.5px 0;
  transform: rotate(45deg);
  margin-top: -1px;
}
.fm-checkbox-label {
  font-size: 13px;
  color: #E4E4E7;
}

.fm-collapsible {
  display: flex;
  flex-direction: column;
}
.fm-col-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 6px 8px;
  cursor: pointer;
  border-radius: 4px;
}
.fm-col-header:hover {
  background-color: rgba(255,255,255,0.03);
}
.fm-col-title {
  font-size: 12px;
  font-weight: 500;
  color: #A1A1AA;
}
.fm-col-icon {
  font-size: 10px;
  color: #71717A;
  transition: transform 0.2s;
}
.fm-col-icon.rotate-180 {
  transform: rotate(180deg);
}

.fm-col-content {
  display: flex;
  flex-direction: column;
  padding: 4px 0 0 0;
}

.fm-user {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  color: #E4E4E7;
}
.fm-avatar {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background-color: #059669;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 10px;
  font-weight: 600;
}
</style>
