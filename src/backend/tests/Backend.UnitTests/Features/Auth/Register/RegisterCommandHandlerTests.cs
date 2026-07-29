using Backend.Application.Abstractions;
using Backend.Application.Features.Auth.Register;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Backend.UnitTests.Features.Auth.Register;

/// <summary>
/// Unit Tests cho RegisterCommandHandler
/// 
/// Kịch bản kiểm thử (Test Scenarios):
/// ✅ TC-01: Đăng ký thành công với dữ liệu hợp lệ
/// ✅ TC-02: Đăng ký thành công → Password phải được hash (không lưu plain text)
/// ✅ TC-03: Đăng ký thành công → AddAsync phải được gọi đúng 1 lần
/// ✅ TC-04: Đăng ký thành công → SaveChanges phải được gọi đúng 1 lần
/// ✅ TC-05: Đăng ký thành công → Response chứa đúng Email và FullName
/// ❌ TC-06: Email đã tồn tại → Phải trả về Failure
/// ❌ TC-07: Email đã tồn tại → Error Code phải là "User.EmailAlreadyExists"
/// ❌ TC-08: Email đã tồn tại → AddAsync KHÔNG được gọi
/// ❌ TC-09: Email đã tồn tại → SaveChanges KHÔNG được gọi
/// </summary>
public class RegisterCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RegisterCommandHandler _handler;

    // ──────────────────────────────────────────────────────
    // Setup: Khởi tạo mocks và handler trước mỗi test
    // ──────────────────────────────────────────────────────
    public RegisterCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new RegisterCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _unitOfWorkMock.Object);
    }

    // ──────────────────────────────────────────────────────
    // Helper: Tạo command hợp lệ cho các test
    // ──────────────────────────────────────────────────────
    private static RegisterCommand CreateValidCommand(string email = "test@gmail.com") =>
        new(
            FullName: "Nguyễn Văn A",
            Email: email,
            Password: "Abc@1234",
            ConfirmPassword: "Abc@1234",
            PhoneNumber: "0912345678"
        );

    // ──────────────────────────────────────────────────────
    // TC-01: Happy Path — Đăng ký thành công
    // ──────────────────────────────────────────────────────
    [Fact(DisplayName = "TC-01: Đăng ký với dữ liệu hợp lệ → phải trả về IsSuccess = true")]
    public async Task Handle_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.IsEmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(h => h.Hash(It.IsAny<string>()))
            .Returns("hashed_password_123");

        var command = CreateValidCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue("vì email chưa tồn tại và dữ liệu hợp lệ");
        result.IsFailure.Should().BeFalse();
        result.Value.Should().NotBeNull();
    }

    // ──────────────────────────────────────────────────────
    // TC-02: Password phải được hash, không lưu plain text
    // ──────────────────────────────────────────────────────
    [Fact(DisplayName = "TC-02: Đăng ký thành công → Password PHẢI được hash qua IPasswordHasher")]
    public async Task Handle_WhenSuccess_ShouldHashPassword()
    {
        // Arrange
        const string plainPassword = "Abc@1234";
        const string expectedHash = "bcrypt_hashed_value_xyz";

        _userRepositoryMock
            .Setup(r => r.IsEmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(h => h.Hash(plainPassword))
            .Returns(expectedHash);

        var command = CreateValidCommand();

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _passwordHasherMock.Verify(
            h => h.Hash(plainPassword),
            Times.Once,
            "IPasswordHasher.Hash() phải được gọi đúng 1 lần với plain password");
    }

    // ──────────────────────────────────────────────────────
    // TC-03: AddAsync phải được gọi đúng 1 lần
    // ──────────────────────────────────────────────────────
    [Fact(DisplayName = "TC-03: Đăng ký thành công → IUserRepository.AddAsync phải được gọi 1 lần")]
    public async Task Handle_WhenSuccess_ShouldCallAddAsyncOnce()
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.IsEmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(h => h.Hash(It.IsAny<string>()))
            .Returns("hashed");

        // Act
        await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once,
            "User phải được lưu vào DB đúng 1 lần");
    }

    // ──────────────────────────────────────────────────────
    // TC-04: SaveChanges phải được gọi để commit transaction
    // ──────────────────────────────────────────────────────
    [Fact(DisplayName = "TC-04: Đăng ký thành công → IUnitOfWork.SaveChangesAsync phải được gọi 1 lần")]
    public async Task Handle_WhenSuccess_ShouldCallSaveChangesOnce()
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.IsEmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(h => h.Hash(It.IsAny<string>()))
            .Returns("hashed");

        // Act
        await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once,
            "SaveChangesAsync phải được gọi để persist dữ liệu");
    }

    // ──────────────────────────────────────────────────────
    // TC-05: Response chứa đúng thông tin user
    // ──────────────────────────────────────────────────────
    [Fact(DisplayName = "TC-05: Đăng ký thành công → Response chứa đúng Email và FullName")]
    public async Task Handle_WhenSuccess_ResponseShouldContainCorrectData()
    {
        // Arrange
        const string email = "nguyen.a@gmail.com";
        const string fullName = "Nguyễn Văn A";

        _userRepositoryMock
            .Setup(r => r.IsEmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(h => h.Hash(It.IsAny<string>()))
            .Returns("hashed");

        var command = new RegisterCommand(fullName, email, "Abc@1234", "Abc@1234", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Value!.Email.Should().Be(email.ToLowerInvariant(),
            "Email phải được chuẩn hóa về lowercase");
        result.Value.FullName.Should().Be(fullName);
        result.Value.UserId.Should().NotBeEmpty("UserId phải là một Guid hợp lệ");
    }

    // ──────────────────────────────────────────────────────
    // TC-06: Email đã tồn tại → Phải trả về Failure
    // ──────────────────────────────────────────────────────
    [Fact(DisplayName = "TC-06: Email đã tồn tại → phải trả về IsFailure = true")]
    public async Task Handle_WhenEmailAlreadyExists_ShouldReturnFailure()
    {
        // Arrange — Email đã có trong DB
        _userRepositoryMock
            .Setup(r => r.IsEmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); // ← Email đã tồn tại

        // Act
        var result = await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue("email đã tồn tại phải dẫn đến Failure");
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
    }

    // ──────────────────────────────────────────────────────
    // TC-07: Error Code phải chính xác
    // ──────────────────────────────────────────────────────
    [Fact(DisplayName = "TC-07: Email đã tồn tại → Error.Code phải là 'User.EmailAlreadyExists'")]
    public async Task Handle_WhenEmailAlreadyExists_ShouldReturnCorrectErrorCode()
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.IsEmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        // Assert
        result.Error.Code.Should().Be("User.EmailAlreadyExists",
            "Error Code phải khớp để frontend có thể xử lý đúng loại lỗi");
    }

    // ──────────────────────────────────────────────────────
    // TC-08: Email đã tồn tại → KHÔNG được lưu vào DB
    // ──────────────────────────────────────────────────────
    [Fact(DisplayName = "TC-08: Email đã tồn tại → AddAsync KHÔNG được gọi")]
    public async Task Handle_WhenEmailAlreadyExists_ShouldNotCallAddAsync()
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.IsEmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "Không được persist user khi email đã tồn tại");
    }

    // ──────────────────────────────────────────────────────
    // TC-09: Email đã tồn tại → KHÔNG commit transaction
    // ──────────────────────────────────────────────────────
    [Fact(DisplayName = "TC-09: Email đã tồn tại → SaveChangesAsync KHÔNG được gọi")]
    public async Task Handle_WhenEmailAlreadyExists_ShouldNotCallSaveChanges()
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.IsEmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never,
            "Không được commit DB transaction khi có lỗi");
    }
}
