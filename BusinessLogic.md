1. Phân hệ Không gian làm việc & Định danh (Workspace & Auth)
Nghiệp vụ cơ bản: Đăng ký, Đăng nhập (JWT), Phân quyền đa tầng, Mời thành viên vào dự án.

☠️ TỬ HUYỆT NGHIỆP VỤ & ĐẶC TẢ KỸ THUẬT:

1.1 Bảo mật JWT (JSON Web Token):

Vấn đề: Token sống quá lâu dễ bị lộ.

Dev Guide (.NET & Vue): * Backend: Cấp 2 loại Token khi Login thành công: AccessToken (hạn 15 phút, trả về trong JSON body) và RefreshToken (hạn 7 ngày, set vào HttpOnly Cookie để tránh bị XSS attack lấy cắp).

Frontend: Dùng Axios Interceptor. Khi API trả về lỗi 401 Unauthorized, tự động gọi API POST /api/auth/refresh-token (trình duyệt tự đính kèm cookie) để lấy AccessToken mới, sau đó gọi lại API vừa fail.

1.2 Ma trận Phân quyền Đa tầng (Multi-level RBAC):

Vấn đề: Quyền của "Kế toán" khác hoàn toàn với quyền của "Dev". Nếu gắn chung 1 bảng sẽ gây loạn hệ thống (Ví dụ: Kế toán không được phép xóa Task, Dev không được phép xem hóa đơn).

Giải pháp Kiến trúc: Bắt buộc chia Role thành 2 cấp độ:

Cấp 1: System Roles (Quyền cấp Hệ thống / Công ty) - Lưu ở bảng Users hoặc UserRoles. Gồm:

SuperAdmin: Trùm cuối, có quyền xóa Workspace, khóa user, đổi cấu hình Gamification.

Accountant (Kế toán): Chỉ truy cập module Thanh toán/Chi phí/Tính lương KPI. Không quan tâm Task.

Marketing: Quản lý các chiến dịch quảng bá sản phẩm, không can thiệp code.

Cấp 2: Project Roles (Quyền cấp Dự án) - Lưu ở bảng ProjectMembers. Gồm:

PO (Product Owner): Định hướng sản phẩm, tạo Backlog, duyệt Task cuối cùng (Acceptance).

SM (Scrum Master): Xóa bỏ rào cản, đảm bảo team chạy đúng Agile, quản lý Sprint.

PM (Project Manager): Quản lý tiến độ, chia việc, ép deadline.

PA (Project Admin/Assistant): Trợ lý dự án, giúp PM dọn dẹp data, xuất báo cáo.

TechLead: Cấp cao của Dev, có quyền Review Code và duyệt Task sang Done.

DEV (Developer): Nhận Task, kéo thả trạng thái, comment, log giờ làm.

QA/Tester: Quyền chuyển Task từ "Testing" sang "Done" hoặc "Reject" (Trả về cho Dev).

Designer: UI/UX, giống Dev nhưng chỉ làm việc với file Figma/Hình ảnh.

DevOps / Deployment (Team Triển khai): Nắm giữ "chìa khóa" hạ tầng dự án. Chịu trách nhiệm thiết lập CI/CD pipeline, cấu hình server, đưa code lên môi trường Staging/Production, và giám sát tài nguyên. Có đặc quyền Rollback (hoàn tác) bản cập nhật nếu xảy ra sự cố.
Dev Guide (.NET & Vue) cho Phân quyền: * Backend (.NET): Viết custom middleware hoặc Action Filter [ProjectAuthorize(Roles = "PM, PO, SM, TechLead, DevOps")]. Khi Request vào (VD: DELETE /api/projects/{projectId}/tasks/{taskId}), Filter này phải lấy UserId (từ JWT) và ProjectId (từ Route), query vào bảng ProjectMembers để kiểm tra cột ProjectRole. Không có quyền -> ném ngay 403 Forbidden.
* Lưu ý riêng cho DevOps: Các API can thiệp vào server như POST /api/projects/{projectId}/deployments/trigger bắt buộc chặn cứng, chỉ cấp quyền [ProjectAuthorize(Roles = "DevOps, TechLead, PM")].

Frontend (Vue 3): Các tab nhạy cảm như "CI/CD Pipelines", "Environment Variables" (Biến môi trường) hoặc nút "Trigger Deploy" dùng directive v-if="['DevOps', 'TechLead', 'PM'].includes(currentProjectRole)" để ẩn hoàn toàn khỏi UI của các role không liên quan.

1.3 Quyền Guest / Stakeholder (Khách hàng / Cổ đông):

Vấn đề: Khách hàng muốn vào xem dự án tiến triển tới đâu, nhưng sợ họ lỡ tay kéo thả làm hỏng Kanban.

Dev Guide (Vue 3): Trong component Kanban, bọc các thẻ <button> và sự kiện @dragstart bằng chỉ thị v-if="!['Guest', 'Stakeholder'].includes(currentProjectRole)".

Dev Guide (.NET): Bắt buộc các API thay đổi dữ liệu (POST, PUT, DELETE) phải chặn role Guest và Stakeholder. Nếu họ gọi API này (kể cả dùng Postman), trả về ngay 403. Chỉ cho phép các API GET (đọc dữ liệu) và POST ở chức năng Comment.

1.4 Vấn nạn "Task Mồ Côi" (Orphan Tasks) & Soft Delete:

Dev Guide (.NET):

API: DELETE /api/projects/{projectId}/members/{userId}.

DB Logic: KHÔNG DÙNG lệnh db.ProjectMembers.Remove(). Thay vào đó: member.Status = 0; member.LeftAt = DateTime.UtcNow;.

Trigger/Logic xử lý Task: Chạy tiếp lệnh xóa liên kết: var orphans = db.TaskAssignments.Where(ta => ta.UserId == removedUserId); db.TaskAssignments.RemoveRange(orphans);. Ghi sự kiện vào Bảng Notifications để báo cho PM phân công lại.

1.5 Đăng xuất an toàn & Thu hồi Token (Token Revocation):

Vấn đề: RefreshToken còn hạn nhưng User lỡ lộ máy tính hoặc bấm đăng xuất, làm sao để chặn?

Dev Guide (.NET): API POST /api/auth/logout. Tiến hành xóa HttpOnly Cookie ở Frontend, đồng thời cập nhật IsRevoked = true cho RefreshToken đó trong Database. Kẻ gian có lấy được token cũ cũng sẽ bị DB từ chối.