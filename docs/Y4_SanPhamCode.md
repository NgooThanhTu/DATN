# 💻 Y4: SẢN PHẨM CODE SAU KHI HOÀN THÀNH SPRINT

**Dự án:** Hệ thống Quản lý Công việc (QuanLyCongViec / SprintA)  
**Ngày lập:** 19/04/2026  
**Phiên bản:** 1.0

---

## I. TỔNG QUAN SẢN PHẨM

### 1.1 Thông tin chung

| Thuộc tính | Giá trị |
|-----------|---------|
| **Tên dự án** | SprintA - Hệ thống Quản trị Dự án & Công việc |
| **Mô tả** | Nền tảng quản lý dự án Agile/Scrum với Kanban Board, tích hợp AI, Gamification |
| **Repository** | `d:\A\QuanLyCongViec` |
| **License** | Internal / Academic |

### 1.2 Technology Stack

| Thành phần | Công nghệ | Phiên bản |
|-----------|-----------|-----------|
| **Backend** | C# ASP.NET Core | .NET 10 |
| **ORM** | Entity Framework Core | Code First |
| **Database** | SQL Server | Latest |
| **Frontend** | Vue 3 | Composition API `<script setup>` |
| **Build Tool** | Vite | Latest |
| **UI Framework** | Element Plus | Latest |
| **CSS** | TailwindCSS | Latest |
| **State** | Pinia | Latest |
| **HTTP** | Axios | Latest |
| **Real-time** | SignalR | .NET Hub |

---

## II. CẤU TRÚC DỰ ÁN

### 2.1 Tổng quan cây thư mục

