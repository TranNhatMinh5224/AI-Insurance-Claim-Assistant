namespace Backend.Application.Features.Packages.GetActivePackages;

public sealed record GetActivePackagesResponse(
    Guid Id,
    string Name,
    string Description,
    decimal BasePrice,
    string CoverageDescription
);
