# 🧪 Y3: TEST CASES - DỰ ÁN QUẢN LÝ CÔNG VIỆC

**Dự án:** Hệ thống Quản lý Công việc (QuanLyCongViec / SprintA)  
**Ngày lập:** 19/04/2026  
**Tester:** QA Team  
**Phiên bản:** 1.0

---

## I. TỔNG QUAN TEST

### 1.1 Phạm vi test

| Phạm vi | Mô tả |
|---------|--------|
| **Included** | Toàn bộ tính năng user-facing: Auth, Project, Task, Board, List, Cycles, Modules, Comments, Labels, Analytics, Views, Admin |
| **Excluded** | Load testing (1000+ users), Penetration testing chuyên sâu |
| **Môi trường** | Frontend: http://localhost:5173 / Backend: http://localhost:5136 |
| **Browser** | Chrome (latest), Firefox (latest), Edge (latest) |
| **Database** | SQL Server + Seed data |

### 1.2 Tài khoản test

```
Admin:     admin@sprinta.com / Admin@123
User 1:    test@example.com / Test@123  
User 2:    user2@test.com / Test@123
```

---

## II. BẢNG TEST CASE CHI TIẾT

### PHẦN 1: AUTHENTICATION (Đăng nhập / Đăng ký)

| TC ID | Tên Test Case | Precondition | Bước thực hiện | Dữ liệu test | Kết quả mong đợi | Mức ưu tiên | Kết quả | Ghi chú |
|-------|--------------|-------------|----------------|--------------|-------------------|-------------|---------|---------|
| TC-AUTH-01 | Đăng nhập thành công | Có tài khoản hợp lệ | 1. Truy cập /login<br>2. Nhập email hợp lệ<br>3. Nhập mật khẩu đúng<br>4. Click "Login" | Email: test@example.com<br>Password: Test@123 | Đăng nhập thành công, chuyển đến Dashboard. Token được lưu | 🔴 Critical | | |
| TC-AUTH-02 | Đăng nhập email sai | Trang Login | 1. Nhập email không tồn tại<br>2. Nhập password bất kì<br>3. Click "Login" | Email: wrong@test.com<br>Password: abc123 | Hiển thị lỗi "Email hoặc mật khẩu không chính xác" | 🔴 Critical | | |
| TC-AUTH-03 | Đăng nhập password sai | Trang Login | 1. Nhập email đúng<br>2. Nhập password sai<br>3. Click "Login" | Email: test@example.com<br>Password: WrongPass | Hiển thị lỗi "Email hoặc mật khẩu không chính xác" | 🔴 Critical | | |
| TC-AUTH-04 | Đăng nhập email rỗng | Trang Login | 1. Để trống email<br>2. Nhập password bất kì<br>3. Click "Login" | Email: (trống)<br>Password: Test@123 | Hiển thị validation "Email is required" | 🟡 High | | |
| TC-AUTH-05 | Đăng nhập password rỗng | Trang Login | 1. Nhập email đúng<br>2. Để trống password<br>3. Click "Login" | Email: test@example.com<br>Password: (trống) | Hiển thị validation "Password is required" | 🟡 High | | |
| TC-AUTH-06 | Đăng ký tài khoản mới | Trang Register | 1. Truy cập /register<br>2. Nhập tên, email, password<br>3. Click "Register" | Name: Test User<br>Email: new@test.com<br>Password: NewPass@123 | Đăng ký thành công, chuyển đến login/dashboard | 🔴 Critical | | |
| TC-AUTH-07 | Đăng ký email đã tồn tại | Trang Register | 1. Nhập email đã có trong DB<br>2. Click "Register" | Email: test@example.com | Hiển thị lỗi "Email đã tồn tại" | 🟡 High | | |
| TC-AUTH-08 | Đăng xuất | Đã đăng nhập | 1. Click avatar/menu<br>2. Click "Sign out" | — | Token bị xóa, chuyển về trang Login | 🟡 High | | |
| TC-AUTH-09 | Token auto-refresh | Đã đăng nhập, token sắp hết hạn | 1. Chờ token gần hết hạn<br>2. Thực hiện API call | — | Token được refresh tự động, không bị logout | 🟡 High | | |

