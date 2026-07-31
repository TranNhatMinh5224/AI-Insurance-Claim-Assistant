# 8. Bảng Đặc tả Công việc Chi tiết (Task Breakdown & Specs)

Tài liệu này đóng vai trò như một **Backlog (Kanban Board)** chia nhỏ toàn bộ khối lượng công việc còn lại của dự án thành các Task cụ thể, kèm theo mô tả (Specs) rõ ràng để quá trình code diễn ra trơn tru.

---

## Epic 1: Hoàn thiện Tầng Cốt lõi (Backend Domain Layer)

### Task 1.1: Khởi tạo toàn bộ Enums
- **Mô tả:** Tạo các file Enum dùng chung cho các Entity.
- **Đặc tả:**
  - `DocumentType`: CCCD, DriverLicense, CarRegistration.
  - `VerificationStatus`: Pending, Valid, Invalid.
  - `PolicyStatus`: PendingApproval, Active, Expired, Canceled.
  - `ClaimStatus`: Pending, AI_Processing, AI_Evaluated, Approved, Rejected.
  - `EvidenceType`: AccidentScene, RepairInvoice.
  - `SuggestedStatus`: Auto_Approve, Manual_Review, High_Risk.

### Task 1.2: Xây dựng các Entities thuộc nhóm Onboarding & Tài sản
- **Mô tả:** Tạo các lớp (class) trong thư mục `Entities`.
- **Đặc tả:** 
  - Tạo `CustomerDocument` (Id, UserId, DocumentType, ImageUrl, MetadataJson, VerificationStatus). Bổ sung phương thức `UpdateMetadata()`.
  - Tạo `Car` (Id, UserId, LicensePlate, Brand, Model, ManufacturingYear). Cấm sửa biển số (LicensePlate) sau khi tạo.

### Task 1.3: Xây dựng các Entities thuộc nhóm Sản phẩm & Hợp đồng
- **Mô tả:** Tạo các bảng quản lý gói bảo hiểm và hợp đồng.
- **Đặc tả:**
  - Tạo `InsurancePackage`: Có field `IsActive`. Viết phương thức `Deactivate()` để khóa gói.
  - Tạo `PolicyTerm`: Có field `QdrantCollectionName`.
  - Tạo `InsurancePolicy`: Khóa ngoại trỏ về `PolicyTermId` (Snapshot). Có phương thức `ActivatePolicy()` để đổi trạng thái sang Active.

### Task 1.4: Xây dựng các Entities thuộc nhóm Bồi thường (Claims)
- **Mô tả:** Tạo các bảng phục vụ quy trình bồi thường.
- **Đặc tả:** Tạo `ClaimRequest`, `ClaimEvidence` và `ClaimAiReport`.

---

## Epic 2: Cấu hình Data Access & Migrations (Infrastructure Layer)

### Task 2.1: Viết các lớp EntityTypeConfiguration (Fluent API)
- **Mô tả:** Ánh xạ các Entity thành bảng trong PostgreSQL.
- **Đặc tả:** 
  - Khai báo Primary Key, Foreign Key. 
  - Đặt độ dài giới hạn cho string (VD: LicensePlate `varchar(20)`).
  - Đảm bảo cột JSON (MetadataJson, ExtractedData) được map thành kiểu `jsonb` trong Postgres.

### Task 2.2: Setup AppDbContext & Migrations
- **Mô tả:** Đưa cấu hình vào DB Context và chạy lệnh tạo Database.
- **Đặc tả:** Khai báo các `DbSet<>`. Chạy lệnh `dotnet ef migrations add InitDomainEntities` và `dotnet ef database update`.

---

## Epic 3: Tích hợp Dịch vụ Hạ tầng (Infrastructure Services)

### Task 3.1: Dựng MinIO Storage Service
- **Mô tả:** Viết Service để Backend .NET có thể tương tác với MinIO.
- **Đặc tả:** Viết interface `IFileStorageService` với hàm `UploadFileAsync` và `GetFileUrlAsync`. Cài đặt bằng thư viện `AWSSDK.S3` (vì MinIO tương thích S3).

### Task 3.2: Dựng RabbitMQ Message Producer
- **Mô tả:** Viết Service đẩy Message từ .NET sang RabbitMQ.
- **Đặc tả:** Viết interface `IMessagePublisher`. Sử dụng `RabbitMQ.Client`. Tạo cấu trúc JSON cho các Event (VD: `ClaimSubmittedEvent`, `PolicyTermCreatedEvent`).

---

## Epic 4: Xây dựng API và Nghiệp vụ (Application Layer & WebApi)

### Task 4.1: Nhóm API Khách hàng (Onboarding & Mua bảo hiểm)
- **Mô tả:** Code các Use Cases bằng CQRS (MediatR).
- **Đặc tả:**
  - `POST /api/documents/upload`: Nhận file ảnh từ Client, lưu vào MinIO, tạo record `CustomerDocument`.
  - `POST /api/cars`: Đăng ký xe mới.
  - `POST /api/policies`: Khách hàng chọn mua bảo hiểm -> Tạo Hợp đồng trạng thái `PendingApproval`.
  - `POST /api/claims`: Submit hồ sơ bồi thường -> Tải ảnh bằng chứng -> Lưu DB -> Bắn Message `ClaimSubmittedEvent` qua RabbitMQ để gọi AI.

### Task 4.2: Nhóm API Quản trị (Super Admin & Staff)
- **Đặc tả:**
  - `POST /api/admin/packages`: CRUD Gói bảo hiểm và Khóa gói.
  - `POST /api/admin/policy-terms`: Upload PDF Điều khoản -> Lưu MinIO -> Bắn Message `PolicyTermCreatedEvent` qua RabbitMQ để Python băm Vector.
  - `POST /api/staff/policies/{id}/approve`: Nhân viên chốt duyệt hợp đồng.
  - `GET /api/staff/claims`: Lấy danh sách hồ sơ bồi thường (Kèm theo màu nhãn AI Cảnh báo).

---

## Epic 5: Xây dựng Tầng AI Service (Python FastAPI)

### Task 5.1: Khởi tạo FastAPI & RabbitMQ Consumer
- **Mô tả:** Dựng Base Project Python.
- **Đặc tả:** Cấu hình thư viện `pika` hoặc `aio-pika` để liên tục lắng nghe hàng đợi từ RabbitMQ.

### Task 5.2: Module RAG (Policy Vectorization)
- **Đặc tả:** Khi nhận được Event PDF mới, dùng `PyMuPDF` đọc text, dùng `BGE-M3` tạo vector embeddings, và lưu vào Qdrant.

### Task 5.3: Module Workflow AI Đa tác tử (LangGraph)
- **Đặc tả:** 
  - Code 6 hàm Agent riêng biệt.
  - Nối các hàm lại bằng StateGraph (LangGraph).
  - Sau khi Decision Agent chốt kết quả, đẩy ngược Event `ClaimEvaluatedEvent` kèm Báo cáo JSON về lại RabbitMQ để .NET cập nhật vào Database.
