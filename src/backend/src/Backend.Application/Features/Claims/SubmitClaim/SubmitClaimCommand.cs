using Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Backend.Application.Features.Claims.SubmitClaim;

public sealed record SubmitClaimCommand(
    Guid PolicyId,
    string IncidentDescription,
    List<IFormFile> EvidenceFiles
) : IRequest<Result<SubmitClaimResponse>>;
