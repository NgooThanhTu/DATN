<template>
  <div class="dashboard-layout">
    <!-- Navbar -->
    <header class="top-nav">
      <div class="nav-left">
        <router-link to="/dashboard" class="nav-brand">
          <img :src="logoImg" alt="SprintA Logo" class="nav-logo" />
          <span>SprintA</span>
        </router-link>
        <span class="nav-link">Trợ lý AI</span>
      </div>

      <div class="nav-center">
        <div class="top-search-create">
          <div class="search-input-mock">
            <i class="fa-solid fa-magnifying-glass"></i>
            <input type="text" placeholder="Tìm kiếm công việc, tài liệu, hoặc hỏi AI..." v-model="searchQuery" />
          </div>
          <button class="btn-create-jira"><i class="fa-solid fa-plus"></i> Tạo mới</button>
        </div>
      </div>

      <div class="nav-right">

        <div class="nav-icon bot-icon active" @click="toggleAIView">
          <i class="fa-solid fa-robot"></i>
        </div>

        <NotificationsDropdown />
        <HelpDropdown />
        <SettingsDropdown />
        <UserDropdown />
      </div>
    </header>

    <div class="main-body">
      <!-- Sidebar -->
      <aside class="sidebar">
        <ul class="side-menu">
          <li @click="goToDashboard"><i class="fa-solid fa-border-all"></i> Dành cho bạn</li>
          <li @click="goToSpace"><i class="fa-regular fa-folder-open"></i> Không gian</li>
          <li><i class="fa-regular fa-clock"></i> Gần đây</li>
          <li class="active"><i class="fa-solid fa-robot"></i> Trợ lý AI</li>
          <li><i class="fa-solid fa-ellipsis"></i> Thêm</li>
        </ul>

        <div class="sidebar-section">
          <div class="section-label">YÊU THÍCH</div>
          <ul class="side-menu">
            <li><span class="dot blue"></span> Phát triển Mobile App</li>
            <li><span class="dot orange"></span> Marketing Quý 1</li>
          </ul>
        </div>

        <div class="sidebar-footer">
          <div class="user-card-mini">
            <div class="user-avatar-small">DN</div>
            <div class="user-info">
              <div class="name">Duy Nghĩa</div>
              <div class="plan">Gói miễn phí</div>
            </div>
            <i class="fa-solid fa-chevron-up"></i>
          </div>
        </div>
      </aside>

      <!-- AI Content Area -->
      <main class="content-area">
        <div class="ai-container">
          <!-- AI Header Tabs -->
          <div class="ai-page-header">
            <h2 class="page-title">Trợ lý AI</h2>
            <div class="jira-tabs">
              <div class="jira-tab">Tổng quan</div>
              <div class="jira-tab">Tồn đọng</div>
              <div class="jira-tab">Bảng</div>
              <div class="jira-tab active">Lịch</div>
              <div class="jira-tab">Lộ trình</div>
              <div class="jira-tab">Trang</div>
              <div class="jira-tab">Biểu mẫu</div>
            </div>
          </div>

          <!-- Chat History -->
          <div class="chat-history">
            <!-- Bot Welcome -->
            <div class="chat-row bot">
              <div class="bot-icon-circle"><i class="fa-solid fa-robot"></i></div>
              <div class="bubble">
                Xin chào! Tôi là trợ lý AI của SprintA. Tôi có thể giúp bạn tổ chức dự án, tóm tắt các luồng thảo luận dài, hoặc tạo lộ trình. Tôi có thể giúp gì cho bạn hôm nay?
              </div>
            </div>

            <!-- User Query -->
            <div class="chat-row user">
              <div class="bubble primary">
                Bạn có thể giúp tôi soạn bản thảo cập nhật phát hành cho dự án Phát triển Mobile App không? Tập trung vào mô-đun xác thực mới mà chúng tôi đã hoàn thành tuần trước.
              </div>
              <div class="user-avatar-circle">DN</div>
            </div>

            <!-- Bot Response -->
            <div class="chat-row bot">
              <div class="bot-icon-circle"><i class="fa-solid fa-robot"></i></div>
              <div class="bubble">
                <p>Chắc chắn rồi! Dựa trên hoạt động trong không gian 'Phát triển Mobile App', đây là bản thảo cho cập nhật phát hành của bạn:</p>
                <div class="draft-box">
                  <strong>Ghi chú phát hành: v2.4.0 - Tăng cường bảo mật</strong><br/>
                  • Triển khai luồng xác thực OAuth2.0.<br/>
                  • Thêm hỗ trợ đăng nhập sinh trắc học (FaceID/TouchID).<br/>
                  • Cấu trúc lại quản lý phiên để tối ưu hiệu suất.
                </div>
                <p>Bạn có muốn tôi đăng nội dung này lên kênh thông báo không?</p>
              </div>
            </div>
          </div>

          <!-- Chat Input -->
          <div class="ai-chat-input-wrapper">
            <div class="input-box">
              <i class="fa-solid fa-paperclip attach-btn"></i>
              <input type="text" placeholder="Hỏi SprintA AI bất cứ điều gì..." />
              <button class="send-btn"><span class="fa fa-paper-plane"></span></button>
            </div>
            <div class="ai-disclaimer">SprintA AI có thể mắc sai sót. Hãy kiểm tra lại các thông tin quan trọng.</div>
          </div>
        </div>
      </main>

      <!-- Right Sidebar: AI Details -->
      <aside class="ai-details-panel">
        <div class="panel-section">
          <div class="section-label">Trợ lý AI</div>
          <div class="section-title">HÀNH ĐỘNG NHANH</div>
          <div class="quick-links">
            <div class="q-link"><i class="fa-solid fa-map-location-dot"></i> Tạo lộ trình</div>
            <div class="q-link"><i class="fa-regular fa-file-lines"></i> Tóm tắt công việc</div>
            <div class="q-link"><i class="fa-solid fa-pen-nib"></i> Soạn bản cập nhật</div>
          </div>
        </div>

        <div class="panel-section mt-30">
          <div class="section-title">NGỮ CẢNH GẦN ĐÂY</div>
          <div class="context-item">
            <div class="c-icon blue"><i class="fa-solid fa-layer-group"></i></div>
            <div class="c-text">
              <div class="c-name">Mô-đun xác thực</div>
              <div class="c-time">Cập nhật 2 giờ trước</div>
            </div>
          </div>
          <div class="context-item">
            <div class="c-icon purple"><i class="fa-solid fa-database"></i></div>
            <div class="c-text">
              <div class="c-name">Backend API</div>
              <div class="c-time">Chờ đồng bộ</div>
            </div>
          </div>
        </div>

        <!-- Upgrade Card -->
        <div class="upgrade-card-wrapper">
          <div class="upgrade-card">
            <div class="plan-label">GÓI PRO</div>
            <div class="plan-desc">Mở khóa các truy vấn AI không giới hạn và quy trình làm việc tùy chỉnh.</div>
            <button class="btn-upgrade">Nâng cấp ngay</button>
          </div>
        </div>
      </aside>
    </div>

    <!-- AI Sidebar (Reuse Popup component logic) -->
    <transition name="slide-right">
      <aside class="ai-sidebar popup" v-if="aiVisible">
        <div class="ai-header">
          <h4><i class="fa-solid fa-robot"></i> AI Chat</h4>
          <i class="fa-solid fa-xmark" style="color: #64748b; cursor: pointer" @click="toggleAI"></i>
        </div>
        <div class="ai-content">
          <div class="chat-message bot">
            <div class="avatar-bot"><i class="fa-solid fa-robot"></i></div>
            <div class="message-bubble">Bạn đang ở trang trợ lý AI toàn màn hình. Bạn có thể sử dụng cửa sổ nhỏ này để chat nhanh khi đang ở các trang khác!</div>
          </div>
        </div>
      </aside>
    </transition>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import logoImg from '../assets/logo_QLCV.png'
