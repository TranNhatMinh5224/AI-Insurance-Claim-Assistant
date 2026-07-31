using Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Backend.Application.Features.Admin.PolicyTerms.UploadPolicyTermDraft;

/// <summary>
/// Bước 1: Admin upload PDF điều khoản lên MinIO draft/
/// Hệ thống sau đó sẽ OCR text để Admin xem trước và xác nhận.
/// </summary>
public sealed record UploadPolicyTermDraftCommand(
    Guid PackageId,
    string Version,
    IFormFile PdfFile
) : IRequest<Result<UploadPolicyTermDraftResponse>>;
