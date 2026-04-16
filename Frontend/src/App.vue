<template>
  <!-- Nội dung trang -->
  <ErrorBoundary>
    <Suspense>
      <router-view />
      <template #fallback>
        <div style="display:flex; justify-content:center; align-items:center; height:100vh; background:var(--bg-layout); color:var(--text-primary);">
          Đang tải giao diện...
        </div>
      </template>
    </Suspense>
  </ErrorBoundary>
</template>

<script setup>
import { onMounted } from 'vue'
import axiosClient from '@/api/axiosClient'
import ErrorBoundary from '@/components/ErrorBoundary.vue'

onMounted(async () => {
  const token = localStorage.getItem('accessToken')
  if (token) {
    try {
      const res = await axiosClient.get('/settings/ThemeSettings')
      const data = res.data?.data
      if (data) {
        for (const key in data) {
          // Map backend camelCase keys to CSS variables if we know them
          // We stored them with EXACTly same keys from Customization.vue
          // For safety, only map known variables or if the key structure matches.
          // In Customization.vue: 'bgLayout' -> '--bg-layout'
          const cssVarMap = {
            'bgImage': '--bg-image',
            'bgLayout': '--bg-layout',
            'bgCard': '--bg-card',
            'bgHover': '--bg-hover',
            'borderColor': '--border-color',
            'textPrimary': '--text-primary'
          }
          if (cssVarMap[key]) {
            document.documentElement.style.setProperty(cssVarMap[key], data[key])
          }
        }
      }
    } catch (e) {
      console.error('Failed to load global theme', e)
    }
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
</style>
