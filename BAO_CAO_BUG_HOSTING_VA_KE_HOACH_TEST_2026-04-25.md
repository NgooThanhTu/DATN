# Báo cáo bug sau khi host public và kế hoạch test vòng

Ngày lập: 25/04/2026  
Môi trường test: `https://sprinta.io.vn` và `https://api.sprinta.io.vn`  
Mục tiêu: tổng hợp bug bạn test từ hosting, chia người phụ trách sửa, và thống nhất cách test theo vòng cho đến khi hết bug.

---

## 1. Nguyên tắc làm việc theo vòng

1. Mỗi người chỉ nhận các bug được phân công trong bảng bên dưới.
2. Sửa xong bug nào thì tự test nhanh tại máy local trước.
3. Sau khi local ổn thì build/publish lên hosting test.
4. Tester ghi kết quả theo mẫu:

```text
BUG-ID:
Kết quả: PASS / FAIL / HALF PASS
Ghi chú:
Ảnh hoặc log:
```

5. Bug nào `PASS` thì xóa khỏi danh sách bug active hoặc chuyển xuống mục `Đã pass`.
6. Bug nào `FAIL` thì giữ lại, bổ sung ghi chú vòng mới, không tạo bug trùng.
7. Mỗi vòng test chỉ tập trung các bug còn active để tránh test lan man.

---

## 2. Phân công đề xuất

| Người phụ trách | Nhóm việc chính | Lý do |
|---|---|---|
| Danh | Profile, avatar/cover, upload ảnh, notification, UI/UX nhỏ | Liên quan nhiều tới giao diện người dùng và upload file |
| Khôi | Dashboard, Drafts, Your Work, date/time, assignee hiển thị trùng | Liên quan nhiều tới workflow task và dữ liệu hiển thị |
| Tú | Views, Analytics, Archives, Spaces, archive/private/public project | Liên quan tới lọc, tổng hợp dữ liệu và trạng thái project |

Ghi chú: AI/Gemini token là vấn đề cấu hình API key/quota, không nên tính là bug FE nếu backend trả lỗi quota/key hợp lệ.

---

## 3. Danh sách bug active

### Nhóm A - Profile, avatar, cover

| Bug ID | Module | Mô tả bug | Kỳ vọng | Người phụ trách | Ưu tiên | Trạng thái |
|---|---|---|---|---|---|---|
| BUG-HOST-001 | Profile | Không căn chỉnh được ảnh đại diện/ảnh bìa | Có thể căn chỉnh hoặc crop ảnh trước/sau khi upload | Danh | High | Active |
| BUG-HOST-002 | Profile | Không cho upload ảnh đại diện quá 5MB nhưng thông báo/quy tắc chưa rõ | Có validate dung lượng rõ ràng, báo lỗi dễ hiểu | Danh | Medium | Active |
| BUG-HOST-003 | Profile | Không upload được ảnh đại diện và ảnh bìa | Upload thành công, reload vẫn hiển thị đúng ảnh | Danh | High | Active |
| BUG-HOST-004 | Layout | Avatar hiển thị góc phải không đồng đều | Avatar topbar/profile thống nhất kích thước, bo góc, fallback chữ | Danh | Medium | Active |

### Nhóm B - Drafts và tạo work item

| Bug ID | Module | Mô tả bug | Kỳ vọng | Người phụ trách | Ưu tiên | Trạng thái |
|---|---|---|---|---|---|---|
| BUG-HOST-005 | Drafts | Tạo draft chỉ assign được 1 người và chỉ update được | Nếu nghiệp vụ cho multi-assignee thì chọn được nhiều; nếu chỉ 1 người thì UI phải rõ | Khôi | High | Active |
| BUG-HOST-006 | Drafts | Draft tự động chuyển project khi chỉ có 1 project | Không tự chuyển sai context; hiển thị project đang chọn rõ ràng | Khôi | High | Active |
| BUG-HOST-007 | Drafts | Labels chưa rõ tạo như thế nào | Có nút tạo label hoặc hướng dẫn UI rõ trong modal | Khôi | Medium | Active |
| BUG-HOST-008 | Drafts | Start day/end day đang hiển thị ngược ngày tháng năm | Format ngày thống nhất `dd/MM/yyyy` | Khôi | High | Active |
| BUG-HOST-009 | Drafts | Nút `Create more` không hoạt động | Tạo task xong giữ modal/form để tạo tiếp | Khôi | Medium | Active |
| BUG-HOST-010 | Drafts | Vị trí trường ngày bắt đầu/kết thúc chưa hợp lý | Đặt ngay vùng thông tin ngày bắt đầu và ngày kết thúc, dễ nhìn | Khôi | Low | Active |

