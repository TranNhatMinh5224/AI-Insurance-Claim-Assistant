using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Packages.GetActivePackages;

// RULE B1: Query
public sealed record GetActivePackagesQuery() : IRequest<Result<List<GetActivePackagesResponse>>>;
