# AI Plane Parallel Repair Guide

## 1. Mục tiêu
Tài liệu này dùng `D:\A\plane` làm chuẩn tham chiếu nghiệp vụ/UI và áp dụng cho dự án `D:\A\QuanLyCongViec`.

Mục tiêu chính:
- Chạy frontend ở `http://localhost:3000` để cảm giác sử dụng gần Plane nhất.
- Không thiết kế lại giao diện lớn. Chỉ bổ sung interaction, state, dropdown, filter, redirect, API binding và các trạng thái còn thiếu.
- Bất kỳ thành phần nào nhìn giống nút, icon button, dropdown, tab, filter chip, menu item, link hoặc row clickable đều phải có hành vi rõ ràng.
- Chia việc cho 3 AI chạy song song: 2 AI Dev sửa theo vùng sở hữu khác nhau, 1 AI QA kiểm tra và có quyền sửa lỗi nhỏ không xung đột.

## 2. Cách chạy local chuẩn localhost:3000
Hiện tại `Frontend/package.json` đang có script:

```json
"dev": "vite --port 5173 --strictPort"
```

Để chạy giống Plane ở port 3000, Dev AI ưu tiên một trong hai cách sau:

```powershell
cd D:\A\QuanLyCongViec\Frontend
npm install
npx vite --host 0.0.0.0 --port 3000 --strictPort
```

Hoặc đổi script dev thành:

```json
"dev": "vite --host 0.0.0.0 --port 3000 --strictPort"
```

Backend chạy theo cấu trúc hiện có của repo:

```powershell
cd D:\A\QuanLyCongViec
dotnet run --project Backend\src\TaskManagement.API\TaskManagement.API.csproj
```

Nếu API không ở `http://localhost:5136/api`, tạo/cập nhật `Frontend/.env`:

```env
VITE_API_BASE_URL=http://localhost:5136/api
```

## 3. Chuẩn tham chiếu từ Plane
Plane có nguyên tắc UX rất rõ: thấy gì click được thì phải phản hồi được. Không có nút trang trí.

Các chuẩn cần học theo:
- Nút trạng thái mở dropdown và chọn xong cập nhật ngay trạng thái hiển thị.
- Nút assignee mở popover có search, checkbox/checkmark, chọn/bỏ chọn được nhiều người.
- Nút ngày bắt đầu/ngày hết hạn mở date picker, chọn xong cập nhật task và lọc được theo ngày.
- Nút priority, label, cycle, module, parent, project đều có menu, search nếu danh sách dài, empty state nếu không có dữ liệu.
- Filter không chỉ là chip tĩnh. Filter phải có flow: chọn field -> chọn operator -> chọn value -> áp dụng -> hiển thị chip -> bỏ từng chip -> clear all.
- Redirect phải đúng ngữ cảnh: chưa login về `/login?redirect=...`, login xong quay lại route cũ, mở space/project/cycle/detail giữ đúng ID.
- Loading, empty, error và permission state phải rõ. Không im lặng khi API lỗi.

Các file Plane nên tham chiếu khi cần logic:
- `D:\A\plane\apps\web\helpers\issue-filter.helper.ts`
- `D:\A\plane\apps\web\helpers\views.helper.ts`
- `D:\A\plane\apps\web\core\store\cycle_filter.store.ts`
- `D:\A\plane\apps\web\core\store\cycle.store.ts`
- `D:\A\plane\apps\web\core\store\dashboard.store.ts`
- `D:\A\plane\apps\web\core\store\workspace\home.ts`