### Nhóm C - Dashboard

| Bug ID | Module | Mô tả bug | Kỳ vọng | Người phụ trách | Ưu tiên | Trạng thái |
|---|---|---|---|---|---|---|
| BUG-HOST-011 | Dashboard | `New work item` chỉ hiện cố định 4 status | Status phải lấy theo cấu hình project/task status, không hard-code | Khôi | High | Active |
| BUG-HOST-012 | Dashboard | ID task dựa theo ký tự người dùng nhập, có nguy cơ trùng | ID phải có cơ chế chống trùng hoặc backend tự sinh/validate | Khôi | High | Active |
| BUG-HOST-013 | Dashboard | Chưa rõ làm sao để có thông báo | Có workflow tạo notification khi task/project thay đổi | Danh | Medium | Active |
| BUG-HOST-014 | Dashboard | Chỉnh sáng/tối làm thanh tìm kiếm trên top bị di chuyển | Toggle theme không làm layout topbar lệch | Danh | Medium | Active |
| BUG-HOST-015 | Dashboard | Chức năng switch account chưa hoạt động | Switch account đổi phiên/tài khoản đúng hoặc ẩn nếu chưa hỗ trợ | Danh | High | Active |

Các chức năng dashboard đã hoạt động: yêu thích, tìm kiếm, open projects.

### Nhóm D - Your Work

| Bug ID | Module | Mô tả bug | Kỳ vọng | Người phụ trách | Ưu tiên | Trạng thái |
|---|---|---|---|---|---|---|
| BUG-HOST-016 | Your Work | Chưa assign nhưng vẫn hiển thị số task đã tạo | Chỉ thống kê đúng task liên quan tới user theo rule đã định | Khôi | High | Active |
| BUG-HOST-017 | Your Work | Recent activity sai ngày tháng năm và sai giờ thực | Hiển thị đúng timezone Việt Nam hoặc format thống nhất | Khôi | High | Active |
| BUG-HOST-018 | Your Work | `Created` chưa rõ là task mình tạo hay người khác tạo cho mình | Label rõ nghĩa: `Tôi tạo`, `Được giao`, hoặc `Người tạo: ...` | Khôi | Medium | Active |
| BUG-HOST-019 | Your Work | Có dữ liệu test lạ như `alo j Cường` | Xóa seed/test data hoặc lọc dữ liệu rác khỏi production | Khôi | Medium | Active |

Các chức năng đã hoạt động: hiển thị đúng task tạo, cập nhật status.

### Nhóm E - Stickies

| Bug ID | Module | Mô tả bug | Kỳ vọng | Người phụ trách | Ưu tiên | Trạng thái |
|---|---|---|---|---|---|---|
| BUG-HOST-020 | Stickies | Chưa rõ cách lưu stickies | Sticky phải tự lưu hoặc có nút save rõ ràng | Danh | Medium | Active |
| BUG-HOST-021 | Stickies | Chưa có search stickies | Có ô tìm kiếm sticky theo nội dung | Danh | Low | Active |

Các chức năng đã hoạt động: đổi màu nền, in đậm, nghiêng, căn trái/giữa/phải, xóa.

### Nhóm F - Rewards

| Bug ID | Module | Mô tả bug | Kỳ vọng | Người phụ trách | Ưu tiên | Trạng thái |
|---|---|---|---|---|---|---|
| BUG-HOST-022 | Rewards | Giao diện quá nhiều chữ, nửa Việt nửa Anh | Chuẩn hóa ngôn ngữ, ưu tiên tiếng Việt hoặc thống nhất một ngôn ngữ | Danh | Medium | Active |
| BUG-HOST-023 | Rewards | Level progress không thể hiện thanh phần trăm rõ ràng | Thanh progress hiển thị % và tiến độ trực quan | Danh | Medium | Active |

Các chức năng đã hoạt động: quy trình điểm đúng, đổi mức độ bởi người khác không làm thay đổi điểm sai.

### Nhóm G - AI

