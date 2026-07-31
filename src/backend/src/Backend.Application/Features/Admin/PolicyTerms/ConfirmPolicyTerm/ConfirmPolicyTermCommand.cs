using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Admin.PolicyTerms.ConfirmPolicyTerm;

/// <summary>
/// Bước 2: Admin xác nhận điều khoản sau khi đọc preview PDF.
/// Hệ thống commit file từ draft → real và tạo record PolicyTerm trong DB.
/// Sau này khi AI xử lý xong OCR, Admin sẽ gọi thêm endpoint ConfirmEmbedding.
/// </summary>
public sealed record ConfirmPolicyTermCommand(
    Guid PackageId,
    string Version,
    string DraftFileName   // Tên file nhận từ bước Upload Draft
) : IRequest<Result<ConfirmPolicyTermResponse>>;
