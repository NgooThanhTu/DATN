<template>
  <NexusLayout>
    <div class="stickies-wrapper">
      <header class="st-header">
        <div class="st-left">
          <i class="fa-solid fa-note-sticky text-muted"></i>
          <span class="st-title flex items-center gap-2">Stickies <span class="bg-[#27272A] text-gray-400 text-[10px] px-1.5 py-0.5 rounded" v-if="stickies.length > 0">{{ stickies.length }}</span></span>
        </div>
        <div class="st-right">
          <button class="plane-toolbar-btn"><i class="fa-solid fa-magnifying-glass"></i></button>
          <button class="plane-primary-btn flex items-center gap-1.5" @click="addSticky">Add sticky</button>
        </div>
      </header>

      <div class="st-body">
        <!-- Empty State -->
        <div v-if="stickies.length === 0" class="empty-state flex flex-col items-center justify-center pt-24 h-full">
           <div class="empty-text-container text-left w-full max-w-3xl">
             <h2 class="text-[18px] font-medium text-[#E4E4E7] mb-2">Stickies are quick notes and to-dos you take down on the fly.</h2>
             <p class="text-[13px] text-[#A1A1AA] mb-10">Capture your thoughts and ideas effortlessly by creating stickies that you can access anytime and from anywhere.</p>
             
             <!-- Mocked background area mirroring Plane design -->
             <div class="empty-bg relative w-full h-[400px] bg-[#141518] border border-[#1E2025] rounded-xl flex flex-col items-center justify-end overflow-hidden">
                <div class="absolute inset-0 flex items-center justify-center opacity-30">
                  <i class="fa-solid fa-note-sticky text-[150px] text-[#27272A] rotate-12"></i>
                  <i class="fa-solid fa-note-sticky text-[120px] text-[#1E2025] -ml-20 -mt-20 -rotate-12"></i>
                </div>
                <button class="plane-primary-btn flex items-center gap-1.5 relative z-10 mb-8" @click="addSticky">
                  <i class="fa-solid fa-plus text-xs"></i> Add sticky
                </button>
             </div>
           </div>
        </div>

        <!-- Populated Grid -->
        <div v-else class="stickies-grid">
          <div class="sticky-card" v-for="sticky in stickies" :key="sticky.id" :style="{ backgroundColor: sticky.color }">
             <textarea 
               class="sticky-input" 
               v-model="sticky.content"
               placeholder="Click to type here..."
               :style="{ 
                 fontWeight: sticky.isBold ? '600' : '400',
                 fontStyle: sticky.isItalic ? 'italic' : 'normal',
                 textAlign: sticky.align
               }"
             ></textarea>
             
             <div class="sticky-footer">
                <div class="sf-left">
                   <!-- Custom Color Popover -->
                   <el-popover placement="bottom-start" trigger="click" popper-class="plane-popover dark custom-swatch-popover" :width="200">
                     <template #reference>
                        <button class="sf-btn"><i class="fa-solid fa-palette"></i></button>
                     </template>
                     <div class="swatch-container">
                        <div class="text-[12px] font-medium text-gray-300 mb-3 px-1">Background colors</div>
                        <div class="color-grid">
                           <div 
                             v-for="c in backgroundColors" 
                             :key="c" 
                             class="color-swatch"
                             :style="{ backgroundColor: c }"
                             @click="sticky.color = c"
                           >
                             <i v-if="sticky.color === c" class="fa-solid fa-check text-[10px] text-white"></i>
                           </div>
                        </div>
                     </div>
                   </el-popover>
                   
                   <button class="sf-btn" :class="{ 'active': sticky.isBold }" @click="sticky.isBold = !sticky.isBold"><i class="fa-solid fa-bold"></i></button>
                   <button class="sf-btn" :class="{ 'active': sticky.isItalic }" @click="sticky.isItalic = !sticky.isItalic"><i class="fa-solid fa-italic"></i></button>
                   <button class="sf-btn" @click="cycleAlignment(sticky)">
                     <i v-if="sticky.align === 'left'" class="fa-solid fa-align-left"></i>
                     <i v-if="sticky.align === 'center'" class="fa-solid fa-align-center"></i>
                     <i v-if="sticky.align === 'right'" class="fa-solid fa-align-right"></i>
                   </button>
                </div>
                <button class="sf-btn trash" @click="deleteSticky(sticky.id)"><i class="fa-solid fa-trash-can"></i></button>
             </div>
          </div>
        </div>
      </div>
    </div>
  </NexusLayout>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import NexusLayout from '@/components/layout/NexusLayout.vue'

