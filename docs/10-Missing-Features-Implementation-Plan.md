# Kế hoạch Bổ sung 2 API Còn Thiếu (Phase 5: Finalizing Epic 4)

Dựa trên tài liệu phân tích hệ thống (`05-System-Features.md`, `08-Task-Breakdown.md`) và tiêu chuẩn **Clean Architecture** (được quy định tại `.Agent/create-api-feature-skill.md`), dưới đây là kế hoạch chi tiết để vá 2 lỗ hổng tính năng cuối cùng của Epic 4.

---

## 1. Feature 1: Khách hàng Hủy Hợp Đồng (Cancel Policy)
*Khách hàng có quyền yêu cầu hủy hợp đồng bảo hiểm đang có hiệu lực trên App Mobile.*

**Endpoint:** `POST /api/v1/policies/{id}/cancel`
**Role:** `Customer`
**Vị trí thư mục:** `Backend.Application/Features/Policies/CancelPolicy/`

### 1.1. Application Layer (CQRS)
- **`CancelPolicyCommand.cs`**: 
  - Input: `Guid PolicyId`.
  - Output: `IRequest<Result<CancelPolicyResponse>>`.
- **`CancelPolicyResponse.cs`**: 
  - Fields: `Guid PolicyId`, `string Status`.
- **`CancelPolicyCommandHandler.cs`**:
  - Inject: `IInsurancePolicyRepository`, `IUnitOfWork`, `ICurrentUserService`.
  - **Logic:**
    1. Lấy `UserId` từ token.
    2. Gọi `_policyRepo.GetByIdAsync(request.PolicyId)`.
    3. Trả về `NotFound` nếu hợp đồng không tồn tại hoặc `policy.UserId != userId` (Bảo mật: không cho hủy hợp đồng người khác).
    4. Trả về `Conflict` nếu hợp đồng không ở trạng thái `Active`.
    5. Gọi Domain Method: `policy.CancelPolicy()`.
    6. `_unitOfWork.SaveChangesAsync()`.
    7. Trả về `CancelPolicyResponse`.

### 1.2. WebApi Layer (Controller)
- **Cập nhật `PoliciesController.cs`**:
  - Thêm endpoint `[HttpPost("{id:guid}/cancel")]`.
  - HTTP 200 OK nếu hủy thành công.
  - HTTP 404/409 xử lý theo `Result.Error.Code`.

---

## 2. Feature 2: Nhân viên Xem Chi tiết Hồ sơ Bồi thường
*Nhân viên cần xem chi tiết hồ sơ để thẩm định, bao gồm danh sách ảnh chụp hiện trường và bản Báo cáo Đánh giá của AI (AI Report).*

**Endpoint:** `GET /api/v1/staff/claims/{id}`
**Role:** `StaffAndAdmin`
**Vị trí thư mục:** `Backend.Application/Features/Staff/Claims/GetClaimById/`

### 2.1. Domain & Infrastructure Updates
- **`IClaimRepository.cs`**:
  - Hàm `GetByIdAsync` hiện tại trả về `ClaimRequest` nhưng chưa load dữ liệu của bảng `ClaimEvidences`.
  - **Action:** Bổ sung phương thức `GetEvidencesByClaimIdAsync(Guid claimId)` để lấy được List các ảnh bằng chứng.

### 2.2. Application Layer (CQRS)
- **`GetClaimByIdQuery.cs`**:
  - Input: `Guid ClaimId`.
  - Output: `IRequest<Result<GetClaimByIdResponse>>`.
- **`GetClaimByIdResponse.cs`**:
  - Fields: 
    - Thông tin chung: `Guid Id`, `Guid PolicyId`, `string IncidentDescription`, `string Status`, `DateTime CreatedAt`.
    - Danh sách bằng chứng: `List<EvidenceDto> Evidences` (gồm `ImageUrl`, `EvidenceType`).
    - Ghi chú: `string? StaffNote`, `Guid? AssignedStaffId`.
- **`GetClaimByIdQueryHandler.cs`**:
  - Inject: `IClaimRepository`.
  - **Logic:**
    1. Gọi `_claimRepo.GetByIdAsync(request.ClaimId)`. Trả về NotFound nếu null.
    2. Gọi `_claimRepo.GetEvidencesByClaimIdAsync(request.ClaimId)` để lấy ảnh.
    3. Gán dữ liệu thủ công (Manual Mapping - theo **RULE G2**) sang `GetClaimByIdResponse` và trả về.

### 2.3. WebApi Layer (Controller)
- **Cập nhật `StaffController.cs`**:
  - Thêm endpoint `[HttpGet("claims/{id:guid}")]`.
  - Trả về chi tiết hồ sơ để UI của Nhân viên có thể vẽ ra danh sách ảnh và thông tin.

---

## 3. Thứ tự Triển khai (Execution Order)
1. **Bước 1:** Cập nhật Repository (thêm tính năng lấy Evidences).
2. **Bước 2:** Code trọn gói Feature 1 (CancelPolicy) từ Application ra đến Controller.
3. **Bước 3:** Code trọn gói Feature 2 (GetClaimById) từ Application ra đến Controller.

✅ Mọi API cam kết tuân thủ tuyệt đối quy định Response chuẩn `ApiResponse<T>`, không sử dụng AutoMapper, và xử lý Exception qua Global Middleware như tài liệu `.Agent` đã hướng dẫn.
