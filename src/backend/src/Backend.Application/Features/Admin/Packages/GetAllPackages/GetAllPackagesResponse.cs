namespace Backend.Application.Features.Admin.Packages.GetAllPackages;

public sealed record GetAllPackagesResponse(
    Guid Id,
    string Name,
    string Description,
    decimal BasePrice,
    string CoverageDescription,
    bool IsActive,
    DateTime CreatedAt
);
