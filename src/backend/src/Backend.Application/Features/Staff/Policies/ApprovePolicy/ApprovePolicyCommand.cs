using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Staff.Policies.ApprovePolicy;

public sealed record ApprovePolicyCommand(Guid PolicyId) : IRequest<Result<ApprovePolicyResponse>>;