```
QuanLyCongViec/
├── Backend/                          # ASP.NET Core Backend
│   ├── TaskManagement.slnx           # Solution file
│   ├── seed_data.sql                 # Database seed data
│   └── src/
│       ├── TaskManagement.API/       # Web API Layer
│       │   ├── Controllers/          # 24 API Controllers
│       │   ├── Extensions/           # DI Extension Methods
│       │   ├── Filters/              # Action Filters
│       │   ├── Hubs/                 # SignalR Hubs
│       │   ├── Middlewares/          # Custom Middlewares
│       │   ├── Program.cs            # App Entry Point
│       │   └── appsettings.json      # Configuration
│       ├── TaskManagement.Application/ # Application/Service Layer
│       ├── TaskManagement.Domain/     # Domain Layer
│       │   ├── Entities/             # 47 Entity Models
│       │   └── Constants/            # Enums & Constants
│       └── TaskManagement.Infrastructure/ # Data/Infrastructure
│
├── Frontend/                         # Vue 3 Frontend
│   ├── index.html                    # SPA Entry
│   ├── package.json                  # Dependencies
│   ├── vite.config.js                # Vite Configuration
│   ├── tailwind.config.js            # TailwindCSS Config
│   └── src/
│       ├── App.vue                   # Root Component
│       ├── main.js                   # Vue App Bootstrap
│       ├── style.css                 # Global Styles
│       ├── api/                      # API Client Layer
│       │   ├── axiosClient.js        # Axios Config + Interceptors
│       │   ├── adminUserApi.js       # Admin API
│       │   └── signalrService.js     # SignalR Service
│       ├── components/               # 24 Reusable Components
│       │   ├── KanbanBoard.vue       # Kanban Board (Drag & Drop)
│       │   ├── ListView.vue          # Table/List View
│       │   ├── TaskDetailModal.vue   # Task Detail Slide Panel
│       │   ├── CyclesTab.vue         # Sprint/Cycle Management
│       │   ├── ModulesTab.vue        # Module Management
│       │   ├── PagesTab.vue          # Page Editor (WYSIWYG)
│       │   ├── ViewsTab.vue          # Custom Views
│       │   ├── FilterBar.vue         # Search & Filter
│       │   ├── CalendarTab.vue       # Calendar View
│       │   ├── TimelineTab.vue       # Gantt/Timeline View
│       │   ├── SpreadsheetTab.vue    # Spreadsheet View
│       │   ├── CreateProjectModal.vue  # Project Creation
│       │   ├── CreateSpaceModal.vue  # Workspace Creation
│       │   ├── AddPeopleModal.vue    # Member Invitation
│       │   ├── LabelManager.vue      # Label CRUD
│       │   ├── NotificationsDropdown.vue # Notifications
│       │   ├── IntakeInbox.vue       # Intake Management
│       │   ├── ModuleList.vue        # Module List
│       │   ├── CustomizeSidebarModal.vue # Sidebar Settings
│       │   ├── HelpDropdown.vue      # Help Menu
│       │   ├── SettingsDropdown.vue   # Settings Menu
│       │   ├── UserDropdown.vue      # User Profile Menu
│       │   ├── ErrorBoundary.vue     # Error Handler
│       │   └── layout/              # Layout Components
│       ├── views/                    # 27 Page Views
│       │   ├── Login.vue             # Login Page
│       │   ├── Register.vue          # Register Page
│       │   ├── Dashboard.vue         # Main Dashboard
│       │   ├── Home.vue              # Home/Landing
│       │   ├── SpaceSummary.vue      # Project Main View
│       │   ├── ManageSpaces.vue      # Workspace Management
│       │   ├── Profile.vue           # User Profile
│       │   ├── CyclesView.vue        # Cycles Page
│       │   ├── ModulesView.vue       # Modules Page
│       │   ├── PagesView.vue         # Pages
│       │   ├── StickiesView.vue      # Sticky Notes
│       │   ├── DraftsView.vue        # Drafts
│       │   ├── GlobalAnalyticsView.vue  # Analytics
│       │   ├── GlobalViewsView.vue   # Global Views
│       │   ├── GlobalArchivesView.vue   # Archives
│       │   ├── YourWorkView.vue      # My Work
│       │   ├── RewardsView.vue       # Gamification/Rewards
│       │   ├── AIPage.vue            # AI Features
│       │   ├── AuditLog.vue          # Audit Log
│       │   ├── UserManagement.vue    # User Management
│       │   ├── IntakesView.vue       # Intake View
│       │   ├── Recent.vue            # Recent Activity
│       │   ├── AcceptInvite.vue      # Invite Acceptance
│       │   ├── GitHubCallback.vue    # OAuth Callback
│       │   └── admin/               # Admin Panel Views
│       ├── store/                    # Pinia State Management
│       │   ├── useWorkTaskStore.js   # Task State
│       │   ├── useProjectStore.js    # Project State
│       │   ├── useSprintStore.js     # Sprint State
│       │   ├── useActivityStore.js   # Activity State
│       │   └── useAdminUserStore.js  # Admin State
│       ├── router/                   # Vue Router
│       │   ├── index.js              # Main Router
│       │   ├── authRoutes.js         # Auth Routes
│       │   ├── dashboardRoutes.js    # Dashboard Routes
│       │   ├── spaceRoutes.js        # Space/Project Routes
│       │   ├── adminRoutes.js        # Admin Routes
│       │   ├── aiRoutes.js           # AI Routes
│       │   └── homeRoutes.js         # Home Routes
│       └── utils/                    # Utility Functions
│
├── docs/                             # Tài liệu dự án
│   ├── Y1_SoDoPhanRaChucNang_KeHoach.md
│   ├── Y2_ProductBacklog_SprintBacklog.md
│   ├── Y3_TestCases.md
│   ├── Y4_SanPhamCode.md (file này)
│   └── Y5_Slide_Scrum.md
│
├── TEST_PLAN.md                      # Kế hoạch test (100+ TC)
├── TEST_EXECUTION_GUIDE.md           # Hướng dẫn thực thi test
├── QUICK_TEST_CHECKLIST.md           # Checklist test nhanh
├── QA_TEST_SUMMARY.md                # Tóm tắt QA
├── CONTEXT.md                        # Quy tắc coding
├── SYSTEM.md                         # Kiến trúc tổng quan
├── run.bat                           # Script chạy dự án
└── test_api.ps1                      # Script test API
```

---

## III. DANH SÁCH BACKEND API CONTROLLERS (24 Controllers)

