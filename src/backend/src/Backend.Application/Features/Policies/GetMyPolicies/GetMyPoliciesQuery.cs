using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Policies.GetMyPolicies;

public sealed record GetMyPoliciesQuery() : IRequest<Result<List<GetMyPoliciesResponse>>>;