### PHẦN 2: WORKSPACE & PROJECT

| TC ID | Tên Test Case | Precondition | Bước thực hiện | Dữ liệu test | Kết quả mong đợi | Mức ưu tiên | Kết quả | Ghi chú |
|-------|--------------|-------------|----------------|--------------|-------------------|-------------|---------|---------|
| TC-WS-01 | Tạo workspace mới | Đã đăng nhập | 1. Click "Create Workspace"<br>2. Nhập tên workspace<br>3. Click "Create" | Name: "Team Alpha" | Workspace được tạo, hiển thị trong danh sách | 🔴 Critical | | |
| TC-WS-02 | Xem danh sách workspace | Đã đăng nhập + có workspace | 1. Truy cập trang Spaces | — | Hiển thị tất cả workspace mà user tham gia | 🔴 Critical | | |
| TC-WS-03 | Mời thành viên vào workspace | Owner/Admin workspace | 1. Vào workspace settings<br>2. Click "Invite"<br>3. Nhập email<br>4. Chọn role | Email: user2@test.com<br>Role: Member | Lời mời được gửi, thành viên có thể accept | 🟡 High | | |
| TC-PROJ-01 | Tạo project mới | Trong workspace | 1. Click "New Project" / "+"<br>2. Nhập tên, chọn icon/màu<br>3. Click "Create" | Name: "Sprint App"<br>Identifier: "SP" | Project tạo thành công, hiển thị trong sidebar | 🔴 Critical | | |
| TC-PROJ-02 | Xem danh sách projects | Trong workspace | 1. Xem sidebar<br>2. Click "Projects" | — | Tất cả projects hiển thị đúng tên, icon | 🔴 Critical | | |
| TC-PROJ-03 | Chỉnh sửa project | Là project member | 1. Click project settings<br>2. Sửa tên, mô tả<br>3. Click "Save" | Name: "Sprint App v2" | Thay đổi được lưu lại ngay | 🟡 High | | |
| TC-PROJ-04 | Xóa project | Project Admin | 1. Click settings → Delete<br>2. Xác nhận xóa | — | Project bị xóa (soft delete), không hiển thị | 🟡 High | | |
| TC-PROJ-05 | Thêm thành viên project | Project Admin | 1. Vào project settings → Members<br>2. Click "Add member"<br>3. Chọn member | — | Member được thêm vào project | 🟡 High | | |
| TC-PROJ-06 | Xóa thành viên project | Project Admin | 1. Vào project Members<br>2. Click remove trên member | — | Member bị loại khỏi project | 🟡 High | | |

### PHẦN 3: TASK MANAGEMENT (CRUD)