## 4. File trọng tâm trong QuanLyCongViec
Frontend:
- `Frontend/src/router/index.js`: guard login, redirect, role guard.
- `Frontend/src/router/dashboardRoutes.js`: dashboard, your work, drafts, views, analytics, archives.
- `Frontend/src/router/spaceRoutes.js`: spaces, space detail, cycles, intakes, modules, views, pages.
- `Frontend/src/api/axiosClient.js`: base API, token, refresh token, redirect khi 401.
- `Frontend/src/components/FilterBar.vue`: filter chip, add filter, clear all.
- `Frontend/src/components/KanbanBoard.vue`: kanban columns, drag/drop, task click, status changed, reorder.
- `Frontend/src/components/TaskDetailModal.vue`: create/detail modal, state, priority, assignee, label, date, cycle, module, parent.
- `Frontend/src/components/ListView.vue`: list/spreadsheet interaction cần giống Plane.
- `Frontend/src/components/SpreadsheetTab.vue`: field inline edit, dropdown/filter/table behavior.
- `Frontend/src/components/CyclesTab.vue`: cycle list, filter/search/add/open cycle.
- `Frontend/src/views/SpaceSummary.vue`: màn hình project chính, tab, data orchestration.
- `Frontend/src/views/DraftsView.vue`: có nhiều dropdown giống Plane, dùng làm mẫu nội bộ.
- `Frontend/src/views/GlobalAnalyticsView.vue`: có nút filter/project dropdown có nguy cơ đang chỉ là UI.
- `Frontend/src/components/HelpDropdown.vue`: menu item cần command/link/toast rõ ràng.

Backend/API:
- `Backend/src/TaskManagement.API`: controller và endpoint thực tế.
- `Backend/src/TaskManagement.Application`: service nghiệp vụ.
- `Backend/src/TaskManagement.Domain`: entity, trạng thái, quan hệ project/task/user.
- `Backend/src/TaskManagement.Infrastructure`: EF, repository, configuration.

## 5. Gap đã phát hiện nhanh
Các gap này cần AI kiểm tra lại bằng code và browser, không được xem là danh sách cuối cùng.

Gap chạy local:
- Frontend hiện chạy `5173`, chưa phải `3000`.
- Cần thống nhất `VITE_API_BASE_URL` với backend thật.

Gap filter:
- `FilterBar.vue` hiện chủ yếu emit `remove`, `clear`, `add`, nhưng chưa thấy builder đầy đủ field/operator/value như Plane.
- Filter default có `Start date` bị lặp 2 lần.
- Cần thêm filter field tối thiểu: status, assignee, creator, priority, label, startDate, dueDate, cycle, module, createdAt, updatedAt.

Gap button/dropdown có nguy cơ chưa hoạt động:
- `CalendarTab.vue`: nút `Options` có icon dropdown nhưng chưa thấy handler/menu.
- `CyclesTab.vue`: nút search và `Filters` có nguy cơ chỉ là UI.
- `GlobalAnalyticsView.vue`: nút `All projects`, `Work item` có nguy cơ chưa có dropdown thật.
- `AIPage.vue`: nút `Nâng cấp ngay` cần route/modal/toast rõ ràng.
- `Dashboard.vue`: các link guide như `Get them in`, `Configure this workspace`, `Personalize now` cần route/modal hoặc bỏ style clickable.
- `AddPeopleModal.vue`: các `integration-btn` cần handler hoặc disabled state có tooltip.
- `HelpDropdown.vue`: từng item cần command, external link, modal, hoặc toast `Chức năng đang phát triển`.
- `TaskDetailModal.vue`: icon expand, markdown, unsubscribe, copy link, ellipsis cần hành vi thật.
- Các thẻ `<a href="#">` phải đổi thành action thật, `@click.prevent`, route thật, hoặc disabled rõ ràng.

Gap nghiệp vụ:
- Chọn status/priority/assignee/date trong modal phải lưu được qua API, không chỉ đổi local state.
- Drag task giữa cột phải gọi API cập nhật status và sortOrder, có rollback nếu lỗi.
- Filter phải áp dụng cùng một nguồn dữ liệu cho kanban/list/spreadsheet/calendar.
- Query filter nên sync lên URL để reload/back/forward không mất context.
- Permission: user không thuộc project không được update/delete task.

## 6. Quy tắc chức năng bắt buộc
Mỗi UI control phải đạt chuẩn sau:
- Có handler rõ ràng: `@click`, `@command`, `@change`, route, submit hoặc disabled có lý do.
- Có phản hồi: mở menu, đổi state, loading, toast, modal, redirect hoặc thông báo lỗi.
- Có dữ liệu thật: lấy từ store/API nếu là project/member/task/status/filter.
- Có trạng thái rỗng: danh sách trống phải hiện empty state, không hiện menu trắng.
- Có trạng thái lỗi: API lỗi phải rollback local change nếu thay đổi quan trọng.
- Có keyboard/basic accessibility: button thật dùng `<button>`, link thật dùng route/link, không dùng div clickable nếu không cần.

