# 3. Cấu trúc Trí tuệ Nhân tạo (Multi-Agent & Deep Learning)

Hệ thống kết hợp sức mạnh của **Deep Learning truyền thống (Tự Train Model)** và **Generative AI (LLM Multi-Agent)** để tạo ra một quy trình kiểm duyệt hoàn toàn tự động và thông minh. Dưới đây là sơ đồ kiến trúc tổng quan của toàn bộ hệ thống AI:

```mermaid
graph TD
    %% Khối Tiền xử lý
    subgraph Preprocessing [Tầng A: Tiền xử lý & Gatekeeper (Tự Train)]
        ResNet[CNN ResNet18 <br> Phân loại ảnh đầu vào]
        YOLO[YOLOv8 <br> Nhận diện vết xước/móp méo]
    end

    %% Khối Multi-Agent (LangGraph)
    subgraph LangGraph [Tầng B: LangGraph Multi-Agent Orchestrator]
        direction TB
        Fraud[Đặc vụ An ninh <br> Tool: OpenCV, EXIF]
        OCR[Đặc vụ Đọc hiểu <br> Tool: PaddleOCR, GPT-4o-mini]
        Assessor[Đặc vụ Giám định <br> Chéo thông tin với YOLO]
        Policy[Đặc vụ Pháp chế <br> Tool: BGE-M3, Qdrant DB]
        Validation[Đặc vụ Thám tử <br> Suy luận Logic]
        
        Decision[Thẩm phán Cuối cùng <br> Tổng hợp Báo cáo & Đề xuất]
    end

    %% Flow dữ liệu
    Input((Dữ liệu từ Khách hàng)) --> Preprocessing
    Preprocessing -- Dữ liệu đã qua làm sạch --> LangGraph
    
    Fraud --> Decision
    OCR --> Decision
    Assessor --> Decision
    Policy --> Decision
    Validation --> Decision

    Decision -- Báo cáo Phân tích --> Output((Nhân viên duyệt))
```

Dưới đây là sơ đồ chi tiết về nhiệm vụ của 6 Agent:

## A. Tiền xử lý bằng Mô hình Tự Train (Gatekeeper Models)
Để tối ưu chi phí gọi API và tăng độ chính xác, hệ thống sử dụng các mô hình Convolutional Neural Network (CNN) tự huấn luyện.

### 1. Mô hình Phân loại Giấy tờ (Document Classifier - ResNet18)
- **Bài toán:** Khách hàng thay vì up CCCD, bằng lái xe lại up nhầm ảnh selfie, ảnh thú cưng. Nếu gọi trực tiếp API OCR sẽ gây lỗi và tốn kém chi phí.
- **Giải pháp (Train Model):** 
  - Gom dataset vài trăm ảnh phân làm 4 class: *CCCD, Bằng lái xe, Đăng ký xe, Ảnh rác*.
  - Dùng mô hình ResNet18 (PyTorch) để huấn luyện phân loại ảnh.
- **Tích hợp:** Khi khách up ảnh lên, mô hình này chặn lại "kiểm tra vé". Nếu phán đoán là "Không phải giấy tờ hợp lệ", hệ thống lập tức chặn lại, yêu cầu khách up ảnh khác, tuyệt đối không gọi OCR.

### 2. Mô hình Nhận diện Thiệt hại Xe (Car Damage Detection - YOLOv8)
- **Bài toán:** Phải dựa vào con người nhìn ảnh để ước lượng xe hỏng ở đâu, xước hay móp. Khách hàng có thể khai khống thiệt hại.
- **Giải pháp (Train Model):** 
  - Tải dataset "Car Damage Detection" trên Kaggle (gồm hàng ngàn ảnh ô tô tai nạn đã được gán nhãn). 
  - Huấn luyện bằng YOLOv8 trên Google Colab để khoanh vùng (Bounding Box) các vết xước (scratch), móp méo (dent), vỡ kính (glass shatter).
