using Backend.Application.Abstractions;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using MediatR;

namespace Backend.Application.Features.Staff.Policies.RejectPolicy;

internal sealed class RejectPolicyCommandHandler 
    : IRequestHandler<RejectPolicyCommand, Result<RejectPolicyResponse>>
{
    private readonly IInsurancePolicyRepository _policyRepo;
    private readonly IUnitOfWork _unitOfWork;

    public RejectPolicyCommandHandler(IInsurancePolicyRepository policyRepo, IUnitOfWork unitOfWork)
    {
        _policyRepo = policyRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RejectPolicyResponse>> Handle(RejectPolicyCommand request, CancellationToken ct)
    {
        var policy = await _policyRepo.GetByIdAsync(request.PolicyId, ct);

        if (policy is null)
            return Result<RejectPolicyResponse>.Failure(
                Error.NotFound("Policy.NotFound", "Không tìm thấy hợp đồng bảo hiểm."));

        if (policy.Status != PolicyStatus.PendingApproval)
            return Result<RejectPolicyResponse>.Failure(
                Error.Conflict("Policy.NotPending", "Hợp đồng không ở trạng thái chờ duyệt."));

        // Domain method: Chuyển Rejected
        policy.RejectPolicy();

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<RejectPolicyResponse>.Success(new RejectPolicyResponse(
            policy.Id,
            policy.Status.ToString()
        ));
    }
}
