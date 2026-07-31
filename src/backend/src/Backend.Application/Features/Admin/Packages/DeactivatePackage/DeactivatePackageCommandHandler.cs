using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Admin.Packages.DeactivatePackage;

internal sealed class DeactivatePackageCommandHandler
    : IRequestHandler<DeactivatePackageCommand, Result<DeactivatePackageResponse>>
{
    private readonly IInsurancePackageRepository _packageRepo;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivatePackageCommandHandler(
        IInsurancePackageRepository packageRepo,
        IUnitOfWork unitOfWork)
    {
        _packageRepo = packageRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DeactivatePackageResponse>> Handle(
        DeactivatePackageCommand request, CancellationToken ct)
    {
        var package = await _packageRepo.GetByIdAsync(request.PackageId, ct);

        if (package is null)
            return Result<DeactivatePackageResponse>.Failure(
                Error.NotFound("Package.NotFound", $"Không tìm thấy gói bảo hiểm với ID '{request.PackageId}'."));

        // Kiểm tra nếu đã bị khóa rồi thì không cần làm gì thêm — idempotent
        if (!package.IsActive)
            return Result<DeactivatePackageResponse>.Failure(
                Error.Conflict("Package.AlreadyInactive", $"Gói bảo hiểm '{package.Name}' đã bị khóa trước đó."));

        // Gọi Domain Method — không được sửa thuộc tính trực tiếp (RULE E2)
        package.Deactivate();

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<DeactivatePackageResponse>.Success(
            new DeactivatePackageResponse(package.Id, package.Name, package.IsActive));
    }
}