| Bug ID | Module | Mô tả bug | Kỳ vọng | Người phụ trách | Ưu tiên | Trạng thái |
|---|---|---|---|---|---|---|
| BUG-HOST-024 | AI | Hết token nhanh khi chat/tạo task | Có thông báo quota rõ ràng; thay API key/quota nếu cần test tiếp | Danh | Medium | Active |

Ghi chú: nếu lỗi là `Gemini API 403 key leaked` hoặc `429 quota exceeded` thì cần xử lý cấu hình key/quota backend, không phải UI bug chính.

### Nhóm H - Spaces

| Bug ID | Module | Mô tả bug | Kỳ vọng | Người phụ trách | Ưu tiên | Trạng thái |
|---|---|---|---|---|---|---|
| BUG-HOST-025 | Spaces | Project private/public không có thông báo rõ khi user khác mở link | Hiển thị thông báo quyền truy cập rõ ràng: private/public/not member/login required | Tú | High | Active |
| BUG-HOST-026 | Spaces | Sau khi login bằng link private/public mà không thấy gì, không rõ đúng hay sai | Có màn hình/empty state nói lý do không thấy project | Tú | High | Active |
| BUG-HOST-027 | Spaces | Lọc project mới/cũ nhưng không biết project tạo khi nào | Hiển thị created date hoặc sort indicator | Tú | Medium | Active |
| BUG-HOST-028 | Spaces | Chưa rõ cách archive/lưu trữ project | Có nút archive hoặc menu hành động rõ ràng theo quyền | Tú | High | Active |

Các chức năng đã hoạt động: tạo project, copy link, lọc, search, filter dự án mới/cũ.

### Nhóm I - Views

| Bug ID | Module | Mô tả bug | Kỳ vọng | Người phụ trách | Ưu tiên | Trạng thái |
|---|---|---|---|---|---|---|
| BUG-HOST-029 | Views | Archived projects chưa có project archive nên không test được | Sau khi archive project thì filter archived projects phải có dữ liệu | Tú | Medium | Active |
| BUG-HOST-030 | Views | Group by và Order by chưa rõ cách test | Có dữ liệu test và tiêu chí rõ: group/order thay đổi danh sách đúng | Tú | Medium | Active |
| BUG-HOST-031 | Views | Show sub-work items chưa rõ cách test | Tạo task cha/con rồi bật/tắt phải thấy khác biệt rõ | Tú | Medium | Active |

Các chức năng đã hoạt động: lọc all projects, my projects, status, display properties gồm ID, Assignee, Start date, Due date, Labels, Priority, State, Estimate, Module, Cycle.

### Nhóm J - Analytics

| Bug ID | Module | Mô tả bug | Kỳ vọng | Người phụ trách | Ưu tiên | Trạng thái |
|---|---|---|---|---|---|---|
| BUG-HOST-032 | Analytics | Overview và Work items luôn hiện all project dù đã lọc project | Analytics phải tính đúng theo project được chọn | Tú | High | Active |

### Nhóm K - Archives

| Bug ID | Module | Mô tả bug | Kỳ vọng | Người phụ trách | Ưu tiên | Trạng thái |
|---|---|---|---|---|---|---|
| BUG-HOST-033 | Archives | Không hoạt động vì chưa có chức năng archive project | Có workflow archive project và danh sách archive hiển thị dữ liệu | Tú | High | Active |

### Nhóm L - Assignee

| Bug ID | Module | Mô tả bug | Kỳ vọng | Người phụ trách | Ưu tiên | Trạng thái |
|---|---|---|---|---|---|---|
| BUG-HOST-034 | Task Detail | Assignee trong chi tiết task hiển thị 2 dòng cho cùng một tài khoản: `T` và `Tú ngô` | Chỉ hiển thị một option/user duy nhất, ưu tiên tên đầy đủ kèm avatar | Khôi | High | Active |

---

## 4. Test case theo module

### TC-HOST-001 - Profile upload avatar/cover

1. Đăng nhập tài khoản thường.
2. Vào Profile.
3. Upload ảnh avatar nhỏ hơn 5MB.
4. Upload ảnh bìa nhỏ hơn 5MB.
5. Reload trang.
6. Kiểm tra avatar topbar, avatar profile, cover profile.

Kỳ vọng:
- Upload thành công.
- Ảnh không mất sau reload.
- Avatar hiển thị đồng đều ở topbar và profile.
- Nếu ảnh lớn hơn 5MB thì báo lỗi rõ ràng.

