using Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Backend.Application.Features.Documents.UploadDraft;

/// <summary>
/// Nhận file ảnh, ném vào MinIO folder "draft/", trả về URL preview.
/// Database KHÔNG được đụng đến ở bước này.
/// </summary>
public sealed record UploadDocumentDraftCommand(
    IFormFile File
) : IRequest<Result<UploadDocumentDraftResponse>>;
