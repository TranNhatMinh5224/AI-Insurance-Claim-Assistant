using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Admin.Packages.GetAllPackages;

public sealed record GetAllPackagesQuery() : IRequest<Result<List<GetAllPackagesResponse>>>;
