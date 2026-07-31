# 1. Kiến trúc Công nghệ (Tech Stack)

Hệ thống tuân thủ nghiêm ngặt kiến trúc Microservices cơ bản, phân tách rõ ràng giữa quản lý luồng nghiệp vụ (Backend Core) và xử lý tính toán AI (AI Service).

## Danh sách Thành phần và Công nghệ

| Thành phần | Công nghệ / Framework | Vai trò |
| :--- | :--- | :--- |
| **Backend Core** | C# .NET 8/9, Clean Architecture | Đóng vai trò là API Gateway, chịu trách nhiệm quản lý người dùng, xử lý logic luồng nghiệp vụ bảo hiểm, phân quyền. |
| **AI Service (LLM)** | Python, FastAPI, LangGraph | Orchestrator chuyên dụng để điều phối đa tác vụ AI (Generative AI Multi-Agent). |
| **Computer Vision (CV)** | PyTorch, YOLOv8, ResNet18, OpenCV | Các mô hình Deep Learning tự huấn luyện để phân loại giấy tờ, nhận diện vết xước và phân tích ảnh. |
| **Message Broker** | RabbitMQ | Xử lý hàng đợi bất đồng bộ (Async Queue) làm cầu nối giao tiếp chịu tải giữa .NET và Python. |
| **Database** | PostgreSQL, EF Core | Lưu trữ hồ sơ khách hàng, thông tin hợp đồng, metadata OCR, user, claims (yêu cầu bồi thường). |
| **Object Storage** | MinIO (S3-compatible) | Lưu trữ các file phi cấu trúc như ảnh gốc, hóa đơn, CCCD một cách an toàn. |
| **Vector DB** | Qdrant | Lưu trữ các vector ngữ nghĩa (embeddings) của các điều khoản hợp đồng bảo hiểm, phục vụ cho kỹ thuật RAG. |

## Sơ đồ giao tiếp (Khái quát)
1. **Client** (Web/App) giao tiếp hoàn toàn thông qua **Backend Core (.NET)**.
2. Khi có tác vụ nặng cần AI (VD: Xử lý Claim, băm Vector), **Backend Core (.NET)** sẽ KHÔNG gọi API trực tiếp bắt user chờ đợi (Synchronous). Thay vào đó, hệ thống sẽ đẩy một Message vào hàng đợi của **RabbitMQ** (Asynchronous).
3. **AI Service (FastAPI)** đóng vai trò Worker (Consumer), liên tục lắng nghe hàng đợi từ RabbitMQ. Khi chộp được Message, nó sẽ kích hoạt chu trình xử lý ngầm, kéo ảnh từ **MinIO**, lưu vector vào **Qdrant**.
4. Xử lý xong, **AI Service** đẩy kết quả ngược lại qua RabbitMQ (hoặc Webhook) để **Backend Core** cập nhật dữ liệu vào **PostgreSQL** và gửi thông báo cho Client.
