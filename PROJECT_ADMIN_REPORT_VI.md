# Báo Cáo Hoàn Chỉnh: Admin, Project Settings, Phân Quyền, Dashboard/Menu và Kế Hoạch Triển Khai

## 1. Mục đích tài liệu

Tài liệu này tổng hợp hiện trạng của hệ thống trong codebase `D:\A\QuanLyCongViec`, tập trung vào:

- Khu vực `Admin` của hệ thống.
- Nút `bánh răng` trên mỗi project.
- Cách truy cập `Admin` và `Project Settings`.
- Logic phân quyền hiện tại.
- Bất cập giữa thiết kế hiện tại và thiết kế nên có.
- Các phase triển khai để giao cho AI hoặc đội phát triển sửa theo từng bước.

Tài liệu này không phân tích code gốc của Plane vì trong workspace hiện tại không có source code Plane. Mọi kết luận bên dưới đều dựa trên code của dự án `QuanLyCongViec`.

---

## 2. Kết luận điều hành

### 2.1. Kết luận ngắn gọn

Trong hệ thống hiện tại:

- `Admin` là khu vực quản trị cấp hệ thống/tổ chức.
- `Project Settings` đúng nghĩa chưa tồn tại như một module riêng.
- Nút `bánh răng` trên card project hiện không mở `Project Settings` thật.
- Thay vào đó, nó redirect sang trang `Admin Configuration`.
- Phân quyền hiện có nhưng chưa đồng bộ giữa frontend, route guard và backend.
- Hệ thống đang trộn lẫn `system admin access` và `project admin access`.

### 2.2. Kết luận thiết kế

Thiết kế đúng nên là:

- `Admin` và `Project Settings` là hai khu vực riêng biệt.
- `Admin` dùng `system roles`.
- `Project Settings` dùng `project roles`.
- Nút gear trên project phải mở `Project Settings` riêng của project đó.
- Các route admin toàn cục không nên bị dùng thay cho project settings.

---

## 3. Hiện trạng hệ thống

## 3.1. Nút bánh răng trên mỗi project hiện đang làm gì

File chính:

- [ManageSpaces.vue](/d:/A/QuanLyCongViec/Frontend/src/views/ManageSpaces.vue:80)
- [spaceRoutes.js](/d:/A/QuanLyCongViec/Frontend/src/router/spaceRoutes.js:43)

### Luồng hiện tại

Trong [ManageSpaces.vue](/d:/A/QuanLyCongViec/Frontend/src/views/ManageSpaces.vue:80), mỗi project card có nút gear:

- Khi click gọi `goToAdmin(space.id)`.
- Hàm này điều hướng tới `/space/{projectId}/settings`.

Tuy nhiên, trong [spaceRoutes.js](/d:/A/QuanLyCongViec/Frontend/src/router/spaceRoutes.js:43):

- Route `/space/:id/settings` không render một page settings riêng.
- Nó redirect sang `/admin/configuration`.
- Đồng thời gửi kèm `query.projectId = to.params.id`.

### Vấn đề hiện tại

Trang [Configuration.vue](/d:/A/QuanLyCongViec/Frontend/src/views/admin/Configuration.vue:1):

- Không đọc `route.query.projectId`.
- Không thay đổi hành vi theo project.
- Bản chất là trang cấu hình hệ thống.

Kết luận:

- Bấm gear của project hiện tại không vào `Project Settings`.
- Nó thực chất mở `System Admin Configuration`.

---

## 3.2. Admin hiện đang là gì

Khu vực admin hiện tại gồm:

- [SettingsDropdown.vue](/d:/A/QuanLyCongViec/Frontend/src/components/SettingsDropdown.vue:1)
- [AdminSidebar.vue](/d:/A/QuanLyCongViec/Frontend/src/components/layout/AdminSidebar.vue:1)
- [adminRoutes.js](/d:/A/QuanLyCongViec/Frontend/src/router/adminRoutes.js:1)
- [AdminLayout.vue](/d:/A/QuanLyCongViec/Frontend/src/components/layout/AdminLayout.vue:1)

### Các màn hình admin hiện có

- `Audit Log`
- `User Management`
- `Configuration`
- `Instance > General settings`
- `Instance > Authentication`
- `Instance > Email / SMTP`
- `Security > Two-Factor Auth`
- `Security > Change Password`
- `Security > IP Whitelist`

Một số màn hình Organization có tồn tại:

- [OrganizationProfile.vue](/d:/A/QuanLyCongViec/Frontend/src/views/admin/OrganizationProfile.vue:1)
- [OrganizationContact.vue](/d:/A/QuanLyCongViec/Frontend/src/views/admin/OrganizationContact.vue:1)

