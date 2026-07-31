using Backend.Application.Abstractions;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using MediatR;

namespace Backend.Application.Features.Admin.Packages.CreatePackage;

internal sealed class CreatePackageCommandHandler
    : IRequestHandler<CreatePackageCommand, Result<CreatePackageResponse>>
{
    private readonly IInsurancePackageRepository _packageRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePackageCommandHandler(IInsurancePackageRepository packageRepo, IUnitOfWork unitOfWork)
    {
        _packageRepo = packageRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreatePackageResponse>> Handle(CreatePackageCommand request, CancellationToken ct)
    {
        if (await _packageRepo.IsNameExistsAsync(request.Name, ct))
            return Result<CreatePackageResponse>.Failure(
                Error.Conflict("Package.NameExists", $"Gói bảo hiểm tên '{request.Name}' đã tồn tại."));

        var package = InsurancePackage.Create(request.Name, request.Description, request.BasePrice, request.CoverageDescription);

        await _packageRepo.AddAsync(package, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<CreatePackageResponse>.Success(
            new CreatePackageResponse(package.Id, package.Name, package.Description, package.BasePrice, package.CoverageDescription, package.IsActive));
    }
}
