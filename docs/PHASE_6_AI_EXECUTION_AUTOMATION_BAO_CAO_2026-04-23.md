# PHASE 6 - AI THUC THI CONG VIEC TU REPO VA DU LIEU THUC CHIEN

## 1. Muc tieu Phase 6

Phase 6 la buoc chuyen tu:

- AI chi **phan tich** repo
- AI chi **goi y** estimate/task

sang muc:

- AI co the **tao work items that su** trong project
- AI co the **phan ra task cha -> subtask**
- AI co the **goi y nguoi phu hop** dua tren velocity/accuracy/reward/lich su lam viec
- AI co the **ho tro PM/PO/SM chot backlog va cycle**

Noi ngan gon: Phase 5 la AI "nhin va de xuat", con Phase 6 la AI "bat tay vao tao va sap viec".

---

## 2. Gia tri sau khi xong Phase 6

Khi xong Phase 6, he thong se co cac gia tri lon sau:

1. Admin/PM/PO co the dua link GitHub vao project va nhan backlog goi y co cau truc.
2. Tu backlog do, AI co the tao work items that vao dung project.
3. AI co the tach tiep thanh subtask thay vi nguoi dung phai nhap tay tung muc.
4. He thong bat dau dung du lieu thuc chien da co:
   - estimate
   - actual/logged hours
   - velocity
   - accuracy
   - rewards
5. PM/SM co them cong cu de phan viec cong bang va nhanh hon.

---

## 3. Pham vi Phase 6

### 3.1. AI tao backlog that tu repo

Tu ket qua `repo analysis`, AI se cho phep:

- chon `quick wins`
- chon `medium tasks`
- chon `risky tasks`
- nhan nut tao thanh work items trong project

Moi work item duoc tao se co toi thieu:

- title
- description
- priority
- suggested estimate hours
- suggested story points neu co
- status mac dinh
- lien ket ve nguon tao boi AI

### 3.2. AI phan ra task cha thanh subtask

Sau khi co task cha, nguoi dung co the:

- bam `AI Breakdown`
- AI tao danh sach subtask
- nguoi dung xem truoc
- chon tao that vao project

Subtask phai:

- dung parent task
- co estimate rieng
- co description ro rang
- co kha nang tiep tuc gan assignee va log thoi gian nhu task thuong

### 3.3. AI goi y assignee

AI chua duoc tu y assign thang nguoi dung o Phase 6, nhung se goi y:

- ai phu hop nhat
- ly do goi y
- velocity gan day
- accuracy estimate
- tai hien tai cua tung nguoi

Quyen quyet dinh cuoi cung van la PM/PO/SM/Admin du an.

### 3.4. AI ho tro chot backlog/cycle

PM/PO/SM/Admin co the xem:

- danh sach task AI de xuat
- tong estimate
- tong story points
- nhom quick/medium/risky
- test plan

Sau do ho co the:

- chap nhan
- loai bo
- sua lai
- dua vao cycle moi

---

## 4. Quyen trong Phase 6

### Duoc dung AI tao viec

- System Admin
- Admin/PM/PO/SM cua project do

### Duoc xem ket qua AI nhung khong duoc tao hang loat

- Member co quyen truy cap project

### Khong duoc thao tac

- Nguoi khong co quyen vao project

---

## 5. Logic nghiep vu can chot

### 5.1. AI khong duoc tao thang vao DB ma khong co xac nhan

Mac dinh:

- AI phan tich
- AI dua preview
- nguoi dung xac nhan
- he thong moi tao work items that

### 5.2. AI tao task phai ton trong estimate da co

- neu task cha da co subtask thi estimate cha la roll-up
- AI khong duoc ghi de estimate cha bang tay sai logic
- AI chi duoc de xuat, service backend la noi quyet dinh hop le hay khong

### 5.3. AI goi y assignee chi la support

Khong cho AI tu y chot nguoi lam.

Ly do:

- tranh phan quyen sai
- tranh giao viec vo ly
- PM/SM phai la nguoi chot cuoi