import HelpDropdown from '../components/HelpDropdown.vue'
import SettingsDropdown from '../components/SettingsDropdown.vue'
import NotificationsDropdown from '../components/NotificationsDropdown.vue'
import UserDropdown from '../components/UserDropdown.vue'

const router = useRouter()
const searchQuery = ref('')
const aiVisible = ref(false)

const toggleAI = () => {
  aiVisible.value = !aiVisible.value
}

const toggleAIView = () => {
  // Toggle the popup even on this page if requested
  toggleAI()
}

const goToDashboard = () => {
  router.push('/dashboard')
}

const goToSpace = () => {
  router.push('/space/my-team')
}
</script>

<style scoped>
.dashboard-layout {
  height: 100vh;
  display: flex;
  flex-direction: column;
  background-color: #0c101a; 
  color: #f1f5f9; 
  overflow: hidden;
}

.top-nav {
  height: 56px;
  background-color: #0c101a; 
  border-bottom: 1px solid #1e293b;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 20px;
  flex-shrink: 0;
}

.nav-left, .nav-right {
  display: flex;
  align-items: center;
  gap: 16px;
  width: 300px; /* Keep sides fixed width to help center */
}

.nav-right {
  justify-content: flex-end;
}

.nav-center {
  flex: 1;
  display: flex;
  justify-content: center;
  align-items: center;
}

