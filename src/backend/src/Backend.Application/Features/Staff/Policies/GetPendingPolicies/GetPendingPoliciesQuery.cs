using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Staff.Policies.GetPendingPolicies;

public sealed record GetPendingPoliciesQuery() : IRequest<Result<List<GetPendingPoliciesResponse>>>;