| TC ID | Tên Test Case | Precondition | Bước thực hiện | Dữ liệu test | Kết quả mong đợi | Mức ưu tiên | Kết quả | Ghi chú |
|-------|--------------|-------------|----------------|--------------|-------------------|-------------|---------|---------|
| TC-TASK-01 | Tạo task mới | Trong project | 1. Click "New Work Item" / "+"<br>2. Nhập tiêu đề<br>3. Chọn Status, Priority<br>4. Click "Save" | Title: "Design Login Page"<br>Status: To Do<br>Priority: High | Task được tạo, xuất hiện trong board/list | 🔴 Critical | | |
| TC-TASK-02 | Xem chi tiết task | Có task trong project | 1. Click vào task card/row | — | Slide panel mở từ bên phải, hiển thị đầy đủ: title, description, status, priority, assignees, labels, dates | 🔴 Critical | | |
| TC-TASK-03 | Sửa tiêu đề task inline | Panel detail mở | 1. Click vào tiêu đề<br>2. Sửa text<br>3. Click outside / blur | New title: "Login Page v2" | Tiêu đề cập nhật NGAY, không cần F5. API PUT gọi thành công | 🔴 Critical | | ✅ FIXED |
| TC-TASK-04 | Thay đổi Status | Panel detail mở | 1. Click Status dropdown<br>2. Chọn status khác | Status: "In Progress" | Status cập nhật ngay cả UI và DB. Icon + text thay đổi. Board cột tương ứng | 🔴 Critical | | |
| TC-TASK-05 | Thay đổi Priority | Panel detail mở | 1. Click Priority dropdown<br>2. Chọn priority | Priority: "High" (Orange ↑) | **Priority cập nhật NGAY, không cần F5!** Icon + Text thay đổi tức thì | 🔴 Critical | | ✅ FIXED - selectPriority() handler |
| TC-TASK-06 | Gán Assignees | Panel detail mở | 1. Click Assignees dropdown<br>2. Search member<br>3. Click chọn<br>4. Nhập % progress | Assignee: "test@example.com"<br>Progress: 50% | Assignee được gán, avatar hiển thị, % lưu | 🔴 Critical | | |
| TC-TASK-07 | Thêm Labels | Panel detail mở | 1. Click Labels dropdown<br>2. Search label<br>3. Click để gán | Label: "Bug" (màu đỏ) | Label hiển thị trên task card với đúng màu | 🟡 High | | |
| TC-TASK-08 | Set Start Date | Panel detail mở | 1. Click Start Date picker<br>2. Chọn ngày | Start date: 2026-04-20 | Hiển thị format "Apr 20", lưu DB | 🟡 High | | |
| TC-TASK-09 | Set Due Date | Panel detail mở | 1. Click Due Date picker<br>2. Chọn ngày | Due date: 2026-04-30 | Hiển thị đúng, lưu DB thành công | 🟡 High | | |
| TC-TASK-10 | Gán Module | Panel detail mở | 1. Click Module dropdown<br>2. Search + chọn module | Module: "Backend" | Module name hiển thị đúng | 🟡 High | | |
| TC-TASK-11 | Gán Cycle (Sprint) | Panel detail mở | 1. Click Cycle dropdown<br>2. Search + chọn cycle | Cycle: "Sprint 1" | Cycle được gán, task xuất hiện trong Cycle view | 🟡 High | | |
| TC-TASK-12 | Gán Parent Task | Panel detail mở | 1. Click Parent dropdown<br>2. Search parent<br>3. Click chọn | Parent: "Feature X" | Parent ID + title hiển thị | 🟡 High | | |
| TC-TASK-13 | Sửa Description (Rich Editor) | Panel detail mở | 1. Click description area<br>2. Format: Bold, Italic, Heading<br>3. Insert image<br>4. Click outside | Description: "**Important** - This needs review" | Formatting lưu, persist khi reopen | 🟡 High | | |
| TC-TASK-14 | Tạo Subtask | Panel detail mở | 1. Click "Add sub-work item"<br>2. Nhập title subtask<br>3. Click "Create" | Subtask title: "Design header" | Subtask hiển thị trong danh sách dưới task chính | 🟡 High | | |
| TC-TASK-15 | AI Split Subtasks | Panel detail mở + có description | 1. Click "AI split into subtasks"<br>2. Chờ AI xử lý | — | AI tạo ra 3-5 subtasks tự động | 🟢 Medium | | |
| TC-TASK-16 | Xóa task | Panel detail mở | 1. Click menu (...) → Delete<br>2. Confirm dialog xuất hiện<br>3. Click "Confirm" | — | Task bị xóa (soft delete), không hiển thị trên board | 🔴 Critical | | |
| TC-TASK-17 | Đính kèm file | Panel detail mở | 1. Click "Attach"<br>2. Chọn file từ máy tính<br>3. Chờ upload | File: test_image.png (2MB) | File upload thành công, hiển thị tên file + icon | 🟡 High | | |
| TC-TASK-18 | Tạo task title rỗng | Trong project | 1. Click "New Work Item"<br>2. Để trống title<br>3. Click "Save" | Title: (trống) | Validation error: "Title is required" | 🟡 High | | |

