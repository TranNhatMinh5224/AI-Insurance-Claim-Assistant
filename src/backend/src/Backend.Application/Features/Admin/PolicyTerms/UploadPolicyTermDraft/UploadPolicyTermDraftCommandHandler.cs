using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Admin.PolicyTerms.UploadPolicyTermDraft;

internal sealed class UploadPolicyTermDraftCommandHandler
    : IRequestHandler<UploadPolicyTermDraftCommand, Result<UploadPolicyTermDraftResponse>>
{
    private const string BucketName = "policy-terms";
    private readonly IFileStorageService _fileStorage;
    private readonly IInsurancePackageRepository _packageRepo;

    public UploadPolicyTermDraftCommandHandler(
        IFileStorageService fileStorage,
        IInsurancePackageRepository packageRepo)
    {
        _fileStorage = fileStorage;
        _packageRepo = packageRepo;
    }

    public async Task<Result<UploadPolicyTermDraftResponse>> Handle(
        UploadPolicyTermDraftCommand request, CancellationToken ct)
    {
        // Kiểm tra PackageId hợp lệ
        var package = await _packageRepo.GetByIdAsync(request.PackageId, ct);
        if (package is null)
            return Result<UploadPolicyTermDraftResponse>.Failure(
                Error.NotFound("Package.NotFound", $"Không tìm thấy gói bảo hiểm với ID '{request.PackageId}'."));

        if (!package.IsActive)
            return Result<UploadPolicyTermDraftResponse>.Failure(
                Error.Conflict("Package.Inactive", "Không thể thêm điều khoản vào gói bảo hiểm đã bị vô hiệu hóa."));

        var extension = Path.GetExtension(request.PdfFile.FileName);
        var uniqueFileName = $"pkg-{request.PackageId}-v{request.Version}-{Guid.NewGuid()}{extension}";

        await using var stream = request.PdfFile.OpenReadStream();
        var previewUrl = await _fileStorage.UploadFileAsync(
            stream, uniqueFileName, request.PdfFile.ContentType, BucketName, isDraft: true, ct);

        return Result<UploadPolicyTermDraftResponse>.Success(
            new UploadPolicyTermDraftResponse(uniqueFileName, previewUrl, request.PackageId, request.Version));
    }
}
