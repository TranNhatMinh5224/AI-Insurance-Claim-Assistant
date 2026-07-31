using Backend.Application.Features.Users.ChangePassword;
using Backend.Application.Features.Users.GetProfile;
using Backend.WebApi.Common;
using Backend.WebApi.Controllers.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WebApi.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize(Roles = Roles.Any)] // Tất cả role đều được xem profile của chính mình
public sealed class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Xem thông tin profile của user đang đăng nhập
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<GetUserProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUserProfileQuery(), cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code.StartsWith("NotFound")
                ? NotFound(ApiResponse<GetUserProfileResponse>.FailureResult(result.Error.Message))
                : Unauthorized(ApiResponse<GetUserProfileResponse>.FailureResult(result.Error.Message));
        }

        return Ok(ApiResponse<GetUserProfileResponse>.SuccessResult("Lấy thông tin thành công", result.Value));
    }

    /// <summary>
    /// Đổi mật khẩu của user đang đăng nhập
    /// </summary>
    [HttpPost("me/change-password")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        // RULE G2: Manual mapping từ Request → Command
        var command = new ChangePasswordCommand(
            CurrentPassword: request.CurrentPassword,
            NewPassword: request.NewPassword,
            ConfirmNewPassword: request.ConfirmNewPassword
        );

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code.StartsWith("Auth")
                ? Unauthorized(ApiResponse<object>.FailureResult(result.Error.Message))
                : BadRequest(ApiResponse<object>.FailureResult(result.Error.Message));
        }

        return Ok(ApiResponse<object>.SuccessResult("Đổi mật khẩu thành công", true));
    }
}