### PHẦN 4: BOARD VIEW (Kanban)

| TC ID | Tên Test Case | Precondition | Bước thực hiện | Dữ liệu test | Kết quả mong đợi | Mức ưu tiên | Kết quả | Ghi chú |
|-------|--------------|-------------|----------------|--------------|-------------------|-------------|---------|---------|
| TC-BOARD-01 | Hiển thị Kanban Board | Project có tasks | 1. Vào project<br>2. Chọn tab "Board" | — | Hiển thị 5+ cột theo status: Backlog, To Do, In Progress, Review, Done. Mỗi task có card hiển thị title, priority icon, assignee avatar | 🔴 Critical | | |
| TC-BOARD-02 | Drag & Drop task | Kanban board hiển thị | 1. Drag task card "Design Login" từ "To Do"<br>2. Drop vào cột "In Progress" | Task: "Design Login"<br>From: To Do → To: In Progress | Task di chuyển sang cột mới. Status tự động cập nhật thành "In Progress" trong DB. Không cần refresh | 🔴 Critical | | |
| TC-BOARD-03 | Filter tasks trên board | Kanban board hiển thị | 1. Click filter icon<br>2. Chọn Priority = "High" | Filter: Priority = High | Chỉ hiển thị tasks có priority High trên board | 🟡 High | | |
| TC-BOARD-04 | Board với 0 tasks | Project mới, chưa có task | 1. Vào project mới<br>2. Chọn tab Board | — | Hiển thị các cột status trống, có message "No items" hoặc nút "+" | 🟢 Medium | | |

### PHẦN 5: LIST VIEW

| TC ID | Tên Test Case | Precondition | Bước thực hiện | Dữ liệu test | Kết quả mong đợi | Mức ưu tiên | Kết quả | Ghi chú |
|-------|--------------|-------------|----------------|--------------|-------------------|-------------|---------|---------|
| TC-LIST-01 | Hiển thị List View | Project có tasks | 1. Chọn tab "List" | — | Hiển thị table: columns ID, Title, Status, Priority, Assignee, Due Date | 🔴 Critical | | |
| TC-LIST-02 | Sort tasks theo cột | List view hiển thị | 1. Click header "Title"<br>2. Click lại | — | Lần 1: Sort A→Z. Lần 2: Sort Z→A. Sort icon thay đổi | 🟡 High | | |
| TC-LIST-03 | Inline edit Priority | List view hiển thị | 1. Click Priority cell<br>2. Chọn "Medium" | — | Priority cập nhật ngay trong bảng | 🟡 High | | |
| TC-LIST-04 | Click row xem detail | List view hiển thị | 1. Click vào row task | — | Slide panel detail mở từ phải | 🔴 Critical | | |

### PHẦN 6: CYCLES & SPRINTS

| TC ID | Tên Test Case | Precondition | Bước thực hiện | Dữ liệu test | Kết quả mong đợi | Mức ưu tiên | Kết quả | Ghi chú |
|-------|--------------|-------------|----------------|--------------|-------------------|-------------|---------|---------|
| TC-CYCLE-01 | Tạo Cycle mới | Trong project | 1. Click tab "Cycles"<br>2. Click "New Cycle"<br>3. Nhập name, start/end date<br>4. Click "Create" | Name: "Sprint 1"<br>Start: 2026-04-19<br>End: 2026-05-03 | Cycle được tạo, hiển thị trong danh sách | 🔴 Critical | | |
| TC-CYCLE-02 | Gán task vào cycle | Task detail + Cycle exist | 1. Mở task detail<br>2. Properties → Cycle<br>3. Chọn "Sprint 1" | — | Task gán vào cycle thành công | 🔴 Critical | | |
| TC-CYCLE-03 | Xem tasks trong cycle | Cycle có tasks | 1. Click vào cycle name | — | Hiển thị danh sách tasks thuộc cycle | 🟡 High | | |
| TC-CYCLE-04 | Xóa cycle | Cycle exist | 1. Click menu → Delete<br>2. Confirm | — | Cycle bị xóa, tasks vẫn còn trong project | 🟡 High | | |