.nav-brand {
  display: flex;
  align-items: center;
  gap: 8px;
  color: white;
  text-decoration: none;
  font-weight: 800;
  font-size: 20px;
}
.nav-logo { height: 28px; }

.nav-link { font-size: 14px; color: #94a3b8; border-left: 1px solid #334155; padding-left: 16px; }

.top-search-create {
  display: flex;
  align-items: center;
  gap: 8px; 
}

.search-input-mock {
  display: flex;
  align-items: center;
  background-color: #22272b;
  border: 1px solid #738496; 
  border-radius: 4px;
  padding: 0 12px;
  width: 550px;
  height: 32px;
}
.search-input-mock i { color: #8c9bab; font-size: 14px; margin-right: 8px; }
.search-input-mock input { background: transparent; border: none; color: #f4f5f7; font-size: 14px; width: 100%; outline: none; }

.btn-create-jira {
  background-color: #579dff; color: #1d2125; border: none; border-radius: 4px;
  padding: 0 16px; height: 32px; font-size: 14px; font-weight: 500; cursor: pointer;
}

.nav-icon {
  color: #94a3b8; font-size: 18px; cursor: pointer; width: 32px; height: 32px;
  display: flex; align-items: center; justify-content: center; border-radius: 50%;
}
.nav-icon:hover { background-color: #1e293b; color: white; }
.nav-icon.active { color: #579dff; background-color: #1a2436; }

.user-avatar {
  background: #fdbba7; color: #1d2125; width: 32px; height: 32px; border-radius: 50%;
  display: flex; align-items: center; justify-content: center; font-weight: 700; font-size: 12px;
}

.main-body { display: flex; flex: 1; overflow: hidden; }

.sidebar { width: 260px; background-color: #0c101a; border-right: 1px solid #1e293b; padding: 16px; display: flex; flex-direction: column;}
.side-menu { list-style: none; padding: 0; margin: 0 0 20px 0; }
.side-menu li { padding: 10px 12px; border-radius: 6px; color: #cbd5e1; font-size: 14px; font-weight: 500; margin-bottom: 4px; cursor: pointer; display: flex; align-items: center; gap: 12px; }
.side-menu li:hover { background-color: #1e293b; color: white; }
.side-menu li.active { background-color: #1a2a47; color: #579dff; }

.sidebar-section { margin-top: 20px; }
.section-label { font-size: 11px; color: #64748b; font-weight: 700; letter-spacing: 0.5px; padding: 8px 12px; text-transform: uppercase; }
.dot { width: 8px; height: 8px; border-radius: 50%; }
.dot.blue { background: #3b82f6; }
.dot.orange { background: #f97316; }

.sidebar-footer { margin-top: auto; border-top: 1px solid #1e293b; padding-top: 16px; }
.user-card-mini { display: flex; align-items: center; gap: 12px; padding: 8px; border-radius: 8px; cursor: pointer; }
.user-card-mini:hover { background: #1e293b; }
.user-avatar-small { width: 28px; height: 28px; background: #579dff; border-radius: 4px; display: flex; align-items: center; justify-content: center; font-size: 11px; font-weight: 700; color: #1d2125; }
.user-info { flex: 1; }
.user-info .name { font-size: 13px; font-weight: 600; color: white; }
.user-info .plan { font-size: 11px; color: #64748b; }

.content-area { flex: 1; background-color: #0c111d; display: flex; justify-content: center; padding: 0; overflow-y: auto;}
.ai-container { width: 100%; max-width: 900px; display: flex; flex-direction: column; height: 100%; }

.ai-page-header { padding: 32px 40px 0; border-bottom: 1px solid #1e293b; }
.page-title { font-size: 20px; font-weight: 600; margin-bottom: 24px; color: #f4f5f7; }

.jira-tabs { display: flex; gap: 24px; }
.jira-tab { padding: 8px 0 12px; color: #94a3b8; font-size: 14px; cursor: pointer; position: relative; }
.jira-tab.active { color: #579dff; }
.jira-tab.active::after { content: ''; position: absolute; bottom: -1px; left: 0; right: 0; height: 2px; background: #579dff; }

.chat-history { flex: 1; padding: 40px; display: flex; flex-direction: column; gap: 32px; overflow-y: auto; }
.chat-row { display: flex; gap: 16px; max-width: 85%; }
.chat-row.user { align-self: flex-end; flex-direction: row-reverse; }

.bot-icon-circle { width: 32px; height: 32px; background: #1d2a4a; border: 1px solid #334155; border-radius: 4px; display: flex; align-items: center; justify-content: center; color: #579dff; flex-shrink: 0; }
.user-avatar-circle { width: 32px; height: 32px; background: #579dff; border-radius: 4px; display: flex; align-items: center; justify-content: center; color: #0c101a; font-weight: 700; font-size: 11px; flex-shrink: 0; }

.bubble { background: #1e2430; padding: 16px 20px; border-radius: 4px 16px 16px 16px; color: #e2e8f0; font-size: 14px; line-height: 1.5; }
.bubble.primary { background: #3b82f6; color: white; border-radius: 16px 4px 16px 16px; }

.draft-box { background: #0c111d; border: 1px solid #334155; border-radius: 8px; padding: 16px; margin: 12px 0; font-family: 'Inter', sans-serif; font-size: 14px; }

.ai-chat-input-wrapper { padding: 24px 40px 32px; }
.input-box { background: #1e2430; border: 1px solid #334155; border-radius: 8px; display: flex; align-items: center; padding: 12px 20px; gap: 12px; }
.input-box input { flex: 1; background: transparent; border: none; color: white; outline: none; font-size: 14px; }
.attach-btn { color: #94a3b8; cursor: pointer; }
.send-btn { background: #579dff; border: none; width: 32px; height: 32px; border-radius: 4px; color: #1d2125; cursor: pointer; }

.ai-disclaimer { text-align: center; font-size: 11px; color: #64748b; margin-top: 12px; }

/* AI Details Panel */
.ai-details-panel { width: 320px; background: #0c101a; border-left: 1px solid #1e293b; padding: 32px 24px; display: flex; flex-direction: column; }
.panel-section { margin-bottom: 40px; }
.panel-section .section-title { font-size: 11px; color: #64748b; font-weight: 700; margin-bottom: 16px; text-transform: uppercase; }

.quick-links { display: flex; flex-direction: column; gap: 8px; }
.q-link { background: transparent; border: 1px solid #1e293b; border-radius: 8px; padding: 12px 16px; font-size: 13px; color: #cbd5e1; display: flex; align-items: center; gap: 12px; cursor: pointer; }
.q-link:hover { border-color: #579dff; color: white; }
.q-link i { color: #579dff; font-size: 14px; }

.context-item { display: flex; align-items: center; gap: 12px; margin-bottom: 20px; cursor: pointer; }
.c-icon { width: 36px; height: 36px; border-radius: 4px; display: flex; align-items: center; justify-content: center; background: #1a2436; font-size: 14px; }
.c-icon.blue { color: #3b82f6; }
.c-icon.purple { color: #a855f7; }
.c-text .c-name { font-size: 14px; font-weight: 500; color: #f4f5f7; }
.c-text .c-time { font-size: 12px; color: #64748b; }

.upgrade-card-wrapper { margin-top: auto; }
.upgrade-card { background: linear-gradient(135deg, #1e293b 0%, #0f111a 100%); border: 1px solid #579dff66; border-radius: 12px; padding: 20px; text-align: left; }
.plan-label { font-size: 11px; font-weight: 800; color: #579dff; margin-bottom: 8px; }
.plan-desc { font-size: 12px; color: #cbd5e1; line-height: 1.4; margin-bottom: 16px; }
.btn-upgrade { width: 100%; background: #3b82f6; border: none; color: white; padding: 10px; border-radius: 6px; font-weight: 600; font-size: 13px; cursor: pointer; }
.btn-upgrade:hover { background: #2563eb; }

/* AI Popup Sidebar */
.ai-sidebar.popup { width: 420px; background-color: #131824; border-left: 1px solid #1e293b; position: fixed; top: 56px; right: 0; bottom: 0; z-index: 100; }
.ai-header { padding: 16px 20px; border-bottom: 1px solid #1e293b; display: flex; justify-content: space-between; align-items: center; }
.ai-header h4 { font-size: 14px; margin: 0; color: #f8fafc; display: flex; align-items: center; gap: 8px; }
.ai-header h4 i { color: #579dff; }
.ai-content { padding: 20px; }
.chat-message { display: flex; gap: 12px; }
.avatar-bot { width: 28px; height: 28px; border-radius: 50%; background-color: #3b82f6; color: white; display: flex; align-items: center; justify-content: center; }
.message-bubble { background-color: #1e293b; padding: 14px; border-radius: 4px 16px 16px 16px; color: #e2e8f0; font-size: 13px; line-height: 1.5; }

.slide-right-enter-active, .slide-right-leave-active { transition: transform 0.3s ease; }
.slide-right-enter-from, .slide-right-leave-to { transform: translateX(100%); }

.mt-30 { margin-top: 30px; }
</style>
