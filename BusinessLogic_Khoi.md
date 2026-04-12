Phân hệ Quản trị Hệ thống & Định danh Toàn cục (System Admin & Directory)
Nghiệp vụ cơ bản: Quản lý vòng đời nhân sự toàn cục, Cấu hình Tổ chức (Tenant).
TỬ HUYỆT NGHIỆP VỤ & ĐẶC TẢ KỸ THUẬT:

7.1 Lỗi ranh giới Quyền (Global Role vs Project Role):
Vấn đề: Giao diện User Management đang hiển thị Dự án (Project) thay vì Danh bạ toàn công ty.
Dev Guide (Vue 3): Xóa bỏ UI dạng Card Dự án. Thay bằng bảng <el-table> hiển thị toàn bộ AppUsers toàn cục. Tách API thành src/api/adminUserApi.js và tạo store useAdminUserStore.js.
Dev Guide (.NET): Viết Custom Middleware [SystemAuthorize(Roles = "SuperAdmin, Admin")]. API GET /api/admin/users: Query thẳng vào bảng AppUsers, tuyệt đối KHÔNG JOIN với bảng ProjectMembers.

7.2 Vòng đời Nhân sự & Cắt điện Khẩn cấp (Kill-Switch):
Vấn đề: Xóa cứng User làm đứt gãy Khóa ngoại của Task/Log.
Dev Guide (Vue 3): Thay icon Thùng rác bằng nút "Đình chỉ" (Suspend) dạng <el-switch> hoặc Nút đỏ cảnh báo.
Dev Guide (.NET):
* API: PUT /api/admin/users/{userId}/suspend.
* Logic: user.IsActive = false;. BẮT BUỘC gọi tiếp UPDATE RefreshTokens SET IsRevoked = 1 WHERE UserId = @userId trong cùng Transaction để ngắt điện toàn tập thiết bị của họ.

7.3 Tách bạch Hồ sơ Cá nhân & Tổ chức (Tenant Profiling & Contact):
Vấn đề: Menu Organization cho điền Họ tên cá nhân là sai. Cần tách biệt Profile công ty và thông tin Liên hệ công khai.
Dev Guide (Vue 3):
* Tab Profile: Chỉ chứa Input cho Tên Tổ chức, Domain, Logo.
* Tab Contact: Dùng <el-switch> cho "Cho phép Liên hệ", "Hồ sơ Công khai". Dùng <el-checkbox-group> cho các "Chủ đề liên hệ được phép".
Dev Guide (.NET): Tạo bảng TenantConfigs lưu cấu hình doanh nghiệp và cờ (flags) liên hệ.

7.4 Cấu hình Giao diện & Hiệu suất Toàn cục (Global Theme & Performance):
Vấn đề: Áp dụng CSS toàn cục và đo đạc API Response Time mà không làm chậm DB.
Dev Guide (Vue 3): Giao diện chọn màu sinh mã HEX. Dùng useCssVar hoặc JS thuần cập nhật biến CSS :root (ví dụ --el-color-primary) để đổi màu Realtime hệ thống. Vẽ biểu đồ "API Response Time" dạng cột Bar bằng thư viện ApexCharts.
Dev Guide (.NET): Biểu đồ Performance đo từ Middleware, nạp số liệu vào IMemoryCache và API chỉ trả về mảng dữ liệu từ Cache, tuyệt đối không Query từ Database để vẽ biểu đồ.

Phân hệ An ninh Mạng & Kiểm toán (Network Security & Audit)
Nghiệp vụ cơ bản: Ép buộc 2FA, Giới hạn IP, Tối ưu Audit Log.
TỬ HUYỆT NGHIỆP VỤ & ĐẶC TẢ KỸ THUẬT:

8.1 Thảm họa Tràn RAM từ Audit Log (Performance Bottleneck):
Vấn đề: Mở tùy chọn "All Time" kéo hàng triệu dòng làm Out of Memory.
Dev Guide (Vue 3): Tuyệt đối CẤM dùng nút "All Time" hay "All Projects". Bắt buộc dùng <el-date-picker type="daterange"> giới hạn tối đa 90 ngày.
Dev Guide (.NET): Cấm ghi log đồng bộ. Ném Object log vào System.Threading.Channels.Channel (In-memory queue). Tạo AuditLogWorker lôi batch 100 logs ra lưu Database 1 lần.

8.2 Ép buộc Chính sách 2FA toàn Công ty (Enforced 2FA Policy):
Vấn đề: Tính năng 2FA đang thiết kế tự nguyện, Admin cần quyền ép buộc mọi người phải cài.
Dev Guide (Vue 3): Admin có switch <el-switch>: "Ép buộc dùng 2FA". Ở luồng Client, dùng Axios Interceptor: Nếu API trả lỗi 403 (Require 2FA), ép Router chuyển hướng sang /setup-2fa và khóa Menu.
Dev Guide (.NET): Bảng TenantConfigs thêm cột Require2FA = true. Nếu check cấu hình Tenant là true mà User chưa cài, API Login trả mã 403 Require Setup 2FA. KHÔNG CẤP Access Token.

8.3 Bức tường lửa mức Ứng dụng (IP Whitelist Guard):
Dev Guide (Vue 3): Layout chia khối: Form thêm IP và bảng <el-table> danh sách IP / Nhật ký truy cập.
Dev Guide (.NET): Viết custom IpWhitelistMiddleware.cs. Load IP Whitelist từ DB lên IMemoryCache (TTL 5 phút). Cấu hình ForwardedHeadersOptions để bắt đúng IP thực tế xuyên qua Nginx.

8.4 Đăng xuất Đa thiết bị (Concurrent Sessions Termination):
Vấn đề: Khi bấm đổi mật khẩu và tick "Đăng xuất thiết bị khác", làm sao kick các máy tính khác ra ngoài?
Dev Guide (Vue 3): Gửi payload API đính kèm cờ logoutOtherDevices = true.
Dev Guide (.NET): Bảng RefreshTokens BẮT BUỘC có thêm cột DeviceId (tạo ngẫu nhiên từ Frontend khi đăng nhập). Logic đổi Pass: UPDATE RefreshTokens SET IsRevoked = 1 WHERE UserId = @id AND DeviceId != @currentDeviceId.

Quy chuẩn UI/UX & Layout Frontend (Dành riêng cho Admin Dashboard)
Mục tiêu: Ép AI code ra đúng giao diện Dark Mode & Glassmorphism như thiết kế ban đầu.
TỬ HUYỆT FRONTEND (Vue 3 + Tailwind + Element Plus):

9.1 Ngôn ngữ Thiết kế (Design Language Guardrails):
Dev Guide (Vue/Tailwind):
* Nền trang (Background): Tím than/Huyền bí (VD: bg-[#130f26]).
* Khối nội dung (Card): Bắt buộc dùng hiệu ứng kính mờ (Glassmorphism). Dùng class Tailwind: bg-[#1f1533]/80 backdrop-blur-md border border-gray-700 rounded-xl p-6 shadow-lg. Tuyệt đối cấm dùng bg-white.
* Màu nhấn (Primary): Xanh ngọc (Teal/Mint - text-teal-400, bg-teal-500).

9.2 Tối ưu Element Plus Components (Chống vỡ Dark Mode):
Dev Guide (Vue/CSS): AI khi sinh code Element Plus phải chú ý:
* Bảng dữ liệu (<el-table>): Bắt buộc override CSS (:deep(.el-table)) ép nền bảng trong suốt (transparent), màu chữ xám sáng (#d1d5db), border mờ nhạt.
* Biểu mẫu (<el-input>, <el-select>): Các input field phải có nền tối bg-black/20, viền xám đậm.
* Trạng thái (Badge): Dùng <el-tag>. User Active (type="success"), Suspended (type="danger").