### PHẦN 7: MODULES

| TC ID | Tên Test Case | Precondition | Bước thực hiện | Dữ liệu test | Kết quả mong đợi | Mức ưu tiên | Kết quả | Ghi chú |
|-------|--------------|-------------|----------------|--------------|-------------------|-------------|---------|---------|
| TC-MOD-01 | Tạo module mới | Trong project | 1. Click tab "Modules"<br>2. Click "New Module"<br>3. Nhập tên<br>4. Click "Create" | Name: "Backend API" | Module được tạo thành công | 🟡 High | | |
| TC-MOD-02 | Gán task vào module | Task detail + Module exist | 1. Mở task detail<br>2. Properties → Module<br>3. Chọn module | Module: "Backend API" | Task gán vào module | 🟡 High | | |
| TC-MOD-03 | Xem tasks trong module | Module có tasks | 1. Click vào module name | — | Hiển thị danh sách tasks thuộc module | 🟡 High | | |

### PHẦN 8: LABELS

| TC ID | Tên Test Case | Precondition | Bước thực hiện | Dữ liệu test | Kết quả mong đợi | Mức ưu tiên | Kết quả | Ghi chú |
|-------|--------------|-------------|----------------|--------------|-------------------|-------------|---------|---------|
| TC-LBL-01 | Tạo label mới | Trong project | 1. Vào Labels setting<br>2. Click "New Label"<br>3. Nhập tên, chọn màu<br>4. Click "Create" | Name: "Bug"<br>Color: #EF4444 (Red) | Label tạo thành công với đúng màu | 🟡 High | | |
| TC-LBL-02 | Gán label cho task | Label exist + Task detail mở | 1. Click Labels dropdown<br>2. Chọn "Bug" | — | Label gán OK, hiển thị badge đúng màu #EF4444 | 🟡 High | | |
| TC-LBL-03 | Xóa label | Label exist | 1. Labels setting → Delete icon<br>2. Confirm | — | Label bị xóa, không hiển thị trên mọi task | 🟢 Medium | | |

### PHẦN 9: COMMENTS & ACTIVITY

| TC ID | Tên Test Case | Precondition | Bước thực hiện | Dữ liệu test | Kết quả mong đợi | Mức ưu tiên | Kết quả | Ghi chú |
|-------|--------------|-------------|----------------|--------------|-------------------|-------------|---------|---------|
| TC-CMT-01 | Thêm comment | Task detail mở | 1. Scroll đến Activity<br>2. Nhập comment<br>3. Click "Submit" | Comment: "This needs review" | Comment hiển thị với tên user, avatar, thời gian | 🔴 Critical | | |
| TC-CMT-02 | Edit comment | Comment of current user exist | 1. Hover comment<br>2. Click (...) → "Edit"<br>3. Sửa text<br>4. Click "Save" | New text: "Updated: needs urgent review" | Comment cập nhật, hiển thị "(edited)" | 🟡 High | | |
| TC-CMT-03 | Delete comment | Comment of current user exist | 1. Hover comment<br>2. Click (...) → "Delete"<br>3. Confirm | — | Comment bị xóa, không hiển thị | 🟡 High | | |
| TC-CMT-04 | Add emoji reaction | Comment exist | 1. Hover comment<br>2. Click emoji icon<br>3. Chọn emoji 👍 | Emoji: 👍 | Reaction counter +1, hiển thị emoji dưới comment | 🟢 Medium | | |
| TC-CMT-05 | Xem Activity log | Task detail mở | 1. Click tab "Activity" | — | Hiển thị lịch sử thay đổi: ai thay đổi gì, lúc nào | 🟡 High | | |

