<template>
  <div class="dashboard-layout">
    <!-- Navbar -->
    <header class="top-nav">
      <div class="nav-left">
        <div class="menu-toggle mobile-only" @click="sidebarVisible = !sidebarVisible">
          <i class="fa-solid fa-bars"></i>
        </div>
        <router-link to="/dashboard" class="nav-brand">
          <img :src="logoImg" alt="SprintA Logo" class="nav-logo" />
          <span class="desktop-only">SprintA</span>
        </router-link>
        <span class="nav-link active desktop-only">Dự án</span>
      </div>

      <div class="nav-center desktop-only">
        <div class="top-search-create">
          <div class="search-input-mock">
            <i class="fa-solid fa-magnifying-glass" style="margin-right: 8px;"></i>
            <input type="text" placeholder="Tìm kiếm" v-model="searchQuery" />
          </div>
          <button class="btn-create-jira"><i class="fa-solid fa-plus"></i> Tạo mới</button>
        </div>
      </div>

      <!-- Task Detail Modal Overlay -->
      <transition name="fade">
        <div class="task-modal-overlay" v-if="showTaskModal" @click.self="showTaskModal = false">
          <div class="task-modal">
            <!-- Modal Header -->
            <header class="modal-header">
              <div class="header-left">
                <i class="fa-solid fa-table-columns"></i>
                <span class="m-crumb">Không gian nhóm</span>
                <i class="fa-solid fa-chevron-right separator"></i>
                <span class="m-crumb">Dự án</span>
                <i class="fa-solid fa-chevron-right separator"></i>
                <span class="m-crumb current">Dự án 1</span>
                <i class="fa-solid fa-plus add-crumb"></i>
                <i class="fa-solid fa-copy copy-crumb"></i>
              </div>
              <div class="header-right">
                <span class="created-at">Đã tạo vào 18 tháng 3</span>
                <div class="btn-ai-header"><i class="fa-solid fa-microchip"></i> Hỏi AI</div>
                <div class="btn-share"><i class="fa-solid fa-share-nodes"></i> Chia sẻ</div>
                <i class="fa-solid fa-ellipsis m-more"></i>
                <i class="fa-solid fa-star m-fav"></i>
                <i class="fa-solid fa-thumbtack m-pin"></i>
                <i class="fa-solid fa-chevron-right m-side-toggle"></i>
                <i class="fa-solid fa-xmark m-close" @click="showTaskModal = false"></i>
              </div>
            </header>

            <div class="modal-body-wrapper">
              <!-- Left Content -->
              <div class="modal-main">
                <div class="task-id-row">
                  <div class="status-badge-small"><i class="fa-solid fa-circle"></i> Trạng thái</div>
                  <span class="task-id-text">SprintA-123</span>
                  <div class="btn-ai-mini"><i class="fa-solid fa-wand-magic-sparkles"></i> Hỏi AI</div>
                </div>

                <h1 class="task-modal-title">{{ selectedTask?.name }}</h1>

                <div class="ai-prompt-bar">
                   <div class="sparkle-icon"><i class="fa-solid fa-wand-magic-sparkles"></i></div>
                   <input type="text" placeholder="Yêu cầu Brain viết mô tả, tạo công việc con hoặc tìm các công việc tương tự" />
                </div>

                <!-- Attributes Grid -->
                <div class="attributes-grid">
                  <div class="attr-item">
                    <div class="attr-label"><i class="fa-solid fa-circle-half-stroke"></i> Trạng thái</div>
                    <div class="attr-value">
                      <div class="status-pill in-progress">
                        <i class="fa-solid fa-circle-half-stroke"></i> ĐANG THỰC HIỆN <i class="fa-solid fa-chevron-down"></i>
                      </div>
                      <i class="fa-solid fa-circle-check check-confirm"></i>
                    </div>
                  </div>
                  <div class="attr-item">
                    <div class="attr-label"><i class="fa-regular fa-user"></i> Người thực hiện</div>
                    <div class="attr-value muted">Trống</div>
                  </div>
                  <div class="attr-item">
                    <div class="attr-label"><i class="fa-regular fa-calendar"></i> Ngày tháng</div>
                    <div class="attr-value"><i class="fa-solid fa-calendar-plus"></i> Bắt đầu -> Hạn chót</div>
                  </div>
                  <div class="attr-item">
                    <div class="attr-label"><i class="fa-regular fa-flag"></i> Độ ưu tiên</div>
                    <div class="attr-value muted">Trống</div>
                  </div>
                  <div class="attr-item">
                    <div class="attr-label"><i class="fa-regular fa-hourglass-half"></i> Thời gian dự kiến</div>
                    <div class="attr-value muted">Trống</div>
                  </div>
                  <div class="attr-item">
                    <div class="attr-label"><i class="fa-regular fa-clock"></i> Theo dõi thời gian</div>
                    <div class="attr-value"><i class="fa-regular fa-circle-play"></i> Thêm thời gian</div>
                  </div>
                  <div class="attr-item">
                    <div class="attr-label"><i class="fa-solid fa-tag"></i> Thẻ</div>
                    <div class="attr-value muted">Trống</div>
                  </div>
                  <div class="attr-item">
                    <div class="attr-label"><i class="fa-solid fa-share-nodes"></i> Mối quan hệ</div>
                    <div class="attr-value muted">Trống</div>
                  </div>
                </div>

                <!-- Description Areas -->
                <div class="content-section">
                  <div class="section-link"><i class="fa-regular fa-file-lines"></i> Thêm mô tả</div>
                  <div class="section-link ai-link"><i class="fa-solid fa-wand-magic-sparkles"></i> Viết bằng AI</div>
                </div>

                <div class="fields-section">
                  <h3>Thêm trường dữ liệu</h3>
                  <button class="add-field-btn"><i class="fa-solid fa-plus"></i> Tạo một trường trong danh sách này</button>
                </div>
              </div>

              <!-- Right Activity Panel -->
              <div class="modal-sidebar">
                <div class="sidebar-header">
                  <h2>Hoạt động</h2>
                  <div class="header-tools">
                    <i class="fa-solid fa-magnifying-glass"></i>
                    <i class="fa-solid fa-arrow-down-wide-short"></i>
                    <i class="fa-solid fa-comments"></i> 1
                    <i class="fa-solid fa-bars-staggered"></i>
                  </div>
                </div>

                <div class="activity-scroll">
                  <div class="comment-card">
                    <div class="c-head">
                      <div class="avatar-sm">DN</div>
                      <div class="c-user">Danh Nguyễn <span class="c-time">13 phút trước</span></div>
                    </div>
                    <div class="c-body">dsa</div>
                    <div class="c-foot">
                       <div class="c-actions">
                         <i class="fa-regular fa-thumbs-up"></i>
                         <i class="fa-regular fa-face-smile"></i>
                       </div>
                       <div class="c-rep">Trả lời</div>
                    </div>
                  </div>
                </div>

                <div class="activity-input">
                  <div class="input-container">
                    <textarea placeholder="Viết bình luận..."></textarea>
                    <div class="input-actions-bar">
                      <div class="bar-left">
                        <i class="fa-solid fa-plus"></i>
                        <button class="btn-comment-type">Bình luận <i class="fa-solid fa-chevron-down"></i></button>
                        <i class="fa-solid fa-wand-magic-sparkles ai"></i>
                        <i class="fa-solid fa-at"></i>
                        <i class="fa-solid fa-paperclip"></i>
                        <i class="fa-solid fa-at"></i>
                        <i class="fa-regular fa-face-smile"></i>
                        <i class="fa-solid fa-ellipsis"></i>
                      </div>
                      <div class="bar-right">
                        <i class="fa-solid fa-paper-plane send-enabled"></i>
                        <i class="fa-solid fa-chevron-down"></i>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </transition>

      <div class="nav-right">
        <div class="nav-icon bot-icon" @click="toggleAI" :class="{ 'active': aiVisible }">
          <i class="fa-solid fa-robot"></i>
        </div>

        <NotificationsDropdown class="desktop-only" />
        <SettingsDropdown class="desktop-only" />
        <HelpDropdown class="desktop-only" />
        <UserDropdown />
      </div>
    </header>

    <div class="main-body">
      <!-- Sidebar -->
      <aside class="sidebar" :class="{ 'show': sidebarVisible }">
        <ul class="side-menu">
          <li @click="goToDashboard"><i class="fa-solid fa-border-all"></i> Dành cho bạn</li>
          <li class="active"><i class="fa-regular fa-folder-open"></i> Không gian</li>
          <li><i class="fa-regular fa-clock"></i> Gần đây</li>
          <li class="ai-item" @click="goToAI"><i class="fa-solid fa-robot"></i> Trợ lý AI</li>
          <li><i class="fa-solid fa-ellipsis"></i> Thêm</li>
        </ul>
      </aside>

      <main class="content-area">
        <div class="content-wrapper">
          <div class="page-header">
            <h1 class="page-title">Bảng điều hướng dự án</h1>
          </div>

          <!-- Tabs -->
          <div class="jira-tabs">
            <div class="jira-tab" :class="{ active: currentTab === 'list' }" @click="currentTab = 'list'">Danh sách</div>
            <div class="jira-tab" :class="{ active: currentTab === 'summary' }" @click="currentTab = 'summary'">Tổng quan</div>
            <div class="jira-tab" :class="{ active: currentTab === 'backlog' }" @click="currentTab = 'backlog'">Tồn đọng</div>
            <div class="jira-tab" :class="{ active: currentTab === 'calendar' }" @click="currentTab = 'calendar'">Lịch</div>
            <div class="jira-tab" :class="{ active: currentTab === 'timeline' }" @click="currentTab = 'timeline'">Lộ trình</div>
            <div class="jira-tab" :class="{ active: currentTab === 'page' }" @click="currentTab = 'page'">Trang</div>
            <div class="jira-tab" :class="{ active: currentTab === 'forms' }" @click="currentTab = 'forms'">Biểu mẫu</div>
            <div class="tab-spacer"></div>
            <div class="jira-tab-icon"><i class="fa-solid fa-filter"></i> Bộ lọc</div>
            <div class="jira-tab-icon"><i class="fa-solid fa-sort"></i> Sắp xếp</div>
          </div>

          <!-- TAB CONTENT: SUMMARY (Grid Charts) -->
          <div class="summary-content" v-if="currentTab === 'summary'">
            <div class="filter-bar">
              <el-button size="small" plain class="dark-btn"><i class="fa-solid fa-filter"></i> Filter</el-button>
            </div>

            <!-- Top 4 Widgets -->
            <div class="top-widgets">
              <div class="widget">
                <i class="fa-regular fa-circle-check widget-icon" style="color: #94a3b8"></i>
                <div class="widget-info">
                  <div class="num">0 completed</div>
                  <div class="sub">in the last 7 days</div>
                </div>
              </div>
              <div class="widget">
                <i class="fa-solid fa-pen widget-icon" style="color: #94a3b8"></i>
                <div class="widget-info">
                  <div class="num">0 updated</div>
                  <div class="sub">in the last 7 days</div>
                </div>
              </div>
              <div class="widget">
                <i class="fa-regular fa-square-plus widget-icon" style="color: #94a3b8"></i>
                <div class="widget-info">
                  <div class="num">0 created</div>
                  <div class="sub">in the last 7 days</div>
                </div>
              </div>
              <div class="widget">
                <i class="fa-regular fa-calendar widget-icon" style="color: #f59e0b"></i>
                <div class="widget-info">
                  <div class="num">0 due soon</div>
                  <div class="sub">in the next 7 days</div>
                </div>
              </div>
            </div>

            <!-- Dashboard Charts Grid -->
            <div class="charts-grid">
              
              <!-- Status overview -->
              <div class="chart-card">
                <div class="chart-header">
                  <h4>Tổng quan trạng thái</h4>
                  <p>Xem nhanh trạng thái các công việc của bạn. <a href="#">Xem tất cả</a></p>
                </div>
                <div class="chart-body donut-body">
                  <div class="donut-chart">
                    <div class="donut-ring"></div>
                    <div class="donut-center">
                      <span class="val">0</span>
                      <span class="lbl">Tổng số công việc</span>
                    </div>
                  </div>
                  <div class="donut-legend">
                    <div class="leg-item"><span class="dot" style="background:#3b82f6"></span> Đang thực hiện: 0</div>
                    <div class="leg-item"><span class="dot" style="background:#84cc16"></span> Cần làm: 0</div>
                  </div>
                </div>
              </div>

              <!-- Activity (Empty) -->
              <div class="chart-card empty-card">
                <i class="fa-solid fa-layer-group empty-icon"></i>
                <h4>Chưa có hoạt động</h4>
                <p>Tạo một vài công việc và mời đồng nghiệp vào không gian của bạn để xem hoạt động.</p>
              </div>

              <!-- Priority breakdown -->
              <div class="chart-card">
                <div class="chart-header">
                  <h4>Phân bổ độ ưu tiên</h4>
                  <p>Xem cái nhìn tổng thể về cách ưu tiên công việc. <a href="#">Cách quản lý ưu tiên</a></p>
                </div>
                <div class="chart-body bar-chart">
                  <div class="bar-y">
                    <span>4</span><span>3</span><span>2</span><span>1</span><span>0</span>
                  </div>
                  <div class="bar-plot">
                    <div class="bar-col"><div class="bar-fill" style="height:0"></div></div>
                    <div class="bar-col"><div class="bar-fill" style="height:0"></div></div>
                    <div class="bar-col"><div class="bar-fill" style="height:0"></div></div>
                    <div class="bar-col"><div class="bar-fill" style="height:0"></div></div>
                    <div class="bar-col"><div class="bar-fill" style="height:0"></div></div>
                    <div class="bar-col"><div class="bar-fill" style="height:0%;background:#64748b"></div></div>
                  </div>
                  <div class="bar-x">
                    <span style="color:#ef4444">︽ Cao nhất</span>
                    <span style="color:#f97316">^ Cao</span>
                    <span style="color:#eab308">= Trung bình</span>
                    <span style="color:#3b82f6">v Thấp</span>
                    <span style="color:#93c5fd">︾ Thấp nhất</span>
                    <span>- Không có</span>
                  </div>
                </div>
              </div>

              <!-- Types of work -->
              <div class="chart-card">
                <div class="chart-header">
                  <h4>Loại hình công việc</h4>
                  <p>Xem phân bổ các công việc theo loại. <a href="#">Xem tất cả</a></p>
                </div>
                <div class="chart-body type-chart">
                  <div class="type-header-row"><span class="th">Loại</span><span class="th">Phân bổ</span></div>
                  
                  <div class="type-row">
                    <div class="t-name"><i class="fa-solid fa-square-check" style="color:#3b82f6"></i> Công việc</div>
                    <div class="t-dist"><div class="t-prog" style="width:0%">0%</div></div>
                  </div>
                  <div class="type-row">
                    <div class="t-name"><i class="fa-solid fa-diagram-project" style="color:#0ea5e9"></i> Việc con</div>
                    <div class="t-dist"><div class="t-prog" style="width:0%">0%</div></div>
                  </div>
                  <div class="type-row">
                    <div class="t-name"><i class="fa-solid fa-bolt" style="color:#a855f7"></i> Epic</div>
                    <div class="t-dist"><div class="t-prog" style="width:0%">0%</div></div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- TAB CONTENT: BOARD (Kanban) - Managed via Backlog Tab -->
          <div class="board-content" v-if="currentTab === 'backlog'">
            <div class="board-toolbar">
              <div class="board-search">
                <i class="fa-solid fa-magnifying-glass"></i>
                <input type="text" placeholder="Tìm kiếm trong bảng..." />
              </div>
              <div class="board-avatars">
                <div class="avatar-stack">
                  <div class="stack-item" style="background: #3b82f6;">DN</div>
                  <div class="stack-item" style="background: #a855f7;">AN</div>
                  <div class="stack-item" style="background: #f59e0b;">TN</div>
                  <div class="stack-count">+0</div>
                </div>
              </div>
              <el-button size="small" plain class="dark-btn"><i class="fa-solid fa-filter"></i> Bộ lọc</el-button>
              <div class="board-grouping">
                <span>Nhóm theo:</span>
                <el-button size="small" plain class="dark-btn">Trạng thái <i class="fa-solid fa-chevron-down"></i></el-button>
              </div>
              <el-button size="small" type="primary" class="sprint-btn">Hoàn thành Sprint</el-button>
            </div>

            <div class="kanban-board">
              <!-- TO DO -->
              <div class="kanban-column">
                <div class="column-header">
                  <span class="column-title">TO DO</span>
                  <span class="column-count-badge">0</span>
                </div>
                <div class="kanban-cards">
                  <div class="btn-create-card"><i class="fa-solid fa-plus"></i> Create</div>
                </div>
              </div>

              <!-- IN PROGRESS -->
              <div class="kanban-column">
                <div class="column-header">
                  <span class="column-title">IN PROGRESS</span>
                  <span class="column-count-badge">0</span>
                  <i class="fa-solid fa-ellipsis header-more"></i>
                </div>
                <div class="kanban-cards">
                  <div class="btn-create-card"><i class="fa-solid fa-plus"></i> Create</div>
                </div>
              </div>

              <!-- DONE -->
              <div class="kanban-column">
                <div class="column-header">
                  <span class="column-title">DONE</span>
                  <i class="fa-solid fa-check-double done-icon"></i>
                </div>
                <div class="kanban-cards">
                  <div class="btn-create-card"><i class="fa-solid fa-plus"></i> Create</div>
                </div>
              </div>

              <!-- ADD STATUS COLUMN BUTTON -->
              <div class="add-column-box">
                <div class="add-column-btn"><i class="fa-solid fa-plus"></i></div>
              </div>
            </div>
          </div>

          <!-- TAB CONTENT: CALENDAR -->
          <div class="calendar-content" v-if="currentTab === 'calendar'">
            <div class="calendar-header-toolbar">
              <h2 class="calendar-month-title">Tháng 3, 2026</h2>
              <div class="calendar-nav-controls">
                <div class="btn-group-jira">
                  <el-button size="small" class="jira-control-btn"><i class="fa-solid fa-chevron-left"></i></el-button>
                  <el-button size="small" class="jira-control-btn today-btn">Hôm nay</el-button>
                  <el-button size="small" class="jira-control-btn"><i class="fa-solid fa-chevron-right"></i></el-button>
                </div>
              </div>
              <div class="calendar-view-toggle">
                <div class="toggle-group-jira">
                  <div class="toggle-item active">Tháng</div>
                  <div class="toggle-item">Tuần</div>
                  <div class="toggle-item">Ngày</div>
                </div>
              </div>
            </div>

            <div class="calendar-grid-container">
              <!-- Weekday Headers -->
              <div class="calendar-week-labels">
                <div class="weekday-label">CN</div>
                <div class="weekday-label">THỨ 2</div>
                <div class="weekday-label">THỨ 3</div>
                <div class="weekday-label">THỨ 4</div>
                <div class="weekday-label">THỨ 5</div>
                <div class="weekday-label">THỨ 6</div>
                <div class="weekday-label">THỨ 7</div>
              </div>

              <!-- Days Grid -->
              <div class="calendar-days-grid">
                <!-- Row 1 -->
                <div class="day-cell muted">24</div>
                <div class="day-cell muted">25</div>
                <div class="day-cell muted">26</div>
                <div class="day-cell muted">27</div>
                <div class="day-cell muted">28</div>
                <div class="day-cell">1</div>
                <div class="day-cell">2</div>

                <!-- Row 2 -->
                <div class="day-cell">3</div>
                <div class="day-cell">4</div>
                <div class="day-cell">5</div>
                <div class="day-cell">6</div>
                <div class="day-cell">7</div>
                <div class="day-cell">8</div>
                <div class="day-cell">9</div>

                <!-- Row 3 -->
                <div class="day-cell">10</div>
                <div class="day-cell">11</div>
                <div class="day-cell today">
                  <span class="day-num-circle">12</span>
                </div>
                <div class="day-cell">13</div>
                <div class="day-cell">14</div>
                <div class="day-cell">15</div>
                <div class="day-cell">16</div>

                <!-- Row 4 -->
                <div class="day-cell">17</div>
                <div class="day-cell">18</div>
                <div class="day-cell">19</div>
                <div class="day-cell">20</div>
                <div class="day-cell">21</div>
                <div class="day-cell">22</div>
                <div class="day-cell">23</div>
              </div>
            </div>
          </div>

          <!-- TAB CONTENT: TIMELINE (ROADMAP) -->
          <div class="timeline-content" v-if="currentTab === 'timeline'">
            <div class="timeline-toolbar">
              <div class="timeline-search">
                <i class="fa-solid fa-magnifying-glass"></i>
                <input type="text" placeholder="Tìm kiếm trong lộ trình..." />
              </div>
              <div class="avatar-circle-sm">DN</div>
              <el-button size="small" plain class="dark-btn">Danh mục trạng thái <i class="fa-solid fa-chevron-down"></i></el-button>
              <div class="toolbar-right-icons">
                <i class="fa-solid fa-sliders"></i>
                <i class="fa-solid fa-ellipsis"></i>
              </div>
            </div>

            <div class="timeline-grid-wrapper">
              <div class="timeline-left-panel">
                <div class="panel-header">Công việc</div>
                <div class="panel-sub-header">Sprints</div>
                <div class="panel-list">
                  <div class="add-epic-btn"><i class="fa-solid fa-plus"></i> Tạo Epic</div>
                </div>
              </div>
              
              <div class="timeline-main-grid">
                <!-- Timeline Headers -->
                <div class="timeline-months-row">
                  <div class="month-col">Tháng 3 '26</div>
                  <div class="month-col">Tháng 4 '26</div>
                  <div class="month-col">Tháng 5 '26</div>
                  <div class="month-col">Tháng 6 '26</div>
                  <div class="month-col">Tháng 7 '26</div>
                </div>
                
                <!-- Timeline Grid Lines -->
                <div class="timeline-rows-container">
                  <div class="timeline-task-row" v-for="i in 4" :key="'row-'+i">
                    <div class="grid-line" v-for="j in 5" :key="'line-'+j"></div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Bottom Navigation -->
            <div class="timeline-bottom-nav">
              <div class="bottom-nav-group">
                <div class="bottom-item">Hôm nay</div>
                <div class="bottom-item">Tuần</div>
                <div class="bottom-item active">Tháng</div>
                <div class="bottom-item">Quý</div>
              </div>
              <div class="bottom-nav-icons">
                <i class="fa-solid fa-circle-info"></i>
                <i class="fa-solid fa-chevron-right"></i>
              </div>
            </div>
          </div>

          <!-- TAB CONTENT: PAGE (KNOWLEDGE BASE) -->
          <div class="pages-content" v-if="currentTab === 'page'">
            <div class="pages-layout-wrapper">
              <div class="pages-main-area">
                <div class="pages-header-section">
                  <h1 class="pages-title">Trang <span class="try-badge">Dùng thử</span></h1>
                  <p class="pages-desc">Ghi lại kiến thức của nhóm và cải thiện cách bạn hoàn thành công việc.</p>
                </div>

                <!-- Browser-like Editor Window -->
                <div class="editor-window">
                  <div class="editor-window-header">
                    <div class="header-dots">
                      <span></span><span></span><span></span>
                    </div>
                  </div>
                  <div class="editor-toolbar-expanded">
                    <div class="t-left">
                      <div class="t-icon"><i class="fa-solid fa-rotate-left"></i></div>
                      <div class="t-icon"><i class="fa-solid fa-rotate-right"></i></div>
                      <div class="t-divider"></div>
                      <div class="t-dropdown">Văn bản thường <i class="fa-solid fa-chevron-down"></i></div>
                      <div class="t-divider"></div>
                      <div class="t-icon active"><i class="fa-solid fa-bold"></i></div>
                      <div class="t-icon"><i class="fa-solid fa-italic"></i></div>
                      <div class="t-icon"><i class="fa-solid fa-ellipsis"></i></div>
                      <div class="t-divider"></div>
                      <div class="t-icon"><i class="fa-solid fa-align-left"></i></div>
                      <div class="t-divider"></div>
                      <div class="t-icon"><i class="fa-solid fa-font"></i> <i class="fa-solid fa-chevron-down" style="font-size: 8px;"></i></div>
                      <div class="t-divider"></div>
                      <div class="t-icon"><i class="fa-solid fa-list-ul"></i></div>
                      <div class="t-icon"><i class="fa-solid fa-list-check"></i></div>
                      <div class="t-divider"></div>
                      <div class="t-icon"><i class="fa-solid fa-image"></i></div>
                      <div class="t-icon"><i class="fa-solid fa-link"></i></div>
                      <div class="t-icon"><i class="fa-solid fa-at"></i></div>
                      <div class="t-icon"><i class="fa-solid fa-face-smile"></i></div>
                      <div class="t-icon"><i class="fa-solid fa-plus"></i> <i class="fa-solid fa-chevron-down" style="font-size: 8px;"></i></div>
                    </div>
                    <div class="t-right">
                      <span class="saving-text">Đang lưu...</span>
                      <div class="t-avatar-circle">D</div>
                      <div class="t-icon"><i class="fa-solid fa-magnifying-glass"></i></div>
                      <div class="t-icon"><i class="fa-solid fa-circle-question"></i></div>
                      <el-button type="primary" size="small" class="publish-btn">Xuất bản</el-button>
                      <el-button size="small" plain class="close-btn">Đóng</el-button>
                      <div class="t-icon"><i class="fa-solid fa-ellipsis"></i></div>
                    </div>
                  </div>
                  <div class="editor-content-area">
                    <h2 class="editor-main-heading">Bắt đầu từ đầu với một trang trống</h2>
                    <p class="editor-main-p">Trang là nơi để nắm bắt tất cả các thông tin quan trọng của bạn. Bắt đầu với một trang trống và thêm các nội dung phong phú như công việc, hình ảnh, macro, các vấn đề của Jira Software và lộ trình.</p>
                    <p class="editor-tip"><span class="mention-tag">@nhắc_tên</span> để gắn thẻ đồng đội của bạn và cộng tác với việc cùng chỉnh sửa và bình luận trực tiếp.</p>
                  </div>
                </div>

                <!-- Footer Bar -->
                <div class="pages-footer-bar">
                  <span>Mở khóa các mẫu, trang dự án và nhiều hơn nữa.</span>
                  <div class="footer-actions">
                    <span class="learn-more">Tìm hiểu thêm</span>
                    <el-button type="primary" size="small" class="try-now-btn">Thử ngay</el-button>
                  </div>
                </div>
              </div>

              <div class="pages-template-sidebar">
                <h3 class="template-sidebar-title">Xem trước mẫu</h3>
                <p class="template-sidebar-subtitle">PHỔ BIẾN VỚI CÁC NHÓM NHƯ BẠN</p>
                <div class="template-list">
                  <div class="template-item active">
                    <div class="template-icon blue"><i class="fa-regular fa-file-lines"></i></div>
                    <div class="template-info">
                      <div class="template-name">Trang trống</div>
                      <div class="template-desc">Bắt đầu một trang từ đầu.</div>
                    </div>
                  </div>
                  <div class="template-item">
                    <div class="template-icon purple"><i class="fa-solid fa-list-check"></i></div>
                    <div class="template-info">
                      <div class="template-name">Yêu cầu sản phẩm</div>
                      <div class="template-desc">Xác định, theo dõi và phạm vi yêu cầu.</div>
                    </div>
                  </div>
                  <div class="template-item">
                    <div class="template-icon green"><i class="fa-solid fa-code-pull-request"></i></div>
                    <div class="template-info">
                      <div class="template-name">Quyết định</div>
                      <div class="template-desc">Ghi lại các quyết định dự án quan trọng.</div>
                    </div>
                  </div>
                  <div class="template-item">
                    <div class="template-icon light-blue"><i class="fa-solid fa-users"></i></div>
                    <div class="template-info">
                      <div class="template-name">Ghi chú cuộc họp</div>
                      <div class="template-desc">Lập chương trình họp, ghi chép hành động.</div>
                    </div>
                  </div>
                  <div class="template-item">
                    <div class="template-icon yellow"><i class="fa-solid fa-rotate-left"></i></div>
                    <div class="template-info">
                      <div class="template-name">Họp tổng kết</div>
                      <div class="template-desc">Điều gì đã tốt? Điều gì có thể tốt hơn?</div>
                    </div>
                  </div>
                </div>
                <div class="explore-more">
                  <i class="fa-regular fa-compass"></i> Khám phá thêm nhiều mẫu
                </div>
              </div>
            </div>
          </div>

          <!-- TAB CONTENT: FORMS (INTAKE) -->
          <div class="forms-content" v-if="currentTab === 'forms'">
            <div class="forms-hero">
              <h1 class="forms-headline">Cách đơn giản để thu thập và theo dõi yêu cầu công việc</h1>
              <p class="forms-subheadline">Sử dụng biểu mẫu để tạo công việc từ các yêu cầu của bên liên quan một cách liền mạch và tự động hóa quy trình tiếp nhận của bạn.</p>
            </div>

            <div class="forms-workflow-visual">
              <div class="visual-card">
                <div class="visual-icon"><i class="fa-regular fa-file-lines"></i></div>
                <div class="visual-label">Gửi biểu mẫu</div>
              </div>
              <div class="visual-arrow"><i class="fa-solid fa-arrow-right"></i></div>
              <div class="visual-card">
                <div class="visual-icon active"><i class="fa-regular fa-square-check"></i></div>
                <div class="visual-label">Theo dõi yêu cầu</div>
              </div>
            </div>

            <div class="forms-features-grid">
              <div class="feature-card">
                <h3 class="feature-title">Lưu lại các chi tiết quan trọng</h3>
                <p class="feature-desc">Thiết kế biểu mẫu tùy chỉnh với nhiều loại trường khác nhau để lấy thông tin mà nhóm của bạn cần để bắt đầu ngay lập tức.</p>
              </div>
              <div class="feature-card">
                <h3 class="feature-title">Ưu tiên công việc</h3>
                <p class="feature-desc">Mọi lượt gửi biểu mẫu sẽ tự động tạo một công việc mới trong dự án của bạn, sẵn sàng để nhóm của bạn giải quyết.</p>
              </div>
            </div>

            <div class="forms-cta-section">
              <el-button type="primary" class="create-form-btn"><i class="fa-solid fa-plus"></i> Tạo biểu mẫu</el-button>
            </div>
          </div>


          <div class="list-tab-wrapper" v-if="currentTab === 'list'">
            
            <div class="list-toolbar">
              <div class="toolbar-left">
                <div class="toolbar-btn primary-tint"><i class="fa-solid fa-layer-group"></i> Nhóm: Trạng thái</div>
                <div class="toolbar-btn"><i class="fa-solid fa-code-branch"></i> Việc con</div>
                <div class="toolbar-btn"><i class="fa-solid fa-columns"></i> Cột</div>
              </div>
              <div class="toolbar-right">
                <div class="toolbar-btn"><i class="fa-solid fa-filter"></i> Bộ lọc</div>
                <div class="toolbar-btn"><i class="fa-regular fa-circle-check"></i> Hoàn thành</div>
                <div class="toolbar-btn"><i class="fa-solid fa-user-plus"></i> Người thực hiện <div class="avatar-tiny">D</div></div>
                <div class="toolbar-icon"><i class="fa-solid fa-magnifying-glass"></i></div>
                <div class="toolbar-btn"><i class="fa-solid fa-gear"></i> Tùy chỉnh</div>
                <button class="add-task-white-btn">Thêm công việc <i class="fa-solid fa-chevron-down" style="font-size:10px; margin-left:6px;"></i></button>
              </div>
            </div>

            <div class="list-view-container">
              <div v-for="group in taskGroups" :key="group.id" class="task-group">
                <!-- Group Header -->
                <div class="group-header">
                  <i class="fa-solid fa-caret-down toggle-icon" 
                     @click="group.expanded = !group.expanded" 
                     :style="{ transform: group.expanded ? 'rotate(0)' : 'rotate(-90deg)' }"></i>
                     
                  <div class="group-badge" :style="{ backgroundColor: group.statusBg, color: group.statusColor }">
                    <i class="fa-regular fa-circle" v-if="group.statusText === 'TO DO'"></i>
                    <i class="fa-solid fa-circle-half-stroke" v-else></i>
                    <span>{{ group.statusText }}</span>
                  </div>
                  
                  <span class="group-count">{{ group.items.length }}</span>
                </div>

                <!-- Group Content -->
                <div class="group-content" v-show="group.expanded">
                  <!-- Table Header -->
                  <div class="list-row header-row">
                    <div class="col-name">Tên</div>
                    <div class="col-assignee">Người thực hiện</div>
                    <div class="col-date">Ngày hết hạn</div>
                    <div class="col-priority">Độ ưu tiên</div>
                    <div class="col-status">Trạng thái</div>
                    <div class="col-comments">Bình luận</div>
                    <div class="col-add"><i class="fa-regular fa-square-plus"></i></div>
                  </div>

                  <!-- Task Items -->
                  <div class="list-row task-row" v-for="task in group.items" :key="task.id">
                    <div class="col-name task-name-cell" @click="openTaskDetail(task)">
                      <i class="fa-regular fa-circle check-icon" v-if="group.statusText === 'TO DO'"></i>
                      <i class="fa-solid fa-circle-half-stroke check-icon" v-else style="color: #a855f7;"></i>
                      <span>{{ task.name }}</span>
                    </div>
                    <div class="col-assignee">
                      <i class="fa-solid fa-user-plus icon-btn"></i>
                    </div>
                    <div class="col-date">
                      <i class="fa-regular fa-calendar-plus icon-btn"></i>
                    </div>
                    <div class="col-priority">
                      <i class="fa-regular fa-flag icon-btn"></i>
                    </div>
                    <div class="col-status">
                      <div class="status-btn" :style="{ backgroundColor: group.statusBg, color: group.statusColor }">
                        <i class="fa-regular fa-circle" v-if="group.statusText === 'TO DO'"></i>
                        <i class="fa-solid fa-circle-half-stroke" v-else></i>
                        <span style="font-weight: 600">{{ task.status }}</span>
                      </div>
                    </div>
                    <div class="col-comments">
                      <el-popover
                        placement="bottom-end"
                        :width="440"
                        trigger="click"
                        popper-class="comment-popover-dark"
                      >
                        <template #reference>
                          <div class="comment-trigger-btn">
                            <i class="fa-regular fa-comment icon-btn" style="font-size: 14px;"></i>
                            <span v-if="task.commentCount" class="comment-count-text">{{ task.commentCount }}</span>
                          </div>
                        </template>

                        <div class="comment-popover-content">
                          <div class="comments-scroll-area">
                            <div class="comment-item">
                              <div class="comment-user">
                                <div class="avatar-circle">DN</div>
                                <div class="user-meta">
                                  <span class="user-name">Danh Nguyễn</span>
                                  <span class="time-stamp">Bây giờ</span>
                                </div>
                              </div>
                              <div class="comment-text">dsa</div>
                              <div class="comment-actions-row">
                                <div class="left-actions">
                                  <i class="fa-regular fa-thumbs-up"></i>
                                  <i class="fa-regular fa-face-smile"></i>
                                </div>
                                <span class="answer-link">Trả lời</span>
                              </div>
                            </div>
                          </div>

                          <div class="comment-input-section">
                            <div class="input-upper">
                              <textarea placeholder="Bình luận hoặc nhập '/' cho lệnh và hành động của AI"></textarea>
                            </div>
                            <div class="input-lower">
                              <div class="lower-left">
                                <div class="tool-btn circle"><i class="fa-solid fa-plus"></i></div>
                                <div class="tool-btn comment-type">
                                  <span>Comment</span>
                                  <i class="fa-solid fa-chevron-down"></i>
                                </div>
                                <div class="tool-icon"><i class="fa-solid fa-paperclip"></i></div>
                                <div class="tool-icon"><i class="fa-solid fa-at"></i></div>
                                <div class="tool-icon"><i class="fa-solid fa-grip"></i></div>
                                <div class="tool-icon"><i class="fa-regular fa-face-smile"></i></div>
                                <div class="tool-icon"><i class="fa-solid fa-video"></i></div>
                                <div class="tool-icon"><i class="fa-solid fa-microphone"></i></div>
                              </div>
                              <div class="lower-right">
                                <i class="fa-solid fa-paper-plane send-icon"></i>
                              </div>
                            </div>
                          </div>
                        </div>
                      </el-popover>
                    </div>
                    <div class="col-add"></div>
                  </div>

                  <!-- Add Task Row -->
                  <div class="list-row add-task-row">
                    <div class="col-name">
                      <i class="fa-solid fa-plus" style="font-size: 12px; margin-right: 8px;"></i> Thêm công việc
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </main>

      <!-- Right AI Sidebar Popup -->
      <transition name="slide-right">
        <aside class="ai-sidebar" v-if="aiVisible">
          <div class="ai-header">
            <h4><i class="fa-solid fa-robot"></i> Trợ lý AI</h4>
            <i class="fa-solid fa-xmark" style="color: #64748b; cursor: pointer" @click="toggleAI"></i>
          </div>
          
          <div class="ai-content">
            <div class="quick-actions-title">THAO TÁC NHANH</div>
            <div class="quick-actions">
              <!-- Dynamically change chips based on tab -->
              <template v-if="currentTab === 'page'">
                <el-button size="small" round plain class="ai-chip">Tóm tắt trang</el-button>
                <el-button size="small" round plain class="ai-chip">Tạo mục hành động</el-button>
                <el-button size="small" round plain class="ai-chip">Sửa giọng văn & ngữ pháp</el-button>
              </template>
              <template v-else-if="currentTab === 'forms'">
                <el-button size="small" round plain class="ai-chip">Tạo biểu mẫu</el-button>
                <el-button size="small" round plain class="ai-chip">Tóm tắt</el-button>
              </template>
              <template v-else-if="currentTab === 'timeline'">
                <el-button size="small" round plain class="ai-chip">Tóm tắt lộ trình</el-button>
                <el-button size="small" round plain class="ai-chip">Gợi ý mốc thời gian</el-button>
              </template>
              <template v-else>
                <el-button size="small" round plain class="ai-chip">Tóm tắt bảng</el-button>
                <el-button size="small" round plain class="ai-chip">Gợi ý thứ tự công việc</el-button>
                <el-button size="small" round plain class="ai-chip">Kiểm tra phụ thuộc</el-button>
              </template>
            </div>

            <div class="chat-message bot">
              <div class="avatar-bot"><i class="fa-solid fa-robot"></i></div>
              <div class="message-bubble">
                Chào bạn! Tôi có thể giúp bạn sắp xếp Không gian nhóm. Bạn có muốn tôi phân tích danh sách "CẦN LÀM" hiện tại và gợi ý độ ưu tiên không?
              </div>
            </div>
          </div>

          <div class="ai-input-area">
            <div class="ai-input-wrapper">
              <input type="text" placeholder="Hỏi AI bất cứ điều gì..." />
              <div class="ai-input-actions">
                <i class="fa-solid fa-paperclip"></i>
                <i class="fa-solid fa-face-smile"></i>
                <button class="send-btn"><i class="fa-solid fa-paper-plane"></i></button>
              </div>
            </div>
          </div>
        </aside>
      </transition>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { Search } from '@element-plus/icons-vue'
