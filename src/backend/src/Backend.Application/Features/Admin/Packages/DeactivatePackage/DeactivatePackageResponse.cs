namespace Backend.Application.Features.Admin.Packages.DeactivatePackage;

public sealed record DeactivatePackageResponse(
    Guid PackageId,
    string Name,
    bool IsActive     // Luôn là false sau khi deactivate thành công
);
