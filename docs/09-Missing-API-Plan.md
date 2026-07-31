# Kế hoạch Bổ sung API còn thiếu (Missing APIs Plan)

Dựa trên phân tích thiết kế hệ thống và tiến độ thực tế, chúng ta đang thiếu hụt trầm trọng các tính năng lấy dữ liệu (Query/Read) và một số hành động chuyển đổi trạng thái quan trọng. 

Dưới đây là kế hoạch chi tiết chia thành 4 chặng (Phase) để hoàn thiện toàn bộ **Epic 4** một cách triệt để.

---

## Phase 1: Hoàn thiện dữ liệu cá nhân của Khách hàng (Customer Queries)
*Mục tiêu: Đảm bảo App Mobile của khách hàng có đủ API để hiển thị danh sách tài sản và giấy tờ.*

| STT | Endpoint | Method | Role | Mô tả |
|---|---|---|---|---|
| 1 | `/api/v1/cars/me` | `GET` | Customer | Lấy danh sách toàn bộ xe do Khách hàng đang sở hữu. |
| 2 | `/api/v1/documents/me` | `GET` | Customer | Lấy danh sách các giấy tờ (CCCD, Đăng ký xe) đã tải lên. |
| 3 | `/api/v1/policies/me` | `GET` | Customer | Lấy lịch sử mua bảo hiểm (Hợp đồng đang chờ duyệt, đang active, hoặc đã hết hạn). |

---

## Phase 2: Cung cấp công cụ cho Quản trị viên (Admin/Staff Queries)
*Mục tiêu: Đảm bảo màn hình Web Portal của Nhân viên và Admin hiển thị được dữ liệu để xử lý công việc.*

| STT | Endpoint | Method | Role | Mô tả |
|---|---|---|---|---|
| 4 | `/api/v1/admin/packages` | `GET` | SuperAdmin | Lấy danh sách toàn bộ gói bảo hiểm (Bao gồm cả các gói đã bị khóa/Deactivated). |
| 5 | `/api/v1/staff/policies/pending` | `GET` | StaffAndAdmin | Lấy danh sách các Hợp đồng mới đang ở trạng thái `PendingApproval` chờ duyệt. |

---

## Phase 3: Nghiệp vụ Xét duyệt Hợp đồng (Staff State Transitions)
*Mục tiêu: Đưa Hợp đồng từ trạng thái "Chờ" sang "Lưu hành".*

| STT | Endpoint | Method | Role | Mô tả |
|---|---|---|---|---|
| 6 | `/api/v1/staff/policies/{id}/approve` | `POST` | StaffAndAdmin | Nhân viên chốt duyệt Hợp đồng. Chuyển trạng thái sang `Active`, tạo thời hạn bảo hiểm (Start Date - End Date), và chốt link PDF Hợp đồng (E-Policy). |
| 7 | `/api/v1/staff/policies/{id}/reject` | `POST` | StaffAndAdmin | Nhân viên từ chối cấp bảo hiểm (Ví dụ: phát hiện xe đang nợ thuế hoặc giấy tờ giả). |

---

## Phase 4: Nghiệp vụ cốt lõi - Đòi bồi thường (Claims)
*Mục tiêu: Đây là tính năng lớn nhất và phức tạp nhất, liên quan đến tích hợp RabbitMQ và AI.*

| STT | Endpoint | Method | Role | Mô tả |
|---|---|---|---|---|
| 8 | `/api/v1/claims` | `POST` | Customer | Khách hàng nộp hồ sơ sự cố. Gắn kèm xe, hợp đồng đang active, mô tả tai nạn và upload ảnh hiện trường. |
| 9 | `/api/v1/claims/me` | `GET` | Customer | Khách hàng theo dõi trạng thái tiến độ xử lý hồ sơ bồi thường của mình. |
| 10 | `/api/v1/staff/claims` | `GET` | StaffAndAdmin | Nhân viên xem danh sách các hồ sơ bồi thường được phân loại theo nhãn màu AI (Xanh, Vàng, Đỏ). |

---

## 🚀 Thứ tự triển khai ưu tiên
Chúng ta sẽ giải quyết theo hướng "Cuốn chiếu": **Phase 1 -> Phase 2 -> Phase 3 -> Phase 4**.
Việc làm Phase 1 và Phase 2 trước sẽ rất nhanh vì nó chỉ là CQRS Query, không có logic phức tạp. Nó tạo nền tảng vững chắc để làm Phase 3 và 4.
