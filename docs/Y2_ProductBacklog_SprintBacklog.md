# 📋 Y2: PRODUCT BACKLOG, RELEASE BACKLOG, SPRINT BACKLOG

**Dự án:** Hệ thống Quản lý Công việc (QuanLyCongViec / SprintA)  
**Ngày lập:** 19/04/2026  
**Product Owner:** [Tên PO]  
**Scrum Master:** [Tên SM]

---

## I. MỤC TIÊU

### 1.1 Mục tiêu Product Backlog

> Xây dựng hệ thống **Quản lý Dự án và Công việc** theo mô hình **Agile/Scrum** với các tính năng:
> - Quản lý workspace, dự án, công việc (task) đầy đủ CRUD
> - Hỗ trợ nhiều chế độ xem: Kanban Board, List View, Calendar, Timeline, Spreadsheet
> - Quản lý Sprint/Cycle, Module, Labels, Dependencies
> - Hệ thống bình luận, thông báo, nhật ký hoạt động thời gian thực (SignalR)
> - Tích hợp AI để chia nhỏ công việc tự động
> - Hệ thống Gamification (điểm thưởng) khuyến khích làm việc
> - Giao diện hiện đại, dark theme, responsive trên mọi thiết bị
> - Bảo mật chặt chẽ: JWT Authentication, IDOR Protection, RBAC

### 1.2 Mục tiêu Release Backlog

#### Release 1.0 (MVP - Minimum Viable Product)
> Hoàn thành các chức năng cốt lõi cho phép nhóm quản lý công việc cơ bản:
> - Đăng ký, đăng nhập, quản lý hồ sơ
> - Tạo workspace, dự án
> - CRUD công việc với đầy đủ thuộc tính
> - Kanban Board + List View
> - Bình luận, gán thành viên

#### Release 2.0 (Full Features)
> Bổ sung tính năng nâng cao:
> - Cycles (Sprint), Modules
> - Calendar, Timeline, Spreadsheet views
> - Custom Views, Filters, Analytics
> - Pages, Sticky Notes, Drafts
> - AI Integration, Gamification
> - Admin panel, Audit Log

### 1.3 Mục tiêu Sprint Backlog

#### Sprint 1 (2 tuần)
> Hoàn thành khung sườn hệ thống: Authentication, Workspace, Project, Task CRUD cơ bản, Kanban Board

#### Sprint 2 (2 tuần)
> Hoàn thành Task properties đầy đủ, List View, Comments, Labels, Cycles, Modules

#### Sprint 3 (2 tuần)
> Hoàn thành tính năng nâng cao: Pages, Analytics, Admin, AI, Gamification

#### Sprint 4 (1 tuần)
> Testing, Bug fixing, Polish UI, Tài liệu, Chuẩn bị trình bày

---

## II. DANH SÁCH USER STORY - PRODUCT BACKLOG

### Bảng User Story tổng hợp

