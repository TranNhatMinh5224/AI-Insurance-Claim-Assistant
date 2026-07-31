using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Policies.CreatePolicy;

public sealed record CreatePolicyCommand(
    Guid CarId,
    Guid PackageId
) : IRequest<Result<CreatePolicyResponse>>;
