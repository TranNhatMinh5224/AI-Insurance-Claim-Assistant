# 00. Ý tưởng Dự án (Project Idea)

## 1. Thực trạng ngành bảo hiểm (Pain Points)
Trong quy trình giải quyết bồi thường bảo hiểm xe cơ giới (đặc biệt là Ô tô) truyền thống, cả khách hàng lẫn doanh nghiệp đều đang gặp phải những vấn đề nhức nhối:
- **Khách hàng chờ đợi quá lâu:** Khách hàng nộp hồ sơ, ảnh hiện trường và hóa đơn xong phải chờ đợi hàng tuần để chuyên viên đọc, đối chiếu thủ công với hàng chục trang hợp đồng rắc rối. Trải nghiệm vô cùng mệt mỏi.
- **Nạn gian lận bảo hiểm (Fraud):** Việc chỉnh sửa ảnh (Photoshop) để làm giả hiện trường tai nạn, phóng đại thiệt hại ngày càng tinh vi. Mắt thường của chuyên viên rất khó phát hiện, gây thất thoát tài chính khổng lồ cho công ty.
- **Quá tải và sai sót (Human Error):** Khi xử lý hàng trăm hồ sơ mỗi ngày, chuyên viên dễ dàng bị mệt mỏi, dẫn đến việc bỏ sót các điều khoản loại trừ quan trọng hoặc nhập sai lệch dữ liệu.

## 2. Giải pháp công nghệ (Proposed Solution)
Dự án hướng tới việc xây dựng một **Nền tảng AI Multi-Agent** (Hệ thống Trí tuệ Nhân tạo Đa tác tử). Thay vì dùng một mô hình AI đơn lẻ dễ bị "ảo giác" (Hallucination), hệ thống tạo ra một "Tổ đội trợ lý ảo" làm việc nhóm với nhau:
1. **Tiền xử lý thông minh (Train Models):** Sử dụng các mô hình Deep Learning tự huấn luyện (CNN ResNet18) để phân loại giấy tờ đầu vào (chặn ảnh rác) và mô hình Thị giác máy tính (YOLOv8) để khoanh vùng, nhận diện thiệt hại xe một cách khách quan.
2. **Phân tích độc lập (Multi-Agent):** Hệ thống không dùng 1 con AI đơn lẻ, mà gọi 1 "tổ đội" (Agent) các chuyên gia:
   - *Đặc vụ An ninh:* Chuyên soi mã ẩn (Metadata/EXIF) của ảnh để rà quét dấu hiệu cắt ghép, gian lận.
   - *Đặc vụ Giám định:* Dùng kết quả từ YOLOv8 để chéo với lời khai của khách hàng.
   - *Đặc vụ Pháp chế:* Đọc và truy xuất chính xác các điều khoản bồi thường trong hợp đồng gốc E-Policy (Công nghệ RAG).
   - *Đặc vụ Thám tử:* Suy luận chéo xem bằng lái có quá hạn không, biển số xe chụp có khớp hồ sơ không.
3. **Quyết định có sự tham gia của con người (Human-in-the-loop):** Đội ngũ AI sẽ tổng hợp lại thành một Báo cáo (Report) ngắn gọn. Giải thích rõ ràng (Reasoning) tại sao hợp lệ/không hợp lệ và đề xuất cho Chuyên viên duyệt. Chuyên viên chỉ tốn 2 phút để đọc và bấm "Duyệt".

## 3. Giá trị mang lại (Value Proposition)
- **Tốc độ đột phá:** Rút ngắn thời gian xử lý hồ sơ từ vài ngày xuống tính bằng phút.
- **Tiết kiệm chi phí vận hành:** Giải phóng chuyên viên khỏi việc nhập liệu và tra cứu thủ công rườm rà. Họ chỉ cần tập trung vào khâu quyết định cuối cùng.
- **Bảo vệ dòng tiền (Anti-Fraud):** Tự động phát hiện và cảnh báo các ca nghi ngờ gian lận ảnh ngay từ giây đầu tiên.
- **Minh bạch tuyệt đối:** AI luôn phải trích dẫn (Citation) dựa vào dòng nào, chương nào của Hợp đồng bảo hiểm để đưa ra đề xuất, loại bỏ hoàn toàn sự cảm tính.
