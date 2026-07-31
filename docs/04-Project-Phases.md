# 4. Giới hạn Phạm vi & Kế hoạch (Out of Scope & Planning)

Để đảm bảo dự án khả thi trong khuôn khổ thời gian phát triển (làm đồ án) và tập trung vào các core feature có giá trị nhất, một số tính năng sẽ được đưa vào danh sách **Out of Scope** (Không nằm trong phạm vi phát triển hiện tại).

## 1. Giới hạn (Out of Scope)
- **Không làm chức năng Thống kê phức tạp (BI Dashboard):** Bỏ qua các màn hình biểu đồ thống kê doanh thu rườm rà. Hệ thống tập trung vào 3 Role chính: **Khách hàng (Customer), Nhân viên duyệt (Staff), và Quản trị viên (Super Admin)**.
- **Không làm Cổng Gara Liên kết (Partnered Garages):** Thực tế khách hay mang xe vào gara liên kết để "Bảo lãnh sửa chữa" (Không cần ứng tiền trước). MVP này sẽ coi như khách tự thanh toán cho Gara rồi nộp Hóa đơn về công ty để đòi lại tiền (Cash Claim). Tính năng Gara sẽ đẩy sang Phase 2.
- **Chỉ áp dụng Một nghiệp vụ duy nhất:** Tập trung xử lý bồi thường **Bảo hiểm xe cơ giới (Ô tô)**. Bỏ qua các loại hình bảo hiểm sức khỏe, nhân thọ để giảm tải độ phức tạp của hệ thống RAG.

## 2. Kế hoạch (Action Plan MVP)
- **Giai đoạn 1: Cấu trúc hạ tầng & Backend Core** 
  - Cấu hình Postgres, MinIO bằng Docker Compose.
  - Hoàn thiện luồng xác thực và quản lý Hợp đồng bản mềm (E-Policy).
- **Giai đoạn 2: Trí tuệ nhân tạo (Train Model & AI Service)**
  - Tự Train mô hình phân loại giấy tờ (ResNet) và nhận diện vết xước xe (YOLOv8) trên Google Colab.
  - Cài đặt FastAPI và tích hợp các model.
- **Giai đoạn 3: Tích hợp Multi-Agent (LangGraph)**
  - Định nghĩa 6 State nodes cơ bản cho các Agent.
  - Lắp ráp Tools (OpenCV, PaddleOCR, Qdrant) cho từng Agent.
- **Giai đoạn 4: System Integration & Workflow**
  - Móc nối API giữa .NET và FastAPI.
  - Giả lập luồng Submit Claim -> Background Job -> Kết quả báo cáo AI.
- **Giai đoạn 5: Frontend / Giao diện người dùng**
  - Màn hình Customer Upload.
  - Màn hình Claim Officer duyệt báo cáo.
