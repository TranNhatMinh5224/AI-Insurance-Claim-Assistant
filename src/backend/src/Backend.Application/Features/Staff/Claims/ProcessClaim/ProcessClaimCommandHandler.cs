using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Staff.Claims.ProcessClaim;

internal sealed class ProcessClaimCommandHandler 
    : IRequestHandler<ProcessClaimCommand, Result<ProcessClaimResponse>>
{
    private readonly IClaimRepository _claimRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public ProcessClaimCommandHandler(
        IClaimRepository claimRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _claimRepo = claimRepo;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<ProcessClaimResponse>> Handle(ProcessClaimCommand request, CancellationToken ct)
    {
        var staffId = _currentUser.GetUserIdOrThrow();

        var claim = await _claimRepo.GetByIdAsync(request.ClaimId, ct);
        if (claim is null)
            return Result<ProcessClaimResponse>.Failure(
                Error.NotFound("Claim.NotFound", "Không tìm thấy hồ sơ bồi thường."));

        // Cập nhật trạng thái và ghi chú của nhân viên
        claim.UpdateStatus(request.NewStatus, staffId, request.StaffNote);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<ProcessClaimResponse>.Success(new ProcessClaimResponse(
            claim.Id,
            claim.Status.ToString(),
            staffId,
            claim.StaffNote
        ));
    }
}
