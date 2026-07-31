using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Claims.GetMyClaims;

public sealed record GetMyClaimsQuery() : IRequest<Result<List<GetMyClaimsResponse>>>;