### TC-HOST-002 - Draft tạo task

1. Vào Drafts.
2. Tạo draft với status, priority, assignee, start day, end day, module, cycle.
3. Thử thêm label.
4. Bấm `Create more`.
5. Reload trang.

Kỳ vọng:
- Ngày hiển thị đúng `dd/MM/yyyy`.
- `Create more` hoạt động đúng.
- Label có thể tạo/chọn rõ ràng.
- Draft không tự đổi project sai.

### TC-HOST-003 - Dashboard work item/status

1. Vào một project có nhiều hơn 4 status.
2. Vào Dashboard.
3. Bấm tạo `New work item`.
4. Kiểm tra danh sách status.
5. Tạo task với ID trùng hoặc giống task cũ.

Kỳ vọng:
- Status lấy theo cấu hình project, không cố định 4 cái.
- ID không được trùng hoặc backend tự sinh mã an toàn.
- UI báo lỗi rõ nếu ID không hợp lệ.

### TC-HOST-004 - Theme và topbar

1. Vào Dashboard.
2. Bật/tắt dark mode/light mode nhiều lần.
3. Quan sát thanh tìm kiếm/topbar.

Kỳ vọng:
- Topbar không bị lệch.
- Search box không nhảy vị trí.

### TC-HOST-005 - Your Work

1. Tạo task chưa assign cho user hiện tại.
2. Tạo task assign cho user hiện tại.
3. Vào Your Work.
4. Kiểm tra số task, danh sách task, recent activity.

Kỳ vọng:
- Chỉ task liên quan mới được tính.
- Ngày giờ recent activity đúng timezone.
- Label `Created` rõ nghĩa.
- Không còn dữ liệu rác production.

### TC-HOST-006 - Stickies

1. Vào Stickies.
2. Tạo sticky mới.
3. Nhập nội dung.
4. Đổi màu/in đậm/in nghiêng/căn lề.
5. Reload trang.
6. Search nội dung sticky.

Kỳ vọng:
- Sticky được lưu.
- Search tìm được sticky.
- Format không mất sau reload.

### TC-HOST-007 - Rewards

1. Vào Rewards.
2. Hoàn thành task hoặc kiểm tra điểm hiện tại.
3. Quan sát level progress.

Kỳ vọng:
- Ngôn ngữ thống nhất.
- Thanh progress có % rõ ràng.
- Không quá nhiều mô tả gây rối.

### TC-HOST-008 - Spaces private/public

1. Tạo project private.
2. Copy link gửi tài khoản khác chưa là member.
3. Mở link khi chưa đăng nhập.
4. Đăng nhập tài khoản khác.
5. Lặp lại với project public.

Kỳ vọng:
- Nếu không có quyền thì báo rõ không có quyền.
- Nếu cần login thì redirect login rồi quay lại đúng context.
- Không hiển thị màn hình trống khó hiểu.

### TC-HOST-009 - Archive project

1. Vào Spaces.
2. Chọn một project.
3. Archive project.
4. Vào Archives.
5. Vào Views và chọn archived projects.

Kỳ vọng:
- Project archive được.
- Archives có dữ liệu.
- Views filter archived projects có kết quả đúng.

### TC-HOST-010 - Views filter/group/order/sub-items

1. Tạo dữ liệu gồm task cha và sub-task.
2. Tạo nhiều task khác status, priority, assignee.
3. Vào Views.
4. Test filter project/status.
5. Test Group by.
6. Test Order by.
7. Test Show sub-work items.

Kỳ vọng:
- Filter đúng dữ liệu.
- Group by nhóm đúng.
- Order by sắp xếp đúng.
- Bật/tắt sub-work items có thay đổi rõ.

### TC-HOST-011 - Analytics theo project

1. Vào Analytics.
2. Chọn một project cụ thể.
3. Kiểm tra Overview.
4. Kiểm tra Work items.
5. Đổi sang project khác.

Kỳ vọng:
- Overview tính đúng project đã chọn.
- Work items lọc đúng project đã chọn.
- Không tự hiện all projects khi đã chọn filter.

### TC-HOST-012 - Assignee trùng option

1. Tạo user có tên `Tú Ngô` hoặc tương tự.
2. Assign task trực tiếp ngoài list.
3. Mở chi tiết task.
4. Bấm assignee.