## 7. Logic filter giống Plane
Filter model đề xuất:

```js
{
  id: crypto.randomUUID(),
  field: 'status',
  operator: 'is',
  value: 'IN PROGRESS',
  label: 'Status',
  displayValue: 'In Progress'
}
```

Field tối thiểu:
- `status`: is, is_not, in, not_in.
- `assignee`: is, is_not, empty, not_empty.
- `creator`: is, is_not.
- `priority`: is, is_not, in.
- `label`: includes, not_includes, empty.
- `startDate`: before, after, between, empty.
- `dueDate`: before, after, between, empty, overdue.
- `cycle`: is, is_not, empty.
- `module`: is, is_not, empty.
- `createdAt`: before, after, between.
- `updatedAt`: before, after, between.

Quy tắc áp dụng:
- Nhiều filter mặc định kết hợp bằng AND.
- Trong cùng field có nhiều value thì dùng IN.
- Clear all xóa toàn bộ filter và reload danh sách.
- Remove chip chỉ xóa đúng filter đó.
- Filter thay đổi phải cập nhật kanban/list/spreadsheet/calendar đồng bộ.
- URL query nên có `?filters=...` hoặc query tường minh để refresh vẫn giữ filter.

## 8. Phân chia 3 AI chạy song song
Tất cả AI phải chạy cùng lúc, không đợi tuần tự. Mỗi AI chỉ sửa vùng mình sở hữu để tránh conflict.

### AI Dev 1 - Frontend Interaction Owner
Phạm vi chính:
- `Frontend/package.json`
- `Frontend/src/router/*`
- `Frontend/src/components/FilterBar.vue`
- `Frontend/src/components/HelpDropdown.vue`
- `Frontend/src/components/CalendarTab.vue`
- `Frontend/src/views/GlobalAnalyticsView.vue`
- Các nút/dropdown chỉ thuộc UI shell, route, filter builder.

Nhiệm vụ:
- Đảm bảo chạy `localhost:3000`.
- Biến filter chip tĩnh thành filter builder có field/operator/value.
- Thêm handler cho nút/dropdown đang có UI nhưng chưa có hành vi.
- Không sửa sâu API/backend, chỉ gọi store/API đã có hoặc tạo TODO rõ nếu endpoint thiếu.
- Không redesign layout lớn, chỉ thêm popover/menu/toast/loading nhỏ.

Prompt dùng cho AI Dev 1:
```text
Bạn là AI Dev 1 phụ trách Frontend Interaction trong D:\A\QuanLyCongViec. Hãy sửa các nút/dropdown/filter/redirect để hành vi giống Plane, không redesign giao diện lớn. Chỉ sở hữu các file: Frontend/package.json, Frontend/src/router/*, Frontend/src/components/FilterBar.vue, Frontend/src/components/HelpDropdown.vue, Frontend/src/components/CalendarTab.vue, Frontend/src/views/GlobalAnalyticsView.vue và các file UI shell liên quan trực tiếp. Mọi control nhìn click được phải có handler, menu, route, toast hoặc disabled state có lý do. Chạy frontend ở localhost:3000. Không sửa file Dev 2 sở hữu trừ khi bắt buộc, nếu bắt buộc thì ghi rõ lý do.
```

### AI Dev 2 - Data/API Business Owner
Phạm vi chính:
- `Frontend/src/store/*`
- `Frontend/src/api/*`
- `Frontend/src/components/KanbanBoard.vue`
- `Frontend/src/components/TaskDetailModal.vue`
- `Frontend/src/components/ListView.vue`
- `Frontend/src/components/SpreadsheetTab.vue`
- `Frontend/src/components/CyclesTab.vue`
- `Frontend/src/views/SpaceSummary.vue`
- Backend service/controller liên quan task/project/member/filter nếu endpoint thiếu.

Nhiệm vụ:
- Đảm bảo status, priority, assignee, label, start date, due date, cycle, module, parent lưu được qua API.
- Đảm bảo drag/drop kanban cập nhật status và sortOrder thật.
- Chuẩn hóa filter predicate để các view dùng chung.
- Thêm rollback/toast khi API lỗi.
- Kiểm tra permission/RBAC khi update/delete.

