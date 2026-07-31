# 2. Quy trình Nghiệp vụ (Business Workflow)

Hệ thống được thiết kế theo luồng tương tác chặt chẽ, phân tách rõ ràng vai trò của 4 chủ thể: **Khách hàng (Customer)**, **Nhân viên (Staff)**, **Quản trị viên (Super Admin)** và **Hệ thống AI (AI Agents)**.

---

## Giai đoạn 1: Số hóa hồ sơ & Cấp phát Hợp đồng (Onboarding)

### 👤 Khách hàng (Customer):
1. Mở ứng dụng, đăng ký và đăng nhập vào Customer Portal.
2. Tải lên hệ thống các hình ảnh giấy tờ tùy thân (CCCD, Bằng lái xe, Giấy đăng ký xe).

### 🤖 Hệ thống AI (AI Pipeline):
3. **Tiền xử lý (ResNet18):** Phân loại ảnh. Nếu khách tải sai ảnh (VD: tải nhầm ảnh chó mèo, selfie), AI lập tức từ chối và yêu cầu tải lại.
4. **Trích xuất dữ liệu (OCR Agent):** Tự động đọc hình ảnh giấy tờ hợp lệ, chuẩn hóa thành văn bản và lưu trữ dưới dạng JSON vào cơ sở dữ liệu. Khách hàng có thể vào mục **Giấy tờ của tôi** để xem cả ảnh gốc lẫn Metadata.
5. **Đăng ký mua Bảo hiểm:** Khách hàng chọn xe trong danh sách (mỗi xe 1 hợp đồng), xem điều khoản và bấm xác nhận đăng ký. Hợp đồng chuyển sang trạng thái chờ duyệt.

### 👨‍💼 Nhân viên & AI (Staff & AI System):
6. **Nhân viên duyệt Hợp đồng:** Nhân viên kiểm tra tính hợp lệ và bấm Duyệt.
7. **Vector hóa Hợp đồng (Policy RAG):** Ngay khi nhân viên duyệt, Hợp đồng chính thức **lưu hành (Active)**. Hệ thống tự động băm nhỏ Hợp đồng điện tử (E-Policy PDF) thành vector và lưu vào Qdrant DB.

### 👤 Khách hàng (Customer):
8. Truy cập mục **My Insurance (Bảo hiểm của tôi)** để xem Hợp đồng bản mềm vừa được lưu hành và tra cứu điều khoản chi tiết.

---

## Giai đoạn 2: Xử lý bồi thường (Claim Processing)

### 👤 Khách hàng (Customer):
1. Chọn xe đang gặp sự cố trong danh sách hợp đồng ở mục My Insurance.
2. Tải lên ảnh hiện trường tai nạn và hóa đơn sửa chữa.
3. Nhập mô tả ngắn gọn và bấm **"Gửi yêu cầu bồi thường"**.

### 🤖 Hệ thống AI (Multi-Agent Orchestration):
4. **Trigger (RabbitMQ Queue):** Backend .NET ghi nhận hồ sơ và đẩy sự kiện (Message) vào hàng đợi RabbitMQ. AI Service (Python) lắng nghe và ngay lập tức bắt tay vào việc.
5. **LangGraph Workflow chạy ngầm (Asynchronous Processing):**
   - *Đặc vụ An ninh:* Phân tích mã ẩn EXIF, soi ELA để xem ảnh hiện trường có bị dùng Photoshop cắt ghép không.
   - *Đặc vụ Giám định:* Kích hoạt YOLOv8 quét ảnh hiện trường để đếm chính xác số vết móp, vết xước (không phụ thuộc vào lời khai của khách).
   - *Đặc vụ Đọc hiểu:* Đọc bóc tách số tiền và danh mục linh kiện trên hóa đơn sửa chữa.
   - *Đặc vụ Pháp chế:* Lục tìm vector hợp đồng của đúng khách hàng này trong Qdrant để xem vết xước đó có nằm trong danh mục được đền bù không.
   - *Đặc vụ Thám tử:* Suy luận chéo xem bằng lái xe có bị hết hạn vào đúng ngày chụp ảnh hiện trường không.
   - *Thẩm phán Cuối cùng:* Thu thập toàn bộ mảnh ghép trên, viết một báo cáo phân tích, kết luận: `Require_Human_Review` hoặc `Auto_Approve`.

### 👨‍💼 Nhân viên Bồi thường (Claim Officer - Human in the loop):
6. Đăng nhập vào trang Quản trị (Admin Portal).
7. Mở danh sách hồ sơ bồi thường (Claim Requests).
8. Không cần căng mắt đọc thủ công hàng chục file đính kèm, chỉ cần đọc **Báo cáo tổng hợp từ AI**.
9. Nhấp nút **Duyệt (Approve)** hoặc **Từ chối (Reject)** dựa trên đề xuất minh bạch của AI. Hệ thống tự động gửi Email thông báo kết quả cho Khách hàng.
