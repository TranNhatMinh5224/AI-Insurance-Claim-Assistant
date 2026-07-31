using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Staff.Claims.GetClaimById;

internal sealed class GetClaimByIdQueryHandler 
    : IRequestHandler<GetClaimByIdQuery, Result<GetClaimByIdResponse>>
{
    private readonly IClaimRepository _claimRepo;

    public GetClaimByIdQueryHandler(IClaimRepository claimRepo)
    {
        _claimRepo = claimRepo;
    }

    public async Task<Result<GetClaimByIdResponse>> Handle(GetClaimByIdQuery request, CancellationToken ct)
    {
        var claim = await _claimRepo.GetByIdAsync(request.ClaimId, ct);

        if (claim is null)
            return Result<GetClaimByIdResponse>.Failure(
                Error.NotFound("Claim.NotFound", "Không tìm thấy hồ sơ bồi thường."));

        var evidences = await _claimRepo.GetEvidencesByClaimIdAsync(request.ClaimId, ct);

        var evidenceDtos = evidences.Select(e => new EvidenceDto(
            e.Id,
            e.EvidenceType.ToString(),
            e.ImageUrl,
            e.CreatedAt
        )).ToList();

        var response = new GetClaimByIdResponse(
            claim.Id,
            claim.InsurancePolicyId,
            claim.IncidentDescription,
            claim.Status.ToString(),
            claim.AssignedStaffId,
            claim.StaffNote,
            claim.CreatedAt,
            evidenceDtos
        );

        return Result<GetClaimByIdResponse>.Success(response);
    }
}
