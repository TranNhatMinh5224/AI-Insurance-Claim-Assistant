using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Staff.Claims.GetAllClaims;

public sealed record GetAllClaimsQuery() : IRequest<Result<List<GetAllClaimsResponse>>>;
