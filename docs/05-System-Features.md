# 5. Đặc tả Chức năng Hệ thống (System Features)

Tài liệu này liệt kê chi tiết toàn bộ các chức năng (Features) sẽ được phát triển trong khuôn khổ phiên bản MVP của dự án, được chia theo từng góc nhìn của người dùng. Các luồng nghiệp vụ được thiết kế chặt chẽ chống rủi ro pháp lý và gian lận.

---

## 1. Phân hệ Khách hàng (Customer - Mobile App / Web)

### 1.1. Xác thực & Quản lý Tài khoản
- **Đăng ký & Đăng nhập:** Cho phép người dùng tạo tài khoản và đăng nhập bằng Email/Mật khẩu (Bảo mật bằng JWT Access Token & Refresh Token).
- **Quên mật khẩu:** Gửi đường link khôi phục mật khẩu (Magic Link) qua địa chỉ Email thực tế.

### 1.2. Mua Bảo hiểm & Quản lý Tài sản (Onboarding)
- **Quản lý Giấy tờ (My Documents):** Xem lại danh sách giấy tờ đã tải lên (CCCD, Đăng ký xe, Bằng lái). Cho phép xem hình ảnh gốc kèm theo **Metadata** (Dữ liệu chữ đã được OCR bóc tách và siêu dữ liệu ảnh).
- **Quản lý Xe (My Cars):** Danh sách các xe sở hữu (Tự động tạo ra từ Giấy đăng ký xe).
- **Mua Bảo hiểm Ô tô:** Cung cấp các gói bảo hiểm thân vỏ xe cơ bản, chọn xe để mua bảo hiểm (Mỗi xe chỉ được phép có 1 Hợp đồng đang hiệu lực).
- **Đăng ký Gói Bảo hiểm:** Khách hàng xác nhận chọn gói bảo hiểm. Trạng thái Hợp đồng lập tức chuyển sang `Chờ duyệt (Pending)` (Bỏ qua hoàn toàn luồng thanh toán).
- **My Insurance (Hợp đồng của tôi):** Xem chi tiết Hợp đồng đã được lưu hành, xem điều khoản, tải Hợp đồng PDF.
- **Gia hạn & Hủy hợp đồng (Lifecycle):** Có tính năng nhắc nhở và nút gia hạn khi Hợp đồng sắp hết hạn; hoặc Yêu cầu hủy hợp đồng (hoàn phí theo tỷ lệ thời gian chưa sử dụng).

### 1.3. Yêu cầu Bồi thường (Claim Submission)
- **Tạo hồ sơ Claim:** Chọn chiếc xe đang gặp sự cố. Nhập mô tả diễn biến tai nạn.
- **Tải lên bằng chứng:** Tải lên ảnh hiện trường và ảnh hóa đơn sửa chữa.
- **Theo dõi tiến độ:** Xem trạng thái hồ sơ (Đang xử lý ngầm, Chờ nhân viên duyệt, Đã duyệt, Bị từ chối). Hệ thống tự động gửi Email thông báo khi có thay đổi trạng thái.

---

## 2. Phân hệ Nhân viên (Staff - Web Admin Portal)

### 2.1. Xét duyệt Bán hàng (Sales Approval)
- **Duyệt Hợp đồng Mới:** Nhân viên kiểm tra hồ sơ giấy tờ xe và bấm **Duyệt (Approve)**. Lúc này hợp đồng chính thức **Lưu hành (Active)** và cấp file PDF Hợp đồng (E-Policy).

### 2.2. Danh sách Hồ sơ Bồi thường (Claim Requests)
- **Danh sách Pending:** Hiển thị yêu cầu bồi thường đang chờ duyệt.
- **Cảnh báo AI:**
  - `🟢 Hồ sơ sạch (Auto-Approve)`: Dữ liệu chuẩn xác, không có dấu hiệu gian lận.
  - `🟡 Cần xác minh (50/50 - Manual Review)`: Thiếu một vài góc ảnh, hoặc AI nghi ngờ nhẹ.
  - `🔴 Rủi ro cao (High Risk)`: Báo động đỏ, phát hiện cắt ghép Photoshop, sai thông tin hoặc trùng lặp hồ sơ.

