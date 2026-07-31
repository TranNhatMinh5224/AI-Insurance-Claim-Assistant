namespace Backend.Application.Features.Documents.CreateDocument;

public sealed record CreateCustomerDocumentResponse(
    Guid DocumentId,
    string ImageUrl,       // URL bản real (vĩnh viễn)
    string DocumentType,
    string Status
);
