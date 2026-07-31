using Backend.Application.Abstractions;
using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Packages.GetActivePackages;

internal sealed class GetActivePackagesQueryHandler 
    : IRequestHandler<GetActivePackagesQuery, Result<List<GetActivePackagesResponse>>>
{
    private readonly IInsurancePackageRepository _packageRepo;

    public GetActivePackagesQueryHandler(IInsurancePackageRepository packageRepo)
    {
        _packageRepo = packageRepo;
    }

    public async Task<Result<List<GetActivePackagesResponse>>> Handle(
        GetActivePackagesQuery request, CancellationToken ct)
    {
        var packages = await _packageRepo.GetAllActiveAsync(ct);

        var response = packages.Select(p => new GetActivePackagesResponse(
            p.Id,
            p.Name,
            p.Description,
            p.BasePrice,
            p.CoverageDescription
        )).ToList();

        return Result<List<GetActivePackagesResponse>>.Success(response);
    }
}
