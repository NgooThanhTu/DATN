# Báo Cáo Phân Tích Admin Và Project Settings

## 1. Tóm tắt điều hành

Mục tiêu của tài liệu này là phân tích đầy đủ khu vực `Admin`, luồng `Project Settings`, nút bánh răng trong từng project, mô hình phân quyền, logic nghiệp vụ liên quan, cấu trúc dashboard/menu, và đề xuất lộ trình nhiều phase để một AI hoặc đội phát triển có thể sửa hệ thống theo hướng rõ ràng, an toàn và gần với trải nghiệm của Plane hơn.

Kết luận quan trọng nhất:

- Trong code hiện tại của `D:\A\QuanLyCongViec`, quyền vào `Admin` và quyền vào `Project Settings` nên là hai loại quyền khác nhau.
- Hiện tại codebase đang trộn lẫn hai khái niệm này.
- Nút bánh răng ở từng project chưa mở một trang `Project Settings` thực sự, mà đang redirect sang khu vực `Admin > Configuration`.
- Có phân quyền, nhưng phân quyền đang không thống nhất giữa frontend, router guard và backend API.

## 2. Phạm vi và giới hạn phân tích

Phân tích này dựa trên code hiện có trong repository `D:\A\QuanLyCongViec`.

Giới hạn:

- Không có source code gốc của Plane trong repository này.
- Vì vậy, mọi so sánh với Plane chỉ có thể là so sánh về mặt định hướng sản phẩm hoặc UX mong muốn, không thể khẳng định chính xác “Plane đang làm gì trong code” từ repository hiện tại.

## 3. Mục tiêu của báo cáo

Tài liệu này trả lời các câu hỏi sau:

- `Admin` hiện tại là gì.
- Vào `Admin` bằng cách nào.
- `Project Settings` hiện tại là gì.
- Vào `Project Settings` bằng cách nào.
- Nút bánh răng của project đang mở đúng hay mở sai.
- Hệ thống đang phân quyền như thế nào.
- Dashboard/menu hiện có những gì.
- Khi tạo project, hệ thống đang seed và cấu hình gì.
- Nên chia thành các phase nào để một AI sửa dần theo thứ tự hợp lý.

## 4. Hiện trạng tổng quan

### 4.1. Kết luận hiện trạng

Hệ thống hiện có:

- một khu vực `Admin` khá rõ ở mức hệ thống/tổ chức
- một nút bánh răng trên từng project
- một route `/space/:id/settings`

Nhưng route `/space/:id/settings` hiện không trỏ vào một màn hình `Project Settings` riêng. Thay vào đó, nó redirect sang `/admin/configuration` kèm `projectId` ở query string. Trang `Configuration` lại không dùng `projectId`, nên thực tế đây không phải là `Project Settings` thật.

### 4.2. Ý nghĩa nghiệp vụ đúng nên có

Về mặt thiết kế sản phẩm:

- `Admin` là khu vực quản trị toàn hệ thống hoặc toàn tổ chức
- `Project Settings` là khu vực quản trị riêng cho từng project

Hai khu vực này cần tách biệt vì:

- phạm vi tác động khác nhau
- đối tượng được cấp quyền khác nhau
- rủi ro khi sửa cấu hình khác nhau

## 5. Cách vào Admin hiện tại

### 5.1. Từ menu settings toàn cục

Frontend có dropdown settings tại:

- [Frontend/src/components/SettingsDropdown.vue](Frontend/src/components/SettingsDropdown.vue)

Trong component này:

- `canAccessAdmin` được tính dựa trên `currentUser.systemRoles`
- các role được phép hiện tại là `System Admin`, `PM`, `PO`

Nút này cho phép đi tới:

- `/admin/audit-log`
- `/admin/users`
- `/admin/configuration`
- `/admin/instance/general`
- `/admin/instance/authentication`
- `/admin/instance/email`
- `/admin/security/2fa`
- `/admin/security/change-password`
- `/admin/security/ip-whitelist`
- một số mục organization/customization

### 5.2. Truy cập trực tiếp bằng route

Các route admin được khai báo tại:

- [Frontend/src/router/adminRoutes.js](Frontend/src/router/adminRoutes.js)

Danh sách role được phép trong route admin hiện đang rộng hơn UI dropdown, bao gồm:

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

Điều này cho thấy UI và router đang không đồng nhất.

### 5.3. Router guard

Router guard nằm tại:

- [Frontend/src/router/index.js](Frontend/src/router/index.js)

Guard đọc:

- token
- `user.systemRoles` từ local storage
- `meta.requiredRoles` của route

Nếu thiếu quyền, guard sẽ chặn truy cập.

### 5.4. Backend authorization cho Admin

Ở backend có filter:

- [Backend/src/TaskManagement.API/Filters/SystemAuthorizeAttribute.cs](Backend/src/TaskManagement.API/Filters/SystemAuthorizeAttribute.cs)

Filter này:

- kiểm tra user trong DB
- lấy system roles thật từ dữ liệu
- xác thực quyền ở backend, không chỉ dựa trên frontend

Ví dụ controller có dùng filter:

- [Backend/src/TaskManagement.API/Controllers/AdminUsersController.cs](Backend/src/TaskManagement.API/Controllers/AdminUsersController.cs)

Như vậy, `Admin` hiện là một khái niệm ở mức hệ thống.

## 6. Cách vào Project Settings hiện tại

### 6.1. Từ nút bánh răng trên project card

Trong:

- [Frontend/src/views/ManageSpaces.vue](Frontend/src/views/ManageSpaces.vue)

mỗi project card có nút bánh răng.

Luồng hiện tại:

- nút gear gọi `goToAdmin(space.id)`
- hàm này điều hướng tới `/space/{projectId}/settings`

### 6.2. UI permission của nút bánh răng project

Trong cùng file `ManageSpaces.vue`, điều kiện để hiện hoặc cho phép bấm nút gear đang dựa trên:

- `currentUser.systemRoles`

với các role được phép:

- `System Admin`
- `Admin`
- `PM`
- `PO`
- `admin`

Đây là một tín hiệu thiết kế chưa đúng, vì quyền vào `Project Settings` đáng ra nên dựa chủ yếu trên `project role`, không phải `system role`.

### 6.3. Route `/space/:id/settings`

Route này được khai báo tại:

- [Frontend/src/router/spaceRoutes.js](Frontend/src/router/spaceRoutes.js)

Hiện trạng:

- `/space/:id/settings`
- không render page settings riêng
- redirect sang `/admin/configuration`
- gắn `projectId` vào query string

### 6.4. Trang cấu hình nhận redirect

Trang được mở là:

- [Frontend/src/views/admin/Configuration.vue](Frontend/src/views/admin/Configuration.vue)

Qua phân tích code, trang này:

- là trang cấu hình hệ thống
- không dùng `route.query.projectId`
- không có context project-specific

Kết luận:

- `Project Settings` hiện chưa tồn tại thật
- nút gear của project đang mở sai mục tiêu
- người dùng tưởng đang vào cấu hình project, nhưng thực chất đang vào cấu hình hệ thống

## 7. Phân quyền hiện tại

### 7.1. Có phân quyền không

Có.

Hệ thống đang phân quyền ở ba tầng:

- tầng UI
- tầng router
- tầng backend API

### 7.2. Phân quyền ở UI

Ví dụ:

- `SettingsDropdown.vue` dùng `systemRoles` để hiện menu admin
- `ManageSpaces.vue` dùng `systemRoles` để hiện nút gear project

Vấn đề:

- hai chỗ này không dùng cùng một danh sách role
- chưa phản ánh đúng phân quyền theo project

### 7.3. Phân quyền ở router

`router/index.js` kiểm tra `meta.requiredRoles` của route.

Vấn đề:

- route admin cho phép tập role rộng hơn UI dropdown
- tạo ra sự lệch giữa “thấy menu” và “vào được route”

### 7.4. Phân quyền ở backend mức hệ thống

`SystemAuthorizeAttribute` là lớp xác thực backend cho hệ thống admin.

Vai trò:

- bảo vệ các API hệ thống
- không tin hoàn toàn vào frontend

### 7.5. Phân quyền ở backend mức project