### 5.4. AI phai co fallback

Neu Gemini loi:

- repo analysis van co fallback
- breakdown task cung nen co fallback co cau truc
- neu that su khong the tao noi dung AI thi phai thong bao ro va khong lam hong UI

---

## 6. Pham vi ky thuat can lam

### Backend

- mo rong `IAiService`
- mo rong `GeminiAiService`
- them endpoint tao work items tu repo analysis
- them endpoint preview breakdown -> confirm create
- them endpoint goi y assignee theo metric
- them luu vet task duoc tao boi AI

### Frontend

- AI page co khu `Create backlog items`
- task detail co khu `AI Breakdown`
- project/AI page co khu `Suggested assignees`
- UI preview truoc khi tao that
- UI phai giu style hien tai, khong pha giao dien goc

### Data/Logic

- tan dung du lieu:
  - WorkTasks
  - TaskAssignments
  - TimeLogs
  - PointTransactions
  - UserWallets
- can them metadata de biet task nao duoc tao boi AI

---

## 7. Chia Phase 6 thanh 4 dot nho

### Dot 6.1 - AI tao work items tu repo analysis

Lam truoc vi day la buoc de nhin thay gia tri ngay.

Ket qua mong doi:

- tu repo analysis co nut tao work items vao project
- tao theo nhom quick/medium/risky
- co preview truoc khi save

### Dot 6.2 - AI breakdown task cha thanh subtask

Ket qua mong doi:

- task detail co AI breakdown
- preview subtasks
- confirm create subtasks

### Dot 6.3 - AI goi y assignee theo du lieu thuc chien

Ket qua mong doi:

- hien top nguoi phu hop
- co ly do, velocity, accuracy, workload

### Dot 6.4 - AI ho tro PM/SM chot backlog/cycle

Ket qua mong doi:

- PM/SM xem tong estimate, tong risk
- chon nhom task dua vao cycle
- co test plan va checklist ban giao

---

## 8. Test case muc tieu cua Phase 6

### Nhom 1 - Repo -> Work items

- `TC61A`: tu repo analysis, bam tao quick wins -> sinh work items vao project
- `TC61B`: medium/risky tasks tao dung priority va estimate
- `TC61C`: task AI tao xuat hien ngay trong work list khong can F5

### Nhom 2 - Task -> Subtasks

- `TC62A`: task detail bam AI breakdown -> hien preview subtasks
- `TC62B`: confirm create -> subtasks tao dung parent
- `TC62C`: estimate parent van dung logic roll-up

### Nhom 3 - Assignee suggestion

- `TC63A`: he thong goi y nguoi phu hop dua tren velocity/accuracy
- `TC63B`: nguoi workload qua cao khong duoc xep top neu co lua chon tot hon
- `TC63C`: PM/SM co the bo qua goi y va tu chon nguoi khac

### Nhom 4 - PM/SM operational review

- `TC64A`: PM/SM xem duoc tong estimate/tong risk/tong test plan
- `TC64B`: chon nhieu task AI de xuat roi dua vao cycle
- `TC64C`: du lieu tao/sua dong bo sang project view ngay

---

## 9. Thu tu lam khuyen nghi

Thu tu nen lam:

1. Dot 6.1
2. Dot 6.2
3. Dot 6.3
4. Dot 6.4

Ly do:

- co task that truoc moi co du lieu de breakdown
- co task va du lieu thuc chien roi moi goi y assignee dung
- cuoi cung moi dua vao cong cu quan tri backlog/cycle

---

## 10. Ket luan chot Phase 6

Phase 6 da duoc chot theo huong:

- khong lam AI cho dep mat
- lam AI de tao gia tri thuc trong du an
- PM/PO/SM/Admin van la nguoi chot cuoi
- AI la tro ly phan tich, goi y va tao nhap nhanh

Huong code tiep theo:

- bat dau bang `Dot 6.1 - AI tao work items that tu repo analysis`

