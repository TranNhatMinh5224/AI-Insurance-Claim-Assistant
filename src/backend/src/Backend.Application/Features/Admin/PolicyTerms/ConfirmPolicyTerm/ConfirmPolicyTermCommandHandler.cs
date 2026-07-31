using Backend.Application.Abstractions;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using MediatR;

namespace Backend.Application.Features.Admin.PolicyTerms.ConfirmPolicyTerm;

internal sealed class ConfirmPolicyTermCommandHandler
    : IRequestHandler<ConfirmPolicyTermCommand, Result<ConfirmPolicyTermResponse>>
{
    private const string BucketName = "policy-terms";
    private readonly IFileStorageService _fileStorage;
    private readonly IPolicyTermRepository _policyTermRepo;
    private readonly IInsurancePackageRepository _packageRepo;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmPolicyTermCommandHandler(
        IFileStorageService fileStorage,
        IPolicyTermRepository policyTermRepo,
        IInsurancePackageRepository packageRepo,
        IUnitOfWork unitOfWork)
    {
        _fileStorage = fileStorage;
        _policyTermRepo = policyTermRepo;
        _packageRepo = packageRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ConfirmPolicyTermResponse>> Handle(ConfirmPolicyTermCommand request, CancellationToken ct)
    {
        // Kiểm tra PackageId hợp lệ
        var package = await _packageRepo.GetByIdAsync(request.PackageId, ct);
        if (package is null)
            return Result<ConfirmPolicyTermResponse>.Failure(
                Error.NotFound("Package.NotFound", "Không tìm thấy gói bảo hiểm."));

        // Commit file từ draft/ → real/
        var realUrl = await _fileStorage.CommitFileAsync(request.DraftFileName, BucketName, ct);

        // Tạo bản ghi PolicyTerm trong DB
        var policyTerm = PolicyTerm.Create(request.PackageId, request.Version, realUrl);

        await _policyTermRepo.AddAsync(policyTerm, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // TODO (Epic 3.2): Bắn event PolicyTermCreatedEvent qua RabbitMQ
        // để Python service OCR và embedding vào Qdrant

        return Result<ConfirmPolicyTermResponse>.Success(new ConfirmPolicyTermResponse(
            policyTerm.Id,
            policyTerm.PackageId,
            policyTerm.Version,
            policyTerm.PdfUrl,
            policyTerm.EmbeddingStatus.ToString()
        ));
    }
}
