using Backend.Application.Abstractions;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using MediatR;

namespace Backend.Application.Features.Staff.Policies.ApprovePolicy;

internal sealed class ApprovePolicyCommandHandler 
    : IRequestHandler<ApprovePolicyCommand, Result<ApprovePolicyResponse>>
{
    private readonly IInsurancePolicyRepository _policyRepo;
    private readonly IUnitOfWork _unitOfWork;

    public ApprovePolicyCommandHandler(IInsurancePolicyRepository policyRepo, IUnitOfWork unitOfWork)
    {
        _policyRepo = policyRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ApprovePolicyResponse>> Handle(ApprovePolicyCommand request, CancellationToken ct)
    {
        var policy = await _policyRepo.GetByIdAsync(request.PolicyId, ct);

        if (policy is null)
            return Result<ApprovePolicyResponse>.Failure(
                Error.NotFound("Policy.NotFound", "Không tìm thấy hợp đồng bảo hiểm."));

        if (policy.Status != PolicyStatus.PendingApproval)
            return Result<ApprovePolicyResponse>.Failure(
                Error.Conflict("Policy.NotPending", "Hợp đồng không ở trạng thái chờ duyệt."));

        // TODO (Epic Nâng cao): Gọi service sinh file PDF E-Policy thật sự.
        // Tạm thời mock một URL để trả về.
        string mockPdfUrl = $"https://s3.insurance.local/policies/e-policy-{policy.Id}.pdf";

        // Domain method: Chuyển Active, set StartDate, EndDate (mặc định 12 tháng)
        policy.ActivatePolicy(mockPdfUrl, durationMonths: 12);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<ApprovePolicyResponse>.Success(new ApprovePolicyResponse(
            policy.Id,
            policy.Status.ToString(),
            policy.EPolicyPdfUrl!,
            policy.StartDate,
            policy.EndDate
        ));
    }
}
