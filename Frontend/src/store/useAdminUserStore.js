import { defineStore } from 'pinia';
import { ref } from 'vue';
import { adminUserApi } from '@/api/adminUserApi';

export const useAdminUserStore = defineStore('adminUsers', () => {
  const users = ref([]);
  const loading = ref(false);

  const fetchUsers = async (search = '') => {
    loading.value = true;
    try {
      const res = await adminUserApi.getUsers({ search });
      if (res.data && res.data.data) {
        users.value = res.data.data;
      }
    } catch (error) {
      console.error('Lỗi khi tải danh sách người dùng', error);
    } finally {
      loading.value = false;
    }
  };

  const suspendUser = async (userId) => {
    try {
      await adminUserApi.suspendUser(userId);
      const userIndex = users.value.findIndex(u => u.id === userId);
      if (userIndex !== -1) {
        users.value[userIndex].isActive = false;
      }
      return true;
    } catch (error) {
      console.error('Lỗi khi vô hiệu hóa người dùng:', error);
      throw error;
    }
  };

  const createUser = async (data) => {
    loading.value = true;
    try {
      await adminUserApi.createUser(data);
      await fetchUsers(); // reload danh sách
      return true;
    } catch (error) {
      console.error('Lỗi khi thêm người dùng:', error);
      throw error;
    } finally {
      loading.value = false;
    }
  };

  return { users, loading, fetchUsers, suspendUser, createUser };
});