| # | Controller | Endpoint Base | Chức năng | HTTP Methods |
|---|-----------|--------------|-----------|-------------|
| 1 | AuthController | `/api/auth` | Đăng ký, đăng nhập, refresh token, OAuth GitHub | POST |
| 2 | WorkspacesController | `/api/workspaces` | CRUD Workspace, Members, Invite | GET, POST, PUT, DELETE |
| 3 | ProjectsController | `/api/projects` | CRUD Project | GET, POST, PUT, DELETE |
| 4 | ProjectMembersController | `/api/projects/{id}/members` | Quản lý thành viên project | GET, POST, DELETE |
| 5 | WorkTasksController | `/api/worktasks` | CRUD Task, Properties, Bulk ops | GET, POST, PUT, PATCH, DELETE |
| 6 | CommentsController | `/api/comments` | CRUD Comment, Reactions | GET, POST, PUT, DELETE |
| 7 | LabelsController | `/api/labels` | CRUD Labels | GET, POST, PUT, DELETE |
| 8 | SprintsController | `/api/sprints` | CRUD Sprint/Cycle | GET, POST, PUT, DELETE |
| 9 | ModulesController | `/api/modules` | CRUD Modules, Task assignment | GET, POST, PUT, DELETE |
| 10 | PagesController | `/api/pages` | CRUD Pages (WYSIWYG) | GET, POST, PUT, DELETE |
| 11 | StickiesController | `/api/stickies` | CRUD Sticky Notes | GET, POST, PUT, DELETE |
| 12 | DraftsController | `/api/drafts` | CRUD Task Drafts | GET, POST, PUT, DELETE |
| 13 | IntakesController | `/api/intakes` | CRUD Intakes | GET, POST, PUT, DELETE |
| 14 | NotificationsController | `/api/notifications` | Đọc/đánh dấu thông báo | GET, PUT |
| 15 | ProjectViewsController | `/api/projectviews` | CRUD Custom Views | GET, POST, PUT, DELETE |
| 16 | TaskDependenciesController | `/api/taskdependencies` | Quản lý phụ thuộc | GET, POST, DELETE |
| 17 | UsersController | `/api/users` | Quản lý profile, avatar | GET, PUT |
| 18 | AdminUsersController | `/api/admin/users` | Admin quản lý users | GET, PUT, DELETE |
| 19 | DepartmentsController | `/api/departments` | CRUD Departments | GET, POST, PUT, DELETE |
| 20 | AuditLogsController | `/api/auditlogs` | Xem audit log | GET |
| 21 | GamificationController | `/api/gamification` | Ví điểm, leaderboard | GET, POST |
| 22 | AiController | `/api/ai` | AI split subtasks | POST |
| 23 | SystemSettingsController | `/api/systemsettings` | Cài đặt hệ thống | GET, PUT |
| 24 | SystemController | `/api/system` | Health check | GET |

**Tổng API endpoints:** ~100+

---

## IV. DANH SÁCH DOMAIN ENTITIES (47 Entities)

| # | Entity | Mô tả | Quan hệ chính |
|---|--------|-------|---------------|
| 1 | User | Người dùng hệ thống | → UserRole, ProjectMember, WorkspaceMember |
| 2 | RefreshToken | JWT Refresh Token | → User |
| 3 | Role | Vai trò (Admin, Member, Guest) | → RolePermission |
| 4 | Permission | Quyền hệ thống | → RolePermission |
| 5 | UserRole | Gán Role cho User | → User, Role |
| 6 | RolePermission | Gán Permission cho Role | → Role, Permission |
| 7 | Organization | Tổ chức | → Workspace |
| 8 | Workspace | Không gian làm việc | → WorkspaceMember, Project |
| 9 | WorkspaceMember | Thành viên workspace | → User, Workspace |
| 10 | Project | Dự án | → ProjectMember, WorkTask, Sprint |
| 11 | ProjectMember | Thành viên dự án | → User, Project |
| 12 | WorkTask | Công việc (Task) | → TaskAssignment, Comment, Label |
| 13 | TaskStatus | Trạng thái công việc | → WorkTask |
| 14 | TaskType | Loại công việc | → WorkTask |
| 15 | TaskAssignment | Gán người thực hiện | → User, WorkTask |
| 16 | TaskDependency | Phụ thuộc giữa tasks | → WorkTask |
| 17 | TaskDraft | Bản nháp task | → User, Project |
| 18 | Label | Nhãn | → IssueLabel |
| 19 | IssueLabel | Gán nhãn cho task | → WorkTask, Label |
| 20 | Sprint | Sprint/Cycle | → Project, WorkTask |
| 21 | Module | Module/Phân hệ | → IssueModule |
| 22 | IssueModule | Gán task vào module | → WorkTask, Module |
| 23 | Comment | Bình luận | → User, WorkTask |
| 24 | CommentAttachment | Đính kèm trong comment | → Comment |
| 25 | Attachment | Đính kèm tệp | → WorkTask |
| 26 | Notification | Thông báo | → User |
| 27 | Page | Trang tài liệu | → Project, User |
| 28 | StickyNote | Ghi chú nhanh | → User |
| 29 | ProjectView | Custom View | → Project |
| 30 | AuditLog | Nhật ký hoạt động | → User |
| 31 | SystemAuditLog | Nhật ký hệ thống | — |
| 32 | Department | Phòng ban | → DepartmentMember |
| 33 | DepartmentMember | Thành viên phòng ban | → User, Department |
| 34 | ProjectDepartmentRole | Vai trò phòng ban | → Project, Department |
| 35 | UserWallet | Ví điểm thưởng | → User |
| 36 | PointTransaction | Giao dịch điểm | → User |
| 37 | Intake | Yêu cầu đầu vào | → Project, User |
| 38 | TimeLog | Chấm công/thời gian | → User, WorkTask |
| 39 | PerformanceReview | Đánh giá hiệu suất | → User |
| 40 | ProjectTemplate | Mẫu dự án | — |
| 41 | TenantConfig | Cấu hình tenant | — |
| 42 | SystemSetting | Cài đặt hệ thống | — |
| 43 | AIFeedback | Phản hồi AI | → User |
| 44 | AIPromptTemplate | Mẫu prompt AI | — |
| 45 | AITokenUsage | Sử dụng token AI | → User |
| 46 | AITrainingDataset | Dataset AI | — |
| 47 | TaskVectorEmbedding | Vector nhúng AI | → WorkTask |

