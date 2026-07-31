using Backend.Domain.Common;
using MediatR;

namespace Backend.Application.Features.Admin.Packages.DeactivatePackage;

/// <summary>
/// Khóa gói bảo hiểm — gói sẽ bị ẩn, Customer mới không thể mua.
/// Các hợp đồng cũ đang dùng gói này KHÔNG bị ảnh hưởng (Price Versioning).
/// PackageId lấy từ Route, không cần body.
/// </summary>
public sealed record DeactivatePackageCommand(Guid PackageId)
    : IRequest<Result<DeactivatePackageResponse>>;
