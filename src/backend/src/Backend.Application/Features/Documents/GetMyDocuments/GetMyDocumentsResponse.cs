namespace Backend.Application.Features.Documents.GetMyDocuments;

public sealed record GetMyDocumentsResponse(
    Guid Id,
    string DocumentType,
    string FileUrl,
    string Status,
    string? OcrData,
    DateTime CreatedAt
);