---

## V. HƯỚNG DẪN CHẠY DỰ ÁN

### 5.1 Yêu cầu hệ thống

```
- .NET 10 SDK
- Node.js 18+
- SQL Server (LocalDB hoặc Express)
- Git
```

### 5.2 Cài đặt và chạy

```bash
# 1. Clone repository
git clone <repository-url>
cd QuanLyCongViec

# 2. Backend
cd Backend/src/TaskManagement.API
dotnet restore
dotnet run
# → API chạy tại http://localhost:5136

# 3. Frontend (terminal mới)
cd Frontend
npm install
npm run dev
# → App chạy tại http://localhost:5173

# 4. Database seed (nếu cần)
# Chạy file Backend/seed_data.sql trên SQL Server
```

### 5.3 Sử dụng script `run.bat`

```bash
# Chạy nhanh cả Backend + Frontend
run.bat
```

### 5.4 Test API

```powershell
# Chạy script test API
.\test_api.ps1
# hoặc
.\test_api_simple.ps1
```

---

## VI. KẾT QUẢ SPRINT

### Sprint 1 - Kết quả

| Hạng mục | Trạng thái | Ghi chú |
|----------|-----------|---------|
| Auth (Login/Register/Token) | | |
| Workspace CRUD | | |
| Project CRUD + Members | | |
| Sidebar Navigation | | |
| Dark Mode Theme | | |

### Sprint 2 - Kết quả

| Hạng mục | Trạng thái | Ghi chú |
|----------|-----------|---------|
| Task CRUD + All Properties | | |
| Kanban Board + Drag & Drop | | |
| List View + Sort + Inline Edit | | |
| Cycles/Sprint CRUD | | |
| Comments + Reactions | | |
| Labels CRUD | | |
| Priority Fix (no F5 refresh) | | ✅ BUG-001 Fixed |

### Sprint 3 - Kết quả

| Hạng mục | Trạng thái | Ghi chú |
|----------|-----------|---------|
| Calendar / Timeline / Spreadsheet Views | | |
| Modules CRUD | | |
| Notifications | | |
| Pages (WYSIWYG) | | |
| Sticky Notes | | |
| Drafts | | |
| Custom Views + Filters | | |
| Analytics (Project + Global) | | |
| SignalR Real-time | | |

### Sprint 4 - Kết quả

| Hạng mục | Trạng thái | Ghi chú |
|----------|-----------|---------|
| Admin Users | | |
| Departments | | |
| Audit Log | | |
| AI Subtask Split | | |
| Gamification | | |
| Full Regression Test | | |
| Bug Fixes | | |

---

## VII. THỐNG KÊ CODE

| Metric | Giá trị |
|--------|---------|
| **Backend Controllers** | 24 |
| **Domain Entities** | 47 |
| **Frontend Components** | 24 |
| **Frontend Views** | 27 |
| **Pinia Stores** | 5 |
| **Router Modules** | 8 |
| **API Endpoints** | ~100+ |
| **Test Cases** | 85 |
| **Known Bugs Fixed** | 1 (Priority refresh) |

---

**Người lập:** ___________  
**Ngày tạo:** 19/04/2026  
**Phiên bản:** 1.0
