using Backend.Application.Features.Auth.Register;
using Backend.Domain.Common;
using Backend.WebApi.Common;
using Backend.WebApi.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Backend.UnitTests.Controllers;

/// <summary>
/// Unit Tests cho AuthController
/// 
/// Kịch bản kiểm thử (Test Scenarios):
/// ✅ TC-C01: Handler trả về Success → Controller phải trả về 200 OK
/// ✅ TC-C02: Handler trả về Success → Response body có success = true
/// ✅ TC-C03: Handler trả về Success → Response data chứa UserId
/// ❌ TC-C04: Handler trả về Failure → Controller phải trả về 400 BadRequest
/// ❌ TC-C05: Handler trả về Failure → Response body có success = false
/// ❌ TC-C06: Handler trả về Failure → Response chứa error message
/// </summary>
public class AuthControllerTests
{
    private readonly Mock<ISender> _senderMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _senderMock = new Mock<ISender>();
        _controller = new AuthController(_senderMock.Object);
    }

    private static RegisterCommand ValidCommand() =>
        new("Nguyễn A", "a@gmail.com", "Abc@1234", "Abc@1234", null);

    private static RegisterResponse ValidResponse() =>
        new(Guid.NewGuid(), "a@gmail.com", "Nguyễn A");

    // ──────────────────────────────────────────────────────
    // TC-C01: Success → 200 OK
    // ──────────────────────────────────────────────────────
    [Fact(DisplayName = "TC-C01: Handler Success → Controller phải trả về 200 OK")]
    public async Task Register_WhenHandlerReturnsSuccess_ShouldReturn200Ok()
    {
        // Arrange
        _senderMock
            .Setup(s => s.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RegisterResponse>.Success(ValidResponse()));

        // Act
        var actionResult = await _controller.Register(ValidCommand(), CancellationToken.None);

        // Assert
        actionResult.Should().BeOfType<OkObjectResult>("Handler thành công phải trả về HTTP 200");
        var okResult = (OkObjectResult)actionResult;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    // ──────────────────────────────────────────────────────
    // TC-C02: Success → Response có success = true
    // ──────────────────────────────────────────────────────
    [Fact(DisplayName = "TC-C02: Handler Success → Response JSON có success = true")]
    public async Task Register_WhenHandlerReturnsSuccess_ResponseShouldHaveSuccessTrue()
    {
        // Arrange
        _senderMock
            .Setup(s => s.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RegisterResponse>.Success(ValidResponse()));

        // Act
        var actionResult = await _controller.Register(ValidCommand(), CancellationToken.None);

        // Assert
        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<RegisterResponse>>().Subject;
        apiResponse.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
    }

    // ──────────────────────────────────────────────────────
    // TC-C03: Success → Response chứa UserId
    // ──────────────────────────────────────────────────────
    [Fact(DisplayName = "TC-C03: Handler Success → Response.Data chứa UserId hợp lệ")]
    public async Task Register_WhenHandlerReturnsSuccess_DataShouldContainUserId()
    {
        // Arrange
        var expectedResponse = ValidResponse();
        _senderMock
            .Setup(s => s.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RegisterResponse>.Success(expectedResponse));

        // Act
        var actionResult = await _controller.Register(ValidCommand(), CancellationToken.None);

        // Assert
        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<RegisterResponse>>().Subject;
        apiResponse.Data!.UserId.Should().Be(expectedResponse.UserId);
    }

    // ──────────────────────────────────────────────────────
    // TC-C04: Failure → 400 BadRequest
    // ──────────────────────────────────────────────────────
    [Fact(DisplayName = "TC-C04: Handler Failure → Controller phải trả về 400 BadRequest")]
    public async Task Register_WhenHandlerReturnsFailure_ShouldReturn400BadRequest()
    {
        // Arrange
        var error = Error.Conflict("User.EmailAlreadyExists", "Email đã được sử dụng");
        _senderMock
            .Setup(s => s.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RegisterResponse>.Failure(error));

        // Act
        var actionResult = await _controller.Register(ValidCommand(), CancellationToken.None);

        // Assert
        actionResult.Should().BeOfType<BadRequestObjectResult>(
            "Handler thất bại phải trả về HTTP 400 Bad Request");
        var badResult = (BadRequestObjectResult)actionResult;
        badResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    // ──────────────────────────────────────────────────────
    // TC-C05: Failure → Response có success = false
    // ──────────────────────────────────────────────────────
    [Fact(DisplayName = "TC-C05: Handler Failure → Response JSON có success = false")]
    public async Task Register_WhenHandlerReturnsFailure_ResponseShouldHaveSuccessFalse()
    {
        // Arrange
        var error = Error.Conflict("User.EmailAlreadyExists", "Email đã được sử dụng");
        _senderMock
            .Setup(s => s.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RegisterResponse>.Failure(error));

        // Act
        var actionResult = await _controller.Register(ValidCommand(), CancellationToken.None);

        // Assert
        var badResult = actionResult.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badResult.Value.Should().BeOfType<ApiResponse<RegisterResponse>>().Subject;
        apiResponse.Success.Should().BeFalse();
        apiResponse.Data.Should().BeNull();
    }

    // ──────────────────────────────────────────────────────
    // TC-C06: Failure → Response chứa error message
    // ──────────────────────────────────────────────────────
    [Fact(DisplayName = "TC-C06: Handler Failure → Response.Message chứa thông báo lỗi")]
    public async Task Register_WhenHandlerReturnsFailure_ShouldContainErrorMessage()
    {
        // Arrange
        const string errorMessage = "Email đã được sử dụng";
        var error = Error.Conflict("User.EmailAlreadyExists", errorMessage);
        _senderMock
            .Setup(s => s.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<RegisterResponse>.Failure(error));

        // Act
        var actionResult = await _controller.Register(ValidCommand(), CancellationToken.None);

        // Assert
        var badResult = actionResult.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badResult.Value.Should().BeOfType<ApiResponse<RegisterResponse>>().Subject;
        apiResponse.Message.Should().Be(errorMessage,
            "Thông báo lỗi phải được truyền chính xác từ Handler đến Response");
    }
}
