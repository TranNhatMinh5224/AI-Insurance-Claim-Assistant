namespace Backend.Application.Features.Documents.UploadDraft;

public sealed record UploadDocumentDraftResponse(
    string FileName,   // Tên file lưu trong MinIO (dùng để commit sau)
    string PreviewUrl  // URL dạng .../draft/xxx.jpg để Frontend hiển thị preview
);
