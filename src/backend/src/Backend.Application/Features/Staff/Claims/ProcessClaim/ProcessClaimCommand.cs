using Backend.Domain.Common;
using Backend.Domain.Enums;
using MediatR;

namespace Backend.Application.Features.Staff.Claims.ProcessClaim;

public sealed record ProcessClaimCommand(
    Guid ClaimId,
    ClaimStatus NewStatus,
    string? StaffNote
) : IRequest<Result<ProcessClaimResponse>>;