### 2.3. Xem Chi tiết & Xét duyệt
- **Đọc Báo cáo AI (AI Report):** Nhân viên đọc một bản Báo cáo Tổng hợp do AI viết:
  - Cảnh báo gian lận (Photoshop, ELA).
  - Cảnh báo gian lận chéo (Cross-claim Fraud): Ảnh này đã từng bị dùng ở một hồ sơ khác chưa?
  - Báo cáo thiệt hại (YOLOv8 quét ra mấy vết xước).
  - Báo cáo pháp lý (Trích dẫn điều khoản RAG từ Hợp đồng của khách).
  - **Đề xuất Trạng thái:** AI đánh giá mức độ hợp lệ của toàn bộ hồ sơ để đề xuất cho nhân viên duyệt (Không xử lý các bài toán tính toán tiền hay bồi thường).
- **Kiểm chứng & Ra quyết định (Approve/Reject):** Nhân viên chốt kết quả và ghi chú lý do.

---

## 3. Phân hệ Quản trị viên (Super Admin - Web Admin Portal)
Đóng vai trò quản lý danh mục và cấu hình lõi của doanh nghiệp.

### 3.1. Quản lý Chính sách & Sản phẩm Bảo hiểm
- **Quản lý Gói Bảo hiểm (Bảo vệ tính toàn vẹn giá):** Để tránh ảnh hưởng đến hợp đồng cũ, khi có thay đổi về giá hoặc quyền lợi, Super Admin KHÔNG được sửa trực tiếp gói cũ. Thay vào đó, chức năng sẽ cho phép **Khóa (Deactivate) gói cũ** và tạo gói mới. (Tức là ẩn gói đó đi, không cho khách hàng mới mua nữa, nhưng hệ thống vẫn tiếp tục hỗ trợ và giải quyết bồi thường bình thường cho các khách hàng cũ đang sử dụng gói này).
- **Quản lý Điều khoản (Terms & Policies):** Thêm các tài liệu chính sách, file điều khoản đính kèm. 
- **Kiểm duyệt Dữ liệu AI (Human-in-the-loop):** *Lưu ý quan trọng:* Khi Super Admin tải lên PDF Điều khoản, hệ thống sẽ tự động chạy OCR để trích xuất thành Văn bản (Text). Super Admin được quyền đọc, kiểm tra và sửa lỗi chính tả văn bản này trên màn hình. Chỉ khi Super Admin bấm nút **"Xác nhận Vector hóa"**, hệ thống mới đẩy dữ liệu vào Qdrant DB. Nhờ đó, Đặc vụ AI Pháp chế (Policy Agent) luôn được học luật chuẩn xác nhất, loại bỏ hoàn toàn rủi ro OCR đọc sai chữ.

---

## 4. Phân hệ Hệ thống AI chạy ngầm (System Core)

- **Tiến trình Bất đồng bộ (RabbitMQ Message Broker):** Mọi tác vụ nặng của AI đều được ném vào hàng đợi để .NET rảnh tay phục vụ user khác, giúp hệ thống không bao giờ bị nghẽn (Timeout).
- **Cổng kiểm duyệt Ảnh (Gatekeeper - ResNet18):** Chặn các bức ảnh rác ngay từ đầu.
- **Nhận diện Thiệt hại (YOLOv8):** Tự động khoanh vùng vết móp méo.
- **Vector hóa Hợp đồng (Embedding):** Tự động băm nhỏ file PDF E-Policy để đẩy vào Qdrant DB.
- **Workflow Đa tác tử (LangGraph Orchestrator):** 
  - *Đặc vụ An ninh:* Bắt gian lận cục bộ (Photoshop) và **Gian lận liên hồ sơ (Cross-claim)** bằng cách so sánh Hash ảnh và Vector Search xem ảnh hóa đơn/hiện trường có bị tái sử dụng ở hồ sơ cũ hay không.
  - *Đặc vụ Giám định:* Chéo thông tin YOLO.
  - *Đặc vụ Đọc hiểu:* Bóc tách hóa đơn.
  - *Đặc vụ Pháp chế:* RAG truy xuất điều khoản bảo hiểm.
  - *Đặc vụ Thám tử:* Suy luận Logic.
  - *Thẩm phán Cuối cùng:* Viết báo cáo tổng hợp & Đề xuất (Approve/Manual Review/Reject).
