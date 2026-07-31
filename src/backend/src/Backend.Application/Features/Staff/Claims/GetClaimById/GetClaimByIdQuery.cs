using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Staff.Claims.GetClaimById;

public sealed record GetClaimByIdQuery(Guid ClaimId) : IRequest<Result<GetClaimByIdResponse>>;
