# 7. Tiến độ Dự án (Project Progress & Task Tracking)

Tài liệu này theo dõi những gì hệ thống đã xây dựng được trên thực tế (Codebase) so với Thiết kế hệ thống, giúp team nắm bắt được tiến độ và các công việc tiếp theo.

---

## 1. Tình trạng Hiện tại (Current Status)

### 1.1. Hạ tầng & Môi trường (Infrastructure)
- **Hoàn thành:** Đã thiết lập xong `Dockerfile` (Multi-stage cho .NET) và `docker-compose` (cho Postgres, MinIO).
- **Hoàn thành:** Cấu hình chuẩn `.gitignore`, `.env.example` và tách biệt môi trường Dev/Prod.

### 1.2. Backend Core (.NET 8/9 Clean Architecture)
Dự án đã dựng thành công bộ khung Clean Architecture và hoàn thiện toàn bộ **Phân hệ Identity (Xác thực người dùng)**. Cụ thể:

**a. Domain Entities đã xây dựng:**
- `User`: Đầy đủ các thuộc tính cốt lõi và các Domain Methods (Tự đóng gói logic kinh doanh).
  - Khởi tạo User mới với Role mặc định.
  - Xử lý đổi mật khẩu (`ChangePassword`).
  - Xử lý Refresh Token (`UpdateRefreshToken`).
  - Xử lý tạo và xác thực Token quên mật khẩu (`GeneratePasswordResetToken`, `ResetPassword`).
- `UserRole` Enum: `Customer`, `Staff`, `SuperAdmin`.

**b. WebApi (Controllers) đã hoàn thiện:**
- `AuthController`:
  - `POST /register`: Đăng ký tài khoản.
  - `POST /login`: Đăng nhập cấp phát JWT Access Token & Refresh Token.
  - `POST /refresh`: Làm mới token khi hết hạn (Tránh người dùng bị văng ra ngoài).
  - `POST /forgot-password`: Gửi link khôi phục.
  - `POST /reset-password`: Đặt lại mật khẩu.
- `UsersController`:
  - CRUD cơ bản quản lý người dùng.
  - `GET /me`: Trích xuất thông tin người dùng đang đăng nhập từ JWT Token (Authorization).

---

## 2. Kế hoạch Tiếp theo (Next Action Items)

Dựa trên Kiến trúc Domain Design (File 06), đây là các Task cần triển khai ngay trong những phiên làm việc tới:

### 🟧 Phase 1: Hoàn thiện Phân hệ Onboarding (Tài sản & Giấy tờ)
- [x] **Tạo Entities:** `CustomerDocument`, `Car`. Cài đặt các Enums (`DocumentType`, `VerificationStatus`).
- [x] **DB Migration:** Config EntityFramework (Fluent API) cho các bảng mới, tạo Migration và Update Database PostgreSQL.
- [x] **MinIO Integration:** Code Service đẩy file ảnh/pdf từ Backend lên Object Storage MinIO và nhận về URL (Đã hỗ trợ Draft/Real).
- [ ] **API Endpoints:** Tạo API upload giấy tờ và API khai báo/quản lý Xe cơ giới.

### 🟧 Phase 2: Phân hệ Product Catalog (Dành cho Super Admin)
- [x] **Tạo Entities:** `InsurancePackage`, `PolicyTerm`.
- [ ] **Logic Khóa Gói:** Code API tạo gói bảo hiểm, Cấm chức năng Delete (Chỉ dùng `IsActive = false` để ẩn/khóa gói).
- [ ] **Tích hợp RabbitMQ (RabbitMQ Producer):** Code hàm thả Message `PolicyTermUpdated` vào Queue khi Super Admin tải lên PDF điều khoản mới.

### 🟧 Phase 3: AI Service (FastAPI) & Qdrant Integration
- [ ] Dựng project Python FastAPI.
- [ ] Dựng RabbitMQ Consumer bên Python lắng nghe sự kiện từ .NET.
- [ ] Viết hàm băm nhỏ file PDF lấy từ MinIO và đưa vào Qdrant (Policy Vectorization).

---
*Cập nhật lần cuối: Xem lịch sử Git commit.*
