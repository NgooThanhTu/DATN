<template>
  <div class="accept-invite-container">
    <div class="glass-card">
      <div v-if="loading" class="loading-state">
        <i class="fa-solid fa-spinner fa-spin text-4xl text-primary mb-4"></i>
        <p>Đang tải thông tin lời mời...</p>
      </div>
      
      <div v-else-if="success" class="success-state">
        <i class="fa-solid fa-circle-check text-6xl text-success mb-4"></i>
        <h2>Tham gia thành công!</h2>
        <p>Chào mừng <strong>{{ formData.fullName }}</strong> đến với nền tảng.</p>
        <el-button type="primary" class="mt-4 w-full glass-btn" @click="$router.push('/login')">
          Đăng nhập ngay
        </el-button>
      </div>

      <div v-else-if="error" class="error-state">
        <i class="fa-solid fa-circle-xmark text-6xl text-danger mb-4"></i>
        <h2>Không thể chấp nhận lời mời</h2>
        <p>{{ error }}</p>
        <el-button type="default" class="mt-4 w-full" @click="$router.push('/login')">
          Trở về Đăng nhập
        </el-button>
      </div>

      <div v-else class="form-state">
        <div class="form-header">
          <i class="fa-solid fa-envelope-open-text text-5xl text-primary mb-4"></i>
          <h2>Thư Mời Tham Gia Dự Án</h2>
          <p>Bạn đã được mời nhận đặc quyền mới trên hệ thống Task Management.</p>
        </div>

        <el-button type="primary" class="w-full glass-btn submit-btn" size="large" :loading="isSubmitting" @click="submitAccept">
          Xác nhận Tham gia
        </el-button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import axiosClient from '@/api/axiosClient';
import { ElMessage } from 'element-plus';

const route = useRoute();
const router = useRouter();

const inviteEmail = ref('');
const loading = ref(false);
const success = ref(false);
const error = ref('');
const isSubmitting = ref(false);

const submitAccept = async () => {
    isSubmitting.value = true;
    try {
      await axiosClient.post('/auth/accept-invite');
      success.value = true;
      ElMessage.success({ message: "Phê duyệt quyền thành công!", duration: 3000 });
    } catch (err) {
      error.value = err.response?.data?.message || "Có lỗi xảy ra khi xác nhận lời mời.";
      ElMessage.error(error.value);
    } finally {
      isSubmitting.value = false;
    }
};
</script>

<style scoped>
.accept-invite-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #0f172a 0%, #1e1e2d 100%);
  padding: 20px;
}

.glass-card {
  background: rgba(30, 30, 45, 0.7);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 24px;
  padding: 40px;
  width: 100%;
  max-width: 480px;
  text-align: center;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.4);
}

.form-header h2 {
  font-size: 24px;
  font-weight: 600;
  color: #f8fafc;
  margin-bottom: 12px;
}

.form-header p {
  color: #94a3b8;
  font-size: 15px;
  line-height: 1.6;
  margin-bottom: 30px;
}

.text-primary {
  color: var(--el-color-primary, #3b82f6);
}

.text-success {
  color: #10b981;
}

.text-danger {
  color: #ef4444;
}

.glass-btn {
  background: linear-gradient(135deg, rgba(59, 130, 246, 0.9), rgba(37, 99, 235, 0.9));
  border: none;
  font-weight: 600;
  letter-spacing: 0.5px;
  transition: all 0.3s ease;
}

.glass-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(59, 130, 246, 0.4);
}

:deep(.el-form-item__label) {
  color: #cbd5e1 !important;
  font-weight: 500;
}

:deep(.el-input__wrapper) {
  background-color: rgba(15, 23, 42, 0.6) !important;
  border-radius: 12px;
  box-shadow: 0 0 0 1px rgba(255, 255, 255, 0.1) inset !important;
}

:deep(.el-input__wrapper.is-focus) {
  box-shadow: 0 0 0 1px var(--el-color-primary) inset !important;
}

:deep(.el-input__inner) {
  color: #f8fafc !important;
}
</style>
