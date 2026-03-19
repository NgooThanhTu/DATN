<template>
  <!-- Màn hình Loading -->
  <transition name="fade">
    <div v-if="isLoading" class="global-loader">
      <img :src="loadingGif" alt="Đang tải..." class="loading-gif" />
    </div>
  </transition>

  <!-- Nội dung trang -->
  <router-view v-show="!isLoading" />

</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import loadingGif from './assets/Motion_graphics_animation_the_cyan_and_blue.gif'

const isLoading = ref(true)
const router = useRouter()
let isInitialLoad = true

// Lần đầu vào web sẽ loading 1.5 giây
onMounted(() => {
  setTimeout(() => {
    isLoading.value = false
  }, 1500)
})

// Bộ lọc: Chỉ bật Loading theo yêu cầu đặc biệt của đường dẫn
const shouldShowLoading = (to, from) => {
  if (isInitialLoad) return false
  
  // Từ trang chủ (Home) bấm vào Login hoặc Register
  if (from.path === '/' && (to.path === '/login' || to.path === '/register')) {
    return true
  }
  // Sau khi đăng nhập/đăng ký (từ Auth qua Dashboard)
  if ((from.path === '/login' || from.path === '/register') && to.path === '/dashboard') {
    return true
  }
  
  return false
}

// Chạy loading thêm cho một số quá trình chuyển trang đã quy định
router.beforeEach((to, from, next) => {
  if (shouldShowLoading(to, from)) {
    isLoading.value = true
  }
  next()
})

router.afterEach((to, from) => {
  if (isInitialLoad) {
    isInitialLoad = false;
    return;
  }
  
  if (shouldShowLoading(to, from)) {
    setTimeout(() => {
      isLoading.value = false
    }, 800) // Thời gian loading chuyển trang ngắn hơn
  }
})
</script>

<style>
body {
  margin: 0;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

* {
  box-sizing: border-box;
}

/* Loader Styles */
.global-loader {
  position: fixed !important;
  inset: 0 !important;
  width: 100vw !important;
  height: 100vh !important;
  background-color: #000000 !important;
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  z-index: 999999 !important;
  overflow: hidden !important;
  margin: 0 !important;
  padding: 0 !important;
}

.loading-gif {
  width: 100% !important;
  height: 100% !important;
  object-fit: cover !important;
  pointer-events: none !important;
}

/* Hiệu ứng mượt mà khi Loading biến mất */
.fade-leave-active {
  transition: opacity 0.6s ease;
}
.fade-leave-to {
  opacity: 0;
}
.fade-enter-active {
  transition: opacity 0.3s ease;
}
.fade-enter-from {
  opacity: 0;
}

</style>