Kỳ vọng:
- Chỉ có một option cho user đó.
- Không hiển thị riêng avatar chữ `T` như một user khác.
- Chọn assignee lưu đúng user.

---

## 5. Mẫu báo cáo vòng test

### Vòng test 1

```text
BUG-HOST-001:
Kết quả:
Ghi chú:

BUG-HOST-002:
Kết quả:
Ghi chú:

BUG-HOST-003:
Kết quả:
Ghi chú:
```

### Quy tắc cập nhật sau mỗi vòng

- Nếu `PASS`: chuyển bug xuống mục `Đã pass` hoặc xóa khỏi danh sách active.
- Nếu `HALF PASS`: ghi rõ còn thiếu bước nào, giữ lại bug.
- Nếu `FAIL`: ghi log/ảnh và giữ nguyên bug.
- Không đổi ID bug giữa các vòng.
- Không tạo bug mới nếu chỉ là cùng lỗi cũ biểu hiện ở màn khác; ghi thêm module liên quan vào bug cũ.

---

## 6. Các bug đã pass

Chưa có dữ liệu. Sau vòng test đầu, chuyển các bug pass xuống đây.

---

## 7. Chia việc theo từng người

Phần này là bảng điều phối chính. Khi giao việc cho từng người, ưu tiên gửi đúng phần của người đó để tránh sửa chồng chéo.

### 7.1. Nguyễn Thành Danh

Phạm vi chính:
- Profile, avatar, ảnh bìa, upload ảnh.
- Notification và switch account.
- Stickies.
- Rewards.
- AI UI/thông báo lỗi quota hoặc API key.
- Các lỗi UI nhỏ như theme làm lệch topbar.

Danh sách bug Danh phụ trách:

| Bug ID | Module | Việc cần sửa | Ưu tiên |
|---|---|---|---|
| BUG-HOST-001 | Profile | Sửa căn chỉnh/crop ảnh đại diện và ảnh bìa | High |
| BUG-HOST-002 | Profile | Validate ảnh quá 5MB và báo lỗi rõ ràng | Medium |
| BUG-HOST-003 | Profile | Sửa upload avatar và cover không thành công | High |
| BUG-HOST-004 | Layout | Đồng bộ avatar ở topbar/profile | Medium |
| BUG-HOST-013 | Dashboard | Làm rõ workflow thông báo | Medium |
| BUG-HOST-014 | Dashboard | Sửa theme sáng/tối làm lệch search topbar | Medium |
| BUG-HOST-015 | Dashboard | Sửa hoặc ẩn switch account nếu chưa hỗ trợ | High |
| BUG-HOST-020 | Stickies | Lưu stickies sau reload | Medium |
| BUG-HOST-021 | Stickies | Search stickies theo nội dung | Low |
| BUG-HOST-022 | Rewards | Chuẩn hóa ngôn ngữ Việt/Anh | Medium |
| BUG-HOST-023 | Rewards | Làm rõ thanh level progress và phần trăm | Medium |
| BUG-HOST-024 | AI | Thông báo quota/API key rõ ràng, không để lỗi thô | Medium |

Không nên đụng:
- Logic filter Analytics/Views/Archives.
- Backend phân quyền project nếu không cần cho bug được giao.
- File host như `Frontend/.env`, `Frontend/public/web.config`, `Backend/src/TaskManagement.API/Program.cs`, `Backend/src/TaskManagement.API/appsettings.json` nếu không được Cường xác nhận.

Prompt mẫu cho AI của Danh:

```text
Bạn đang sửa dự án SprintA. Chỉ tập trung các bug UI/Profile/Stickies/Rewards/Notification sau:
BUG-HOST-001, BUG-HOST-002, BUG-HOST-003, BUG-HOST-004, BUG-HOST-013, BUG-HOST-014, BUG-HOST-015, BUG-HOST-020, BUG-HOST-021, BUG-HOST-022, BUG-HOST-023, BUG-HOST-024.

Yêu cầu:
- Đọc code trước khi sửa.
- Không đụng file host/deploy: Frontend/.env, Frontend/public/web.config, Backend/src/TaskManagement.API/Program.cs, Backend/src/TaskManagement.API/appsettings.json.
- Không refactor lan rộng.
- Sau mỗi bug, ghi rõ file đã sửa và cách test.
- Chạy npm run build sau khi sửa frontend.

Luồng test cần pass:
- Upload avatar/cover nhỏ hơn 5MB thành công, reload không mất ảnh.
- Ảnh lớn hơn 5MB báo lỗi rõ ràng.
- Avatar topbar/profile hiển thị đồng đều.
- Toggle theme không làm lệch thanh search.
- Stickies lưu được sau reload và search được.
- Rewards thống nhất ngôn ngữ và progress hiển thị rõ.
- AI hiển thị lỗi quota/key bằng thông báo thân thiện.
```

