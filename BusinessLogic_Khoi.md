BỔ SUNG: CẤP ĐỘ QUẢN TRỊ HỆ THỐNG & WORKFLOW IT HELPDESK
Workflow Toàn diện của Dự án (Bản thực thi) 
Luồng 1: Thiết lập Vành đai An ninh & Định danh (Organization Security Flow)
•	Admin truy cập thư mục Directory $\rightarrow$ Khởi tạo cấu trúc nhóm: it-support-team (Agent) và all-staff (Customer).
•	Admin chuyển sang Security $\rightarrow$ Áp dụng Access policies (Chính sách truy cập).
•	Hệ thống kiểm tra: Ép buộc toàn bộ tài khoản thuộc it-support-team phải xác thực 2 bước (2FA) trước khi cấp quyền truy cập.
Luồng 2: Khởi tạo Không gian Thực chiến (Project & Workspace Setup)
•	Admin vào giao diện tạo Project $\rightarrow$ Lựa chọn Space templates có sẵn.
•	Chọn Basic IT service management $\rightarrow$ Hệ thống tự động sinh các Issue Types (Ticket lỗi, Yêu cầu thiết bị) và gán sẵn SLA mặc định.
•	Admin cấu hình thanh điều hướng (Navigation) $\rightarrow$ Tắt các View không cần thiết, ghim Calendar và Reports lên sidebar.
Luồng 3: Áp dụng Ma trận Phân quyền (Project Roll-out Flow)
•	Admin mở Project Settings $\rightarrow$ Map (Gắn) các Group đã tạo ở Luồng 1 vào Project Roles.
•	it-support-team $\rightarrow$ Gán Role: Service Desk Team (Được kéo thả thẻ Kanban, trả lời khách).
•	all-staff $\rightarrow$ Gán Role: Customer (Chỉ được xem và tạo ticket qua Portal).
________________________________________
CÁC NGHIỆP VỤ CHÍNH TRONG DỰ ÁN: 
7. Phân hệ Quản trị Tổ chức & Định danh (Org Admin & Directory)
Nghiệp vụ cơ bản: Quản lý vòng đời User (Onboarding/Offboarding), phân nhóm và đồng bộ danh bạ.
TỬ HUYỆT NGHIỆP VỤ & ĐẶC TẢ KỸ THUẬT: 
•	7.1 Vấn nạn Phân mảnh Danh tính (Shadow IT):
o	Vấn đề: User tự dùng email domain của công ty để tạo các workspace rác ngoài tầm kiểm soát của tổ chức, gây rò rỉ dữ liệu.
o	System/Admin Guide: Tại phần "Cài đặt tổ chức", bắt buộc thực hiện Xác minh Tên miền (Domain Verification). Sau khi verify, Admin có quyền "Claim" (Thu nạp) toàn bộ các tài khoản đang trôi nổi dùng đuôi email công ty vào chung một Directory để quản lý tập trung.
•	7.2 Lỗ hổng Thu hồi Quyền Khẩn cấp (Offboarding Security):
o	Vấn đề: Nhân sự IT nghỉ việc mang theo Session JWT chưa hết hạn hoặc Admin quên xóa thủ công, dẫn đến rủi ro phá hoại.
o	System/Admin Guide: Tại mục "Nhà cung cấp thông tin" (IdP), thiết lập đồng bộ SCIM với Microsoft Entra ID hoặc Zalo/Google Workspace. Khi HR báo nghỉ và vô hiệu hóa email, luồng API sẽ tự động Disable User trên Jira, hủy ngay lập tức các Token hiện tại.
8. Phân hệ Chuẩn hóa Workspace & Trải nghiệm (Project Setup)
Nghiệp vụ cơ bản: Chọn Template dự án, tinh gọn giao diện làm việc.
TỬ HUYỆT NGHIỆP VỤ & ĐẶC TẢ KỸ THUẬT: 
•	8.1 Rác Dữ liệu từ Custom Fields (Custom Field Bloat):
o	Vấn đề: Nếu mỗi dự án Admin tự tạo một vài trường dữ liệu mới (Ví dụ: "Ngày duyệt", "Ngày approve"), DB sẽ phình to và hệ thống truy vấn chậm đi.
o	System/Admin Guide: Ép dùng chung Space templates (Ví dụ: Dùng Basic IT service management cho mọi team Support/Admin). Kế thừa 100% các trường dữ liệu hệ thống đã tối ưu, hạn chế tối đa việc tạo mới Custom Field ngoài luồng.
•	8.2 Nhiễu loạn UI/UX & Mất tập trung (Navigation Noise):
o	Vấn đề: Team IT Helpdesk nội bộ không viết code, nhưng giao diện Kanban lại hiển thị các module của Dev (GitHub, CI/CD, Deployments), gây rối mắt.
o	System/Admin Guide: Tại phần cấu hình View của dự án (Hình 5), thực hiện nguyên tắc "Least Privilege UI". Tắt/Ẩn hoàn toàn thẻ Development và Deployments.
o	Chỉ chọn Add to navigation (Thêm vào menu) các tính năng thực chiến: Mở Calendar để team xem lịch trực ca hệ thống, mở Reports để đo đếm biểu đồ SLA.
•	8.3 Quản lý quyền theo cá nhân (The "Individual Permission" Anti-Pattern):
o	Vấn đề: Add từng tên (Ví dụ: Nguyễn Văn A, Trần Thị B) vào dự án. Khi dự án có 100 người đổi team, Admin sẽ bị ngợp trong việc rà soát và gỡ quyền thủ công.
o	System/Admin Guide: Áp dụng chặt chẽ Ma trận RBAC: Tuyệt đối không cấp quyền cho User độc lập. Phải nhóm User vào Groups ở cấp độ Tổ chức. Sau đó vào Dự án, lấy cái Group đó gán cho một Role. Khi cần rút người, chỉ cần xóa User ra khỏi Group ở Directory là 100% các quyền tại các dự án tự động bốc hơi.

