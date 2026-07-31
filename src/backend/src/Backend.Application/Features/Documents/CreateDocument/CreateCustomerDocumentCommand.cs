using Backend.Domain.Common;
using Backend.Domain.Enums;
using MediatR;

namespace Backend.Application.Features.Documents.CreateDocument;

/// <summary>
/// Xác nhận lưu giấy tờ: copy file từ draft/ sang real/, ghi 1 dòng vào DB.
/// </summary>
public sealed record CreateCustomerDocumentCommand(
    string DraftFileName,   // Tên file đã upload ở bước Draft
    DocumentType DocumentType
) : IRequest<Result<CreateCustomerDocumentResponse>>;