import logoImg from '../assets/logo_QLCV.png'
import HelpDropdown from '../components/HelpDropdown.vue'
import SettingsDropdown from '../components/SettingsDropdown.vue'
import NotificationsDropdown from '../components/NotificationsDropdown.vue'
import UserDropdown from '../components/UserDropdown.vue';

const searchQuery = ref('')
const aiVisible = ref(true)
const currentTab = ref('list')
const showTaskModal = ref(false)
const selectedTask = ref(null)

const openTaskDetail = (task) => {
  selectedTask.value = task
  showTaskModal.value = true
}

const router = useRouter()
const goToDashboard = () => {
  router.push('/dashboard')
}

const goToAI = () => {
  router.push('/ai-assistant')
}

const sidebarVisible = ref(false)

const toggleAI = () => {
  aiVisible.value = !aiVisible.value
}

// Dữ liệu cho tab LIST
const taskGroups = ref([
  {
    id: 'grp-progress',
    statusText: 'IN PROGRESS',
    statusBg: '#6b21a8',
    statusColor: '#ffffff',
    expanded: true,
    items: []
  },
  {
    id: 'grp-todo',
    statusText: 'TO DO',
    statusBg: '#374151',
    statusColor: '#9ca3af',
    expanded: true,
    items: []
  }
])
</script>

