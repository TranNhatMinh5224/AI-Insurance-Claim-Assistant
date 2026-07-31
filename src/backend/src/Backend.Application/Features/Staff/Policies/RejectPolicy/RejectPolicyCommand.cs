using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Staff.Policies.RejectPolicy;

public sealed record RejectPolicyCommand(Guid PolicyId) : IRequest<Result<RejectPolicyResponse>>;