| ID | Epic | User Story | Priority | Story Points | MoSCoW |
|----|------|-----------|----------|-------------|--------|
| **US-001** | Auth | Là người dùng, tôi muốn **đăng ký tài khoản** với email/mật khẩu để tham gia hệ thống | 🔴 Must | 5 | Must Have |
| **US-002** | Auth | Là người dùng, tôi muốn **đăng nhập** bằng email/mật khẩu để truy cập hệ thống | 🔴 Must | 3 | Must Have |
| **US-003** | Auth | Là người dùng, tôi muốn **đăng nhập bằng GitHub** (OAuth) để tiện lợi hơn | 🟢 Low | 5 | Could Have |
| **US-004** | Auth | Là người dùng, tôi muốn **quản lý hồ sơ** (tên, avatar, email) của mình | 🟡 Medium | 3 | Should Have |
| **US-005** | Auth | Là người dùng, tôi muốn hệ thống **tự động refresh token** để không bị đăng xuất khi đang làm việc | 🔴 Must | 3 | Must Have |
| **US-006** | Workspace | Là người dùng, tôi muốn **tạo workspace** để nhóm nhiều dự án vào cùng một không gian làm việc | 🔴 Must | 5 | Must Have |
| **US-007** | Workspace | Là người quản lý, tôi muốn **mời thành viên** vào workspace bằng email | 🔴 Must | 5 | Must Have |
| **US-008** | Workspace | Là người quản lý, tôi muốn **quản lý vai trò thành viên** (Admin, Member, Guest) trong workspace | 🟡 Medium | 5 | Should Have |
| **US-009** | Project | Là người dùng, tôi muốn **tạo dự án mới** với tên, mô tả, icon, màu sắc | 🔴 Must | 5 | Must Have |
| **US-010** | Project | Là người dùng, tôi muốn **xem danh sách dự án** mà mình tham gia | 🔴 Must | 3 | Must Have |
| **US-011** | Project | Là người quản lý, tôi muốn **chỉnh sửa thông tin dự án** (tên, mô tả, icon) | 🔴 Must | 3 | Must Have |
| **US-012** | Project | Là người quản lý, tôi muốn **xóa dự án** không còn sử dụng | 🟡 Medium | 2 | Should Have |
| **US-013** | Project | Là người quản lý, tôi muốn **quản lý thành viên dự án** (thêm/xóa/đổi quyền) | 🔴 Must | 5 | Must Have |
| **US-014** | Task | Là người dùng, tôi muốn **tạo công việc mới** với tiêu đề, mô tả, trạng thái, mức ưu tiên | 🔴 Must | 5 | Must Have |
| **US-015** | Task | Là người dùng, tôi muốn **xem chi tiết công việc** trong slide panel bên phải | 🔴 Must | 5 | Must Have |
| **US-016** | Task | Là người dùng, tôi muốn **chỉnh sửa tiêu đề** công việc inline (không cần refresh) | 🔴 Must | 3 | Must Have |
| **US-017** | Task | Là người dùng, tôi muốn **thay đổi trạng thái** (To Do, In Progress, Review, Done, Cancelled) ngay lập tức | 🔴 Must | 3 | Must Have |
| **US-018** | Task | Là người dùng, tôi muốn **thay đổi mức ưu tiên** (Urgent/High/Medium/Low/None) và thấy cập nhật ngay | 🔴 Must | 3 | Must Have |
| **US-019** | Task | Là người dùng, tôi muốn **gán người thực hiện** (assignees) với tỉ lệ % tiến độ | 🔴 Must | 5 | Must Have |
| **US-020** | Task | Là người dùng, tôi muốn **gán nhãn** (labels) cho công việc để phân loại | 🟡 Medium | 3 | Should Have |
| **US-021** | Task | Là người dùng, tôi muốn **đặt ngày bắt đầu và hạn chót** cho công việc | 🔴 Must | 3 | Must Have |
| **US-022** | Task | Là người dùng, tôi muốn **viết mô tả** sử dụng rich text editor (bold, italic, heading, color, hình ảnh) | 🟡 Medium | 5 | Should Have |
| **US-023** | Task | Là người dùng, tôi muốn **tạo subtask** (công việc con) cho một task chính | 🟡 Medium | 5 | Should Have |
| **US-024** | Task | Là người dùng, tôi muốn **gán parent task** để tạo quan hệ phân cấp | 🟡 Medium | 3 | Should Have |
| **US-025** | Task | Là người dùng, tôi muốn **đính kèm tệp** vào công việc | 🟡 Medium | 5 | Should Have |
| **US-026** | Task | Là người dùng, tôi muốn **xóa công việc** với xác nhận (soft delete) | 🔴 Must | 2 | Must Have |
| **US-027** | Task | Là người dùng, tôi muốn **quản lý phụ thuộc** giữa các công việc (depends on, blocks) | 🟢 Low | 5 | Could Have |
| **US-028** | Board | Là người dùng, tôi muốn **xem Kanban Board** với các cột theo trạng thái | 🔴 Must | 8 | Must Have |
| **US-029** | Board | Là người dùng, tôi muốn **kéo thả task** giữa các cột để thay đổi trạng thái | 🔴 Must | 8 | Must Have |
| **US-030** | Board | Là người dùng, tôi muốn **lọc task trên board** theo priority, assignee, label | 🟡 Medium | 5 | Should Have |
| **US-031** | List | Là người dùng, tôi muốn **xem task dạng bảng** (List View) với các cột sortable | 🔴 Must | 5 | Must Have |
| **US-032** | List | Là người dùng, tôi muốn **sắp xếp task** theo tên, status, priority | 🟡 Medium | 3 | Should Have |
| **US-033** | List | Là người dùng, tôi muốn **inline edit** priority, status trực tiếp trong bảng | 🟡 Medium | 5 | Should Have |
| **US-034** | Calendar | Là người dùng, tôi muốn **xem task trên lịch** theo ngày bắt đầu/hạn chót | 🟡 Medium | 5 | Should Have |
| **US-035** | Timeline | Là người dùng, tôi muốn **xem Gantt/Timeline** để theo dõi tiến độ | 🟢 Low | 8 | Could Have |
| **US-036** | Spreadsheet | Là người dùng, tôi muốn **xem dạng bảng tính** (spreadsheet) để quản lý dữ liệu | 🟢 Low | 5 | Could Have |
| **US-037** | Cycle | Là người dùng, tôi muốn **tạo cycle (sprint)** với tên, ngày bắt đầu, ngày kết thúc | 🔴 Must | 5 | Must Have |
| **US-038** | Cycle | Là người dùng, tôi muốn **gán task vào cycle** để quản lý theo sprint | 🔴 Must | 3 | Must Have |
| **US-039** | Cycle | Là người dùng, tôi muốn **xem danh sách task trong cycle** | 🔴 Must | 3 | Must Have |
| **US-040** | Cycle | Là người dùng, tôi muốn **xem Burndown Chart** của sprint | 🟡 Medium | 8 | Should Have |
| **US-041** | Module | Là người dùng, tôi muốn **tạo module** để nhóm các task theo tính năng/phân hệ | 🟡 Medium | 3 | Should Have |
| **US-042** | Module | Là người dùng, tôi muốn **gán task vào module** | 🟡 Medium | 2 | Should Have |
| **US-043** | Comment | Là người dùng, tôi muốn **bình luận** trên công việc để trao đổi với nhóm | 🔴 Must | 5 | Must Have |
| **US-044** | Comment | Là người dùng, tôi muốn **sửa/xóa bình luận** của mình | 🟡 Medium | 3 | Should Have |
| **US-045** | Comment | Là người dùng, tôi muốn **thêm reaction (emoji)** vào bình luận | 🟢 Low | 3 | Could Have |
| **US-046** | Label | Là người quản lý, tôi muốn **tạo/sửa/xóa label** với tên và màu sắc tùy chỉnh | 🟡 Medium | 3 | Should Have |
| **US-047** | Notification | Là người dùng, tôi muốn **nhận thông báo** khi được gán task, bình luận mới | 🟡 Medium | 5 | Should Have |
| **US-048** | Notification | Là người dùng, tôi muốn **xem danh sách thông báo** trong dropdown | 🟡 Medium | 3 | Should Have |
| **US-049** | Page | Là người dùng, tôi muốn **tạo trang tài liệu** (Pages) với WYSIWYG editor | 🟢 Low | 8 | Could Have |
| **US-050** | Sticky | Là người dùng, tôi muốn **tạo ghi chú nhanh** (Sticky Notes) | 🟢 Low | 3 | Could Have |
| **US-051** | Draft | Là người dùng, tôi muốn **lưu bản nháp** công việc trước khi publish | 🟢 Low | 5 | Could Have |
| **US-052** | View | Là người dùng, tôi muốn **tạo custom view** với bộ lọc và sắp xếp riêng | 🟡 Medium | 5 | Should Have |
| **US-053** | View | Là người dùng, tôi muốn **lưu bộ lọc** để sử dụng lại | 🟡 Medium | 3 | Should Have |
| **US-054** | Analytics | Là người dùng, tôi muốn **xem thống kê dự án** (task by status, priority, assignee) | 🟡 Medium | 8 | Should Have |
| **US-055** | Analytics | Là người dùng, tôi muốn **xem thống kê toàn workspace** | 🟢 Low | 5 | Could Have |
| **US-056** | Admin | Là admin, tôi muốn **quản lý người dùng** toàn hệ thống (xem, khóa, xóa) | 🟡 Medium | 5 | Should Have |
| **US-057** | Admin | Là admin, tôi muốn **quản lý phòng ban** (department) | 🟢 Low | 5 | Could Have |
| **US-058** | Admin | Là admin, tôi muốn **xem audit log** các thao tác quan trọng | 🟡 Medium | 5 | Should Have |
| **US-059** | Admin | Là admin, tôi muốn **cài đặt hệ thống** (system settings) | 🟢 Low | 3 | Could Have |
| **US-060** | AI | Là người dùng, tôi muốn dùng **AI chia nhỏ task** thành subtasks tự động | 🟢 Low | 8 | Could Have |
| **US-061** | Gamification | Là người dùng, tôi muốn **nhận điểm thưởng** khi hoàn thành task | 🟢 Low | 5 | Could Have |
| **US-062** | Gamification | Là người dùng, tôi muốn **xem bảng xếp hạng** và ví điểm của mình | 🟢 Low | 3 | Could Have |
| **US-063** | Realtime | Là người dùng, tôi muốn **thấy cập nhật real-time** khi đồng nghiệp thay đổi task | 🟡 Medium | 8 | Should Have |
| **US-064** | Intake | Là người dùng, tôi muốn **tạo yêu cầu (intake)** và phân loại trước khi tạo task | 🟢 Low | 5 | Could Have |
| **US-065** | Nav | Là người dùng, tôi muốn **sidebar navigation** mượt mà và có thể tùy chỉnh | 🔴 Must | 5 | Must Have |
| **US-066** | Nav | Là người dùng, tôi muốn **dark mode** làm giao diện mặc định | 🔴 Must | 3 | Must Have |
| **US-067** | Responsive | Là người dùng, tôi muốn hệ thống **responsive trên mobile/tablet** | 🟡 Medium | 5 | Should Have |