Backend có filter riêng cho project:

- [Backend/src/TaskManagement.API/Filters/ProjectAuthorizeAttribute.cs](Backend/src/TaskManagement.API/Filters/ProjectAuthorizeAttribute.cs)

Filter này:

- đọc `projectId`
- kiểm tra user có phải member project hay không
- kiểm tra `ProjectRole`
- chặn write operation cho `Guest` và `Stakeholder`

Ví dụ controller dùng filter:

- [Backend/src/TaskManagement.API/Controllers/ProjectMembersController.cs](Backend/src/TaskManagement.API/Controllers/ProjectMembersController.cs)

Các action như thêm member, xóa member, đổi role dùng:

- `[ProjectAuthorize("PM, Admin")]`

Điều này chứng minh codebase đã có khái niệm `project-level authorization`.

### 7.6. Kết luận về phân quyền

Về bản chất, hệ thống đã có:

- `system authorization`
- `project authorization`

Nhưng frontend đang dùng sai tầng quyền cho nút gear project.

## 8. Admin và Project Settings có phải hai loại quyền khác nhau không

Câu trả lời nên là: `Có`.

### 8.1. Quyền vào Admin

Là quyền ở mức:

- hệ thống
- tổ chức
- instance

Ví dụ chức năng:

- quản lý user toàn hệ thống
- cấu hình authentication
- cấu hình email/SMTP
- cấu hình IP whitelist
- audit log
- configuration mặc định toàn hệ thống

### 8.2. Quyền vào Project Settings

Là quyền ở mức:

- một project cụ thể

Ví dụ chức năng:

- quản lý members của project
- quản lý role trong project
- cấu hình labels
- cấu hình modules
- cấu hình states
- cấu hình cycles/sprints
- danger zone cho project đó

### 8.3. Ví dụ nghiệp vụ đúng

- Một `Project Manager` có thể được phép sửa project A nhưng không được vào admin hệ thống.
- Một `System Admin` có thể vào admin hệ thống và có thể có quyền override project settings.
- Một thành viên thường không được vào cả hai khu vực này.

### 8.4. Kết luận thiết kế

`Admin access` và `Project Settings access` phải được tách riêng trong:

- UI
- route
- backend authorization
- tài liệu nghiệp vụ

## 9. Dashboard và menu hiện tại

### 9.1. Sidebar chính

Sidebar chính nằm tại:

- [Frontend/src/components/layout/NexusSidebar.vue](Frontend/src/components/layout/NexusSidebar.vue)

Các mục chính:

- Home
- Drafts
- Your work
- Stickies
- Rewards

Khu vực workspace:

- Projects
- More

Trong `More`:

- Views
- Analytics
- Archives

Theo từng project:

- Work items
- Cycles
- Modules
- Views
- Pages

### 9.2. Dashboard

Trang dashboard:

- [Frontend/src/views/Dashboard.vue](Frontend/src/views/Dashboard.vue)

Chức năng nổi bật:

- hiển thị project
- search project
- favorites
- mở project
- tạo work item nhanh

## 10. Khu vực Admin hiện có gì

### 10.1. Layout admin

Admin layout và sidebar nằm tại:

- [Frontend/src/components/layout/AdminLayout.vue](Frontend/src/components/layout/AdminLayout.vue)
- [Frontend/src/components/layout/AdminSidebar.vue](Frontend/src/components/layout/AdminSidebar.vue)

### 10.2. Menu admin hiện tại

Admin sidebar hiện có:

- Audit Log
- User Management
- Configuration
- Instance
- Security

Chi tiết:

- `Audit Log`: log hệ thống
- `User Management`: quản lý user, department, mapping role
- `Configuration`: cấu hình trạng thái mặc định và theme
- `Instance > General settings`
- `Instance > Authentication`
- `Instance > Email`
- `Security > Two-Factor Auth`
- `Security > Change Password`
- `Security > IP Whitelist`

### 10.3. Điểm lệch menu

Trong `SettingsDropdown.vue` vẫn có các mục organization/profile/contact, nhưng `AdminSidebar.vue` lại đang comment out nhóm Organization. Điều này cho thấy menu admin chưa đồng bộ giữa các điểm vào.