Nhưng sidebar admin hiện tại chưa hiển thị rõ phần này vì đang bị comment trong [AdminSidebar.vue](/d:/A/QuanLyCongViec/Frontend/src/components/layout/AdminSidebar.vue:27).

---

## 3.3. Dashboard và menu điều hướng hiện có gì

### Dashboard chính

File:

- [Dashboard.vue](/d:/A/QuanLyCongViec/Frontend/src/views/Dashboard.vue:1)

Dashboard hiện có:

- Tổng quan project
- Thống kê số project
- Thống kê project yêu thích
- Ô tìm kiếm project
- Card project
- Tạo work item nhanh

### Sidebar ứng dụng

File:

- [NexusSidebar.vue](/d:/A/QuanLyCongViec/Frontend/src/components/layout/NexusSidebar.vue:1)

Các nhóm menu hiện có:

- `Home`
- `Drafts`
- `Your work`
- `Stickies`
- `Rewards`

Favorites:

- hiển thị các cycle/sprint được gắn favorite

Workspace:

- `Projects`
- `More`

Trong `More` có:

- `Views`
- `Analytics`
- `Archives`

Trong từng project có:

- `Work items`
- `Cycles`
- `Modules`
- `Views`
- `Pages`

Kết luận:

- Dashboard/menu của app đã khá đầy đủ cho luồng làm việc chính.
- Tuy nhiên menu `Project Settings` chưa tồn tại trong cây điều hướng project.

---

## 4. Phân tích phân quyền hiện tại

## 4.1. Phân quyền truy cập Admin

### Frontend button-level

File:

- [SettingsDropdown.vue](/d:/A/QuanLyCongViec/Frontend/src/components/SettingsDropdown.vue:72)

Nút gear admin toàn cục chỉ hiển thị nếu user có một trong các role:

- `System Admin`
- `PM`
- `PO`

### Frontend route-level

File:

- [adminRoutes.js](/d:/A/QuanLyCongViec/Frontend/src/router/adminRoutes.js:1)

Route admin cho phép danh sách rộng hơn:

- `SuperAdmin`
- `Admin`
- `System Admin`
- `Organization Admin`
- `AccessAdmin`
- `PM`
- `PO`
- `PROJECT_MANAGER`
- `Developer`
- `DEV`

### Frontend router guard

File:

- [router/index.js](/d:/A/QuanLyCongViec/Frontend/src/router/index.js:23)

Guard đọc:

- `localStorage.user.systemRoles`
- so với `meta.requiredRoles`

### Backend API-level

File:

- [SystemAuthorizeAttribute.cs](/d:/A/QuanLyCongViec/Backend/src/TaskManagement.API/Filters/SystemAuthorizeAttribute.cs:1)

Một số controller admin dùng filter này, ví dụ:

- [AdminUsersController.cs](/d:/A/QuanLyCongViec/Backend/src/TaskManagement.API/Controllers/AdminUsersController.cs:15)

### Kết luận

Có phân quyền admin, nhưng hiện tại:

- UI, route và backend chưa đồng nhất hoàn toàn.
- Có chỗ cho `Developer/DEV` vào admin route.
- Có chỗ lại không cho.
- Dễ gây lệch giữa “thấy nút”, “vào được route” và “gọi được API”.

---

## 4.2. Phân quyền truy cập Project Settings

### Hiện trạng

Không có `ProjectSettings.vue` thật.

Nút gear project đang check theo `systemRoles` trong:

- [ManageSpaces.vue](/d:/A/QuanLyCongViec/Frontend/src/views/ManageSpaces.vue:114)

Các role được cho bấm gear:

- `System Admin`
- `Admin`
- `PM`
- `PO`
- `admin`

### Vấn đề

Đây không phải cách đúng để bảo vệ project settings, vì:

- `Project Settings` nên dựa vào `project role`.
- Không nên dựa chủ yếu vào `system role`.

### Phân quyền project-level thực tế đang có ở backend

File:

- [ProjectAuthorizeAttribute.cs](/d:/A/QuanLyCongViec/Backend/src/TaskManagement.API/Filters/ProjectAuthorizeAttribute.cs:1)

Filter này:

- lấy `projectId` từ route
- tìm `ProjectMembers`
- check `ProjectRole`
- chặn member không hợp lệ
- chặn `Guest` và `Stakeholder` cho write methods

Ví dụ controller dùng filter này:

- [ProjectMembersController.cs](/d:/A/QuanLyCongViec/Backend/src/TaskManagement.API/Controllers/ProjectMembersController.cs:29)

Ở đây:

- thêm member chỉ cho `PM, Admin`
- xóa member chỉ cho `PM, Admin`
- đổi role member chỉ cho `PM, Admin`

### Kết luận

Project-level authorization đã có nền tảng ở backend.

Nhưng:

- project gear hiện tại chưa dùng hệ quyền này
- chưa có route project settings riêng để gắn project authorization đúng cách

---

## 5. Logic nghiệp vụ tạo project hiện tại

File frontend:

- [CreateSpaceModal.vue](/d:/A/QuanLyCongViec/Frontend/src/components/CreateSpaceModal.vue:1)

File backend:

- [ProjectService.cs](/d:/A/QuanLyCongViec/Backend/src/TaskManagement.Infrastructure/Services/ProjectService.cs:431)

## 5.1. Thông tin khi tạo project

Frontend hiện cho nhập:

- `Project name`
- `Project ID / key`
- `Description`
- `Visibility`
- `Lead`
- `Cover`
- `Icon`

## 5.2. Business logic backend khi tạo project

Backend hiện:

- tạo project record
- lưu `cover/icon` trong `NavigationConfig`
- seed default task statuses
- seed task types theo template
- add creator vào `ProjectMembers` với role `PROJECT_MANAGER`
- nếu có `leadUserId` khác creator thì add `PROJECT_LEAD`
- nếu lead là creator thì đổi creator sang `PROJECT_LEAD`

### Nhận xét

Đây là logic tạo project hợp lý ở mức khởi tạo.

Tuy nhiên hệ thống chưa có bước tiếp theo:

- sau khi project tạo xong, chưa có module `Project Settings` để quản trị vòng đời project đó.

---

## 6. Vấn đề thiết kế chính

Các vấn đề lớn nhất hiện tại là:

### 6.1. Trộn lẫn `Admin` và `Project Settings`

- `Admin` là cấu hình hệ thống/tổ chức.
- `Project Settings` phải là cấu hình của từng project.
- Hiện tại gear project lại đẩy vào admin config.

### 6.2. Không có `Project Settings` module riêng

Chưa có:

- route thực
- page thực
- tab chức năng riêng theo project

### 6.3. Phân quyền không đồng bộ

Không đồng nhất giữa:

- [ManageSpaces.vue](/d:/A/QuanLyCongViec/Frontend/src/views/ManageSpaces.vue:114)
- [SettingsDropdown.vue](/d:/A/QuanLyCongViec/Frontend/src/components/SettingsDropdown.vue:72)
- [adminRoutes.js](/d:/A/QuanLyCongViec/Frontend/src/router/adminRoutes.js:1)
- [SystemAuthorizeAttribute.cs](/d:/A/QuanLyCongViec/Backend/src/TaskManagement.API/Filters/SystemAuthorizeAttribute.cs:1)

### 6.4. Thiếu ma trận quyền chuẩn

Hiện chưa có tài liệu chuẩn trả lời:

- ai vào được admin
- ai vào được project settings
- ai được quản lý member
- ai được chỉnh status/module/label/cycle rule

### 6.5. Redirect sai nghiệp vụ

`/space/:id/settings` hiện không phải project settings.

Đây là một sai lệch nghiệp vụ rõ ràng và cần ưu tiên sửa.

---

## 7. Thiết kế mục tiêu nên có

## 7.1. Tách riêng hai khu vực

### Khu vực 1: Admin

Mức:

- hệ thống / tổ chức

Mục tiêu:

- quản lý user toàn hệ thống
- quản lý auth
- security
- SMTP
- tenant/org level config
- audit log

Route:

- `/admin/*`

Phân quyền:

- `system roles`

### Khu vực 2: Project Settings

Mức:

- từng project

Mục tiêu:

- quản trị member/role trong project
- trạng thái công việc của project
- labels
- modules
- cycle rules
- tùy chọn project
- danger zone của project

Route:

- `/space/:id/settings`

Phân quyền:

- `project roles`

---

## 7.2. Ma trận quyền đề xuất

### System-level

- `SuperAdmin`: toàn quyền admin
- `System Admin`: toàn quyền admin
- `Organization Admin`: quyền admin tổ chức
- `Admin`: tùy chính sách, có thể vào admin

### Project-level

- `PROJECT_MANAGER`: vào project settings
- `PROJECT_LEAD`: vào project settings
- `PM`: vào project settings
- `PO`: vào project settings

### Không nên có quyền vào project settings

- `DEV`
- `Guest`
- `Stakeholder`

### Quy tắc override

- `System Admin` có thể override project settings
- `SuperAdmin` có thể override mọi project

---

## 7.3. Cấu trúc Project Settings đề xuất

Nên có page:

- `ProjectSettings.vue`

Nên có các tab:

1. `General`
2. `Members & Roles`
3. `Workflow States`
4. `Labels`
5. `Modules`
6. `Cycles / Sprint Rules`
7. `Views / Defaults`
8. `Danger Zone`

---

## 8. Các phase triển khai đề xuất