**Tổng:** 67 User Stories | **Tổng Story Points:** ~300

---

## III. RELEASE BACKLOG

### Release 1.0 — MVP (Must Have + Should Have chính)

| Sprint | User Stories | Mục tiêu | Story Points |
|--------|------------|----------|-------------|
| Sprint 1 | US-001→US-013, US-065, US-066 | Auth + Workspace + Project | ~58 |
| Sprint 2 | US-014→US-033, US-037→US-039, US-043 | Task Core + Board + List + Cycles + Comments | ~100 |

**Tổng Release 1.0:** ~158 SP

### Release 2.0 — Full Features (Should Have + Could Have)

| Sprint | User Stories | Mục tiêu | Story Points |
|--------|------------|----------|-------------|
| Sprint 3 | US-034→US-036, US-040→US-042, US-044→US-055, US-063, US-067 | Views, Analytics, Modules, Notifications, Realtime | ~110 |
| Sprint 4 | US-056→US-062, US-064 | Admin, AI, Gamification, Intake, Testing | ~47 |

**Tổng Release 2.0:** ~157 SP  
**Tổng dự án:** ~315 SP

---

## IV. SPRINT BACKLOG

### Sprint 1: Authentication + Workspace + Project (2 tuần)

**Mục tiêu Sprint:** Xây dựng nền tảng hệ thống — người dùng có thể đăng ký, đăng nhập, tạo workspace, tạo project và quản lý thành viên.