- **Tích hợp:** Chuyển kết quả nhận diện thành văn bản (VD: "Phát hiện 1 vết móp cản trước - 92%") để làm dữ liệu đầu vào cho Agent xử lý.

---

## B. Tổ đội Đa tác tử (LangGraph Multi-Agent)
Hệ thống sử dụng **LangGraph** để điều phối 6 Agent hoạt động tuần tự và phối hợp với nhau. Các Agent sử dụng **Tool Calling** và **Prompt Engineering** thay vì phải train lại từ đầu.

*Lưu ý về Tối ưu Chi phí API (Prompt Routing):* Để tránh lãng phí tiền gọi API, hệ thống không dùng LLM xịn cho tất cả. Các Đặc vụ 1, 2, 3, 4, 5 chỉ dùng Model nhỏ, tốc độ cao (VD: `GPT-4o-mini` hoặc `Claude 3 Haiku`). Chỉ riêng Thẩm phán Cuối cùng mới được cấp quyền gọi Model lớn (`GPT-4o` hoặc `Claude 3.5 Sonnet`) để đảm bảo chất lượng suy luận logic cuối cùng.

### 1. Fraud Detection Agent (Đặc vụ An ninh)
- **Chức năng:** Soi mã ẩn (Metadata/EXIF) của ảnh để rà quét dấu hiệu cắt ghép, gian lận bằng Photoshop.
- **Công cụ:** Code Python đọc EXIF (bắt mốc thời gian, tên phần mềm) và thuật toán ELA (Error Level Analysis) từ thư viện OpenCV để phát hiện vùng pixel bị chỉnh sửa.

### 2. OCR Agent (Đặc vụ Đọc hiểu)
- **Chức năng:** Trích xuất thông tin chữ viết từ hình ảnh đã qua cổng kiểm duyệt ResNet18 (CCCD, Hóa đơn).
- **Công cụ:** PaddleOCR (chạy local để đọc text tiếng Việt) kết hợp với LLM (GPT-4o-mini) để nắn chỉnh, ép chuẩn text thô thành cấu trúc JSON rõ ràng.

### 3. Damage Assessor Agent (Đặc vụ Giám định)
- **Chức năng:** Đối chiếu mô tả tai nạn của khách hàng với ảnh chụp hiện trường.
- **Công cụ:** Gọi mô hình **YOLOv8** (ở phần A) để nhận diện vùng hỏng. Nếu khách hàng khai "Vỡ đèn hậu" nhưng YOLOv8 chỉ thấy "Móp cản trước", Agent sẽ ghi nhận mâu thuẫn (Discrepancy).

### 4. Policy Agent (Đặc vụ Pháp chế - RAG)
- **Chức năng:** Thư viện sống, đọc hiểu hợp đồng bảo hiểm bản mềm (E-Policy) để tìm điều khoản bồi thường và điểm loại trừ.
- **Công cụ:** Qdrant Vector DB và mô hình nhúng BGE-M3 (chuyên trị tiếng Việt).

### 5. Validation Agent (Đặc vụ Thám tử)
- **Chức năng:** Suy luận logic chéo đa chiều. 
  - *Ví dụ:* Tên trên hóa đơn sửa chữa có khớp với tên chủ xe? Bằng lái xe có còn hạn tại thời điểm xảy ra tai nạn (lấy từ Metadata ảnh)?
- **Công cụ:** GPT-4o-mini (Zero-shot reasoning).

### 6. Decision Agent (Thẩm phán Cuối cùng)
- **Chức năng:** Thu thập 5 báo cáo của các Agent trên, đối chiếu logic. Đánh giá hồ sơ có hoàn toàn hợp lệ theo quy định bảo hiểm hay không. Viết một Report phân tích tổng hợp gửi cho nhân viên và chốt trạng thái: `Auto_Approve` hoặc `Require_Human_Review`.
- **Công cụ:** GPT-4o hoặc Claude 3.5 Sonnet (Mô hình tạo sinh ngôn ngữ lớn nhất để viết văn phong thuyết phục cho chuyên viên đọc).
