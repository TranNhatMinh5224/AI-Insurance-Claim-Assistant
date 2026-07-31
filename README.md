<div align="center">
  <h1>🛡️ AI Insurance Claim Assistant</h1>
  <p><i>Hệ thống Đánh giá và Xét duyệt Bồi thường Bảo hiểm Tự động dựa trên Kiến trúc Microservices và Trí tuệ Nhân tạo Đa tác tử (Multi-Agent AI).</i></p>

  <!-- Badges -->
  <img src="https://img.shields.io/badge/.NET_10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/Python-3776AB?style=for-the-badge&logo=python&logoColor=white" />
  <img src="https://img.shields.io/badge/Clean_Architecture-222222?style=for-the-badge" />
  <img src="https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white" />
  <img src="https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white" />
  <img src="https://img.shields.io/badge/LangGraph-FF4F00?style=for-the-badge" />
</div>

<br/>

## 📖 Giới thiệu Dự án (Overview)

**AI Insurance Claim Assistant** là giải pháp số hóa toàn diện quy trình mua bán và giải quyết bồi thường bảo hiểm xe cơ giới. Dự án giải quyết bài toán nhức nhối của ngành bảo hiểm: **thời gian chờ đợi bồi thường quá lâu** và **chi phí nhân sự khổng lồ để phát hiện gian lận (Fraud Detection)**.

Hệ thống kết hợp sự bền bỉ của **.NET Core (Clean Architecture)** trong quản lý quy trình tài chính, với sức mạnh suy luận của hệ thống **Multi-Agent AI (Python/LangGraph)** để tự động đánh giá hồ sơ bồi thường chỉ trong vài giây.

## 🚀 Tính năng Cốt lõi (Key Features)

### 1. Dành cho Khách hàng (Customer App)
- **Onboarding & Quản lý Tài sản:** Số hóa Giấy tờ xe (Car Registration, Driver License) bằng OCR. Đăng ký xe vào hệ thống.
- **Mua Bảo hiểm Siêu tốc:** Khách hàng chốt gói bảo hiểm. Hợp đồng PDF (E-Policy) được sinh tự động và gắn liền với Điều khoản Luật tại thời điểm mua (Bảo vệ tính toàn vẹn giá).
- **Yêu cầu Bồi thường (Claim Submission):** Nộp mô tả tai nạn và upload ảnh hiện trường trực tiếp trên App.
- **Tracking Real-time:** Theo dõi tiến độ hồ sơ từ lúc AI tiếp nhận đến khi Nhân viên phê duyệt.

### 2. Dành cho Trí tuệ Nhân tạo (The AI Core)
Thay vì dùng 1 model AI khổng lồ, hệ thống vận hành một **Đội Đặc vụ AI (Multi-Agent System)** phối hợp nhịp nhàng:
- 🕵️ **Gatekeeper & Fraud Agent:** Quét ảnh bằng YOLOv8 & ResNet18 để bắt các lỗi móp méo, phát hiện ảnh chỉnh sửa Photoshop (ELA) hoặc tái sử dụng ảnh cũ (Cross-claim fraud) qua mã băm Hash/Vector Search.
- ⚖️ **Policy Agent (RAG):** Truy xuất kho dữ liệu Vector (Qdrant) để đọc hiểu điều khoản hợp đồng của chính khách hàng đó, đối chiếu xem vụ tai nạn có thuộc phạm vi bồi thường không.
- 🧠 **Judge Agent:** Tổng hợp báo cáo từ các Agent trên, đưa ra kết luận: `Xanh (Tự động duyệt)`, `Vàng (Cần nhân viên xem xét)`, `Đỏ (Rủi ro gian lận cao)`.

### 3. Dành cho Nhân viên / Quản trị viên (Staff Admin Portal)
- **Dashboard Quản lý:** Xét duyệt hợp đồng mới, đọc Báo cáo Tổng hợp (AI Report) để chốt duyệt/từ chối bồi thường.
- **Product Lifecycle:** Quản lý vòng đời Gói bảo hiểm. Đăng tải PDF Điều khoản để hệ thống tự động băm (Vectorize) phục vụ AI học luật.

---