| # | User Story ID | Task | Assignee | Est (h) | Status |
|---|--------------|------|----------|---------|--------|
| 1 | US-001 | Thiết kế database schema (User, RefreshToken) | | | |
| 2 | US-001 | API Đăng ký (POST /api/auth/register) | | | |
| 3 | US-001 | Frontend form Register.vue | | | |
| 4 | US-002 | API Đăng nhập (POST /api/auth/login) | | | |
| 5 | US-002 | Frontend form Login.vue + Token storage | | | |
| 6 | US-005 | API Refresh Token + Axios interceptor | | | |
| 7 | US-004 | API Profile (GET/PUT /api/users/profile) | | | |
| 8 | US-004 | Frontend Profile.vue | | | |
| 9 | US-006 | DB schema (Workspace, WorkspaceMember) | | | |
| 10 | US-006 | API CRUD Workspace | | | |
| 11 | US-006 | Frontend ManageSpaces.vue | | | |
| 12 | US-007 | API Invite member + Accept invite | | | |
| 13 | US-007 | Frontend AddPeopleModal.vue | | | |
| 14 | US-008 | API Role management + RBAC middleware | | | |
| 15 | US-009 | DB schema (Project, ProjectMember) | | | |
| 16 | US-009 | API CRUD Project | | | |
| 17 | US-009 | Frontend CreateProjectModal.vue | | | |
| 18 | US-010 | API List projects | | | |
| 19 | US-010 | Frontend project list in sidebar | | | |
| 20 | US-011 | API Update project settings | | | |
| 21 | US-012 | API Delete project (soft delete) | | | |
| 22 | US-013 | API ProjectMembers CRUD | | | |
| 23 | US-065 | Frontend Sidebar navigation + Layout | | | |
| 24 | US-066 | Dark mode CSS theme | | | |