## 11. Logic nghiệp vụ khi tạo project

### 11.1. UI tạo project

Modal tạo project nằm tại:

- [Frontend/src/components/CreateSpaceModal.vue](Frontend/src/components/CreateSpaceModal.vue)

Input chính:

- tên project
- key/identifier
- description
- visibility
- lead
- cover
- icon

### 11.2. Backend create project

Service xử lý:

- [Backend/src/TaskManagement.Infrastructure/Services/ProjectService.cs](Backend/src/TaskManagement.Infrastructure/Services/ProjectService.cs)

Logic đáng chú ý:

- tạo entity `Project`
- lưu `cover/icon` vào `NavigationConfig`
- seed default task statuses:
  - `TO DO`
  - `IN PROGRESS`
  - `DONE`
- seed task types theo template
- gán creator thành `PROJECT_MANAGER`
- nếu chọn lead khác creator thì thêm `PROJECT_LEAD`
- nếu lead chính là creator thì cập nhật vai trò creator thành `PROJECT_LEAD`

### 11.3. Ý nghĩa nghiệp vụ

Ngay khi tạo project, hệ thống đã có:

- cấu trúc workflow cơ bản
- role khởi tạo cơ bản
- khả năng hiển thị ngoài dashboard/discovery

Tuy nhiên:

- chưa có `Project Settings` thật để quản trị các cấu hình đó sau khi tạo

## 12. Các điểm sai hoặc chưa hoàn chỉnh trong thiết kế hiện tại

### 12.1. Nút gear project đang mở sai mục tiêu

Vấn đề lớn nhất:

- người dùng nghĩ đang mở `Project Settings`
- nhưng code thực tế mở `Admin Configuration`

### 12.2. Không có màn hình Project Settings thật

Hiện không thấy page kiểu:

- `ProjectSettings.vue`
- hoặc module settings riêng theo project

### 12.3. Quyền của project gear đang dựa vào system role

Điều này sai về thiết kế vì:

- quản trị project nên dựa trên `project role`
- không nên bắt một project manager phải có `system admin` mới được sửa project

### 12.4. Phân quyền không đồng nhất

Không đồng nhất giữa:

- `ManageSpaces.vue`
- `SettingsDropdown.vue`
- `adminRoutes.js`
- `SystemAuthorizeAttribute`
- `ProjectAuthorizeAttribute`

### 12.5. `Configuration.vue` là system-wide

Nó không nên đóng vai trò `Project Settings`.

### 12.6. Security consistency gap

Một số endpoint trong:

- [Backend/src/TaskManagement.API/Controllers/SystemSettingsController.cs](Backend/src/TaskManagement.API/Controllers/SystemSettingsController.cs)

dùng logic kiểm tra admin theo helper method thay vì toàn bộ đều dùng attribute authorization thống nhất. Đây là điểm cần rà soát để tránh lệch mức bảo vệ.

## 13. Thiết kế mục tiêu nên có

### 13.1. Tách hai khu vực rõ ràng

Nên có hai khu vực độc lập:

- `Admin`
- `Project Settings`

### 13.2. Admin

Admin chỉ dành cho:

- cấu hình hệ thống
- cấu hình tổ chức
- user management
- security
- audit
- instance-level settings

Route đề xuất:

- `/admin/*`

### 13.3. Project Settings

Project Settings chỉ dành cho:

- thành viên có quyền quản trị project
- cấu hình riêng của từng project

Route đề xuất:

- `/space/:id/settings`

Page đề xuất:

- `Frontend/src/views/ProjectSettings.vue`

### 13.4. Cấu trúc tab đề xuất cho Project Settings

Nên có các tab:

- General
- Members & Roles
- States
- Labels
- Modules
- Cycles
- Integrations
- Danger Zone

### 13.5. Quy tắc phân quyền đề xuất

