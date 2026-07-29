using Backend.Application.Features.Users.GetProfile;
using Backend.WebApi.Common;
using Backend.WebApi.Controllers.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.WebApi.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize] // Yêu cầu phải có JWT Access Token hợp lệ
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
        // Lấy UserId từ JWT Claims
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized(ApiResponse<object>.FailureResult("Invalid token payload"));

        var query = new GetUserProfileQuery(userId);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code.StartsWith("NotFound")
                ? NotFound(ApiResponse<GetUserProfileResponse>.FailureResult(result.Error.Message))
                : BadRequest(ApiResponse<GetUserProfileResponse>.FailureResult(result.Error.Message));
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
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized(ApiResponse<object>.FailureResult("Invalid token payload"));

        // RULE G2: Khởi tạo Command thủ công từ Request (Manual Mapping)
        var command = new Backend.Application.Features.Users.ChangePassword.ChangePasswordCommand(
            UserId: userId,
            CurrentPassword: request.CurrentPassword,
            NewPassword: request.NewPassword,
            ConfirmNewPassword: request.ConfirmNewPassword
        );

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code.StartsWith("Validation")
                ? BadRequest(ApiResponse<object>.FailureResult(result.Error.Message))
                : BadRequest(ApiResponse<object>.FailureResult(result.Error.Message)); // Hoặc xử lý lỗi khác
        }

        return Ok(ApiResponse<object>.SuccessResult("Đổi mật khẩu thành công", true));
    }
}