**Capacity:** ___ giờ/sprint  
**Sprint Goal:** Auth + Workspace + Project CRUD hoạt động end-to-end

---

### Sprint 2: Task Management + Board + List + Cycles + Comments (2 tuần)

**Mục tiêu Sprint:** Người dùng có thể tạo, chỉnh sửa task đầy đủ thuộc tính, xem Kanban Board, List View, tạo Sprint/Cycle, bình luận.

| # | User Story ID | Task | Assignee | Est (h) | Status |
|---|--------------|------|----------|---------|--------|
| 1 | US-014 | DB schema (WorkTask, TaskStatus, TaskAssignment) | | | |
| 2 | US-014 | API CRUD WorkTask | | | |
| 3 | US-014 | Frontend TaskDetailModal.vue | | | |
| 4 | US-015 | Slide panel detail task | | | |
| 5 | US-016 | Inline edit title (no refresh) | | | |
| 6 | US-017 | Status dropdown + API update | | | |
| 7 | US-018 | Priority dropdown + selectPriority handler | | | |
| 8 | US-019 | Assignees dropdown + % progress | | | |
| 9 | US-020 | Labels dropdown + IssueLabel API | | | |
| 10 | US-021 | Date pickers (start/due date) | | | |
| 11 | US-022 | Rich text editor for description | | | |
| 12 | US-023 | Subtask creation API + UI | | | |
| 13 | US-024 | Parent task assignment | | | |
| 14 | US-025 | File attachments (upload API + UI) | | | |
| 15 | US-026 | Delete task (soft delete + confirmation) | | | |
| 16 | US-028 | KanbanBoard.vue component | | | |
| 17 | US-029 | Drag & drop implementation | | | |
| 18 | US-030 | FilterBar.vue component | | | |
| 19 | US-031 | ListView.vue component | | | |
| 20 | US-032 | Column sorting trong list view | | | |
| 21 | US-033 | Inline edit trong list view | | | |
| 22 | US-037 | DB schema Sprint + API CRUD Sprint | | | |
| 23 | US-038 | Gán task vào cycle API | | | |
| 24 | US-039 | CyclesTab.vue component | | | |
| 25 | US-043 | Comments API + UI trong detail | | | |
| 26 | US-046 | Labels CRUD API + LabelManager.vue | | | |

**Capacity:** ___ giờ/sprint  
**Sprint Goal:** Task CRUD full properties + Kanban + List + Cycles + Comments

---

### Sprint 3: Advanced Views + Analytics + Modules + Notifications (2 tuần)

**Mục tiêu Sprint:** Bổ sung các tính năng nâng cao: Calendar, Timeline, Spreadsheet views, Analytics, Modules, Notifications, Real-time.

