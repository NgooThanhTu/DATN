<template>
  <el-dropdown trigger="click" popper-class="settings-dropdown-popper">
    <div class="settings-nav-icon setting-trigger">
      <i class="fa-solid fa-gear"></i>
    </div>
    <template #dropdown>
      <el-dropdown-menu class="jira-settings-menu">
        <div class="settings-content-wrapper">
          <div class="settings-section">
            <h4 class="settings-section-title">Cài đặt quản trị Jira</h4>
            
            <div class="settings-item" @click="handleCommand('audit')">
              <div class="settings-item-icon">
                <i class="fa-solid fa-clock-rotate-left"></i>
              </div>
              <div class="settings-item-info">
                <div class="item-name">Audit log (Nhật ký kiểm tra)</div>
                <div class="item-desc">Theo dõi và quản lý nhật ký hoạt động hệ thống.</div>
              </div>
            </div>

            <div class="settings-item" @click="handleCommand('users')">
              <div class="settings-item-icon">
                <i class="fa-solid fa-users-gear"></i>
              </div>
              <div class="settings-item-info">
                <div class="item-name">Quản lý người dùng</div>
                <div class="item-desc">Quản lý người dùng, nhóm và yêu cầu truy cập.</div>
                <i class="fa-solid fa-arrow-up-right-from-square external-icon"></i>
              </div>
            </div>
          </div>
        </div>
      </el-dropdown-menu>
    </template>
  </el-dropdown>
</template>

<script setup>
const handleCommand = (cmd) => {
  console.log('Settings command:', cmd);
};
</script>

<style scoped>
.jira-settings-menu {
  width: 320px !important;
  background-color: #1d2125 !important;
  border: none !important;
  padding: 8px 0 !important;
  color: #8c9bab;
}

.settings-content-wrapper {
  padding: 8px 0;
}

.settings-section-title {
  padding: 0 16px;
  font-size: 11px;
  font-weight: 700;
  text-transform: uppercase;
  color: #64748b;
  margin-bottom: 8px;
  letter-spacing: 0.5px;
}

.settings-item {
  display: flex;
  padding: 10px 16px;
  cursor: pointer;
  transition: all 0.2s;
  position: relative;
}

.settings-item:hover {
  background-color: #2c333a;
}

.settings-item-icon {
  width: 32px;
  min-width: 32px;
  display: flex;
  justify-content: center;
  margin-top: 2px;
  font-size: 16px;
  color: #8c9bab;
}

.settings-item-info {
  flex: 1;
}

.item-name {
  font-size: 13px;
  font-weight: 500;
  color: #f4f5f7;
  margin-bottom: 2px;
}

.item-desc {
  font-size: 11px;
  line-height: 1.4;
  color: #94a3b8;
}

.external-icon {
  position: absolute;
  right: 16px;
  top: 12px;
  font-size: 10px;
  color: #64748b;
}

.settings-nav-icon {
  width: 32px;
  height: 32px;
  border-radius: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #f8fafc; /* Brighter for better visibility */
  cursor: pointer;
  transition: all 0.2s;
}

.settings-nav-icon i {
  font-size: 18px; /* Standard navbar size */
}

.settings-nav-icon:hover {
  background-color: #1e293b;
  color: white;
}
</style>

<style>
/* High z-index popper to stay on top of AI popups */
.el-popper.settings-dropdown-popper {
  background: #1d2125 !important;
  border: 1px solid #333c43 !important;
  padding: 0 !important;
  z-index: 100001 !important; /* Extremely high z-index */
  box-shadow: 0 10px 40px rgba(0,0,0,0.5) !important;
}

.el-popper.settings-dropdown-popper .el-popper__arrow::before {
  background: #1d2125 !important;
  border-color: #333c43 !important;
}
</style>
