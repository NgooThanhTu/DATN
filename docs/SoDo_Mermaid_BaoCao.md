# 📊 TỔNG HỢP SƠ ĐỒ MERMAID CHO BÁO CÁO DỰ ÁN

**Dự án:** QuanLyCongViec (SprintA)  
**Ngày tạo:** 19/04/2026  
**Phiên bản:** 2.0 (Đã tối ưu)

> **Hướng dẫn sử dụng:**  
> - Copy từng block `mermaid` vào [Mermaid Live Editor](https://mermaid.live) để xem trước  
> - Xuất PNG/SVG từ Live Editor rồi dán vào Word/PowerPoint  
> - Hoặc paste trực tiếp vào file `.md` nếu dùng GitHub/Notion  

---

## 1. SƠ ĐỒ PHÂN RÃ CHỨC NĂNG (Function Decomposition)

> **Lưu ý:** Sơ đồ được tách thành nhiều phần nhỏ để khi xuất PNG chữ đủ lớn, dễ đọc trong báo cáo Word.

### 1.0 Tổng quan hệ thống (Cấp 1)

```mermaid
graph TD
    ROOT["HỆ THỐNG QUẢN LÝ CÔNG VIỆC<br/>SprintA"]
    ROOT --> M1["1. Xác thực"]
    ROOT --> M2["2. Workspace & Dự án"]
    ROOT --> M3["3. Công việc"]
    ROOT --> M4["4. Agile / Sprint"]
    ROOT --> M5["5. Module"]
    ROOT --> M6["6. Cộng tác"]
    ROOT --> M7["7. Phân tích"]
    ROOT --> M8["8. Quản trị"]
    ROOT --> M9["9. Tích hợp AI"]
```

### 1.1 Xác thực & Workspace (Module 1 + 2)

```mermaid
graph TD
    M1["1. Xác thực"] --> F11["1.1 Đăng ký"]
    M1 --> F12["1.2 Đăng nhập"]
    M1 --> F13["1.3 Hồ sơ cá nhân"]
    M1 --> F14["1.4 Token & Phiên"]
    M1 --> F15["1.5 OAuth GitHub"]

    M2["2. Workspace & Dự án"] --> F21["2.1 CRUD Workspace"]
    M2 --> F22["2.2 Thành viên WS"]
    M2 --> F23["2.3 CRUD Project"]
    M2 --> F24["2.4 Cài đặt Project"]
    M2 --> F25["2.5 Thành viên Project"]
    M2 --> F26["2.6 Mời thành viên"]
```

### 1.2 Quản lý Công việc (Module 3)

```mermaid
graph TD
    M3["3. Quản lý Công việc"]

    M3 --> G1["Quản lý thuộc tính"]
    M3 --> G2["Chế độ xem"]

    G1 --> F31["3.1 CRUD Task"]
    G1 --> F32["3.2 Trạng thái"]
    G1 --> F33["3.3 Độ ưu tiên"]
    G1 --> F34["3.4 Gán người"]
    G1 --> F35["3.5 Nhãn"]
    G1 --> F36["3.6 Ngày tháng"]
    G1 --> F37["3.7 Sub-task"]
    G1 --> F38["3.8 Phụ thuộc"]

    G2 --> F39["3.9 Kanban Board"]
    G2 --> F3A["3.10 List View"]
    G2 --> F3B["3.11 Calendar"]
    G2 --> F3C["3.12 Timeline"]
    G2 --> F3D["3.13 Kéo thả"]
    G2 --> F3E["3.14 Lọc / Sắp xếp"]
```

### 1.3 Agile, Module, Cộng tác (Module 4 + 5 + 6)

```mermaid
graph TD
    M4["4. Agile / Sprint"] --> F41["4.1 CRUD Sprint"]
    M4 --> F42["4.2 Gán Task vào Sprint"]
    M4 --> F43["4.3 Sprint Backlog"]
    M4 --> F44["4.4 Burndown Chart"]

    M5["5. Module"] --> F51["5.1 CRUD Module"]
    M5 --> F52["5.2 Gán Task"]
    M5 --> F53["5.3 Tiến độ Module"]

    M6["6. Cộng tác"] --> F61["6.1 Bình luận"]
    M6 --> F62["6.2 Thông báo"]
    M6 --> F63["6.3 Pages / Wiki"]
    M6 --> F64["6.4 Sticky Notes"]
    M6 --> F65["6.5 SignalR Real-time"]
```

### 1.4 Phân tích, Quản trị, AI (Module 7 + 8 + 9)

```mermaid
graph TD
    M7["7. Phân tích"] --> F71["7.1 Analytics dự án"]
    M7 --> F72["7.2 Analytics toàn cục"]
    M7 --> F73["7.3 Custom Views"]
    M7 --> F74["7.4 Audit Log"]

    M8["8. Quản trị"] --> F81["8.1 Quản lý Users"]
    M8 --> F82["8.2 Phòng ban"]
    M8 --> F83["8.3 RBAC"]
    M8 --> F84["8.4 Gamification"]

    M9["9. Tích hợp AI"] --> F91["9.1 AI chia nhỏ Task"]
    M9 --> F92["9.2 AI gợi ý"]
    M9 --> F93["9.3 Prompt Templates"]
```

---

## 2. SƠ ĐỒ KIẾN TRÚC HỆ THỐNG (System Architecture)

```mermaid
graph TB
    subgraph Client["CLIENT - Browser"]
        VUE["Vue 3 + Pinia"]
        AXIOS["Axios HTTP"]
        SRC["SignalR Client"]
    end

    subgraph Backend["ASP.NET Core 10 Web API"]
        MW["Middleware<br/>JWT | CORS | Logging"]
        CTRL["24 Controllers<br/>100+ Endpoints"]
        HUB["SignalR Hub"]
    end

    subgraph Domain["Domain Layer"]
        ENT["47 Entities"]
        SVC["Business Logic"]
    end

    subgraph Infra["Infrastructure"]
        EF["EF Core ORM"]
        DB[("SQL Server")]
    end

    subgraph Ext["External"]
        GH["GitHub OAuth"]
        AI_S["AI Service"]
    end

    VUE --> AXIOS
    VUE --> SRC
    AXIOS -->|REST JSON| MW
    SRC -->|WebSocket| HUB
    MW --> CTRL
    CTRL --> SVC
    SVC --> ENT
    ENT --> EF
    EF --> DB
    CTRL -.->|OAuth| GH
    CTRL -.->|API| AI_S
```

---

## 3. SƠ ĐỒ QUAN HỆ THỰC THỂ - ERD (Core Entities)

```mermaid
erDiagram
    User ||--o{ WorkspaceMember : "tham gia"
    User ||--o{ ProjectMember : "tham gia"
    User ||--o{ TaskAssignment : "được gán"
    User ||--o{ Comment : "viết"

    Workspace ||--o{ WorkspaceMember : "có"
    Workspace ||--o{ Project : "chứa"

    Project ||--o{ ProjectMember : "có"
    Project ||--o{ WorkTask : "chứa"
    Project ||--o{ Sprint : "có"
    Project ||--o{ Module : "có"
    Project ||--o{ Label : "có"

    WorkTask ||--o{ TaskAssignment : "gán cho"
    WorkTask ||--o{ Comment : "có"
    WorkTask ||--o{ IssueLabel : "gắn nhãn"
    WorkTask ||--o{ TaskDependency : "phụ thuộc"
    WorkTask }o--o| Sprint : "thuộc sprint"

    Label ||--o{ IssueLabel : "gắn vào"
    Module ||--o{ IssueModule : "chứa"
    WorkTask ||--o{ IssueModule : "thuộc module"

    User {
        guid Id PK
        string Email
        string FullName
        string PasswordHash
        datetime CreatedAt
    }

    Workspace {
        guid Id PK
        string Name
        string Slug
    }

    Project {
        guid Id PK
        guid WorkspaceId FK
        string Name
        string Identifier
    }

    WorkTask {
        guid Id PK
        guid ProjectId FK
        guid ParentTaskId FK
        guid SprintId FK
        string Title
        string StatusName
        int Priority
        int SortOrder
        datetime DueDate
    }

    Sprint {
        guid Id PK
        guid ProjectId FK
        string Name
        datetime StartDate
        datetime EndDate
    }

    Comment {
        guid Id PK
        guid WorkTaskId FK
        guid UserId FK
        string Content
        datetime CreatedAt
    }

    Label {
        guid Id PK
        string Name
        string Color
    }
```

---

## 4. SƠ ĐỒ USE CASE

> **Lưu ý:** Tách thành 3 sơ đồ theo từng Actor để tránh mũi tên chồng chéo, dễ đọc khi in ra giấy.

### 4.1 Use Case - Người dùng (User)

```mermaid
graph LR
    U(("Người dùng"))

    U --> UC1["Đăng ký"]
    U --> UC2["Đăng nhập"]
    U --> UC3["Quản lý hồ sơ"]
    U --> UC4["Tạo Workspace"]
    U --> UC5["Tạo Project"]
    U --> UC6["CRUD Task"]
    U --> UC7["Kanban Board"]
    U --> UC8["Kéo thả Task"]
    U --> UC9["List / Calendar / Timeline"]
    U --> UC10["Tạo Sprint"]
    U --> UC11["Bình luận"]
    U --> UC12["Pages Wiki"]
    U --> UC13["Analytics"]
```

### 4.2 Use Case - Admin

```mermaid
graph LR
    A(("Admin"))

    A --> UC1["Quản lý Users"]
    A --> UC2["Quản lý Phòng ban"]
    A --> UC3["Phân quyền RBAC"]
    A --> UC4["Audit Log"]
    A --> UC5["System Settings"]
    A --> UC6["Mời thành viên"]
```

### 4.3 Use Case - AI Agent

```mermaid
graph LR
    AI(("AI Agent"))

    AI --> UC1["Chia nhỏ Task tự động"]
    AI --> UC2["Gợi ý công việc"]
    AI --> UC3["Quản lý Prompt Templates"]
```

---

## 5. SƠ ĐỒ QUY TRÌNH SCRUM (Scrum Flow)

```mermaid
graph LR
    PB["Product Backlog<br/>67 User Stories"]
    SP["Sprint Planning"]
    SB["Sprint Backlog"]

    subgraph Sprint["SPRINT - 2 tuần"]
        DS["Daily Standup<br/>15 phút"]
        DEV["Phát triển"]
        TEST["Testing"]
        DS --> DEV --> TEST --> DS
    end

    SR["Sprint Review<br/>Demo"]
    RETRO["Retrospective"]
    INC["Product<br/>Increment"]

    PB --> SP --> SB --> Sprint
    Sprint --> SR --> INC
    SR --> RETRO
    RETRO -->|"Cải tiến"| PB
```

---

## 6. SƠ ĐỒ RELEASE & SPRINT TIMELINE

```mermaid
gantt
    title Kế hoạch Release & Sprint - SprintA
    dateFormat  YYYY-MM-DD
    axisFormat  %d/%m

    section Release 1.0 MVP
    Sprint 1 - Auth + WS + Project     :s1, 2026-03-03, 14d
    Sprint 2 - Task + Board + Cycles   :s2, after s1, 14d

    section Release 2.0 Full
    Sprint 3 - Views + Analytics       :s3, after s2, 14d
    Sprint 4 - Admin + AI + Test       :s4, after s3, 7d

    section Milestones
    M1 Auth Done          :milestone, 2026-03-17, 0d
    M2 Task Core Done     :milestone, 2026-03-31, 0d
    M3 Sprint Review      :milestone, 2026-04-14, 0d
    M4 Nộp sản phẩm       :milestone, 2026-04-21, 0d
```

---

## 7. SƠ ĐỒ TRIỂN KHAI (Deployment)

```mermaid
graph TB
    subgraph Dev["Development"]
        VSC["VS Code + Copilot"]
        GIT["GitHub Repo"]
    end

    subgraph Server["Server - localhost"]
        BE["ASP.NET Core<br/>Port 5136"]
        FE["Vite Dev<br/>Port 5173"]
        SR["SignalR Hub"]
    end

    subgraph Data["Database"]
        SQL[("SQL Server")]
    end

    Browser["Browser<br/>Vue 3 SPA"]

    VSC -->|git push| GIT
    GIT -->|git pull| VSC
    VSC -->|dotnet run| BE
    VSC -->|npm run dev| FE

    Browser -->|HTTP :5173| FE
    FE -->|Proxy :5136| BE
    Browser -->|WebSocket| SR
    BE --> SQL
    SR --> BE
```

---

## 8. SEQUENCE DIAGRAM: Luồng Đăng Nhập

```mermaid
sequenceDiagram
    actor U as Người dùng
    participant FE as Vue Frontend
    participant API as AuthController
    participant DB as SQL Server

    U->>FE: Nhập Email + Password
    FE->>API: POST /api/auth/login
    API->>DB: Tìm User theo Email
    DB-->>API: User record
    API->>API: Verify PasswordHash

    alt Thành công
        API->>DB: Lưu RefreshToken
        API-->>FE: 200 OK - JWT + User
        FE->>FE: Lưu token vào localStorage
        FE-->>U: Chuyển đến Dashboard
    else Sai mật khẩu
        API-->>FE: 401 Unauthorized
        FE-->>U: Hiển thị lỗi
    end
```

---

## 9. SEQUENCE DIAGRAM: Luồng Kéo Thả Kanban

```mermaid
sequenceDiagram
    actor U as Người dùng
    participant Board as KanbanBoard
    participant Store as Pinia Store
    participant API as WorkTasksController
    participant DB as SQL Server
    participant Hub as SignalR Hub

    U->>Board: Kéo Task sang cột mới
    Board->>Store: Optimistic Update UI
    Board->>API: PUT /worktasks/{id}/reorder
    API->>DB: UPDATE StatusName, SortOrder

    alt Thành công
        DB-->>API: 200 OK
        API->>Hub: Broadcast TaskUpdated
        Hub-->>Board: Real-time sync
    else Conflict
        DB-->>API: ConcurrencyException
        API-->>Board: 409 Conflict
        Board->>Store: Revert trạng thái cũ
        Board-->>U: Thông báo lỗi
    end
```

---

## 10. SEQUENCE DIAGRAM: Luồng Tạo Task + Bình Luận

```mermaid
sequenceDiagram
    actor U as Người dùng
    participant Modal as TaskDetailModal
    participant API as WorkTasksController
    participant CMT as CommentsController
    participant DB as SQL Server

    U->>Modal: Click Thêm công việc
    Modal->>API: POST /projects/{id}/WorkTasks
    API->>DB: INSERT WorkTask
    DB-->>API: Task mới
    API-->>Modal: 201 Created
    Modal-->>U: Mở chi tiết Task

    U->>Modal: Gõ bình luận
    Modal->>CMT: POST /worktasks/{id}/comments
    CMT->>DB: INSERT Comment
    CMT->>DB: INSERT Notification
    DB-->>CMT: OK
    CMT-->>Modal: 201 Created
    Modal-->>U: Hiển thị bình luận mới
```

---

## 11. SƠ ĐỒ TRẠNG THÁI TASK (State Diagram)

```mermaid
stateDiagram-v2
    [*] --> TODO : Task được tạo
    TODO --> IN_PROGRESS : Bắt đầu làm
    IN_PROGRESS --> IN_REVIEW : Gửi review
    IN_REVIEW --> DONE : Approve
    IN_REVIEW --> IN_PROGRESS : Yêu cầu sửa
    IN_PROGRESS --> TODO : Hoãn lại
    DONE --> [*]

    TODO --> CANCELLED : Huỷ
    IN_PROGRESS --> CANCELLED : Huỷ
    CANCELLED --> [*]
```

---

## 12. SƠ ĐỒ CLEAN ARCHITECTURE (Layer Diagram)

> **Clean Architecture là gì?**  
> Là cách tổ chức code backend thành **nhiều tầng (layer) tách biệt**, mỗi tầng có trách nhiệm riêng.  
> Mục đích: Khi thay đổi database (ví dụ từ SQL Server sang PostgreSQL), anh CHỈ cần sửa tầng Infrastructure mà không đụng tới code nghiệp vụ (Domain). Tương tự, khi đổi giao diện từ Vue sang React, chỉ sửa tầng Presentation.  
>
> **Ý nghĩa từng tầng trong dự án của anh:**  
> | Tầng | Vai trò | Thư mục trong dự án |
> |------|---------|--------------------|
> | **Presentation** | Giao diện người dùng nhìn thấy | `Frontend/src/` (Vue 3, Pinia) |
> | **API** | Nhận request HTTP, gọi logic xử lý | `TaskManagement.API/Controllers/` |
> | **Application** | Chuyển đổi dữ liệu (DTO), kiểm tra đầu vào | `TaskManagement.Application/` |
> | **Domain** | Quy tắc nghiệp vụ cốt lõi, Entity | `TaskManagement.Domain/Entities/` |
> | **Infrastructure** | Kết nối database, migrations | `TaskManagement.Infrastructure/` |
> | **Data Store** | Cơ sở dữ liệu thực tế | SQL Server |

```mermaid
graph TB
    L1["Presentation<br/>Vue 3 + Pinia + Element Plus"]
    L2["API Layer<br/>24 Controllers + Middleware + SignalR"]
    L3["Application Layer<br/>DTOs + Validation + Mapping"]
    L4["Domain Layer<br/>47 Entities + Business Rules"]
    L5["Infrastructure Layer<br/>DbContext + EF Core + Migrations"]
    L6[("SQL Server")]

    L1 -->|HTTP / WebSocket| L2
    L2 --> L3
    L3 --> L4
    L4 --> L5
    L5 --> L6
```

---

> **Ghi chú:**  
> - Tất cả sơ đồ đã được tối ưu để chạy đúng trên Mermaid Live Editor  
> - Sơ đồ phân rã chức năng đã tách thành 5 phần nhỏ để xuất PNG cho rõ chữ  
> - Sơ đồ Use Case tách 3 phần theo Actor để tránh chồng chéo  
> - Xuất PNG/SVG tại: https://mermaid.live