<style scoped>
/* =========================================
   LAYOUT & NAVBAR
========================================== */
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
  padding: 0 16px;
  flex-shrink: 0;
  width: 100%;
}

.nav-left, .nav-right {
  display: flex;
  align-items: center;
  gap: 16px;
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

.top-search-create {
  display: flex;
  align-items: center;
  gap: 8px; /* space between search and create */
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
  transition: background-color 0.2s, border-color 0.2s;
}

.search-input-mock:focus-within {
  background-color: #2c333a;
  border-color: #579dff;
}

.search-input-mock i {
  color: #8c9bab;
  font-size: 14px;
  margin-right: 8px;
}

.search-input-mock input {
  background: transparent;
  border: none;
  color: #f4f5f7;
  font-size: 14px;
  width: 100%;
  outline: none;
}

.search-input-mock input::placeholder {
  color: #8c9bab;
}

.btn-create-jira {
  display: flex;
  align-items: center;
  gap: 6px;
  background-color: #579dff; 
  color: #1d2125;
  border: none;
  border-radius: 4px;
  padding: 0 16px;
  height: 32px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.2s;
}

.btn-create-jira:hover {
  background-color: #85b8ff;
}

.nav-icon {
  color: #94a3b8;
  font-size: 18px;
  cursor: pointer;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
}
.nav-icon:hover { background-color: #1e293b; color: white; }
.nav-icon.active { color: #60a5fa; background-color: #1e293b; }

.user-avatar {
  background: #fdbba7; 
  color: #1d2125;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 14px;
  cursor: pointer;
}

/* =========================================
   SIDEBAR
========================================== */
.main-body {
  display: flex;
  flex: 1;
  overflow: hidden;
}

.sidebar {
  width: 260px;
  background-color: #0c101a; 
  border-right: 1px solid #1e293b;
  padding: 24px 16px;
}

.side-menu {
  list-style: none;
  padding: 0; margin: 0;
}

.section-label {
  font-size: 11px;
  color: #64748b;
  font-weight: 700;
  letter-spacing: 0.5px;
  padding: 8px 12px;
  margin-bottom: 8px;
}

.side-menu li {
  padding: 10px 12px;
  border-radius: 6px;
  color: #cbd5e1;
  font-size: 14px;
  font-weight: 500;
  margin-bottom: 4px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 12px;
}
.side-menu li:hover { background-color: #1e293b; color: white; }
.side-menu li.active { background-color: #1e3a8a; color: #60a5fa; }

.sub-space-item {
  padding: 12px !important;
  margin: 12px 0 16px 16px !important;
  background-color: #162032 !important;
  border: 1px solid #334155;
  border-radius: 8px !important;
}
.space-icon-small {
  width: 32px; height: 32px;
  background: linear-gradient(135deg, #a855f7, #6366f1);
  border-radius: 8px;
  display: flex; align-items: center; justify-content: center;
  font-size: 12px; font-weight: 700; color: white;
}
.space-text { display: flex; flex-direction: column; }
.space-text .name { font-size: 14px; font-weight: 600; color: white; }
.space-text .sub { font-size: 11px; color: #94a3b8; }

/* =========================================
   MAIN CONTENT
========================================== */
.content-area {
  flex: 1;
  background-color: #0f111a; 
  padding: 32px 40px;
  overflow-y: auto;
}
.content-wrapper { width: 100%; max-width: none; margin: 0; }

.page-title {
  font-size: 24px;
  color: #f4f5f7;
  font-weight: 600;
  margin-bottom: 24px;
}

/* Tabs */
.jira-tabs {
  display: flex;
  align-items: center;
  gap: 24px;
  border-bottom: 1px solid #1e293b;
  margin-bottom: 24px;
}

.jira-tab {
  padding: 12px 0;
  color: #94a3b8;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  position: relative;
}
.jira-tab:hover { color: #e2e8f0; }
.jira-tab.active { color: #579dff; font-weight: 600; }
.jira-tab.active::after {
  content: ''; position: absolute; bottom: -1px; left: 0; right: 0;
  height: 2px; background-color: #579dff;
}

.tab-spacer { flex: 1; }
.jira-tab-icon {
  color: #94a3b8; font-size: 13px; cursor: pointer; display: flex; align-items: center; gap: 6px;
}

/* =========================================
   SUMMARY TAB CONTENT
========================================== */
.summary-content {
  display: flex; flex-direction: column; gap: 24px;
}
.dark-btn {
  background-color: #1e293b !important;
  color: #f1f5f9 !important;
  border-color: #334155 !important;
}

.top-widgets {
  display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px;
}
.widget {
  background-color: #1e2430;
  border: 1px solid #2d3748;
  border-radius: 8px; padding: 16px;
  display: flex; align-items: center; gap: 16px;
}
.widget-icon { font-size: 20px; }
.widget-number { font-size: 16px; font-weight: 600; color: #f8fafc; margin-bottom: 2px;}
.widget-sub { font-size: 11px; color: #94a3b8; }

.charts-grid {
  display: grid; grid-template-columns: 1fr 1fr; gap: 16px;
}
.chart-card {
  background-color: #1e2430;
  border: 1px solid #2d3748;
  border-radius: 8px; padding: 20px;
}
.chart-header h4 { margin: 0 0 4px; font-size: 15px; color: #f8fafc; }
.chart-header p { margin: 0; font-size: 12px; color: #94a3b8; }
.chart-header a { color: #579dff; text-decoration: none; }

.empty-card {
  display: flex; flex-direction: column; align-items: center; justify-content: center; text-align: center;
  padding: 40px;
}
.empty-icon { font-size: 32px; color: #579dff; margin-bottom: 16px; }

.donut-body { display: flex; align-items: center; gap: 32px; margin-top: 24px; }
.donut-chart {
  position: relative; width: 120px; height: 120px; border-radius: 50%;
  background: conic-gradient(#3b82f6 0% 50%, #84cc16 50% 100%);
  display: flex; align-items: center; justify-content: center;
}
.donut-ring {
  position: absolute; width: 90px; height: 90px;
  background-color: #1e2430; border-radius: 50%;
}
.donut-center { position: relative; display: flex; flex-direction: column; align-items: center; }
.donut-center .val { font-size: 24px; font-weight: 700; color: white; }
.donut-center .lbl { font-size: 10px; color: #94a3b8; width: 60px; text-align: center; line-height: 1.2;}
.leg-item { font-size: 12px; color: #cbd5e1; margin-bottom: 8px; display: flex; align-items: center; gap: 8px; }
.dot { width: 10px; height: 10px; border-radius: 2px; }

/* Bar chart placeholder */
.bar-chart { display: flex; gap: 12px; margin-top: 24px; height: 150px; position: relative;}
.bar-y { display: flex; flex-direction: column; justify-content: space-between; font-size: 10px; color: #64748b; }
.bar-plot { flex: 1; display: flex; border-left: 1px solid #334155; border-bottom: 1px solid #334155; padding-left: 10px;}
.bar-col { flex: 1; display: flex; align-items: flex-end; justify-content: center; margin: 0 4px; position: relative; }
.bar-fill { width: 32px; border-radius: 2px 2px 0 0; }
.bar-x { position: absolute; bottom: -20px; left: 24px; right: 0; display: flex; justify-content: space-around; font-size: 9px; font-weight: 600; }

.type-chart { margin-top: 20px; }
.type-header-row { display: flex; justify-content: space-between; font-size: 11px; color: #64748b; margin-bottom: 12px; border-bottom: 1px solid #334155; padding-bottom: 8px;}
.type-row { display: flex; align-items: center; margin-bottom: 16px; }
.t-name { width: 120px; font-size: 13px; color: #cbd5e1; display: flex; align-items: center; gap: 8px;}
.t-dist { flex: 1; background-color: #334155; height: 16px; border-radius: 4px; overflow: hidden; }
.t-prog { background-color: #64748b; height: 100%; display: flex; align-items: center; padding-left: 8px; font-size: 10px; color: white; transition: width 0.5s;}

/* =========================================
   LIST TAB CONTENT (From previous step)
========================================== */
.list-tab-wrapper { display: flex; flex-direction: column; width: 100%; }

.list-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}
.toolbar-left, .toolbar-right { display: flex; align-items: center; gap: 12px; }
.toolbar-btn {
  display: flex; align-items: center; gap: 8px;
  background-color: transparent; border: 1px solid #3f3f46; border-radius: 20px;
  padding: 6px 12px; color: #d4d4d8; font-size: 13px; cursor: pointer; transition: all 0.2s;
}
.toolbar-btn:hover { background-color: #27272a; color: white;}
.toolbar-btn.primary-tint { background-color: #3b0764; border-color: #6b21a8; color: #d8b4fe; }
.toolbar-icon { color: #a1a1aa; cursor: pointer; padding: 0 8px; font-size: 14px;}
.avatar-tiny { background:#f8fafc; color:#0c101a; border-radius:50%; width:16px; height:16px; display:inline-flex; align-items:center; justify-content:center; font-size:10px; font-weight:bold; margin-left:4px; }
.add-task-white-btn {
  background-color: #f8fafc; color: #0f172a; border: none; border-radius: 6px;
  padding: 8px 16px; font-weight: 600; font-size: 13px; display: flex; align-items: center; cursor: pointer;
}

.list-view-container { display: flex; flex-direction: column; gap: 32px; }
.task-group { border-bottom: 1px solid #1e293b; padding-bottom: 24px; }
.group-header { display: flex; align-items: center; gap: 12px; margin-bottom: 8px; padding-left: 8px;}
.toggle-icon { color: #a1a1aa; cursor: pointer; font-size: 14px; transition: transform 0.2s; }
.group-badge { display: flex; align-items: center; gap: 6px; padding: 4px 10px; border-radius: 4px; font-weight: 700; font-size: 11px; }
.group-count { color: #a1a1aa; font-size: 13px; }

.list-row { display: flex; align-items: center; padding: 0 16px; border-bottom: 1px dashed #27272a;}
.header-row { color: #71717a; font-size: 12px; height: 36px; border-bottom: none !important;}
.task-row { height: 48px; transition: background 0.2s; cursor: pointer; background-color: transparent; border-bottom: 1px solid #1e293b;}
.task-row:hover { background-color: #1e2430; }

.nav-center {
  flex: 1;
  display: flex;
  justify-content: center;
  align-items: center;
}
.col-name { display: flex; align-items: center; gap: 12px; color: #f8fafc; font-size: 14px; font-weight: 500; padding-left: 24px; flex: 1; min-width: 250px;}
.col-assignee { width: 120px; font-size: 13px; color: #cbd5e1; }
.col-date { width: 120px; font-size: 13px; color: #cbd5e1; }
.col-priority { width: 100px; font-size: 14px; }
.col-status { width: 140px; }
.col-comments { width: 80px; text-align: left; color: #a1a1aa; font-size: 14px; }
.col-add { width: 40px; text-align: right; color: #a1a1aa; font-size: 14px;}

.check-icon { font-size: 16px; color: #52525b; }
.icon-btn { color: #a1a1aa; font-size: 14px; cursor: pointer;}
.icon-btn:hover { color: #f8fafc; }
.status-btn { display: inline-flex; padding: 4px 10px; border-radius: 4px; font-size: 10px; line-height: 1; gap: 6px; align-items: center;}

.add-task-row { height: 40px; cursor: pointer; border-bottom: none; color: #a1a1aa;}
.add-task-row:hover { color: #f8fafc; }
.add-task-row .col-name { padding-left: 24px; font-size: 13px; font-weight: 400;}

/* =========================================
   AI SIDEBAR (Right)
========================================== */
.ai-sidebar {
  width: 320px;
  background-color: #131824;
  border-left: 1px solid #1e293b;
  display: flex;
  flex-direction: column;
}

@media (max-width: 1200px) {
  .ai-sidebar {
    position: fixed;
    right: 0;
    top: 56px;
    bottom: 0;
    z-index: 1000;
  }
}

@media (max-width: 900px) {
  .search-input-mock {
    width: 100% !important;
  }
}

@media (max-width: 768px) {
  .sidebar {
    position: fixed;
    left: -260px;
    top: 56px;
    bottom: 0;
    z-index: 1001;
    transition: left 0.3s ease;
  }
  .sidebar.show {
    left: 0;
  }
  .content-area {
    padding: 20px 16px;
  }
  .top-widgets {
    grid-template-columns: 1fr 1fr;
  }
  .charts-grid {
    grid-template-columns: 1fr;
  }
  .nav-center {
    display: none;
  }
  .nav-left, .nav-right {
    width: auto;
  }
  .jira-tabs {
    overflow-x: auto;
    padding-bottom: 8px;
  }
  .jira-tab {
    white-space: nowrap;
  }
}

.mobile-only {
  display: none;
}

@media (max-width: 768px) {
  .mobile-only {
    display: flex;
  }
  .desktop-only {
    display: none;
  }
}

.menu-toggle {
  font-size: 20px;
  color: #94a3b8;
  cursor: pointer;
  margin-right: 12px;
}

.ai-header {
  padding: 16px 20px;
  border-bottom: 1px solid #1e293b;
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.ai-header h4 { font-size: 14px; display: flex; align-items: center; gap: 8px; margin: 0; color: #f8fafc;}
.ai-header h4 i { color: #579dff; }

.ai-content { flex: 1; padding: 24px 20px; overflow-y: auto; }
.quick-actions-title { font-size: 10px; font-weight: 600; color: #64748b; margin-bottom: 12px; letter-spacing: 0.5px;}
.quick-actions-col { display: flex; flex-direction: column; gap: 8px; margin-bottom: 32px; }
.action-btn {
  background-color: transparent; border: 1px solid #334155; color: #cbd5e1;
  padding: 10px 16px; border-radius: 8px; font-size: 12px; cursor: pointer;
  display: flex; align-items: center; gap: 10px; transition: all 0.2s;
}
.action-btn:hover { background-color: #1e293b; color: white; border-color: #475569; }

.chat-message { display: flex; gap: 12px; margin-bottom: 20px; }
.avatar-bot {
  width: 28px; height: 28px; border-radius: 50%; background-color: #3b82f6; color: white;
  display: flex; align-items: center; justify-content: center; font-size: 14px;
}
.message-bubble {
  flex: 1; background-color: #1e293b; padding: 14px 16px; border-radius: 4px 16px 16px 16px;
  color: #e2e8f0; font-size: 13px; line-height: 1.5;
}

.ai-input-area { padding: 20px; border-top: 1px solid #1e293b; }
.ai-input-wrapper { background-color: #1e293b; border-radius: 12px; display: flex; flex-direction: column; overflow: hidden; }
.ai-input-wrapper input { background: transparent; border: none; padding: 16px; color: white; font-size: 13px; outline: none; }
.ai-input-actions { display: flex; align-items: center; padding: 4px 16px 12px; gap: 12px; }
.ai-input-actions i { color: #64748b; cursor: pointer; transition: color 0.1s;}
.ai-input-actions i:hover { color: white; }
.send-btn { background: #3b82f6; border: none; width: 28px; height: 28px; border-radius: 50%; color: white; display: flex; align-items: center; justify-content: center; margin-left: auto; cursor: pointer; }
.send-btn i { font-size: 12px; }

/* Animations */
.slide-right-enter-active, .slide-right-leave-active { transition: transform 0.3s ease; }
.slide-right-enter-from, .slide-right-leave-to { transform: translateX(100%); }

/* Comment Popover Specific - Dùng :global để chắc chắn tác động được vào Element teleported popper */
:global(.el-popper.comment-popover-dark) {
  background-color: #1d2125 !important;
  border: 1px solid #334155 !important;
  padding: 0 !important;
  border-radius: 8px !important;
  box-shadow: 0 12px 32px rgba(0,0,0,0.6) !important;
}

:global(.comment-popover-dark .el-popper__arrow::before) {
  background-color: #1d2125 !important;
  border: 1px solid #334155 !important;
}

.comment-trigger-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  cursor: pointer;
}
.comment-trigger-btn:hover .icon-btn { color: white; }
.comment-count-text { font-size: 13px; font-weight: 700; color: #cbd5e1; }

.comment-popover-content {
  display: flex;
  flex-direction: column;
  max-height: 500px;
}

.comments-scroll-area {
  padding: 20px;
  overflow-y: auto;
  max-height: 300px;
  border-bottom: 1px solid #2c333a;
  background-color: #1d2125;
}

.comment-item {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.comment-user {
  display: flex;
  gap: 12px;
  align-items: center;
}
.avatar-circle {
  width: 32px;
  height: 32px;
  background-color: #475569;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 11px;
  font-weight: 700;
  color: white;
}
.user-meta {
  display: flex;
  align-items: baseline;
  gap: 8px;
}
.user-name { font-size: 14px; font-weight: 700; color: #f1f5f9; }
.time-stamp { font-size: 11px; color: #64748b; }

.comment-text {
  font-size: 14px;
  color: #e2e8f0;
  padding-left: 44px;
}

.comment-actions-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-left: 44px;
  margin-top: 4px;
}
.left-actions { display: flex; gap: 16px; color: #64748b; font-size: 14px; }
.left-actions i:hover { color: white; cursor: pointer; }
.answer-link { font-size: 12px; font-weight: 600; color: #cbd5e1; cursor: pointer; }
.answer-link:hover { color: white; }

.comment-input-section {
  padding: 16px 20px;
  background-color: #161a1d;
  border-radius: 0 0 8px 8px;
}

.input-upper textarea {
  width: 100%;
  background: transparent;
  border: none;
  color: #cbd5e1;
  font-size: 14px;
  resize: none;
  height: 48px;
  outline: none;
  font-family: inherit;
}

.input-lower {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 12px;
}

.lower-left {
  display: flex;
  align-items: center;
  gap: 12px;
}

.tool-btn {
  background-color: #222;
  border-radius: 6px;
  padding: 6px 10px;
  display: flex;
  align-items: center;
  gap: 6px;
  cursor: pointer;
}
.tool-btn.circle { border-radius: 50%; width: 28px; height: 28px; justify-content: center; padding: 0; }
.tool-btn.comment-type { font-size: 12px; font-weight: 600; color: #cbd5e1; }
.tool-btn:hover { background-color: #333; }

.tool-icon {
  color: #64748b;
  font-size: 14px;
  cursor: pointer;
  transition: color 0.2s;
}
.tool-icon:hover { color: white; }

.send-icon {
  color: #475569;
  font-size: 18px;
  cursor: pointer;
}
.send-icon:hover { color: #579dff; }


/* TASK DETAIL MODAL */
.task-modal-overlay {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background-color: rgba(0, 0, 0, 0.7);
  backdrop-filter: blur(2px);
  z-index: 2000;
  display: flex;
  align-items: center;
  justify-content: center;
}

.task-modal {
  width: 95%;
  max-width: 1300px;
  height: 90vh;
  background-color: #0c0c0c;
  border-radius: 12px;
  border: 1px solid #2c333a;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  box-shadow: 0 20px 60px rgba(0,0,0,0.8);
}

.modal-header {
  height: 48px;
  background-color: #161a1d;
  border-bottom: 1px solid #2c333a;
  padding: 0 16px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-shrink: 0;
}

.header-left { display: flex; align-items: center; gap: 8px; font-size: 13px; color: #94a3b8; }
.header-left i { font-size: 14px; }
.m-crumb { cursor: pointer; }
.m-crumb:hover { color: white; }
.m-crumb.current { color: #f1f5f9; font-weight: 600; }
.separator { font-size: 10px; color: #475569; }
.add-crumb, .copy-crumb { color: #64748b; margin-left: 4px; cursor: pointer; font-size: 12px; }

.header-right { display: flex; align-items: center; gap: 16px; color: #64748b; }
.created-at { font-size: 12px; }
.btn-ai-header { background: #3b0764; color: #d8b4fe; padding: 4px 10px; border-radius: 4px; font-size: 12px; font-weight: 600; cursor: pointer; display: flex; align-items: center; gap: 6px; }
.btn-share { background: #222; color: #cbd5e1; padding: 4px 10px; border-radius: 4px; font-size: 12px; font-weight: 600; cursor: pointer; display: flex; align-items: center; gap: 6px; }
.m-more, .m-fav, .m-pin, .m-side-toggle, .m-close { cursor: pointer; font-size: 14px; transition: color 0.1s; }
.m-more:hover, .m-fav:hover, .m-close:hover { color: white; }

.modal-body-wrapper { flex: 1; display: flex; overflow: hidden; }

/* Left Section */
.modal-main { flex: 1; padding: 40px; overflow-y: auto; background-color: #0c0c0c; }
.task-id-row { display: flex; align-items: center; gap: 12px; margin-bottom: 24px; }
.status-badge-small { display: flex; align-items: center; gap: 6px; font-size: 11px; font-weight: 700; color: #64748b; text-transform: uppercase; letter-spacing: 0.5px; }
.task-id-text { font-size: 12px; color: #94a3b8; font-family: monospace; }
.btn-ai-mini { font-size: 12px; color: #579dff; font-weight: 600; cursor: pointer; display: flex; align-items: center; gap: 6px; }

.task-modal-title { font-size: 32px; font-weight: 700; color: #f1f5f9; margin-bottom: 24px; }

.ai-prompt-bar {
  background-color: #1a1a1a;
  border-radius: 8px;
  padding: 12px 16px;
  display: flex;
  align-items: center;
  gap: 12px;
  border: 1px solid #333;
  margin-bottom: 40px;
}
.sparkle-icon { color: #a855f7; font-size: 16px; }
.ai-prompt-bar input { background: transparent; border: none; flex: 1; color: #94a3b8; font-size: 14px; outline: none; }

.attributes-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 20px 40px;
  margin-bottom: 40px;
}
.attr-item { display: flex; flex-direction: column; gap: 4px; }
.attr-label { font-size: 12px; font-weight: 700; color: #64748b; display: flex; align-items: center; gap: 8px; }
.attr-label i { width: 14px; text-align: center; }
.attr-value { font-size: 14px; color: #e2e8f0; display: flex; align-items: center; gap: 10px; }
.attr-value.muted { color: #475569; }

.status-pill {
  background-color: #3b0764; color: #d8b4fe; padding: 4px 10px; border-radius: 4px;
  font-size: 12px; font-weight: 800; display: flex; align-items: center; gap: 8px; cursor: pointer;
}
.check-confirm { color: #22c55e; font-size: 14px; }

.content-section { display: flex; flex-direction: column; gap: 16px; margin-bottom: 60px; }
.section-link { color: #64748b; font-size: 14px; display: flex; align-items: center; gap: 10px; cursor: pointer; }
.section-link:hover { color: #f1f5f9; }
.section-link.ai-link { color: #a855f7; font-weight: 600; }

.fields-section h3 { font-size: 16px; font-weight: 600; color: #64748b; margin-bottom: 16px; }
.add-field-btn { background: transparent; border: 1px dashed #334155; color: #475569; padding: 12px 20px; border-radius: 8px; cursor: pointer; font-size: 13px; text-align: left; }
.add-field-btn:hover { border-color: #64748b; color: #94a3b8; }

/* Right Sidebar */
.modal-sidebar {
  width: 440px;
  background-color: #0c0c0c;
  border-left: 1px solid #2c333a;
  display: flex;
  flex-direction: column;
}

.sidebar-header {
  padding: 24px 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid #1a1a1a;
}
.sidebar-header h2 { font-size: 18px; font-weight: 700; color: #f1f5f9; }
.header-tools { display: flex; align-items: center; gap: 16px; color: #64748b; font-size: 14px; }
.header-tools i { cursor: pointer; }

.activity-scroll { flex: 1; padding: 20px; overflow-y: auto; }
.comment-card { margin-bottom: 24px; padding-bottom: 16px; border-bottom: 1px solid #1a1a1a;}
.c-head { display: flex; align-items: center; gap: 10px; margin-bottom: 8px; }
.avatar-sm { width: 24px; height: 24px; background: #64748b; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 10px; font-weight: 700; color: white; }
.c-user { font-size: 13px; font-weight: 700; color: #f1f5f9; }
.c-time { font-size: 11px; font-weight: 400; color: #475569; margin-left: 6px; }
.c-body { font-size: 14px; color: #cbd5e1; padding-left: 34px; line-height: 1.5; }
.c-foot { display: flex; align-items: center; justify-content: space-between; padding-left: 34px; margin-top: 12px; }
.c-actions { display: flex; gap: 12px; color: #64748b; font-size: 12px; }
.c-rep { font-size: 12px; font-weight: 600; color: #64748b; cursor: pointer; }

.activity-input { padding: 20px; background-color: #0c0c0c; border-top: 1px solid #1a1a1a; }
.input-container { background-color: #111; border: 1px solid #222; border-radius: 8px; display: flex; flex-direction: column; }
.input-container textarea { background: transparent; border: none; padding: 12px 16px; color: #cbd5e1; font-size: 13px; resize: none; min-height: 48px; outline: none; }

.input-actions-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  border-top: 1px solid #1a1a1a;
}
.bar-left { display: flex; align-items: center; gap: 12px; color: #64748b; font-size: 12px; }
.bar-left i { cursor: pointer; }
.bar-left i.ai { color: #a855f7; }
.btn-comment-type { background: #222; border: none; border-radius: 4px; color: #cbd5e1; font-size: 11px; padding: 4px 8px; display: flex; align-items: center; gap: 4px; cursor: pointer;}

.bar-right { display: flex; align-items: center; gap: 8px; color: #475569; }
.send-enabled { color: #3b82f6; font-size: 16px; cursor: pointer; }

/* =========================================
   CALENDAR VIEW
========================================== */
.calendar-content {
  padding: 0 10px;
  height: 100%;
}

.calendar-header-toolbar {
  display: flex;
  align-items: center;
  margin-bottom: 24px;
}

.calendar-month-title {
  font-size: 24px;
  font-weight: 700;
  color: #f1f5f9;
  margin-right: 24px;
}

.calendar-nav-controls {
  margin-right: auto;
}

.btn-group-jira {
  display: flex;
  background-color: #22272b;
  border-radius: 4px;
  overflow: hidden;
  border: 1px solid #334155;
}

.jira-control-btn {
  background: transparent !important;
  border: none !important;
  border-right: 1px solid #334155 !important;
  color: #94a3b8 !important;
  margin: 0 !important;
  height: 32px !important;
  padding: 0 12px !important;
  border-radius: 0 !important;
}

.jira-control-btn:last-child {
  border-right: none !important;
}

.jira-control-btn:hover {
  background-color: #2c333a !important;
  color: white !important;
}

.today-btn {
  font-weight: 600 !important;
}

.toggle-group-jira {
  display: flex;
  background-color: #22272b;
  border: 1px solid #334155;
  border-radius: 4px;
  padding: 2px;
}

.toggle-item {
  padding: 4px 16px;
  font-size: 13px;
  font-weight: 600;
  color: #94a3b8;
  cursor: pointer;
  border-radius: 3px;
  transition: all 0.2s;
}

.toggle-item.active {
  background-color: #3b82f6;
  color: white;
}

.calendar-grid-container {
  border: 1px solid #1e293b;
  border-radius: 6px;
  overflow: hidden;
  background-color: #0c101a;
}

.calendar-week-labels {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  background-color: #161a1d;
  border-bottom: 1px solid #1e293b;
}

.weekday-label {
  padding: 10px;
  text-align: left;
  font-size: 11px;
  font-weight: 800;
  color: #64748b;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.calendar-days-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  grid-auto-rows: minmax(120px, auto);
}

.day-cell {
  border-right: 1px solid #1e293b;
  border-bottom: 1px solid #1e293b;
  padding: 10px;
  color: #f1f5f9;
  font-size: 13px;
  font-weight: 500;
  position: relative;
}

.day-cell:nth-child(7n) {
  border-right: none;
}

.day-cell.muted {
  color: #334155;
}

.day-cell.today {
  background-color: rgba(59, 130, 246, 0.05);
}

.day-num-circle {
  width: 24px;
  height: 24px;
  background-color: #3b82f6;
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  margin-bottom: 8px;
}

.calendar-event {
  margin-top: 8px;
  padding: 6px 8px;
  border-radius: 4px;
  font-size: 11px;
  display: flex;
  flex-direction: column;
  gap: 2px;
  cursor: pointer;
}

.calendar-event.blue { background-color: #1e3a8a; border-left: 3px solid #3b82f6; }
.calendar-event.orange { background-color: #78350f; border-left: 3px solid #f59e0b; }
.calendar-event.green { background-color: #064e3b; border-left: 3px solid #10b981; }

.event-id { font-weight: 700; color: #60a5fa; opacity: 0.9; }
.event-name { color: #e2e8f0; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }

/* =========================================
   TIMELINE VIEW (ROADMAP)
========================================== */
.timeline-content {
  padding: 0 10px;
  display: flex;
  flex-direction: column;
  height: calc(100vh - 200px);
}

.timeline-toolbar {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 16px;
}

.timeline-search {
  display: flex;
  align-items: center;
  background-color: #22272b;
  border: 1px solid #334155;
  border-radius: 4px;
  padding: 0 10px;
  width: 200px;
  height: 32px;
}

.timeline-search i { font-size: 14px; color: #8c9bab; margin-right: 8px; }
.timeline-search input { background: transparent; border: none; color: white; width: 100%; outline: none; font-size: 13px; }

.avatar-circle-sm {
  width: 24px;
  height: 24px;
  background-color: #f59e0b;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 10px;
  font-weight: 700;
  color: #1d2125;
}

.toolbar-right-icons {
  margin-left: auto;
  display: flex;
  gap: 16px;
  color: #8c9bab;
}

.timeline-grid-wrapper {
  flex: 1;
  display: flex;
  border: 1px solid #1e293b;
  border-radius: 5px;
  overflow: hidden;
  background-color: #161a1d;
}

.timeline-left-panel {
  width: 260px;
  border-right: 1px solid #1e293b;
  display: flex;
  flex-direction: column;
}

.panel-header {
  padding: 12px 16px;
  font-size: 13px;
  font-weight: 700;
  color: #f1f5f9;
  background-color: #111;
  border-bottom: 1px solid #1e293b;
}

.panel-sub-header {
  padding: 8px 16px;
  font-size: 11px;
  font-weight: 700;
  color: #64748b;
  text-transform: uppercase;
}

.epic-row {
  padding: 10px 16px;
  font-size: 13px;
  color: #94a3b8;
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
  border-bottom: 1px solid rgba(255,255,255,0.03);
}

.epic-row:hover { background-color: #1e293b; color: white; }

.add-epic-btn {
  padding: 12px 16px;
  color: #8c9bab;
  font-size: 13px;
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
}

.add-epic-btn:hover { color: white; }

.timeline-main-grid {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow-x: auto;
}

.timeline-months-row {
  display: flex;
  background-color: #111;
  border-bottom: 1px solid #1e293b;
}

.month-col {
  min-width: 200px;
  flex: 1;
  padding: 12px;
  font-size: 12px;
  font-weight: 500;
  color: #94a3b8;
  text-align: center;
  border-right: 1px solid #1e293b;
}

.timeline-rows-container {
  flex: 1;
  position: relative;
}

.timeline-task-row {
  height: 48px;
  display: flex;
  border-bottom: 1px solid rgba(255,255,255,0.03);
  position: relative;
}

.grid-line {
  min-width: 200px;
  flex: 1;
  border-right: 1px solid rgba(255,255,255,0.03);
  height: 100%;
}

.gantt-bar {
  position: absolute;
  top: 12px;
  height: 24px;
  border-radius: 4px;
  cursor: pointer;
}

.gantt-bar.purple { background-color: #7c3aed; }
.gantt-bar.blue { background-color: #2563eb; }
.gantt-bar.green { background-color: #10b981; }

.timeline-bottom-nav {
  position: fixed;
  bottom: 24px;
  right: 480px; /* Adjust based on AI sidebar */
  background-color: #22272b;
  border: 1px solid #334155;
  border-radius: 6px;
  padding: 4px;
  display: flex;
  align-items: center;
  box-shadow: 0 4px 12px rgba(0,0,0,0.5);
  z-index: 50;
}

.bottom-nav-group {
  display: flex;
  border-right: 1px solid #334155;
}

.bottom-item {
  padding: 4px 12px;
  font-size: 12px;
  font-weight: 600;
  color: #94a3b8;
  cursor: pointer;
  border-radius: 3px;
}

.bottom-item.active {
  background-color: #3b82f633;
  color: #60a5fa;
}

.bottom-nav-icons {
  padding: 0 12px;
  display: flex;
  gap: 12px;
  color: #8c9bab;
  font-size: 12px;
}

/* =========================================
   PAGES VIEW (ENHANCED)
========================================== */
.pages-content {
  height: calc(100vh - 180px);
  overflow: hidden;
}

.pages-layout-wrapper {
  display: flex;
  height: 100%;
}

.pages-main-area {
  flex: 1;
  padding: 0 40px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
}

.pages-header-section {
  margin-bottom: 24px;
}

.pages-title {
  font-size: 24px;
  font-weight: 700;
  color: #f1f5f9;
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 8px;
}

.pages-desc {
  font-size: 14px;
  color: #94a3b8;
}

/* Editor Window Mockup */
.editor-window {
  background-color: #1d2125;
  border-radius: 8px 8px 0 0;
  border: 1px solid #333c43;
  display: flex;
  flex-direction: column;
  flex: 1;
  box-shadow: 0 10px 40px rgba(0,0,0,0.4);
}

.editor-window-header {
  height: 32px;
  background-color: #2c333a;
  border-bottom: 1px solid #333c43;
  display: flex;
  align-items: center;
  padding: 0 16px;
}

.header-dots {
  display: flex;
  gap: 6px;
}

.header-dots span {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background-color: #444;
}

.editor-toolbar-expanded {
  padding: 8px 16px;
  background-color: #1d2125;
  border-bottom: 1px solid #333c43;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 12px;
}

.t-left, .t-right {
  display: flex;
  align-items: center;
  gap: 8px;
}

.t-icon {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #8c9bab;
  cursor: pointer;
  border-radius: 4px;
  font-size: 14px;
}

.t-icon:hover { background-color: #333c43; color: white; }
.t-icon.active { color: #579dff; background-color: rgba(87, 157, 255, 0.1); }

.t-divider {
  width: 1px;
  height: 20px;
  background-color: #333c43;
}

.t-dropdown {
  padding: 0 12px;
  height: 32px;
  display: flex;
  align-items: center;
  gap: 8px;
  color: #8c9bab;
  font-size: 13px;
  cursor: pointer;
  border-radius: 4px;
}
.t-dropdown:hover { background-color: #333c43; }

.saving-text {
  font-size: 11px;
  color: #64748b;
  margin-right: 8px;
}

.t-avatar-circle {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  background-color: #f59e0b;
  color: #1d2125;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 10px;
  font-weight: 700;
}

.publish-btn { background-color: #579dff !important; border: none !important; font-weight: 600 !important; }
.close-btn { background-color: #333c43 !important; border: none !important; color: #f1f5f9 !important; font-weight: 600 !important; }

.editor-content-area {
  padding: 60px 100px;
  color: #f1f5f9;
}

.editor-main-heading { font-size: 32px; font-weight: 700; margin-bottom: 24px; }
.editor-main-p { font-size: 16px; color: #94a3b8; line-height: 1.6; margin-bottom: 24px; }
.editor-tip { font-size: 14px; color: #94a3b8; }
.mention-tag { background-color: #1e293b; color: #8c9bab; padding: 2px 6px; border-radius: 4px; font-size: 12px; }

/* Template Sidebar */
.pages-template-sidebar {
  width: 300px;
  padding: 24px;
  background-color: #0c101a;
  border-left: 1px solid #1e293b;
  display: flex;
  flex-direction: column;
}

.template-sidebar-title { font-size: 16px; font-weight: 700; color: #f1f5f9; margin-bottom: 4px; }
.template-sidebar-subtitle { font-size: 11px; font-weight: 800; color: #64748b; margin-bottom: 24px; letter-spacing: 0.5px; }

.template-list { display: flex; flex-direction: column; gap: 8px; }

.template-item {
  display: flex;
  gap: 12px;
  padding: 12px;
  border-radius: 8px;
  cursor: pointer;
  transition: background 0.2s;
}

.template-item:hover { background-color: #1e293b; }
.template-item.active { background-color: rgba(59, 130, 246, 0.15); border: 1px solid #3b82f633; }

.template-icon {
  width: 32px;
  height: 32px;
  min-width: 32px;
  border-radius: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
}

.template-icon.blue { background-color: #0c4a6e; color: #38bdf8; }
.template-icon.purple { background-color: #4c1d95; color: #a78bfa; }
.template-icon.green { background-color: #064e3b; color: #34d399; }
.template-icon.light-blue { background-color: #1e3a8a; color: #60a5fa; }
.template-icon.yellow { background-color: #713f12; color: #fbbf24; }

.template-name { font-size: 13px; font-weight: 700; color: #f1f5f9; margin-bottom: 2px; }
.template-desc { font-size: 12px; color: #64748b; line-height: 1.4; }

.explore-more {
  margin-top: auto;
  padding: 16px;
  font-size: 13px;
  color: #8c9bab;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
}
.explore-more:hover { color: white; }

/* Footer Bar */
.pages-footer-bar {
  background-color: #22272b;
  border: 1px solid #333c43;
  padding: 12px 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 24px;
  border-radius: 6px 6px 0 0;
  font-size: 13px;
  color: #94a3b8;
}

.footer-actions { display: flex; align-items: center; gap: 16px; }
.learn-more { cursor: pointer; color: #8c9bab; }
.learn-more:hover { text-decoration: underline; }
.try-now-btn { background-color: #3b82f6 !important; border: none !important; font-weight: 600 !important; }

/* =========================================
   FORMS VIEW
========================================== */
.forms-content {
  padding: 60px 40px;
  max-width: 900px;
  margin: 0 auto;
  text-align: center;
}

.forms-headline {
  font-size: 36px;
  font-weight: 800;
  color: white;
  margin-bottom: 24px;
}

.forms-subheadline {
  font-size: 16px;
  color: #94a3b8;
  line-height: 1.6;
  max-width: 600px;
  margin: 0 auto 60px;
}

.forms-workflow-visual {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 32px;
  margin-bottom: 80px;
}

.visual-card {
  width: 180px;
  height: 160px;
  background-color: #1d2125;
  border: 1px solid #333c43;
  border-radius: 12px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 16px;
}

.visual-icon {
  font-size: 32px;
  color: #579dff;
}

.visual-icon.active { color: #579dff; }

.visual-label {
  font-size: 14px;
  font-weight: 700;
  color: #f1f5f9;
}

.visual-arrow { color: #333c43; font-size: 24px; }

.forms-features-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 32px;
  text-align: left;
  margin-bottom: 60px;
}

.feature-card {
  padding-left: 16px;
  border-left: 2px solid #3b82f6;
}

.feature-title {
  font-size: 18px;
  font-weight: 700;
  color: #f1f5f9;
  margin-bottom: 8px;
}

.feature-desc {
  font-size: 14px;
  color: #94a3b8;
  line-height: 1.5;
}

.forms-cta-section { margin-top: 40px; }

.create-form-btn {
  padding: 12px 24px !important;
  font-weight: 700 !important;
  background-color: #3b82f6 !important;
  border: none !important;
}





/* =========================================
   BOARD VIEW (KANBAN)
========================================== */
.board-content {
  padding: 0 10px;
}

.board-toolbar {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 24px;
}

.board-search {
  display: flex;
  align-items: center;
  background-color: #22272b;
  border: 1px solid #333c43;
  border-radius: 4px;
  padding: 0 10px;
  width: 240px;
  height: 32px;
}

.board-search i {
  color: #8c9bab;
  font-size: 14px;
  margin-right: 8px;
}

.board-search input {
  background: transparent;
  border: none;
  color: #f4f5f7;
  font-size: 14px;
  width: 100%;
  outline: none;
}

.board-avatars {
  display: flex;
  align-items: center;
}

.avatar-stack {
  display: flex;
  align-items: center;
}

.stack-item {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  border: 2px solid #1d2125;
  margin-left: -8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 11px;
  font-weight: 700;
  color: white;
  cursor: pointer;
}

.stack-item:first-child { margin-left: 0; }

.stack-count {
  margin-left: 8px;
  color: #8c9bab;
  font-size: 13px;
  font-weight: 500;
}

.board-grouping {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  color: #8c9bab;
}

.sprint-btn {
  background-color: #0b2149 !important;
  border-color: #0b2149 !important;
  color: #579dff !important;
  font-weight: 600 !important;
  margin-left: auto;
}

.kanban-board {
  display: flex;
  gap: 12px;
  height: calc(100vh - 280px);
  overflow-x: auto;
  align-items: flex-start;
  padding-bottom: 10px;
}

.kanban-column {
  background-color: #161a1d;
  width: 280px;
  min-width: 280px;
  border-radius: 8px;
  padding: 12px 10px;
  display: flex;
  flex-direction: column;
}

.column-header {
  display: flex;
  align-items: center;
  padding: 4px 8px 16px;
  color: #8c9bab;
}

.column-title {
  font-size: 11px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.8px;
}

.column-count-badge {
  margin-left: 8px;
  background-color: #333c43;
  color: #f1f5f9;
  font-size: 11px;
  font-weight: 700;
  padding: 2px 6px;
  border-radius: 10px;
}

.header-more, .done-icon {
  margin-left: auto;
  font-size: 12px;
  cursor: pointer;
}
.done-icon { color: #4ade80; }

.kanban-cards {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.kanban-card {
  background-color: #22272b;
  border: 1px solid #333c43;
  border-radius: 4px;
  padding: 12px;
  cursor: pointer;
  box-shadow: 0 1px 3px rgba(0,0,0,0.3);
  transition: background 0.2s;
}

.kanban-card:hover {
  background-color: #2c333a;
}

.kanban-card.active-card {
  background-color: #2c333a;
  border-color: #579dff;
}

.card-title-row {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 12px;
}

.card-title {
  font-size: 15px;
  color: #f4f5f7;
  flex: 1;
  margin: 0;
  font-weight: 500;
}

.edit-icon, .more-icon {
  font-size: 14px;
  color: #8c9bab;
  opacity: 0.6;
}
.edit-icon:hover, .more-icon:hover { opacity: 1; color: white; }

.card-badges {
  margin-bottom: 16px;
}

.date-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 8px;
  background-color: rgba(255, 255, 255, 0.05);
  border: 1px solid #333c43;
  border-radius: 4px;
  font-size: 11px;
  color: #cbd5e1;
}

.date-badge.overdue {
  background-color: rgba(239, 68, 68, 0.1);
  border-color: #ef4444;
  color: #f87171;
}

.card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-task-id {
  font-size: 12px;
  color: #8c9bab;
  display: flex;
  align-items: center;
  gap: 6px;
  font-weight: 500;
}

.footer-right-icons {
  display: flex;
  align-items: center;
  gap: 12px;
  color: #8c9bab;
}

.link-tag {
  background-color: #333c43;
  padding: 0 6px;
  border-radius: 4px;
  font-size: 11px;
}

.avatar-circle-xs {
  width: 22px;
  height: 22px;
  border-radius: 50%;
  background-color: #475569;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 10px;
  color: white;
}

.btn-create-card {
  padding: 8px 10px;
  color: #8c9bab;
  font-size: 13px;
  cursor: pointer;
  border-radius: 4px;
  display: flex;
  align-items: center;
  gap: 6px;
  margin-top: 4px;
}

.btn-create-card:hover {
  background-color: rgba(255,255,255,0.05);
  color: #f4f5f7;
}

.add-column-box {
  width: 40px;
  min-width: 40px;
  display: flex;
  justify-content: center;
}

.add-column-btn {
  width: 40px;
  height: 40px;
  background-color: #161a1d;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  color: #8c9bab;
  cursor: pointer;
  transition: all 0.2s;
  margin-top: 0;
}

.add-column-btn:hover {
  background-color: #2c333a;
  color: white;
}

/* AI SIDEBAR */
.ai-sidebar {
  width: 420px;
  background-color: #0c101a;
  border-left: 1px solid #1e293b;
  display: flex;
  flex-direction: column;
  height: 100%;
}

.ai-header {
  padding: 24px;
  border-bottom: 1px solid #1e293b;
}

.ai-header h4 {
  font-size: 16px;
  display: flex;
  align-items: center;
  gap: 8px;
}

.ai-header h4 i {
  color: #60a5fa; 
}

.ai-content {
  flex: 1;
  padding: 24px;
  overflow-y: auto;
}

.quick-actions-title {
  font-size: 11px;
  font-weight: 700;
  color: #64748b;
  margin-bottom: 12px;
  text-align: center;
}

.quick-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  justify-content: center;
  margin-bottom: 32px;
}

.ai-chip {
  background-color: transparent !important;
  color: #94a3b8 !important;
  border-color: #334155 !important;
  font-size: 12px !important;
}

.ai-chip:hover {
  background-color: #1e3a8a !important;
  color: #60a5fa !important;
  border-color: #60a5fa !important;
}

.chat-message {
  margin-bottom: 20px;
}

.message-bubble {
  background-color: #1e293b;
  padding: 16px;
  border-radius: 12px 12px 12px 0;
  color: #e2e8f0;
  font-size: 14px;
  line-height: 1.5;
  margin-bottom: 8px;
}

.message-meta {
  font-size: 11px;
  color: #64748b;
  padding-left: 4px;
}

.ai-input-area {
  padding: 20px;
  background-color: #0c101a;
  border-top: 1px solid #1e293b;
}

.ai-input-wrapper {
  background-color: #1e293b;
  border: 1px solid #334155;
  border-radius: 12px;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.ai-input-wrapper input {
  background: transparent;
  border: none;
  padding: 16px;
  color: white;
  font-size: 14px;
  outline: none;
}

.ai-input-wrapper input::placeholder {
  color: #64748b;
}

.ai-input-actions {
  display: flex;
  align-items: center;
  padding: 8px 16px 12px;
  gap: 12px;
}

.ai-input-actions i {
  color: #64748b;
  cursor: pointer;
  transition: color 0.1s;
}

.ai-input-actions i:hover {
  color: white;
}

.send-btn {
  background: #3b82f6;
  border: none;
  width: 28px;
  height: 28px;
  border-radius: 6px;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-left: auto;
  cursor: pointer;
  transition: opacity 0.2s;
}

.send-btn:hover {
  opacity: 0.9;
}

.send-btn i {
  color: white !important;
  font-size: 12px;
}

.slide-right-enter-active, .slide-right-leave-active { transition: transform 0.3s ease; }
.slide-right-enter-from, .slide-right-leave-to { transform: translateX(100%); }

.fade-enter-active, .fade-leave-active { transition: opacity 0.2s; }
.fade-enter-from, .fade-leave-to { opacity: 0; }
</style>

