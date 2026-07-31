# Nền tảng AI Multi-Agent Hỗ trợ Xử lý Bồi thường Bảo hiểm Xe cơ giới (Ô tô) - MVP

Chào mừng đến với tài liệu đặc tả dự án (End-to-End Documentation). Dự án này là một nền tảng tiên tiến sử dụng mô hình Multi-Agent (LangGraph) để tự động hóa và hỗ trợ quá trình bồi thường bảo hiểm xe ô tô.

## Cấu trúc Tài liệu

Để dễ dàng theo dõi và triển khai, tài liệu dự án được chia thành các phần sau:

1. [Ý tưởng Dự án (Project Idea)](./00-Project-Idea.md): Nêu bật thực trạng (Pain points) và giải pháp đột phá mà hệ thống mang lại.
2. [Kiến trúc & Công nghệ (Tech Stack)](./01-Architecture-TechStack.md): Tổng quan về các thành phần hệ thống như .NET, FastAPI, PostgreSQL, MinIO, và Qdrant.
3. [Quy trình Nghiệp vụ (Business Workflows)](./02-Business-Workflows.md): Mô tả chi tiết 2 giai đoạn chính là Onboarding (Số hóa hồ sơ) và Claim Processing (Xử lý bồi thường).
4. [Hệ thống Multi-Agent AI (Core)](./03-AI-Multi-Agent-System.md): Trái tim của hệ thống, phân tích nhiệm vụ và công cụ của 5 Agent độc lập (Fraud, OCR, Policy, Validation, Decision).
5. [Phạm vi & Kế hoạch Triển khai (Project Plan)](./04-Project-Phases.md): Các giới hạn của dự án (Out of Scope) và lộ trình phát triển.
6. [Đặc tả Chức năng Hệ thống (Features)](./05-System-Features.md): Liệt kê toàn bộ các tính năng từ phía Khách hàng, Nhân viên duyệt, cho đến Luồng xử lý ngầm của AI.
7. [Thiết kế Domain Backend (Entity Models)](./06-Backend-Domain-Design.md): Phân rã cấu trúc thực thể, cơ sở dữ liệu và thiết kế Domain cho .NET Clean Architecture.
8. [Tiến độ Dự án (Project Progress)](./07-Project-Progress.md): Báo cáo tiến độ code thực tế và các công việc tiếp theo (Task Tracking).
9. [Bảng Đặc tả Công việc (Task Breakdown)](./08-Task-Breakdown.md): Danh sách chi tiết các Task (Backend, DB, AI) cần thực hiện (Kanban Backlog).
10. [Kiến trúc Hệ thống & Codebase (Architecture)](./09-System-Architecture-And-Folder-Structure.md): Sơ đồ nguyên lý hoạt động của Microservices và cấu trúc thư mục Clean Architecture.

---
*Dự án tập trung vào tính ứng dụng cao, tối ưu chi phí bằng Prompt Engineering và Tool Calling thay vì tốn kém cho việc Train Model truyền thống.*
