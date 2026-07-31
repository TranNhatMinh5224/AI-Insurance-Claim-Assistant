using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Staff.Policies.GetPendingPolicies;

internal sealed class GetPendingPoliciesQueryHandler 
    : IRequestHandler<GetPendingPoliciesQuery, Result<List<GetPendingPoliciesResponse>>>
{
    private readonly IInsurancePolicyRepository _policyRepo;

    public GetPendingPoliciesQueryHandler(IInsurancePolicyRepository policyRepo)
    {
        _policyRepo = policyRepo;
    }

    public async Task<Result<List<GetPendingPoliciesResponse>>> Handle(GetPendingPoliciesQuery request, CancellationToken ct)
    {
        var policies = await _policyRepo.GetPendingPoliciesAsync(ct);

        var response = policies.Select(p => new GetPendingPoliciesResponse(
            p.Id,
            p.UserId,
            p.CarId,
            p.PackageId,
            p.PremiumAmount,
            p.CreatedAt
        )).ToList();

        return Result<List<GetPendingPoliciesResponse>>.Success(response);
    }
}
