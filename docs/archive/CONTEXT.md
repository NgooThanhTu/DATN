# SPRINTA PROJECT - AI & DEV CONTEXT (CONTEXT.md)

## 1. IDENTITY & MISSION
- **Tên dự án:** SprintA (Nền tảng Quản trị Dự án Agile & Gamification).
- **Core Stack:** .NET 10 (Backend), Vue 3 Composition API (Frontend), SQL Server.
- **Mục tiêu:** Sinh code Clean Architecture, chống lặp code, tuyệt đối tránh Git Conflict khi làm việc nhóm.

## 2. ANTI-CONFLICT RULES (LUẬT CODE CHỐNG XUNG ĐỘT)
- **Database Rules:** KHÔNG BAO GIỜ viết cấu hình bảng (HasKey, Property) trực tiếp vào `OnModelCreating` của `AppDbContext.cs`. 
  -> **BẮT BUỘC** tạo file cấu hình riêng kế thừa `IEntityTypeConfiguration<T>` cho mỗi Entity.
- **Dependency Injection (DI) Rules:** KHÔNG BAO GIỜ nhồi nhét đăng ký service (`AddScoped`, `AddTransient`) trực tiếp vào `Program.cs`. 
  -> **BẮT BUỘC** sử dụng Extension Methods (ví dụ: `ServiceCollectionExtensions.cs` cho từng phân hệ).
- **Frontend Routing & State:** KHÔNG nhồi nhét route vào `router/index.js` và KHÔNG dùng chung 1 file Pinia Store.
  -> **BẮT BUỘC** chia nhỏ thành các module (VD: `authRoutes.js`, `kanbanRoutes.js`, `useTaskStore.js`, `useUserStore.js`).
- **API Clients (Axios):** Tách file gọi API theo từng phân hệ (VD: `api/taskApi.js`, `api/sprintApi.js`), không gộp chung vào một file `api.js` khổng lồ.

## 3. GIT WORKFLOW (LUẬT KIỂM SOÁT PHIÊN BẢN)
1. Không bao giờ commit trực tiếp lên nhánh `main`.
2. Định dạng nhánh: `feature/<tên-module>-<tính-năng>` (VD: `feature/gamification-add-points`).
3. LUÔN LUÔN chạy `git pull origin main` trước khi tạo nhánh mới hoặc trước khi chỉnh sửa các file dùng chung (`Program.cs`, `AppDbContext.cs`, `package.json`).
4. File dùng chung bị khóa bằng miệng: Khi cần sửa file gốc, DEV phải chat báo cáo trên group team, sửa xong push ngay lập tức.

## 4. AI VIBE CODING GUARDRAILS (RÀNG BUỘC KHI AI SINH CODE)
- Khi yêu cầu tạo một Table/Entity mới, AI phải thực hiện đúng 3 bước:
  1. Tạo class Entity model.
  2. Tạo file Configuration riêng implement `IEntityTypeConfiguration`.
  3. Chỉ thêm đúng 1 dòng `DbSet<T>` vào `AppDbContext`.
- Luôn bọc các logic nghiệp vụ (Business logic) liên quan đến nhiều bảng trong một `Transaction` (ví dụ: `BeginTransactionAsync`) để đảm bảo tính toàn vẹn dữ liệu.
- **Tối ưu Output:** Khi sửa code, AI **CHỈ OUTPUT** đoạn mã cần sửa hoặc các function bị ảnh hưởng. **TUYỆT ĐỐI KHÔNG** in lại toàn bộ file hàng trăm dòng nếu không có thay đổi, tránh làm trôi lịch sử chat và gây rối cho Developer.