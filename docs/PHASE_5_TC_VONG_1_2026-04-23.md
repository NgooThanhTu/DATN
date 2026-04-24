# PHASE 5 - TEST CASE VONG 1

## Muc tieu
Kiem tra 2 nghiep vu AI dau tien da duoc noi vao task detail:
- AI suggest estimate
- AI split task thanh sub-work items

## Luu y truoc khi test
- Backend phai dang chay va da cau hinh Gemini API key neu muon test AI that.
- Neu Gemini tam loi/quota timeout, he thong se tra fallback message.
- Nen test tren 1 task co:
  - title ro rang
  - description ro rang
  - co priority/story point de AI goi y hop ly hon

## Thu tu test

### TC56 - AI suggest estimate tra ve goi y
1. Mo `Task detail` cua 1 task.
2. Tim khu `Estimate`.
3. Bam nut `AI suggest`.

Ky vong:
- Hien loading `AI is thinking...`
- Sau do hien panel `AI estimate suggestion`
- Panel co cac thong tin:
  - `Suggested hours`
  - `Suggested story points`
  - `Complexity / days`
  - `Reasoning`

### TC57 - Apply AI suggestion cap nhat task
1. Sau khi TC56 co ket qua, bam `Apply AI suggestion`.
2. Kiem tra lai trong task detail:
  - estimate
  - story points

Ky vong:
- Estimate cua task duoc doi theo AI suggestion
- Story points duoc doi theo AI suggestion
- Luu lai dung, khong tu reset ve 0

### TC58 - AI split into subtasks hoat dong
1. Trong `Task detail`, bam `AI split into subtasks`.
2. Cho AI tao subtask.

Ky vong:
- Tao duoc danh sach sub-work items moi
- Subtask moi hien ngay trong khu subtask
- Moi subtask co title/description/estimate co ban

### TC59 - Fallback khi AI loi van than thien
1. Test trong truong hop Gemini loi hoac het quota.
2. Bam `AI suggest` hoac `AI split into subtasks`.

Ky vong:
- Khong vo trang trang
- Khong crash task detail
- Hien thong bao loi/fallback de user biet va thao tac tiep

## Cach bao lai
Gui lai cho minh theo format:

- `TC56: Pass/Fail + ghi chu`
- `TC57: Pass/Fail + ghi chu`
- `TC58: Pass/Fail + ghi chu`
- `TC59: Pass/Fail + ghi chu`