## 🏗️ Kiến trúc Hệ thống & Công nghệ (Architecture & Tech Stack)

Dự án được xây dựng theo mô hình **Event-Driven Microservices** nhằm phân tách rạch ròi giữa nghiệp vụ tài chính (.NET) và tính toán nặng (Python AI).

### 1. Backend Core (C# / .NET 10)
- **Architecture:** Clean Architecture (Domain, Application, Infrastructure, WebApi).
- **Design Patterns:** CQRS (Command Query Responsibility Segregation) triển khai qua `MediatR`.
- **Database:** PostgreSQL (EF Core 10), sử dụng Fluent API configurations và Unit of Work.
- **Security:** JWT Authentication/Authorization (Role-based), BCrypt Hashing.
- **Storage:** MinIO (S3-compatible) quản lý file, ảnh hiện trường.

### 2. AI Service (Python / FastAPI)
- **Framework:** FastAPI, LangChain, LangGraph (StateGraph Workflow).
- **Vector Database:** Qdrant DB.
- **Embeddings & LLM:** BGE-M3 (Vector hóa tiếng Việt siêu việt), kết hợp với LLM API (OpenAI/Anthropic).
- **Computer Vision:** YOLOv8 (Damage Detection), ResNet18.

### 3. Message Broker & Giao tiếp
- **RabbitMQ:** Đóng vai trò xương sống giao tiếp (Asynchronous Messaging) giữa C# và Python. 
  - *Ví dụ:* Khi .NET nhận đơn bồi thường $\rightarrow$ bắn event `ClaimSubmitted` vào RabbitMQ $\rightarrow$ Python tiêu thụ event, chạy AI $\rightarrow$ bắn event `ClaimEvaluated` trả kết quả về .NET.

---

## 📂 Cấu trúc Mã nguồn (Project Structure)

```text
AI-Insurance-Claim-Assistant/
├── src/
│   ├── backend/            # .NET 10 Clean Architecture (Core Business)
│   │   ├── Backend.Domain         # Entities, Enums, Exceptions
│   │   ├── Backend.Application    # CQRS, MediatR, FluentValidation, Interfaces
│   │   ├── Backend.Infrastructure # EF Core, PostgreSQL, MinIO, RabbitMQ Configs
│   │   └── Backend.WebApi         # Controllers, Middlewares, DI Setup
│   │
│   ├── ai-agent/           # Python FastAPI (Multi-Agent System) - Sắp triển khai
│   └── mobile-app/         # React Native (Customer Frontend) - Sắp triển khai
├── docs/                   # Tài liệu thiết kế hệ thống chi tiết (Features, DB Schema...)
├── docker-compose.yml      # Tự động hóa cài đặt PostgreSQL, MinIO, RabbitMQ, Qdrant
└── README.md
```

---

## 🎯 Điểm Nhấn Kỹ thuật dành cho Nhà Tuyển Dụng
- **Tư duy thiết kế Domain-Driven Design (DDD):** Domain Model đóng gói chặt chẽ các nghiệp vụ cốt lõi (VD: `policy.ActivatePolicy()`, `policy.CancelPolicy()`), không dùng rò rỉ (anemic domain model).
- **CQRS & Manual Mapping:** Phân tách hoàn toàn luồng Đọc/Ghi (Read/Write). Tuân thủ nguyên tắc không lạm dụng AutoMapper để kiểm soát hiệu năng và compile-time safety.
- **Robust Error Handling:** Mọi Use Case trả về kiểu `Result<T>` thay vì throw exception bừa bãi. Controller tự động map Error Code sang chuẩn HTTP Status Codes (400, 401, 403, 404, 409).
- **Scalability:** Việc đẩy các tác vụ AI mất thời gian qua RabbitMQ giúp API của .NET luôn đạt tốc độ phản hồi cực nhanh, không bao giờ bị nghẽn (non-blocking).

---
> 💡 *Dự án này là minh chứng cho khả năng thiết kế kiến trúc phần mềm Backend vững chắc, tư duy hệ thống lớn (System Design) và khả năng tích hợp linh hoạt với các công nghệ Trí tuệ nhân tạo hiện đại.*
