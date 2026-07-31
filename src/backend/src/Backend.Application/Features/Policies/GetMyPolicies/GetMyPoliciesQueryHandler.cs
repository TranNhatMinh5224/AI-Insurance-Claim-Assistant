using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Policies.GetMyPolicies;

internal sealed class GetMyPoliciesQueryHandler 
    : IRequestHandler<GetMyPoliciesQuery, Result<List<GetMyPoliciesResponse>>>
{
    private readonly IInsurancePolicyRepository _policyRepo;
    private readonly ICurrentUserService _currentUser;

    public GetMyPoliciesQueryHandler(IInsurancePolicyRepository policyRepo, ICurrentUserService currentUser)
    {
        _policyRepo = policyRepo;
        _currentUser = currentUser;
    }

    public async Task<Result<List<GetMyPoliciesResponse>>> Handle(GetMyPoliciesQuery request, CancellationToken ct)
    {
        var userId = _currentUser.GetUserIdOrThrow();
        var policies = await _policyRepo.GetByUserIdAsync(userId, ct);

        var response = policies.Select(p => new GetMyPoliciesResponse(
            p.Id,
            p.CarId,
            p.PackageId,
            p.Status.ToString(),
            p.PremiumAmount,
            p.StartDate,
            p.EndDate,
            p.EPolicyPdfUrl,
            p.CreatedAt
        )).ToList();

        return Result<List<GetMyPoliciesResponse>>.Success(response);
    }
}
