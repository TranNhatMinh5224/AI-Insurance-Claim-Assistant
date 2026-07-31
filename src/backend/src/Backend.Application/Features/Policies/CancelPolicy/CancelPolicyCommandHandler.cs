using Backend.Application.Abstractions;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using MediatR;

namespace Backend.Application.Features.Policies.CancelPolicy;

internal sealed class CancelPolicyCommandHandler 
    : IRequestHandler<CancelPolicyCommand, Result<CancelPolicyResponse>>
{
    private readonly IInsurancePolicyRepository _policyRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CancelPolicyCommandHandler(
        IInsurancePolicyRepository policyRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _policyRepo = policyRepo;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<CancelPolicyResponse>> Handle(CancelPolicyCommand request, CancellationToken ct)
    {
        var userId = _currentUser.GetUserIdOrThrow();

        var policy = await _policyRepo.GetByIdAsync(request.PolicyId, ct);
        
        if (policy is null || policy.UserId != userId)
            return Result<CancelPolicyResponse>.Failure(
                Error.NotFound("Policy.NotFound", "Không tìm thấy hợp đồng bảo hiểm hợp lệ."));

        if (policy.Status != PolicyStatus.Active)
            return Result<CancelPolicyResponse>.Failure(
                Error.Conflict("Policy.NotActive", "Chỉ có thể hủy những hợp đồng đang có hiệu lực."));

        // Gọi domain method
        policy.CancelPolicy();

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<CancelPolicyResponse>.Success(new CancelPolicyResponse(
            policy.Id,
            policy.Status.ToString()
        ));
    }
}