// Plane's realistic dark theme sticky colors (as requested: 8 colors, contrasting, not bright)
const backgroundColors = [
  '#3F3F46', // Zinc 700 (Gray)
  '#4C2B2D', // Deep Red
  '#4A314D', // Deep Purple
  '#5C4532', // Deep Bronze/Brown
  '#1B4D3E', // Forest Green
  '#1D4C5C', // Ocean Blue/Teal
  '#1E3A8A', // Dark Navy Blue
  '#3B2E58'  // Violet
]

const stickies = ref([])
const createCount = ref(0) // tracks how many we've made to cycle colors

// Load from local storage if exists
onMounted(() => {
  const saved = localStorage.getItem('plane_stickies');
  if (saved) {
    try {
      stickies.value = JSON.parse(saved);
      createCount.value = stickies.value.length;
    } catch(e) {}
  }
})

// Save to storage automatically every time we modify anything
const saveToStorage = () => {
    localStorage.setItem('plane_stickies', JSON.stringify(stickies.value));
}

const addSticky = () => {
  const nextColorIdx = createCount.value % backgroundColors.length;
  stickies.value.push({
    id: Date.now(),
    content: '',
    color: backgroundColors[nextColorIdx],
    isBold: false,
    isItalic: false,
    align: 'left'
  });
  createCount.value++;
  saveToStorage();
}

const deleteSticky = (id) => {
  stickies.value = stickies.value.filter(s => s.id !== id);
  saveToStorage();
}

const cycleAlignment = (sticky) => {
  if (sticky.align === 'left') sticky.align = 'center';
  else if (sticky.align === 'center') sticky.align = 'right';
  else sticky.align = 'left';
  saveToStorage();
}
</script>

<style scoped>
.stickies-wrapper {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background: #0D0F11;
  color: #E4E4E7;
}

.st-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 24px;
  border-bottom: 1px solid #1E2025;
}
.st-left {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  font-weight: 500;
}
.text-muted { color: #A1A1AA; }
.st-title { color: #E4E4E7; }

.st-right {
  display: flex;
  gap: 12px;
}
.plane-toolbar-btn {
  background: transparent;
  border: none;
  color: #D4D4D8;
  cursor: pointer;
  padding: 6px;
  border-radius: 4px;
  transition: background 0.2s;
}
.plane-toolbar-btn:hover { background: #1E2025; }
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
  box-shadow: 0 1px 3px rgba(0,0,0,0.3);
}
.plane-primary-btn:hover { background: #0284C7; }

.st-body {
  flex: 1;
  overflow-y: auto;
  padding: 24px;
}

.stickies-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 24px;
  align-items: start; /* prevents stretching */
}

.sticky-card {
  border-radius: 6px;
  height: 280px;
  display: flex;
  flex-direction: column;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.4);
  transition: transform 0.2s, box-shadow 0.2s;
}
.sticky-card:hover { 
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.5);
}

.sticky-input {
  flex: 1;
  background: transparent;
  border: none;
  outline: none;
  color: rgba(255, 255, 255, 0.95);
  padding: 16px;
  font-family: inherit;
  font-size: 14px;
  resize: none;
  line-height: 1.5;
}
.sticky-input::placeholder { color: rgba(255, 255, 255, 0.4); font-style: normal; font-weight: normal;}

.sticky-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  opacity: 0;
  transition: opacity 0.2s;
}
.sticky-card:hover .sticky-footer, .sticky-card:focus-within .sticky-footer { opacity: 1; }

.sf-left { display: flex; gap: 4px; }
.sf-btn {
  background: transparent;
  border: none;
  color: rgba(255,255,255,0.5);
  cursor: pointer;
  padding: 6px;
  font-size: 14px;
  border-radius: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
  transition: 0.2s;
}
.sf-btn:hover { color: white; background: rgba(255,255,255,0.1); }
.sf-btn.active { color: white; background: rgba(255,255,255,0.2); }
.trash:hover { color: #F87171; background: rgba(248, 113, 113, 0.1); }

/* Color Swatches Popover Override */
.swatch-container {
  padding: 8px;
}
.color-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 12px;
}
.color-swatch {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  cursor: pointer;
  transition: transform 0.1s;
  display: flex;
  align-items: center;
  justify-content: center;
}
.color-swatch:hover {
  transform: scale(1.1);
  box-shadow: 0 0 0 2px rgba(255,255,255,0.2)
}

:deep(.custom-swatch-popover) {
  background-color: #16181D !important;
  border: 1px solid #2D2F36 !important;
  border-radius: 8px !important;
}
</style>
