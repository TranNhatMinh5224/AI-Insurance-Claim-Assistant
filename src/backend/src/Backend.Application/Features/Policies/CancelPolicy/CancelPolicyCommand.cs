using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Policies.CancelPolicy;

public sealed record CancelPolicyCommand(Guid PolicyId) : IRequest<Result<CancelPolicyResponse>>;
