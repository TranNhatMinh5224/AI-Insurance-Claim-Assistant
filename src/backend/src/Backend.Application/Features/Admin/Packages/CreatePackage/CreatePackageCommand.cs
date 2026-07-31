using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Admin.Packages.CreatePackage;

public sealed record CreatePackageCommand(
    string Name,
    string Description,
    decimal BasePrice,
    string CoverageDescription
) : IRequest<Result<CreatePackageResponse>>;
