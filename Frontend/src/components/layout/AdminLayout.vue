<template>
  <div class="admin-layout">
    <AdminSidebar />
    <main class="admin-content">
      <div class="content-wrapper">
        <slot></slot>
      </div>
    </main>
  </div>
</template>

<script setup>
import AdminSidebar from './AdminSidebar.vue'
</script>

<style scoped>
.admin-layout {
  display: flex;
  min-height: 100vh;
  background-color: var(--bg-layout);
  color: var(--text-primary);
  font-family: 'Inter', system-ui, sans-serif;
}

.admin-content {
  flex: 1;
  background-color: #e8eaed; /* Match the Light Neumorphism background from Figma */
  padding: 32px 48px;
  overflow-y: auto;
  height: 100vh;
}

.content-wrapper {
  max-width: 1200px;
  margin: 0 auto;
}
</style>