Prompt dùng cho AI Dev 2:
```text
Bạn là AI Dev 2 phụ trách Data/API Business trong D:\A\QuanLyCongViec. Hãy đảm bảo mọi hành động giống Plane không chỉ đổi UI local mà lưu được qua store/API/backend: status, priority, assignee, label, plannedStartDate, dueDate, cycle, module, parent, drag/drop sortOrder. Sở hữu Frontend/src/store/*, Frontend/src/api/*, KanbanBoard.vue, TaskDetailModal.vue, ListView.vue, SpreadsheetTab.vue, CyclesTab.vue, SpaceSummary.vue và backend endpoint/service liên quan. Không redesign UI lớn. Không sửa vùng Dev 1 trừ khi bắt buộc. Mọi update quan trọng phải có loading/error/rollback và không vi phạm permission project.
```

### AI QA - Parallel Reviewer and Fixer
Phạm vi chính:
- Đọc toàn bộ frontend/backend.
- Có thể sửa lỗi nhỏ không xung đột: typo route, missing import, handler bị gọi sai tên, disabled/toast nhỏ.
- Không refactor lớn, không đổi UI lớn.

Nhiệm vụ:
- Tạo checklist theo màn hình và chạy kiểm tra khi Dev 1/Dev 2 đang sửa.
- Tìm nút có UI nhưng không hoạt động.
- Test redirect, filter, dropdown, drag/drop, modal create/edit, API lỗi.
- Báo bug theo format thống nhất.

Prompt dùng cho AI QA:
```text
Bạn là AI QA chạy song song trong D:\A\QuanLyCongViec. Hãy kiểm tra theo chuẩn Plane: mọi button/icon/dropdown/link/tab/filter chip phải có hành vi rõ ràng. Tập trung phát hiện UI control không handler, dropdown không chọn được, filter không áp dụng, redirect sai, API update không lưu, drag/drop không rollback khi lỗi. Bạn được sửa lỗi nhỏ không xung đột như missing import, sai route, sai tên function, thiếu disabled/toast. Không redesign UI lớn, không refactor lớn. Báo bug theo format: Severity, File, Control, Expected, Actual, Repro steps, Suggested fix.
```

## 9. Checklist QA theo màn hình
Authentication:
- `/login` login xong quay lại `redirect` nếu có.
- `/register` không redirect sai khi đã login.
- 401 API tự refresh token, refresh fail thì về `/login`.

Dashboard:
- Card/link nào click được phải mở route/modal đúng.
- Không còn `<a href="#">` làm trang nhảy lên đầu.
- Empty state rõ khi chưa có project/space.

Spaces/Project:
- Tạo space/project xong redirect đúng trang chi tiết.
- Sidebar/topbar dropdown mở/đóng được.
- Tab overview, cycles, modules, views, pages chuyển đúng route.

Work items:
- Create task: title required, description optional, Save gọi API, Create more giữ modal.
- Detail task: edit title/description/status/priority/assignee/label/date/cycle/module/parent lưu được.
- Copy link copy đúng URL hiện tại.
- Ellipsis mở menu action: edit, duplicate, archive/delete nếu có nghiệp vụ.

Kanban:
- Card click mở detail.
- Drag trong cùng cột đổi sortOrder.
- Drag sang cột khác đổi status và sortOrder.
- API lỗi thì card quay về vị trí cũ và hiện toast.

Filter:
- Add filter mở builder.
- Chọn field/operator/value xong chip hiển thị đúng.
- Remove chip cập nhật danh sách ngay.
- Clear all xóa filter và reload data.
- Filter áp dụng đồng bộ kanban/list/spreadsheet/calendar.

Dropdown/Popover:
- Search trong assignee/label/cycle/module lọc đúng.
- Chọn item có checkmark hoặc selected state.
- Click ngoài đóng popover.
- Không có dữ liệu thì hiện empty state.

Admin/God mode nếu có:
- Route admin guard rõ ràng.
- User không đủ role không vào được hoặc có thông báo rõ.
- Các nút cấu hình Lưu/Hủy/Reset có API hoặc local state rõ.

## 10. Cách phát hiện nút có nhưng chưa hoạt động
Tìm trong code:

```powershell
cd D:\A\QuanLyCongViec\Frontend
rg "<button|<a href=\"#\"|class=\".*btn|icon-btn|toolbar-btn|filter-action|el-dropdown-item" src
```

Quy tắc kiểm tra:
- `<button>` không có `@click`, `type="submit"`, `disabled`, hoặc nằm trong dropdown reference thì phải xem là nghi vấn.
- `<a href="#">` là bug nếu không có `@click.prevent` hoặc route thật.
- `el-dropdown-item` không có `command` hoặc parent dropdown không có `@command` là nghi vấn.
- Icon có class `icon-btn`, `toolbar-btn`, `filter-action`, `plane-toolbar-btn` mà không có handler là nghi vấn.
- Nút chỉ `console.log` mà không thay đổi UI/API là chưa đạt.

## 11. Format báo cáo bug
```md
### BUG-001: Nút Filters trong Cycles không mở menu
Severity: High
File: Frontend/src/components/CyclesTab.vue
Control: button.filter-action "Filters"
Expected: Click mở filter builder giống Plane, chọn status/date/member để lọc cycle.
Actual: Click không có phản hồi.
Repro: Vào /space/{id}/cycles, click Filters.
Suggested fix: Gắn @click mở popover FilterBar hoặc reuse filter builder chung.
Owner: Dev 1
Status: Open
```

## 12. Definition of Done
Một chức năng được xem là xong khi:
- UI control có handler hoặc disabled state rõ.
- Dropdown/popover mở, search, chọn, bỏ chọn, đóng được.
- Thay đổi quan trọng lưu qua API hoặc có TODO endpoint rõ ràng nếu backend chưa có.
- Có loading, empty, error state.
- Không đổi giao diện lớn ngoài bổ sung interaction nhỏ.
- Build frontend không lỗi.
- Không phá route login/redirect.
- QA đã kiểm ít nhất dashboard, space detail, task modal, kanban, filter, cycles.

## 13. Quy tắc phối hợp chống conflict
- Dev 1 không sửa store/API/backend nếu Dev 2 đang xử lý.
- Dev 2 không sửa router/filter shell nếu Dev 1 đang xử lý.
- QA ưu tiên đọc và báo bug; chỉ sửa lỗi nhỏ độc lập.
- Nếu cần sửa cùng file, AI phải ghi rõ phần mình sửa và lý do.
- Không xóa code đang dùng nếu chưa thay bằng logic tương đương.
- Không thêm thư viện mới nếu chưa cần thiết. Dùng Vue 3, Pinia, Element Plus, Tailwind, Axios đã có.

## 14. Nghiệp vụ nên bổ sung thêm
Các nghiệp vụ này giúp dự án tiến gần Plane hơn nhưng nên làm sau khi nút/dropdown/filter cơ bản ổn:
- Saved Views: lưu bộ filter/sort/display thành view riêng.
- My Work: lọc task assigned to me, created by me, due soon, overdue.
- Quick create: tạo task nhanh trong từng status column.
- Bulk update: chọn nhiều task rồi đổi status/assignee/label.
- Activity log: ghi lại thay đổi status, assignee, priority, due date.
- Notification: nhắc khi được assign hoặc task gần due date.
- Permission theo project role: owner/admin/member/viewer.

## 15. Ưu tiên triển khai
P0:
- Chạy frontend ở port 3000.
- Sửa redirect login và API base URL.
- Tìm và xử lý nút có UI nhưng không hoạt động ở màn hình chính.

P1:
- Hoàn thiện TaskDetailModal: status, priority, assignee, date, label, cycle, module, parent.
- Hoàn thiện KanbanBoard: drag/drop status + sortOrder + rollback.
- Hoàn thiện FilterBar builder.

P2:
- Đồng bộ filter cho list/spreadsheet/calendar.
- Saved views và query URL.
- QA regression tự động bằng checklist/script.

## 16. Kết quả rà song song từ 3 AI
Phần này bổ sung sau khi 3 AI đọc repo song song. Đây là danh sách ưu tiên để bắt đầu sửa ngay.