| # | User Story ID | Task | Assignee | Est (h) | Status |
|---|--------------|------|----------|---------|--------|
| 1 | US-034 | CalendarTab.vue component | | | |
| 2 | US-035 | TimelineTab.vue (Gantt) | | | |
| 3 | US-036 | SpreadsheetTab.vue | | | |
| 4 | US-040 | Burndown chart implementation | | | |
| 5 | US-041 | Module CRUD API + ModulesTab.vue | | | |
| 6 | US-042 | Assign task to module API | | | |
| 7 | US-044 | Edit/Delete comment | | | |
| 8 | US-045 | Emoji reactions on comments | | | |
| 9 | US-047 | Notification system (backend) | | | |
| 10 | US-048 | NotificationsDropdown.vue | | | |
| 11 | US-049 | Pages CRUD + PagesTab.vue + WYSIWYG | | | |
| 12 | US-050 | Sticky Notes + StickiesView.vue | | | |
| 13 | US-051 | Drafts + DraftsView.vue | | | |
| 14 | US-052 | Custom Views API + ViewsTab.vue | | | |
| 15 | US-053 | Save filter as view | | | |
| 16 | US-054 | Project Analytics dashboard | | | |
| 17 | US-055 | GlobalAnalyticsView.vue | | | |
| 18 | US-063 | SignalR real-time updates | | | |
| 19 | US-067 | Responsive mobile/tablet testing | | | |

**Capacity:** ___ giờ/sprint  
**Sprint Goal:** Full view modes + Analytics + Notifications + Modules working

---

### Sprint 4: Admin + AI + Gamification + Testing + Polish (1 tuần)

**Mục tiêu Sprint:** Hoàn thiện hệ thống Admin, AI, Gamification. Chạy test, fix bugs, hoàn thiện tài liệu.

| # | User Story ID | Task | Assignee | Est (h) | Status |
|---|--------------|------|----------|---------|--------|
| 1 | US-056 | Admin Users management | | | |
| 2 | US-057 | Departments management | | | |
| 3 | US-058 | Audit Log view | | | |
| 4 | US-059 | System Settings | | | |
| 5 | US-060 | AI Split Subtasks feature | | | |
| 6 | US-061 | Gamification - points system | | | |
| 7 | US-062 | Leaderboard + Wallet view | | | |
| 8 | US-064 | Intake inbox feature | | | |
| 9 | US-003 | GitHub OAuth integration | | | |
| 10 | US-027 | Task Dependencies | | | |
| 11 | — | Full regression testing (100+ test cases) | | | |
| 12 | — | Bug fixing round 1 | | | |
| 13 | — | Bug fixing round 2 | | | |
| 14 | — | UI Polish & Performance tuning | | | |
| 15 | — | Chuẩn bị slide trình bày | | | |
| 16 | — | Viết tài liệu hướng dẫn | | | |

**Capacity:** ___ giờ/sprint  
**Sprint Goal:** Full features + Stable system + Ready for submission

---

## V. BURNDOWN CHART

> **Ghi chú:** Sử dụng https://www.burndownfortrello.com/ để sinh Burndown Chart khi tracking trên Trello.

### Mẫu Burndown Data

| Ngày | Planned Points Remaining | Actual Points Remaining |
|------|--------------------------|------------------------|
| Day 1 | 58 | 58 |
| Day 2 | 54 | 55 |
| Day 3 | 50 | 50 |
| Day 4 | 46 | 48 |
| Day 5 | 42 | 43 |
| Day 6 | 38 | 40 |
| Day 7 | 34 | 35 |
| Day 8 | 29 | 30 |
| Day 9 | 25 | 25 |
| Day 10 | 20 | 18 |

---

## VI. TIÊU CHÍ CHẤP NHẬN (Definition of Done)

Một User Story được coi là **Done** khi:

- [ ] Code đã được review và merge vào branch chính
- [ ] Unit test / Integration test PASS
- [ ] Manual test PASS theo test case
- [ ] Không có bug Critical hoặc High
- [ ] UI responsive trên desktop
- [ ] API trả về đúng format chuẩn
- [ ] Document / comment code đầy đủ

---

**Người lập:** ___________  
**Ngày tạo:** 19/04/2026  
**Phiên bản:** 1.0