### PHẦN 10: NAVIGATION & UI

| TC ID | Tên Test Case | Precondition | Bước thực hiện | Dữ liệu test | Kết quả mong đợi | Mức ưu tiên | Kết quả | Ghi chú |
|-------|--------------|-------------|----------------|--------------|-------------------|-------------|---------|---------|
| TC-NAV-01 | Sidebar navigation | Đã đăng nhập | 1. Click các item trong sidebar | — | Active state đúng, chuyển trang mượt mà | 🔴 Critical | | |
| TC-NAV-02 | Breadcrumb navigation | Trong project/task | 1. Xem breadcrumb<br>2. Click parent links | — | Chuyển đúng trang theo hierarchy | 🟡 High | | |
| TC-NAV-03 | Dark mode | Đã đăng nhập | 1. Kiểm tra giao diện | — | Dark theme hiển thị mặc định, màu sắc đúng design | 🔴 Critical | | |
| TC-NAV-04 | Sidebar collapse | Desktop mode | 1. Click collapse button | — | Sidebar thu nhỏ/mở rộng mượt mà | 🟢 Medium | | |

### PHẦN 11: ANALYTICS

| TC ID | Tên Test Case | Precondition | Bước thực hiện | Dữ liệu test | Kết quả mong đợi | Mức ưu tiên | Kết quả | Ghi chú |
|-------|--------------|-------------|----------------|--------------|-------------------|-------------|---------|---------|
| TC-ANA-01 | Project Analytics | Project có tasks | 1. Click tab "Analytics" | — | Hiển thị: tổng tasks, tasks by status (chart), tasks by priority, tasks by assignee | 🟡 High | | |
| TC-ANA-02 | Global Analytics | Workspace có projects | 1. Click "Analytics" trong sidebar | — | Hiển thị thống kê toàn workspace: projects, tasks, members | 🟢 Medium | | |

### PHẦN 12: VIEWS & FILTERS

| TC ID | Tên Test Case | Precondition | Bước thực hiện | Dữ liệu test | Kết quả mong đợi | Mức ưu tiên | Kết quả | Ghi chú |
|-------|--------------|-------------|----------------|--------------|-------------------|-------------|---------|---------|
| TC-VIEW-01 | Tạo custom view | Trong project | 1. Click "+" ngoài view list<br>2. Nhập tên<br>3. Chọn type<br>4. Set filters | Name: "High Priority Tasks"<br>Filter: Priority=High | View tạo OK, hiển thị filtered tasks | 🟡 High | | |
| TC-VIEW-02 | Filter tasks | Trong view | 1. Click filter icon<br>2. Set filter Status="In Progress" | — | Chỉ hiển thị tasks In Progress | 🟡 High | | |
| TC-VIEW-03 | Calendar view | Project có tasks với dates | 1. Chọn tab "Calendar" | — | Tasks hiển thị trên lịch theo ngày | 🟢 Medium | | |
| TC-VIEW-04 | Timeline view | Project có tasks với dates | 1. Chọn tab "Timeline" | — | Gantt chart hiển thị tasks theo timeline | 🟢 Medium | | |
| TC-VIEW-05 | Spreadsheet view | Project có tasks | 1. Chọn tab "Spreadsheet" | — | Bảng tính với tất cả fields editable | 🟢 Medium | | |

### PHẦN 13: ADMIN

