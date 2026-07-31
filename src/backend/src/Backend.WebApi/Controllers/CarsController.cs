using Backend.Application.Features.Cars.CreateCar;
using Backend.Application.Features.Cars.GetMyCars;
using Backend.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WebApi.Controllers;

[ApiController]
[Route("api/v1/cars")]
[Authorize(Roles = Roles.Customer)] // Chỉ Customer đăng ký xe của mình
public sealed class CarsController : ControllerBase
{
    private readonly ISender _sender;

    public CarsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Đăng ký xe cơ giới mới — yêu cầu đăng nhập.
    /// Biển số xe phải là duy nhất trong toàn hệ thống.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreateCarResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<CreateCarResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CreateCarResponse>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterCar(
        [FromBody] CreateCarCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code.StartsWith("Car")
                ? Conflict(ApiResponse<CreateCarResponse>.FailureResult(result.Error.Message))
                : BadRequest(ApiResponse<CreateCarResponse>.FailureResult(result.Error.Message));
        }

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<CreateCarResponse>.SuccessResult("Đăng ký xe thành công!", result.Value));
    }

    /// <summary>
    /// [Customer] Lấy danh sách các xe mà khách hàng đang sở hữu.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<List<GetMyCarsResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyCars(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMyCarsQuery(), cancellationToken);

        return Ok(ApiResponse<List<GetMyCarsResponse>>.SuccessResult(
            "Lấy danh sách xe thành công.", result.Value));
    }
}
