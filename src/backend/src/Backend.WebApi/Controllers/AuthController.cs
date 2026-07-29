using Backend.Application.Features.Auth.Login;
using Backend.Application.Features.Auth.Register;
using Backend.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WebApi.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Đăng ký tài khoản khách hàng mới
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<RegisterResponse>.FailureResult(result.Error.Message));

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<RegisterResponse>.SuccessResult(
                "Đăng ký tài khoản thành công!",
                result.Value));
    }

    /// <summary>
    /// Đăng nhập — trả về JWT Access Token
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            // Validation error (email rỗng, sai format) → 400 Bad Request
            // Auth error (sai credentials) → 401 Unauthorized
            return result.Error.Code.StartsWith("Validation")
                ? BadRequest(ApiResponse<LoginResponse>.FailureResult(result.Error.Message))
                : Unauthorized(ApiResponse<LoginResponse>.FailureResult(result.Error.Message));
        }

        return Ok(ApiResponse<LoginResponse>.SuccessResult(
            "Đăng nhập thành công!",
            result.Value));
    }

    /// <summary>
    /// Làm mới Access Token khi token cũ đã hoặc sắp hết hạn
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<Backend.Application.Features.Auth.RefreshToken.RefreshTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] Backend.WebApi.Controllers.Requests.RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        // Gán tay thủ công từ Request -> Command (Rule G2)
        var command = new Backend.Application.Features.Auth.RefreshToken.RefreshTokenCommand(
            AccessToken: request.AccessToken,
            RefreshToken: request.RefreshToken
        );

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code.StartsWith("Validation")
                ? BadRequest(ApiResponse<object>.FailureResult(result.Error.Message))
                : Unauthorized(ApiResponse<object>.FailureResult(result.Error.Message));
        }

        return Ok(ApiResponse<Backend.Application.Features.Auth.RefreshToken.RefreshTokenResponse>.SuccessResult(
            "Làm mới token thành công!",
            result.Value));
    }

    /// <summary>
    /// Gửi link đặt lại mật khẩu qua email
    /// </summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] Backend.WebApi.Controllers.Requests.ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new Backend.Application.Features.Auth.ForgotPassword.ForgotPasswordCommand(
            Email: request.Email,
            FrontendResetUrl: request.FrontendResetUrl
        );

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.FailureResult(result.Error.Message));
        }

        // Dù thành công hay không tìm thấy email, vẫn trả về thông báo chung để bảo mật
        return Ok(ApiResponse<object>.SuccessResult(
            "Nếu email tồn tại trong hệ thống, chúng tôi đã gửi một link đặt lại mật khẩu. Vui lòng kiểm tra hộp thư của bạn.",
            true));
    }

    /// <summary>
    /// Đặt lại mật khẩu mới bằng Token nhận từ email
    /// </summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] Backend.WebApi.Controllers.Requests.ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new Backend.Application.Features.Auth.ResetPassword.ResetPasswordCommand(
            Email: request.Email,
            Token: request.Token,
            NewPassword: request.NewPassword
        );

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.FailureResult(result.Error.Message));
        }

        return Ok(ApiResponse<object>.SuccessResult(
            "Đặt lại mật khẩu thành công! Bạn có thể đăng nhập bằng mật khẩu mới.",
            true));
    }
}
