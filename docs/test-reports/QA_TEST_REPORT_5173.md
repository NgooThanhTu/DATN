# 🚨 BÁO CÁO KIỂM THỬ TOÀN DIỆN DỰ ÁN QUẢN LÝ CÔNG VIỆC (LOCALHOST:5173)

**Ngày test:** 19/04/2026
**Môi trường:** URL `http://localhost:5173` (đã thống nhất tiếp tục sử dụng port này theo `package.json`). Backend chạy song song ở `http://localhost:5136`.
**Người thực hiện:** QA Subagent

---

## I. TÌNH TRẠNG CHUNG (OVERALL STATUS)
Dự án đã lên hình cơ bản với layout và UI khá sát mẫu Plane. Các chức năng đọc (GET) hoạt động tạm ổn và giao tiếp SignalR base đã kết nối. 

Tuy nhiên, **logic nghiệp vụ cập nhật (PUT/PATCH) đang gãy hoàn toàn** do backend trả về lỗi 500, và **nhiều UI Element là nút tĩnh chưa có handler** đúng như dự đoán trong `AI_PLANE_PARALLEL_REPAIR_GUIDE.md`.

---

## II. CHI TIẾT CÁC LỖI PHÁT HIỆN ĐƯỢC (GAP & BUGS)

### 1. Dashboard (Bảng điều khiển)
- ✅ Các nút chuyển hướng như **"Configure this workspace"** (tới /spaces) và **"Personalize now"** (tới /profile) hoạt động tốt.
- 🔴 **LỖI (Nghiệp vụ - UI Shell):** Nút **"Get them in"** (Invite) và **"Add quick link"** hoàn toàn vô tác dụng khi click. Điều này vi phạm nguyên tắc "Nút nhìn click được thì phải có hành vi rõ ràng" trong Guide.

### 2. Task Management (Modal Chi tiết Công việc)
- ✅ Cập nhật Frontend trước đó: Các dropdown Priority, Status đã mở bình thường.
- 🔴 **LỖI NGHIÊM TRỌNG (Backend API 500):** Khi thay đổi Trạng thái (Status) từ "TO DO" sang "IN PROGRESS", hoặc khi thay đổi Mức độ ưu tiên (Priority), giao diện báo lỗi _"Không thể cập nhật công việc"_. API `PATCH/PUT /api/projects/../WorkTasks/..` ném về **Error 500 Internal Server Error**. 
  - *Nguyên nhân có thể:* Database Update đang lỗi khóa phụ (Foreign Key), Logic Validation ở Entity/Service ném exception do sai format ID, hoặc user authentication header thiếu sau qua bypass dev login.

### 3. Kanban Board
- ✅ Tạo mới task diễn ra bình thường (API POST hoạt động ổn).
- 🔴 **LỖI (Kéo thả):** Kéo thả Task (Drag & Drop) sang cột khác bị lỗi. UI cho phép kéo nhưng khi thả ra, card dội lại vị trí cũ hoặc không nhảy status do cập nhật API bị lỗi 500 như đã phát hiện ở trên.

### 4. Cycles & Modules Navigation
- 🔴 **LỖI ĐIỀU HƯỚNG (Router/Link):** Khi đang ở trong chi tiết Project, bấm vào các Menu Sidebar tương ứng ("Cycles", "Modules") **không phản hồi**. Không tự navigate sang view được (bắt buộc phải gõ thẳng URL mới vào được trang).
- ✅ Nút "Filtes" hoặc mở modal tạo mới ("Add cycle/module") bên trong trang hoạt động trơn tru.

### 5. Authentication (Xác thực)
- 🔴 Thông tin tài khoản `test@example.com` / `Test@123` đang báo không tồn tại (Có thể DB seed thiếu hoặc lỗi hash password). Phải dùng nút **"🚀 Dev Login"** để được đi xuyên qua.

---

## III. KẾT LUẬN & ĐỀ XUẤT CHO DEV 1, DEV 2

Dựa trên tài liệu `AI_PLANE_PARALLEL_REPAIR_GUIDE.md`, dưới đây là việc cần phân công để fix gấp:

### Việc của AI Dev 1 (Frontend Interaction Owner)
1. Cần sửa gấp nút `Cycles` và `Modules` ở sidebar nội bộ Project để sử dụng đúng `<router-link>` hoặc `@click` navigate. Chắc chắn do code tĩnh với `href="#"`.
2. Sửa các nút chết trên trang Dashboard (`Get them in`, `Add quick link`). Nếu chưa làm tính năng thì phải thêm disabled hoặc phát Toast notification "Tính năng đang phát triển".

### Việc của AI Dev 2 (Data / API Business Owner) - ƯU TIÊN SỐ 1
1. **[CRITICAL]** Kiểm tra lỗi **500 Internal Server Error** sinh ra từ Controller `WorkTasksController`. Log backend (console backend) đang báo lỗi khi xử lý `UpdateTask` (Status, Priority). Có thể do C# AutoMapper thiếu field, Validation lỗi, hoặc SQL lỗi trigger.
2. Đảm bảo Drag & Drop Kanban khi xịt (500 error) sẽ có "rollback local change" để card không bị đứng sai cột mặt dù DB chưa cập nhật.
3. Fix seed data của `test@example.com` để đăng nhập mượt mà không cần dùng dev bypass.

📌 **Dev 2 cần kiểm tra backend log ngay bây giờ để xem tại sao EndPoint PUT/PATCH WorkTasks lại Crash 500.**