Khuyến nghị chia thành `5 phase`.

## Phase 1: Audit và chốt kiến trúc

### Mục tiêu

Chốt rõ:

- `Admin` là gì
- `Project Settings` là gì
- luồng nào là system-level
- luồng nào là project-level

### Việc cần làm

- rà tất cả route, button, guard, API
- xác định các điểm đang redirect sai
- chốt matrix quyền
- chốt target UX

### Deliverable

- current state report
- target state report
- permission matrix

---

## Phase 2: Chuẩn hóa phân quyền

### Mục tiêu

Đồng bộ quyền giữa:

- button
- route guard
- backend API

### Việc cần làm

- tách `system admin guard`
- tách `project settings guard`
- chuẩn hóa role list
- bỏ việc dùng `systemRoles` cho project settings

### Deliverable

- chuẩn hóa frontend role checks
- chuẩn hóa router guard
- chuẩn hóa backend authorization

---

## Phase 3: Tạo Project Settings thật

### Mục tiêu

Biến `/space/:id/settings` thành project settings thật.

### Việc cần làm

- tạo `ProjectSettings.vue`
- sửa route `/space/:id/settings`
- bỏ redirect sang `/admin/configuration`
- load project-specific data
- gắn project authorization

### Deliverable

- page project settings hoàn chỉnh
- điều hướng đúng từ nút gear project

---

## Phase 4: Làm sạch Admin hiện tại

### Mục tiêu

Đưa khu vực admin quay về đúng vai trò system-level.

### Việc cần làm

- bỏ project-specific behavior ra khỏi admin configuration
- đồng bộ `SettingsDropdown` và `AdminSidebar`
- làm rõ organization vs instance vs security

### Deliverable

- admin shell rõ ràng, nhất quán
- menu admin đồng bộ

---

## Phase 5: QA, regression và polish

### Mục tiêu

Đảm bảo không còn sai quyền hoặc redirect sai.

### Việc cần làm

- test user thường
- test project manager/project lead
- test system admin
- test trực tiếp URL
- test case denied access
- test gear project
- test gear admin toàn cục

### Deliverable

- test checklist
- bug list sau refactor
- release-ready handoff

---

## 9. Tài liệu giao việc cho AI/dev

## 9.1. Câu hỏi cốt lõi AI phải trả lời trước khi sửa

1. User nào được vào `Admin`?
2. User nào được vào `Project Settings`?
3. `Project Settings` có phải route riêng không?
4. Gear project đang mở đúng page chưa?
5. Backend đang kiểm quyền ở system level hay project level?

## 9.2. Yêu cầu bắt buộc khi giao AI sửa

- Không dùng lại `Admin Configuration` để thay thế `Project Settings`.
- Không dựa vào `systemRoles` để bảo vệ project settings.
- Mọi API project settings phải có project-level authorization.
- Route `/space/:id/settings` phải render page thật.
- UI, route và backend phải dùng cùng một ma trận quyền.

---

## 10. Đề xuất cấu trúc file sau khi sửa

### Frontend

- `Frontend/src/views/ProjectSettings.vue`
- `Frontend/src/router/spaceRoutes.js` cập nhật route thật
- `Frontend/src/components/project-settings/*`

### Backend

- `ProjectSettingsController.cs` hoặc tách controller theo domain:
  - `ProjectMembersController`
  - `ProjectWorkflowController`
  - `ProjectLabelsController`
  - `ProjectModulesController`
  - `ProjectCyclesSettingsController`

### Shared

- một file constants/permissions chuẩn cho role mapping

---

## 11. Checklist xác nhận hoàn thành

Hệ thống được coi là sửa đúng khi:

- Gear ở project mở đúng `Project Settings`
- Gear admin toàn cục mở đúng `Admin`
- User có quyền project nhưng không có quyền admin vẫn vào được project settings
- User có quyền admin nhưng không phải member project không tự động trở thành project manager
- Route `/space/:id/settings` không redirect sang `/admin/configuration`
- API project settings được bảo vệ bằng project-level authorization
- UI và backend dùng cùng logic quyền

---

## 12. Kết luận cuối

Hệ thống hiện tại đã có nền tảng tốt cho:

- admin toàn cục
- project membership
- project-level authorization
- dashboard/menu làm việc chính

Nhưng điểm thiếu lớn nhất là:

- chưa có `Project Settings` thật
- đang dùng sai `Admin Configuration` thay thế cho project settings
- phân quyền đang chưa tách lớp rõ ràng

Nếu triển khai theo 5 phase bên trên, hệ thống sẽ:

- rõ ràng hơn về mặt kiến trúc
- đúng nghiệp vụ hơn
- dễ giao tiếp cho AI/dev sửa tiếp
- dễ mở rộng giống Plane hơn trong tương lai

