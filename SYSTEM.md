# TASK MANAGEMENT SYSTEM - CORE AI CONTEXT (SYSTEM.md)

## 1. TỔNG QUAN DỰ ÁN (PROJECT OVERVIEW)
- **Tên dự án:** Hệ thống Quản trị Dự án và Công việc (Agile/Scrum & Gamification).
- **Mục tiêu:** Quản lý Sprint, Task Dependencies, điểm thưởng (User Wallet) và tích hợp AI.
- **Nguyên tắc cốt lõi:** Code sạch (Clean Code), bảo mật chặt chẽ (chống IDOR), tuân thủ nghiêm ngặt quy trình Agile.

## 2. STACK CÔNG NGHỆ (TECH STACK)
- **Backend:** C# .NET 10 (Web API), Entity Framework Core (Code First).
- **Frontend:** Vue 3 (Composition API với `<script setup>`), Vite, Element Plus UI, TailwindCSS, Pinia, Axios.
- **Database:** SQL Server.

## 3. QUY CHUẨN KIẾN TRÚC BACKEND (.NET 10)
- **Kiến trúc N-Tier:** KHÔNG ĐƯỢC viết business logic trong Controller. Phải gọi qua tầng Service.
- **Data Transfer Objects (DTOs):** Bắt buộc dùng DTO cho Input (Requests) và Output (Responses). Không bao giờ trả thẳng Entity Model ra ngoài API để tránh rò rỉ dữ liệu.
- **Chuẩn format API Response:** Mọi API phải trả về một Wrapper chuẩn: 
  `{ "statusCode": 200, "message": "Success", "data": { ... } }`
- **Xử lý lỗi (Exception Handling):** Dùng Global Exception Middleware. Bọc logic phức tạp trong `try-catch`.
- **Database Rules:** - Khóa chính (Primary Key) bắt buộc là `Guid`.
  - Không bao giờ xóa cứng (Hard Delete). Bắt buộc dùng Soft Delete (cờ `IsDeleted = true`).
  - **Quy tắc Thời gian:** TUYỆT ĐỐI KHÔNG dùng `DateTime.Now`. Bắt buộc phải dùng `DateTime.UtcNow`.
  - **Optimistic Concurrency:** Các lệnh Update Entity quan trọng phải có check Concurrency.

## 4. QUY CHUẨN KIẾN TRÚC FRONTEND (VUE 3)
- **Component-Driven:** Tách nhỏ UI thành các component tái sử dụng (VD: `TaskCard.vue`, `SprintBoard.vue`).
- **State Management:** Dùng Pinia để lưu trữ `Token` và `User Profile` toàn cục. Không lạm dụng LocalStorage cho dữ liệu nhạy cảm.
- **API Call:** Tách riêng các hàm gọi API ra thư mục `src/services` (VD: `authService.js`), không gọi Axios trực tiếp trong Component.
- **CSS Rule:** Dùng TailwindCSS để chia Layout (Flex/Grid) và Spacing. Dùng Element Plus cho Core UI Components. Lập tức xử lý xung đột `preflight` nếu có.

## 5. GUARDRAILS CHO AI (AI VIBE CODING RULES)
- **CẤM** sinh ra các thư viện bên thứ 3 (npm/nuget packages) nếu chưa hỏi ý kiến.
- **BẮT BUỘC** kiểm tra quyền sở hữu dữ liệu (IDOR) & Project RBAC trong mọi API Update/Delete: *User A chỉ được thao tác trên Project mà User A tham gia với đúng Role tương ứng.*
- Khi sinh code xử lý Database phức tạp (Kéo thả task, cộng điểm ví), **BẮT BUỘC** dùng Database Transaction (`BeginTransactionAsync`).
