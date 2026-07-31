using Backend.Application.Features.Packages.GetActivePackages;
using Backend.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WebApi.Controllers;

[ApiController]
[Route("api/v1/packages")]
[AllowAnonymous] // Cho phép tất cả mọi người (kể cả chưa đăng nhập) xem danh sách gói bảo hiểm
public sealed class PackagesController : ControllerBase
{
    private readonly ISender _sender;

    public PackagesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lấy danh sách các gói bảo hiểm đang mở bán (IsActive = true).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<GetActivePackagesResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActivePackages(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetActivePackagesQuery(), cancellationToken);

        return Ok(ApiResponse<List<GetActivePackagesResponse>>.SuccessResult(
            "Lấy danh sách gói bảo hiểm thành công", result.Value));
    }
}