Mẫu báo cáo vòng test cho Danh:

```text
Nguyễn Thành Danh - Vòng test:

BUG-HOST-001:
Kết quả:
Ghi chú:

BUG-HOST-002:
Kết quả:
Ghi chú:

BUG-HOST-003:
Kết quả:
Ghi chú:
```

### 7.2. Đinh Tuấn Khôi

Phạm vi chính:
- Dashboard work item.
- Drafts.
- Your Work.
- Date/time format.
- Assignee và task detail.

Danh sách bug Khôi phụ trách:

| Bug ID | Module | Việc cần sửa | Ưu tiên |
|---|---|---|---|
| BUG-HOST-005 | Drafts | Xác định và sửa logic assign trong draft | High |
| BUG-HOST-006 | Drafts | Sửa draft tự chuyển project sai context | High |
| BUG-HOST-007 | Drafts | Làm rõ tạo/chọn labels trong draft | Medium |
| BUG-HOST-008 | Drafts | Sửa ngày bị ngược ở start/end day | High |
| BUG-HOST-009 | Drafts | Sửa nút Create more | Medium |
| BUG-HOST-010 | Drafts | Cải thiện vị trí trường ngày | Low |
| BUG-HOST-011 | Dashboard | Status trong New work item phải lấy động theo project | High |
| BUG-HOST-012 | Dashboard | Chống trùng ID task hoặc validate ID | High |
| BUG-HOST-016 | Your Work | Sửa thống kê task chưa assign | High |
| BUG-HOST-017 | Your Work | Sửa recent activity sai ngày giờ | High |
| BUG-HOST-018 | Your Work | Làm rõ label Created | Medium |
| BUG-HOST-019 | Your Work | Xóa/lọc dữ liệu test rác | Medium |
| BUG-HOST-034 | Task Detail | Sửa assignee hiện trùng `T` và `Tú Ngô` | High |

Không nên đụng:
- Analytics/Views/Archives nếu không liên quan task detail.
- Gemini/API key.
- File host/deploy.

Prompt mẫu cho AI của Khôi:

```text
Bạn đang sửa dự án SprintA. Chỉ tập trung bug Dashboard/Drafts/Your Work/Task Detail:
BUG-HOST-005 đến BUG-HOST-012, BUG-HOST-016 đến BUG-HOST-019, BUG-HOST-034.

Yêu cầu:
- Đọc các store/component liên quan trước khi sửa.
- Không hard-code status. Status phải lấy theo project/task status từ API hoặc store hiện có.
- Ngày giờ hiển thị thống nhất dd/MM/yyyy và đúng timezone Việt Nam nếu có giờ.
- Assignee trong Task Detail không được duplicate cùng một user. Chỉ hiển thị một option/user, ưu tiên tên đầy đủ kèm avatar.
- Không đụng file host/deploy.
- Sau khi sửa chạy npm run build.

Luồng test cần pass:
- Draft tạo được với status/priority/assignee/date/module/cycle.
- Create more hoạt động.
- Start day/end day không bị đảo ngày tháng.
- New work item dùng status động theo project.
- Your Work chỉ tính đúng task liên quan user.
- Recent activity đúng ngày giờ.
- Assignee không còn hiện 2 option cho cùng user.
```

Mẫu báo cáo vòng test cho Khôi:

```text
Đinh Tuấn Khôi - Vòng test:

BUG-HOST-005:
Kết quả:
Ghi chú:

BUG-HOST-006:
Kết quả:
Ghi chú:

BUG-HOST-034:
Kết quả:
Ghi chú:
```

### 7.3. Ngô Thanh Tú

Phạm vi chính:
- Spaces.
- Views.
- Analytics.
- Archives.
- Project public/private/archive/filter.

Danh sách bug Tú phụ trách:

