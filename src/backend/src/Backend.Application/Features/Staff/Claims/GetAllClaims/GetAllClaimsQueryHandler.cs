using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Staff.Claims.GetAllClaims;

internal sealed class GetAllClaimsQueryHandler 
    : IRequestHandler<GetAllClaimsQuery, Result<List<GetAllClaimsResponse>>>
{
    private readonly IClaimRepository _claimRepo;

    public GetAllClaimsQueryHandler(IClaimRepository claimRepo)
    {
        _claimRepo = claimRepo;
    }

    public async Task<Result<List<GetAllClaimsResponse>>> Handle(GetAllClaimsQuery request, CancellationToken ct)
    {
        var claims = await _claimRepo.GetAllAsync(ct);

        var response = claims.Select(c => new GetAllClaimsResponse(
            c.Id,
            c.InsurancePolicyId,
            c.IncidentDescription,
            c.Status.ToString(),
            c.AssignedStaffId,
            c.CreatedAt
        )).ToList();

        return Result<List<GetAllClaimsResponse>>.Success(response);
    }
}