- `System Admin`: vào Admin, có thể override vào Project Settings
- `Admin`: vào Admin, có thể override Project Settings tùy chính sách
- `PROJECT_MANAGER`: vào Project Settings
- `PROJECT_LEAD`: vào Project Settings
- `PM`, `PO`: vào Project Settings nếu là role hợp lệ trong business rule
- `Developer`, member thường: không vào Admin; chỉ vào Project Settings nếu được cấp rõ ràng
- `Guest`, `Stakeholder`: không có quyền quản trị Project Settings

## 14. Ma trận quyền đề xuất

| Nhóm vai trò | Vào Admin | Vào Project Settings | Ghi chú |
|---|---|---|---|
| System Admin | Có | Có | Quyền hệ thống, có thể override |
| Admin | Có | Có hoặc theo policy | Cần chốt rõ policy |
| Organization Admin | Có | Không mặc định hoặc theo policy | Phụ thuộc mô hình tổ chức |
| PROJECT_MANAGER | Không | Có | Quyền quản trị project |
| PROJECT_LEAD | Không | Có | Quyền quản trị project |
| PM | Không hoặc có giới hạn | Có nếu thuộc project | Cần thống nhất naming |
| PO | Không hoặc có giới hạn | Có nếu thuộc project | Cần thống nhất naming |
| Developer / DEV | Không | Không mặc định | Chỉ thao tác work item |
| Member thường | Không | Không mặc định | Không cấu hình project |
| Guest | Không | Không | Read-only hoặc hạn chế |
| Stakeholder | Không | Không | Read-only hoặc hạn chế |

## 15. Lộ trình sửa theo phase

### Phase 1. Audit và chốt phạm vi

Mục tiêu:

- xác nhận hiện trạng
- đóng băng scope
- thống nhất khái niệm `Admin` và `Project Settings`

Việc cần làm:

- liệt kê toàn bộ route, button, sidebar, dropdown liên quan
- liệt kê toàn bộ API admin và project settings
- lập ma trận quyền hiện tại
- xác định các điểm sai lệch

Deliverable:

- tài liệu current state
- tài liệu target state
- permission matrix v1

### Phase 2. Chuẩn hóa mô hình phân quyền

Mục tiêu:

- thống nhất quyền giữa frontend, router và backend

Việc cần làm:

- định nghĩa rõ `system roles` và `project roles`
- xác định quyền nào thuộc `Admin`
- xác định quyền nào thuộc `Project Settings`
- sửa router guard và UI visibility theo cùng một rule
- rà backend authorization để tránh chênh lệch

Deliverable:

- permission matrix chính thức
- danh sách role chuẩn
- rule access chuẩn cho từng route và API

### Phase 3. Xây Project Settings thật

Mục tiêu:

- thay vì redirect sang admin config, tạo page project settings riêng

Việc cần làm:

- tạo `ProjectSettings.vue`
- sửa route `/space/:id/settings`
- bỏ redirect sang `/admin/configuration`
- load dữ liệu project theo `:id`
- tạo các tab settings theo project

Deliverable:

- project settings UI thật
- route thật
- project-context thật

### Phase 4. Làm sạch khu vực Admin

Mục tiêu:

- trả `Admin` về đúng vai trò hệ thống

Việc cần làm:

- bỏ logic project-specific khỏi admin config nếu có
- đồng bộ `SettingsDropdown`, `AdminSidebar`, `adminRoutes`
- chuẩn hóa menu organization/instance/security

Deliverable:

- admin area nhất quán
- menu admin nhất quán
- không còn nhập nhằng với project settings

### Phase 5. QA, regression và rollout

Mục tiêu:

- đảm bảo sau refactor hệ thống không gãy quyền và không gãy điều hướng

Việc cần làm:

- test user thường
- test project manager
- test system admin
- test deep link
- test hidden menu
- test API unauthorized/forbidden
- test redirect cũ

Deliverable:

- checklist kiểm thử
- bug list sau refactor
- bản sẵn sàng release

## 16. Đề xuất cụ thể cho AI sửa

Một AI nên nhận task theo thứ tự sau:

1. Đọc toàn bộ route admin, route project, dropdown settings, project card gear button.
2. Xây ma trận quyền hiện tại và ma trận quyền mục tiêu.
3. Tạo `ProjectSettings.vue` và sửa route `/space/:id/settings` để render thật.
4. Chuyển toàn bộ logic nút gear project sang dựa trên `project role`.
5. Giữ `Admin` chỉ cho system-level functions.
6. Đồng bộ lại `SettingsDropdown.vue`, `AdminSidebar.vue`, `adminRoutes.js`, `router/index.js`.
7. Rà soát API backend để bảo đảm mọi endpoint admin dùng bảo vệ nhất quán.

## 17. Tiêu chí nghiệm thu

Hệ thống được xem là đạt yêu cầu khi:

- bấm gear project mở đúng `Project Settings`
- `Project Settings` không còn redirect sang `Admin`
- user có quyền project nhưng không có quyền system vẫn vào được project settings hợp lệ
- user có quyền admin nhưng không thuộc project không bị buộc phải là member project nếu policy cho phép override
- menu admin và route admin nhất quán
- backend từ chối đúng các request không có quyền
- dashboard và sidebar không hiển thị sai nút theo role

## 18. Kết luận cuối cùng

Hiện tại `D:\A\QuanLyCongViec` đã có nền tảng tốt cho cả `system authorization` lẫn `project authorization`, nhưng phần UX và routing đang trộn hai tầng này lại với nhau. Điểm sai trung tâm là nút bánh răng của project chưa mở `Project Settings` thật mà đang mở cấu hình hệ thống.

Nếu sửa đúng theo lộ trình 5 phase ở trên, hệ thống sẽ:

- rõ quyền hơn
- dễ bảo trì hơn
- đúng nghiệp vụ hơn
- gần với trải nghiệm quản trị project chuyên nghiệp hơn

## 19. Phụ lục: file tham chiếu chính

Frontend:

- [Frontend/src/views/ManageSpaces.vue](Frontend/src/views/ManageSpaces.vue)
- [Frontend/src/router/spaceRoutes.js](Frontend/src/router/spaceRoutes.js)
- [Frontend/src/components/SettingsDropdown.vue](Frontend/src/components/SettingsDropdown.vue)
- [Frontend/src/router/adminRoutes.js](Frontend/src/router/adminRoutes.js)
- [Frontend/src/router/index.js](Frontend/src/router/index.js)
- [Frontend/src/views/admin/Configuration.vue](Frontend/src/views/admin/Configuration.vue)
- [Frontend/src/components/layout/AdminSidebar.vue](Frontend/src/components/layout/AdminSidebar.vue)
- [Frontend/src/components/layout/AdminLayout.vue](Frontend/src/components/layout/AdminLayout.vue)
- [Frontend/src/components/layout/NexusSidebar.vue](Frontend/src/components/layout/NexusSidebar.vue)
- [Frontend/src/views/Dashboard.vue](Frontend/src/views/Dashboard.vue)
- [Frontend/src/components/CreateSpaceModal.vue](Frontend/src/components/CreateSpaceModal.vue)

Backend:

- [Backend/src/TaskManagement.API/Filters/SystemAuthorizeAttribute.cs](Backend/src/TaskManagement.API/Filters/SystemAuthorizeAttribute.cs)
- [Backend/src/TaskManagement.API/Filters/ProjectAuthorizeAttribute.cs](Backend/src/TaskManagement.API/Filters/ProjectAuthorizeAttribute.cs)
- [Backend/src/TaskManagement.API/Controllers/AdminUsersController.cs](Backend/src/TaskManagement.API/Controllers/AdminUsersController.cs)
- [Backend/src/TaskManagement.API/Controllers/ProjectMembersController.cs](Backend/src/TaskManagement.API/Controllers/ProjectMembersController.cs)
- [Backend/src/TaskManagement.API/Controllers/SystemSettingsController.cs](Backend/src/TaskManagement.API/Controllers/SystemSettingsController.cs)
- [Backend/src/TaskManagement.Infrastructure/Services/ProjectService.cs](Backend/src/TaskManagement.Infrastructure/Services/ProjectService.cs)
- [Backend/src/TaskManagement.Application/DTOs/Project/ProjectDiscoveryDto.cs](Backend/src/TaskManagement.Application/DTOs/Project/ProjectDiscoveryDto.cs)
