# 6. Thiết kế Domain Model (Entities & Relationships)

Dựa trên các đặc tả nghiệp vụ, kiến trúc Backend (.NET Clean Architecture) của hệ thống sẽ được cấu trúc theo phương pháp Domain-Driven Design (DDD). Dưới đây là các Thực thể (Entities) cốt lõi và các mối quan hệ của chúng:

---

## 1. Phân hệ Identity & User (Người dùng)

### **User**
Đại diện cho tất cả người dùng trong hệ thống (Khách hàng, Nhân viên, Super Admin).
- `Id` (Guid)
- `Email` (string)
- `PasswordHash` (string)
- `FullName` (string)
- `PhoneNumber` (string)
- `Role` (Enum: `Customer`, `Staff`, `SuperAdmin`)
- `IsActive` (bool)
- `RefreshToken` (string?)
- `RefreshTokenExpiryTime` (DateTime?)
- `PasswordResetToken` (string?)
- `PasswordResetTokenExpiryTime` (DateTime?)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime)

---

## 2. Phân hệ Tài sản & Định danh (Onboarding)

### **CustomerDocument** (Giấy tờ định danh)
Lưu trữ thông tin giấy tờ do khách hàng tải lên. Chịu sự quản lý của AI Gatekeeper và OCR.
- `Id` (Guid)
- `UserId` (Guid) - *FK*
- `DocumentType` (Enum: `CCCD`, `DriverLicense`, `CarRegistration`)
- `ImageUrl` (string) - *Đường dẫn file trên MinIO*
- `MetadataJson` (jsonb) - *Dữ liệu chữ (Tên, Biển số...) do OCR bóc tách*
- `VerificationStatus` (Enum: `Valid`, `Invalid`)

### **Car** (Xe cơ giới)
Tài sản được bảo hiểm. Tự động sinh ra khi khách hàng tải lên Giấy đăng ký xe hợp lệ.
- `Id` (Guid)
- `UserId` (Guid) - *FK*
- `LicensePlate` (string) - *Biển số (Unique/UserId)*
- `Brand` (string) - *Hãng xe (VD: Toyota)*
- `Model` (string) - *Dòng xe (VD: Vios)*
- `ManufacturingYear` (int) - *Năm sản xuất*

---

## 3. Phân hệ Quản trị Sản phẩm (Product Catalog)

### **InsurancePackage** (Gói bảo hiểm)
Quản lý bởi Super Admin. Triển khai cơ chế *Soft-Delete/Khóa* để bảo vệ tính toàn vẹn.
- `Id` (Guid)
- `Name` (string) - *VD: Bảo hiểm Thân vỏ Cơ bản*
- `Description` (string)
- `IsActive` (bool) - *Khóa = false, khách mới không mua được nữa.*

### **PolicyTerm** (Điều khoản bảo hiểm)
Quản lý các tài liệu PDF điều khoản theo Version để AI Policy Agent đọc (RAG). Tích hợp cơ chế Human-in-the-loop để kiểm duyệt Text trước khi nhúng.
- `Id` (Guid)
- `PackageId` (Guid) - *FK*
- `Version` (string) - *VD: v1.0, v1.1*
- `PdfUrl` (string) - *File lưu ở MinIO*
- `ExtractedText` (string?) - *Văn bản OCR từ PDF, chờ admin duyệt và sửa lỗi.*
- `EmbeddingStatus` (Enum: `PendingReview`, `ApprovedAndEmbedded`) - *Trạng thái duyệt vector.*
- `QdrantCollectionName` (string) - *Tên bảng vector trong Qdrant để tra cứu*
- `IsCurrent` (bool) - *Là bản mới nhất?*

---

## 4. Phân hệ Hợp đồng (Policies)

### **InsurancePolicy** (Hợp đồng Bảo hiểm)
Hợp đồng ràng buộc giữa Khách hàng (User) và Tài sản (Car) theo 1 Gói cụ thể.
- `Id` (Guid)
- `UserId` (Guid) - *FK*
- `CarId` (Guid) - *FK (1 Xe chỉ có 1 Hợp đồng Active)*
- `PackageId` (Guid) - *FK*
- `PolicyTermId` (Guid) - *FK (Snapshot: Neo cứng vào đúng Version điều khoản lúc mua)*
- `Status` (Enum: `PendingApproval`, `Active`, `Expired`, `Canceled`)
- `StartDate` (DateTime)
- `EndDate` (DateTime)
- `EPolicyPdfUrl` (string) - *Bản mềm hợp đồng gửi cho khách*

---

## 5. Phân hệ Xử lý Bồi thường (Claims)

### **ClaimRequest** (Yêu cầu bồi thường)
Hồ sơ yêu cầu do khách hàng tạo ra khi gặp tai nạn.
- `Id` (Guid)
- `InsurancePolicyId` (Guid) - *FK*
- `IncidentDescription` (string) - *Lời khai sự cố*
- `Status` (Enum: `Pending`, `AI_Processing`, `AI_Evaluated`, `Approved`, `Rejected`)
- `AssignedStaffId` (Guid?) - *FK (Nhân viên được giao duyệt)*
- `StaffNote` (string) - *Lý do từ chối hoặc ghi chú duyệt*
- `CreatedAt` (DateTime)

### **ClaimEvidence** (Bằng chứng bồi thường)
Chứa các hình ảnh hiện trường hoặc hóa đơn. Dùng để đối chiếu chống gian lận chéo.
- `Id` (Guid)
- `ClaimRequestId` (Guid) - *FK*
- `EvidenceType` (Enum: `AccidentScene`, `RepairInvoice`)
- `ImageUrl` (string) - *Lưu ở MinIO*
- `ImageHash` (string) - *Mã pHash để truy quét xem ảnh này đã từng bị nộp ở Claim nào trước đây chưa (Cross-claim fraud)*
- `ExtractedData` (jsonb) - *Kết quả YOLOv8 (vết xước) hoặc OCR (tên gara)*

### **ClaimAiReport** (Báo cáo AI)
Báo cáo phân tích tổng hợp từ hệ thống đa tác tử (LangGraph).
- `Id` (Guid)
- `ClaimRequestId` (Guid) - *FK*
- `FraudAnalysis` (string) - *Đánh giá của Đặc vụ An ninh*
- `DamageAnalysis` (string) - *Đánh giá của Đặc vụ Giám định*
- `LogicAnalysis` (string) - *Đánh giá của Đặc vụ Thám tử*
- `PolicyMatched` (string) - *Luật đền bù trích từ Đặc vụ Pháp chế*
- `SuggestedStatus` (Enum: `Auto_Approve`, `Manual_Review`, `High_Risk`) - *Nhãn màu Xanh, Vàng, Đỏ*
- `FinalReportSummary` (string) - *Đoạn văn bản báo cáo cho nhân viên đọc*
