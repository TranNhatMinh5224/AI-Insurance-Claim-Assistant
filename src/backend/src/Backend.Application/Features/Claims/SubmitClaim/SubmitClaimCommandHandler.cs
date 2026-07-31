using Backend.Application.Abstractions;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Backend.Application.Features.Claims.SubmitClaim;

internal sealed class SubmitClaimCommandHandler 
    : IRequestHandler<SubmitClaimCommand, Result<SubmitClaimResponse>>
{
    private const string BucketName = "claim-evidences";
    private readonly IClaimRepository _claimRepo;
    private readonly IInsurancePolicyRepository _policyRepo;
    private readonly IFileStorageService _fileStorage;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public SubmitClaimCommandHandler(
        IClaimRepository claimRepo,
        IInsurancePolicyRepository policyRepo,
        IFileStorageService fileStorage,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _claimRepo = claimRepo;
        _policyRepo = policyRepo;
        _fileStorage = fileStorage;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<SubmitClaimResponse>> Handle(SubmitClaimCommand request, CancellationToken ct)
    {
        var userId = _currentUser.GetUserIdOrThrow();

        // 1. Kiểm tra hợp đồng hợp lệ
        var policy = await _policyRepo.GetByIdAsync(request.PolicyId, ct);
        if (policy is null || policy.UserId != userId)
            return Result<SubmitClaimResponse>.Failure(
                Error.NotFound("Policy.NotFound", "Không tìm thấy hợp đồng bảo hiểm hợp lệ."));

        if (policy.Status != PolicyStatus.Active)
            return Result<SubmitClaimResponse>.Failure(
                Error.Conflict("Policy.NotActive", "Chỉ có thể yêu cầu bồi thường cho hợp đồng đang có hiệu lực."));

        if (request.EvidenceFiles is null || request.EvidenceFiles.Count == 0)
            return Result<SubmitClaimResponse>.Failure(
                Error.Validation("Claim.NoEvidence", "Phải cung cấp ít nhất một ảnh bằng chứng."));

        // 2. Tạo ClaimRequest
        var claim = ClaimRequest.Create(policy.Id, request.IncidentDescription);
        await _claimRepo.AddRequestAsync(claim, ct);

        // 3. Xử lý ảnh bằng chứng (Upload MinIO)
        int evidenceCount = 0;
        foreach (var file in request.EvidenceFiles)
        {
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            using var stream = file.OpenReadStream();
            
            // Upload thẳng vào thư mục real/ (Không qua bước draft)
            var fileUrl = await _fileStorage.UploadFileAsync(stream, fileName, file.ContentType, BucketName, isDraft: false, ct);
            
            // Tạm thời để trống Hash, hệ thống AI sẽ update sau khi check ảnh
            var evidence = ClaimEvidence.Create(claim.Id, EvidenceType.AccidentScene, fileUrl, imageHash: string.Empty);
            await _claimRepo.AddEvidenceAsync(evidence, ct);
            evidenceCount++;
        }

        await _unitOfWork.SaveChangesAsync(ct);

        // TODO (Epic 5): Bắn event RabbitMQ "ClaimSubmittedEvent" để AI bắt đầu đánh giá

        return Result<SubmitClaimResponse>.Success(new SubmitClaimResponse(
            claim.Id,
            claim.Status.ToString(),
            evidenceCount,
            claim.CreatedAt
        ));
    }
}