### Dev 1 - Chuẩn Plane cần tham chiếu sâu hơn
Các module Plane nên mở khi cần clone hành vi chi tiết:
- `D:\A\plane\apps\web\core\components\dropdowns\state\dropdown.tsx`
- `D:\A\plane\apps\web\core\components\dropdowns\member\dropdown.tsx`
- `D:\A\plane\apps\web\core\components\dropdowns\date.tsx`
- `D:\A\plane\apps\web\core\components\work-item-filters\filters-row.tsx`
- `D:\A\plane\apps\web\core\components\rich-filters\filters-row.tsx`
- `D:\A\plane\apps\web\core\components\issues\issue-layouts\filters\header\filters\state.tsx`
- `D:\A\plane\apps\web\core\components\issues\issue-layouts\filters\header\filters\assignee.tsx`
- `D:\A\plane\apps\web\core\components\issues\issue-layouts\filters\header\filters\start-date.tsx`
- `D:\A\plane\apps\web\app\routes\redirects\core\index.ts`

Quy tắc Plane quan trọng:
- Dropdown single-select chọn xong đóng menu; multi-select giữ menu mở.
- Dropdown dài phải có search và empty state.
- Applied filter phải có chip remove riêng và `Clear all`.
- URL cũ nên redirect về canonical route thay vì để user ở màn lỗi.
- Auth redirect phải validate đường dẫn, tránh redirect lung tung.

### Dev 2 - Gap cụ thể trong QuanLyCongViec
Các điểm cần kiểm tra/sửa trước:
- `Frontend/package.json`: dev port hiện là `5173`.
- `Frontend/src/views/SpaceSummary.vue`: filter/display có UI nhưng flow filter/order/groupBy/show-subitems cần nối logic thật.
- `Frontend/src/views/SpaceSummary.vue`: `show sub-work items` đang có nguy cơ bị disabled hoặc chưa có nghiệp vụ đầy đủ.
- `Frontend/src/components/ListView.vue`: các field `Labels`, `Start date`, `Due date`, `Cycle`, `Modules` có nguy cơ đang bị khóa cứng hoặc chưa inline edit được.
- `Frontend/src/components/ViewsTab.vue`: `handleAddFilter()` có nguy cơ no-op, filter view chưa đi vào query thật.
- `Frontend/src/views/ManageSpaces.vue`: nút `Created date`, `Filters`, gear/admin cần handler/route rõ.
- `Frontend/src/views/Dashboard.vue`: các quickstart link `href="#"` cần đổi thành route/modal/action thật.
- `Frontend/src/views/YourWorkView.vue`: tab `Created`, `Subscribed` và overview có nguy cơ dùng mock/hardcode.
- `Frontend/src/router/index.js`: role guard admin đang bị comment phần chặn, cần quyết định bật lại hoặc hiển thị lý do rõ.
- `Frontend/vite.config.js` và backend CORS: nếu chuyển frontend sang port 3000 thì CORS/proxy phải khớp.

### QA - Checklist ưu tiên kiểm thử
QA cần test theo thứ tự:
- Auth: login, register, accept invite, GitHub callback, redirect sau login.
- Dashboard/ManageSpaces: mọi card/link/nút tạo project/space/filter/sort/admin.
- SpaceSummary: tab board/list/calendar/spreadsheet/timeline, filter, display, add work item.
- TaskDetailModal: state, priority, assignee, label, start date, due date, cycle, module, parent, comment, subtask, copy link, ellipsis.
- Kanban: click card, drag cùng cột, drag khác cột, API lỗi rollback.
- Drafts: edit/copy/move/delete/save/discard và toàn bộ dropdown trong modal.
- Admin: user management, audit log, configuration, organization profile/contact, 2FA, password, IP whitelist.

Format bug bắt buộc:
```md
Bug ID: BUG-<screen>-<yyyymmdd>-<seq>
Title: [Screen] [Action] [Symptom]
Severity: Critical | High | Medium | Low
Priority: P0 | P1 | P2 | P3
Environment: browser, OS, route, account
Preconditions: dữ liệu cần có
Steps to Reproduce: các bước cụ thể
Expected Result: kết quả đúng
Actual Result: kết quả thực tế
Evidence: screenshot, console, network nếu có
API/Request Info: endpoint, payload, status code nếu có
Owner: Dev 1 | Dev 2 | QA
Status: New | Confirmed | In Progress | Fixed | Retest | Closed
```

