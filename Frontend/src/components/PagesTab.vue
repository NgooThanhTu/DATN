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

const props = defineProps({
  projectId: { type: String, required: true }
})

const pages = ref([])
const activePage = ref(null)
const loading = ref(false)
const saving = ref(false)
const activeTab = ref('Public')

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

async function archivePage(pageId) {
  try {
    await ElMessageBox.confirm('Bạn có chắc muốn lưu trữ trang này?', 'Xác nhận', { type: 'warning' })
    await axiosClient.put(`/projects/${props.projectId}/pages/${pageId}/archive`)
    if (activePage.value?.id === pageId) activePage.value = null
    fetchPages()
    ElNotification({ title: 'Thành công', message: 'Đã lưu trữ trang', type: 'success' })
  } catch (e) {
      if (e !== 'cancel') console.error(e)
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
            <button class="filter-btn outlined"><i class="fa-solid fa-arrow-down-up-across-line"></i> Date modified</button>
            <button class="filter-btn outlined"><i class="fa-solid fa-filter"></i> Filters</button>
         </div>
      </div>

      <!-- List -->
      <div class="pages-list" v-loading="loading">
         <div v-if="filteredPages.length === 0" class="empty-state-full flex flex-col items-center justify-center py-24">
            <!-- Graphical mock icon for empty state -->
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
               <button class="icon-btn action-hide"><i class="fa-solid fa-globe"></i></button>
               <button class="icon-btn action-hide"><i class="fa-solid fa-circle-info"></i></button>
               <button class="icon-btn action-hide"><i class="fa-regular fa-star"></i></button>
               <button class="icon-btn action-hide btn-box" @click.stop="archivePage(page.id)"><i class="fa-solid fa-ellipsis"></i></button>
            </div>
         </div>
      </div>
    </div>
    
    <!-- Word-like Editor View -->
    <div v-else class="page-editor-view">
       <!-- Editor Navigation Header -->
       <div class="editor-header">
          <div class="eh-left">
             <button class="back-btn" @click="activePage = null"><i class="fa-solid fa-chevron-left" style="font-size: 12px"></i></button>
             <i class="fa-solid fa-certificate" style="color: #F59E0B"></i>
             <span style="margin-left: 8px; color: #71717A">CYBWF</span>
             <i class="fa-solid fa-chevron-right" style="font-size: 9px; margin: 0 8px; color: #71717A"></i>
             <i class="fa-regular fa-file-lines" style="color: #A1A1AA"></i>
             <span style="color: #A1A1AA; margin-left: 8px">Pages</span>
             <i class="fa-solid fa-chevron-right" style="font-size: 9px; margin: 0 8px; color: #71717A"></i>
             <i class="fa-regular fa-file-lines" style="color: #E4E4E7"></i>
             <span style="color: #E4E4E7; margin-left: 8px">{{ activePage.title || 'Untitled' }}</span>
             
             <span v-if="saving" class="status-saving">Saving...</span>
             <span v-if="!saving" class="status-saved"><i class="fa-solid fa-check"></i></span>
          </div>
          <div class="eh-right">
             <button class="icon-btn"><i class="fa-solid fa-unlock-keyhole"></i></button>
             <button class="icon-btn"><i class="fa-solid fa-link"></i></button>
             <button class="icon-btn"><i class="fa-solid fa-ellipsis"></i></button>
             <div class="v-divider"></div>
             <button class="icon-btn"><i class="fa-solid fa-table-columns"></i></button>
          </div>
       </div>

       <!-- WYSIWYG Toolbar (Floating style like Word/Notion) -->
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
  background: #0EA5E9;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 6px 16px;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition: background 0.2s;
}
.primary-action:hover { background: #0284C7; }

/* Nav Tabs */
.pages-nav {
  display: flex;
  gap: 24px;
  padding: 8px 24px 0;
  border-bottom: 1px solid #1E2025;
}
.nav-tab {
  font-size: 14px;
  font-weight: 500;
  color: #A1A1AA;
  padding-bottom: 12px;
  cursor: pointer;
  border-bottom: 2px solid transparent;
  margin-bottom: -1px;
  transition: color 0.2s;
}
.nav-tab:hover { color: #E4E4E7; }
.nav-tab.active {
  color: #38BDF8;
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
  transition: border-color 0.2s, background 0.2s;
}
.filter-btn.outlined:hover { background: #1E2025; border-color: #3F3F46; }

/* List */
.pages-list {
  display: flex;
  flex-direction: column;
  padding: 0 24px 24px;
}

.empty-state {
  text-align: center;
  padding: 40px 0;
  color: #71717A;
  font-size: 14px;
}

.page-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  border-bottom: 1px solid transparent;
  border-radius: 8px;
  cursor: pointer;
  transition: background 0.2s;
}
.page-row:hover { background: #16181D; }
.page-row:not(:last-child) { border-bottom: 1px solid #16181D; }

.pr-left {
  display: flex;
  align-items: center;
  gap: 12px;
}
.doc-icon { color: #71717A; font-size: 18px; }
.page-title { font-size: 14px; font-weight: 500; color: #E4E4E7; }

.pr-right {
  display: flex;
  align-items: center;
  gap: 16px;
}
.avatar-xxs {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  background: #0F766E;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 11px;
  font-weight: 600;
  color: white;
}
.action-hide { opacity: 0; transition: opacity 0.2s; }
.hover-actions-container:hover .action-hide { opacity: 1; }

.icon-btn {
  background: transparent;
  border: none;
  color: #A1A1AA;
  font-size: 14px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
}
.icon-btn:hover { color: #E4E4E7; }

.btn-box {
  width: 28px;
  height: 28px;
  background: #16181D;
  border: 1px solid #27272A;
  border-radius: 4px;
  transition: 0.2s;
}
.btn-box:hover { background: #27272A; }

/* Editor View */
.editor-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 24px;
  border-bottom: 1px solid #1E2025;
}
.eh-left, .eh-right {
  display: flex;
  align-items: center;
}
.eh-left {
  font-size: 13px;
  font-weight: 500;
  color: #A1A1AA;
}
.eh-right {
  gap: 20px;
}

.back-btn {
  background: #1B1C20;
  border: 1px solid #27272A;
  border-radius: 4px;
  padding: 4px 8px;
  color: #A1A1AA;
  cursor: pointer;
  margin-right: 12px;
  display: flex;
  align-items: center;
}
.back-btn:hover { color: #E4E4E7; background: #27272A; }

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
  padding: 16px 0;
  border-bottom: 1px solid #1E2025;
}
.wysiwyg-toolbar {
  display: flex;
  align-items: center;
  background: #1B1C20;
  border: 1px solid #27272A;
  border-radius: 8px;
  padding: 4px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
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
  padding: 64px 32px;
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
.editor-tiptap-content :deep(.ProseMirror p.is-editor-empty:first-child::before) {
  content: "Press '/' for commands...";
  float: left;
  color: #52525B;
  pointer-events: none;
  height: 0;
}
.editor-tiptap-content :deep(p) { margin-bottom: 0.8em; }
.editor-tiptap-content :deep(strong) { color: #fff; font-weight: 700; }
.editor-tiptap-content :deep(u) { text-decoration: underline; }
.editor-tiptap-content :deep(ul) { list-style-type: disc; margin-left: 20px; }
.editor-tiptap-content :deep(ol) { list-style-type: decimal; margin-left: 20px; }
.editor-tiptap-content :deep(img) { border-radius: 8px; max-width: 100%; border: 1px solid #27272A; margin: 16px 0; }
</style>
