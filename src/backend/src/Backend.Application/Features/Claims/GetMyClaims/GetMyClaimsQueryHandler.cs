using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Claims.GetMyClaims;

internal sealed class GetMyClaimsQueryHandler 
    : IRequestHandler<GetMyClaimsQuery, Result<List<GetMyClaimsResponse>>>
{
    private readonly IClaimRepository _claimRepo;
    private readonly ICurrentUserService _currentUser;

    public GetMyClaimsQueryHandler(IClaimRepository claimRepo, ICurrentUserService currentUser)
    {
        _claimRepo = claimRepo;
        _currentUser = currentUser;
    }

    public async Task<Result<List<GetMyClaimsResponse>>> Handle(GetMyClaimsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.GetUserIdOrThrow();
        var claims = await _claimRepo.GetByUserIdAsync(userId, ct);

        var response = claims.Select(c => new GetMyClaimsResponse(
            c.Id,
            c.InsurancePolicyId,
            c.IncidentDescription,
            c.Status.ToString(),
            c.CreatedAt
        )).ToList();

        return Result<List<GetMyClaimsResponse>>.Success(response);
    }
}