| TC ID | Tên Test Case | Precondition | Bước thực hiện | Dữ liệu test | Kết quả mong đợi | Mức ưu tiên | Kết quả | Ghi chú |
|-------|--------------|-------------|----------------|--------------|-------------------|-------------|---------|---------|
| TC-ADM-01 | Quản lý users | Admin account | 1. Vào Admin panel<br>2. Click "Users" | — | Danh sách users hiển thị đầy đủ: name, email, role, status | 🟡 High | | |
| TC-ADM-02 | Khóa tài khoản | Admin + User list | 1. Tìm user<br>2. Click "Lock" / Toggle | — | User bị khóa, không thể đăng nhập | 🟡 High | | |
| TC-ADM-03 | Xem Audit Log | Admin | 1. Click "Audit Log" | — | Hiển thị lịch sử thao tác: ai làm gì, lúc nào | 🟡 High | | |
| TC-ADM-04 | Quản lý phòng ban | Admin | 1. Click "Departments"<br>2. CRUD operations | Name: "Engineering" | Phòng ban CRUD hoạt động | 🟢 Medium | | |

### PHẦN 14: PAGES & STICKY NOTES

| TC ID | Tên Test Case | Precondition | Bước thực hiện | Dữ liệu test | Kết quả mong đợi | Mức ưu tiên | Kết quả | Ghi chú |
|-------|--------------|-------------|----------------|--------------|-------------------|-------------|---------|---------|
| TC-PAGE-01 | Tạo page mới | Trong project | 1. Click "Pages" tab<br>2. Click "New Page"<br>3. Nhập nội dung | Title: "Meeting Notes" | Page tạo OK với WYSIWYG editor | 🟢 Medium | | |
| TC-STICKY-01 | Tạo sticky note | Đã đăng nhập | 1. Click "Stickies"<br>2. Click "+"<br>3. Nhập nội dung | Content: "Remember to review PRs" | Sticky note tạo OK, hiển thị | 🟢 Medium | | |

### PHẦN 15: PERFORMANCE

| TC ID | Tên Test Case | Precondition | Bước thực hiện | Dữ liệu test | Kết quả mong đợi | Mức ưu tiên | Kết quả | Ghi chú |
|-------|--------------|-------------|----------------|--------------|-------------------|-------------|---------|---------|
| TC-PERF-01 | Load project 100+ tasks | Project có 100+ tasks | 1. Click vào project<br>2. Xem Kanban/List | — | Page load < 3 giây. Scrolling 60 FPS | 🟡 High | | |
| TC-PERF-02 | Open task detail | Project có tasks | 1. Click vào task | — | Panel mở < 1 giây | 🟡 High | | |
| TC-PERF-03 | Search tasks | Project có tasks | 1. Nhập search term | Search: "login" | Results hiển thị < 500ms | 🟢 Medium | | |

### PHẦN 16: RESPONSIVE

| TC ID | Tên Test Case | Precondition | Bước thực hiện | Dữ liệu test | Kết quả mong đợi | Mức ưu tiên | Kết quả | Ghi chú |
|-------|--------------|-------------|----------------|--------------|-------------------|-------------|---------|---------|
| TC-RESP-01 | Desktop 1920px | Bất kì trang nào | 1. Test trên 1920x1080 | — | Layout đầy đủ, không bị cắt | 🟡 High | | |
| TC-RESP-02 | Tablet 768px | Bất kì trang nào | 1. Resize hoặc DevTools → 768px | — | Sidebar ẩn/collapse, content responsive | 🟢 Medium | | |
| TC-RESP-03 | Mobile 375px | Bất kì trang nào | 1. Resize → 375px | — | Mobile menu, touch-friendly buttons | 🟢 Medium | | |

### PHẦN 17: ERROR HANDLING

| TC ID | Tên Test Case | Precondition | Bước thực hiện | Dữ liệu test | Kết quả mong đợi | Mức ưu tiên | Kết quả | Ghi chú |
|-------|--------------|-------------|----------------|--------------|-------------------|-------------|---------|---------|
| TC-ERR-01 | Network timeout | API running | 1. Tắt backend server<br>2. Thử tạo task | — | Hiển thị error message, có nút retry | 🟡 High | | |
| TC-ERR-02 | Invalid input validation | Trang create task | 1. Tạo task title rỗng<br>2. Click Save | Title: "" | Validation: "Title is required" | 🟡 High | | |
| TC-ERR-03 | Unauthorized access | Token hết hạn | 1. Xóa token manually<br>2. Truy cập protected route | — | Redirect về trang Login | 🔴 Critical | | |
| TC-ERR-04 | 404 Not Found | URL sai | 1. Truy cập /abc123 | — | Hiển thị trang 404 hoặc redirect Home | 🟢 Medium | | |

