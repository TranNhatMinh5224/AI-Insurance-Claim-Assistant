using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Admin.Packages.GetAllPackages;

internal sealed class GetAllPackagesQueryHandler 
    : IRequestHandler<GetAllPackagesQuery, Result<List<GetAllPackagesResponse>>>
{
    private readonly IInsurancePackageRepository _packageRepo;

    public GetAllPackagesQueryHandler(IInsurancePackageRepository packageRepo)
    {
        _packageRepo = packageRepo;
    }

    public async Task<Result<List<GetAllPackagesResponse>>> Handle(GetAllPackagesQuery request, CancellationToken ct)
    {
        var packages = await _packageRepo.GetAllAsync(ct);

        var response = packages.Select(p => new GetAllPackagesResponse(
            p.Id,
            p.Name,
            p.Description,
            p.BasePrice,
            p.CoverageDescription,
            p.IsActive,
            p.CreatedAt
        )).ToList();

        return Result<List<GetAllPackagesResponse>>.Success(response);
    }
}