| Bug ID | Module | Việc cần sửa | Ưu tiên |
|---|---|---|---|
| BUG-HOST-025 | Spaces | Thông báo rõ khi mở link private/public không đủ quyền | High |
| BUG-HOST-026 | Spaces | Empty state rõ khi login xong nhưng không thấy project | High |
| BUG-HOST-027 | Spaces | Hiển thị ngày tạo project khi lọc mới/cũ | Medium |
| BUG-HOST-028 | Spaces | Bổ sung hoặc làm rõ archive project | High |
| BUG-HOST-029 | Views | Archived projects có dữ liệu sau khi archive | Medium |
| BUG-HOST-030 | Views | Làm rõ và sửa Group by/Order by nếu sai | Medium |
| BUG-HOST-031 | Views | Làm rõ Show sub-work items | Medium |
| BUG-HOST-032 | Analytics | Overview và Work items phải lọc đúng project được chọn | High |
| BUG-HOST-033 | Archives | Archives hoạt động sau khi có archive project | High |

Không nên đụng:
- Profile/avatar/upload.
- Drafts/task detail assignee.
- Gemini/API key.
- File host/deploy.

Prompt mẫu cho AI của Tú:

```text
Bạn đang sửa dự án SprintA. Chỉ tập trung bug Spaces/Views/Analytics/Archives:
BUG-HOST-025 đến BUG-HOST-033.

Yêu cầu:
- Đọc logic project store/API trước khi sửa.
- Không đổi file host/deploy.
- Không refactor toàn bộ layout.
- Project private/public phải có thông báo rõ khi user không có quyền hoặc chưa login.
- Archive project phải có workflow rõ: archive được, xem trong Archives được, Views filter archived projects có dữ liệu.
- Analytics phải tính theo project filter đã chọn, không luôn dùng all projects.
- Group by, Order by, Show sub-work items phải có dữ liệu test và hành vi rõ.
- Sau khi sửa chạy npm run build.

Luồng test cần pass:
- User khác mở link private/public nhận thông báo đúng.
- Project hiển thị ngày tạo khi lọc mới/cũ.
- Archive project xong thấy trong Archives.
- Views filter archived projects có dữ liệu.
- Analytics đổi project filter thì Overview và Work items đổi đúng.
- Show sub-work items bật/tắt có khác biệt rõ.
```

Mẫu báo cáo vòng test cho Tú:

```text
Ngô Thanh Tú - Vòng test:

BUG-HOST-025:
Kết quả:
Ghi chú:

BUG-HOST-028:
Kết quả:
Ghi chú:

BUG-HOST-032:
Kết quả:
Ghi chú:
```

---

## 8. Cách làm với AI để tránh sửa sai

Khi mỗi bạn dùng AI để sửa, hãy gửi kèm đúng bug ID và phạm vi. Không gửi toàn bộ danh sách bug cho một người nếu người đó chỉ sửa một nhóm nhỏ.

Checklist bắt buộc trước khi AI sửa:

1. Nói rõ bug ID cần sửa.
2. Nói rõ module/màn hình.
3. Nói rõ file không được đụng.
4. Yêu cầu AI đọc code trước, không đoán.
5. Yêu cầu AI ghi file đã sửa.
6. Yêu cầu AI đưa test case ngắn sau khi sửa.
7. Chạy build trước khi báo pass.

Prompt tổng quát dùng cho mọi người:

```text
Bạn đang làm trong repo SprintA. Hãy sửa đúng bug ID được giao, không sửa lan sang module khác.

Bug cần sửa:
- BUG-HOST-...

Quy tắc:
- Đọc code liên quan trước khi sửa.
- Không đụng file host/deploy: Frontend/.env, Frontend/public/web.config, Backend/src/TaskManagement.API/Program.cs, Backend/src/TaskManagement.API/appsettings.json.
- Không xóa logic cũ nếu không chứng minh được nó sai.
- Không refactor lớn.
- Mỗi bug sửa xong phải ghi:
  - File đã sửa
  - Logic đã đổi
  - Cách test
  - Rủi ro còn lại
- Nếu sửa frontend thì chạy npm run build.
- Nếu sửa backend thì chạy dotnet build/publish nếu môi trường cho phép.
```

Mẫu gửi kết quả sau khi AI sửa:

```text
Tên người sửa:
Bug đã sửa:
File đã sửa:
Đã build chưa:
Kết quả local:
Ghi chú cần tester kiểm tra:
```