---

## III. TÓM TẮT TEST CASE

| Phần | Số TC | Critical | High | Medium | Low |
|------|-------|----------|------|--------|-----|
| Authentication | 9 | 3 | 4 | 0 | 2 |
| Workspace & Project | 8 | 2 | 6 | 0 | 0 |
| Task Management | 18 | 5 | 10 | 1 | 2 |
| Board View | 4 | 2 | 1 | 1 | 0 |
| List View | 4 | 2 | 2 | 0 | 0 |
| Cycles | 4 | 2 | 2 | 0 | 0 |
| Modules | 3 | 0 | 3 | 0 | 0 |
| Labels | 3 | 0 | 2 | 1 | 0 |
| Comments | 5 | 1 | 2 | 1 | 1 |
| Navigation | 4 | 2 | 1 | 1 | 0 |
| Analytics | 2 | 0 | 1 | 1 | 0 |
| Views & Filters | 5 | 0 | 2 | 3 | 0 |
| Admin | 4 | 0 | 2 | 1 | 1 |
| Pages & Stickies | 2 | 0 | 0 | 2 | 0 |
| Performance | 3 | 0 | 2 | 1 | 0 |
| Responsive | 3 | 0 | 1 | 2 | 0 |
| Error Handling | 4 | 1 | 2 | 1 | 0 |
| **TỔNG** | **85** | **20** | **43** | **16** | **6** |

---

## IV. BUG ĐÃ PHÁT HIỆN VÀ SỬA

### BUG-001: Priority Selection Requires Page Refresh ✅ FIXED

| Thuộc tính | Giá trị |
|-----------|---------|
| **Test Case:** | TC-TASK-05 |
| **Severity:** | 🔴 Critical |
| **Status:** | ✅ FIXED & VERIFIED |
| **Component:** | Frontend/src/components/TaskDetailModal.vue |
| **Root Cause:** | Priority dropdown gọi trực tiếp `updateTaskField()` mà không update local state trước |
| **Fix:** | Thêm hàm `selectPriority()` cập nhật `selectedTask.priority` trước khi emit event |
| **Verification:** | Priority icon thay đổi ngay, text thay đổi ngay, persist khi reopen |

```javascript
// FIX APPLIED (line 1319 - TaskDetailModal.vue)
const selectPriority = (priority) => {
  if (!props.selectedTask) return;
  props.selectedTask.priority = priority;  // ✅ Instant UI update
  if (!props.selectedTask.isNew) {
    updateTaskField(props.selectedTask, 'priority', priority);
  }
};
```

---

## V. CẬP NHẬT TIẾN ĐỘ LÊN TRELLO

> **Ghi chú:** Các test case trên cần được tạo thành các card trên Trello.com theo cấu trúc:

### Board Trello đề xuất

```
📋 Trello Board: "QuanLyCongViec - QA Testing"

Lists:
├── 📝 Backlog (TC chưa test)
├── 🔄 In Progress (Đang test)  
├── ✅ Passed (Test OK)
├── ❌ Failed (Test FAIL)
├── 🐛 Bug Found
└── 🔧 Bug Fixed & Verified
```

### Screenshot template cho Trello

> Cần capture screenshots của:
> 1. Board tổng quan với tất cả lists
> 2. Chi tiết từng card test case
> 3. Burndown chart (từ burndownfortrello.com)
> 4. Labels phân loại (Critical, High, Medium, Low)

---

**Người lập:** ___________  
**Ngày tạo:** 19/04/2026  
**Phiên bản:** 1.0
