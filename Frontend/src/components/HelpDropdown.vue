<template>
  <el-dropdown trigger="click" popper-class="help-dropdown-popper global-help-popper">
    <div class="help-trigger-btn">
      <i class="fa-regular fa-circle-question"></i>
    </div>
    <template #dropdown>
      <el-dropdown-menu class="dark-help-menu">
        <div class="help-header">
          <h3>Trợ giúp</h3>
          <i class="fa-solid fa-xmark close-btn"></i>
        </div>
        <el-dropdown-item><i class="fa-regular fa-lightbulb"></i> Tìm hiểu cập nhật mới</el-dropdown-item>
        <el-dropdown-item><i class="fa-regular fa-file-lines"></i> Đọc về điều hướng mới</el-dropdown-item>
        <el-dropdown-item><i class="fa-solid fa-book"></i> Duyệt tài liệu hướng dẫn</el-dropdown-item>
        <el-dropdown-item><i class="fa-solid fa-desktop"></i> Học kỹ năng điều hành dự án</el-dropdown-item>
        <el-dropdown-item divided><i class="fa-regular fa-comment"></i> Hỏi cộng đồng trực tuyến</el-dropdown-item>
        <el-dropdown-item><i class="fa-solid fa-triangle-exclamation"></i> Liên hệ hỗ trợ</el-dropdown-item>
        <el-dropdown-item><i class="fa-regular fa-comments"></i> Phản hồi về SprintA</el-dropdown-item>
        <el-dropdown-item><i class="fa-solid fa-keyboard"></i> Phím tắt</el-dropdown-item>
        <el-dropdown-item><i class="fa-solid fa-paper-plane"></i> Tải ứng dụng Mobile</el-dropdown-item>
        <div class="help-footer">
          <a href="#">Giới thiệu</a>
          <a href="#">Điều khoản</a>
          <a href="#">Bảo mật</a>
        </div>
      </el-dropdown-menu>
    </template>
  </el-dropdown>
</template>

<script setup>
// No extra logic needed for now
</script>

<style scoped>
.help-trigger-btn {
  color: #94a3b8;
  font-size: 18px;
  cursor: pointer;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 6px;
  transition: all 0.2s;
}

.help-trigger-btn:hover {
  color: white;
  background-color: #1e293b;
}

/* Specific styles that were in the views */
.dark-help-menu {
  background: #22272b;
  border: none;
  width: 320px;
  padding: 0;
}

.help-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 20px;
  color: #b3bac5;
  border-bottom: 1px solid #333;
}

.help-header h3 {
  font-size: 14px;
  font-weight: 600;
  margin: 0;
}

.close-btn {
  cursor: pointer;
  font-size: 14px;
  opacity: 0.6;
}

.close-btn:hover {
  opacity: 1;
}

:deep(.el-dropdown-menu__item) {
  color: #b3bac5 !important;
  padding: 10px 20px !important;
  font-size: 14px !important;
  display: flex;
  align-items: center;
  gap: 12px;
}

:deep(.el-dropdown-menu__item i) {
  font-size: 16px;
  width: 16px;
  text-align: center;
}

:deep(.el-dropdown-menu__item:hover) {
  background-color: rgba(255, 255, 255, 0.05) !important;
  color: white !important;
}

:deep(.el-dropdown-menu__item--divided) {
  border-top-color: #333 !important;
}

.help-footer {
  display: flex;
  justify-content: space-evenly;
  padding: 16px 20px;
  background-color: #1d2125;
  border-top: 1px solid #333;
  margin-top: 8px;
}

.help-footer a {
  color: #8c9bab;
  font-size: 12px;
  text-decoration: none;
}

.help-footer a:hover {
  color: white;
  text-decoration: underline;
}
</style>

<style>
/* Global popper styles to override Element Plus defaults */
.el-popper.help-dropdown-popper.global-help-popper {
  background: #22272b !important;
  border: 1px solid #333 !important;
  border-radius: 8px !important;
  padding: 0 !important;
  box-shadow: 0 8px 16px rgba(0,0,0,0.6) !important;
}

.el-popper.help-dropdown-popper.global-help-popper .el-popper__arrow::before {
  background: #22272b !important;
  border-color: #333 !important;
}
</style>
