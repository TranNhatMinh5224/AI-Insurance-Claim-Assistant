namespace Backend.Application.Features.Admin.Packages.CreatePackage;

public sealed record CreatePackageResponse(
    Guid PackageId,
    string Name,
    string Description,
    decimal BasePrice,
    string CoverageDescription,
    bool IsActive
);
