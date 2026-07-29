using Backend.Application.Features.Auth.Register;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Backend.UnitTests.Features.Auth.Register;

/// <summary>
/// Unit Tests cho RegisterCommandValidator (FluentValidation)
/// 
/// Kịch bản kiểm thử (Test Scenarios):
/// ✅ TC-V01: Dữ liệu hoàn toàn hợp lệ → Không có lỗi
/// ✅ TC-V02: PhoneNumber bỏ trống → Không có lỗi (field optional)
/// ❌ TC-V03: FullName để trống → Có lỗi
/// ❌ TC-V04: FullName quá ngắn (1 ký tự) → Có lỗi
/// ❌ TC-V05: FullName quá dài (>100 ký tự) → Có lỗi
/// ❌ TC-V06: Email để trống → Có lỗi
/// ❌ TC-V07: Email sai định dạng → Có lỗi
/// ❌ TC-V08: Password để trống → Có lỗi
/// ❌ TC-V09: Password quá ngắn (<8 ký tự) → Có lỗi
/// ❌ TC-V10: Password không có chữ hoa → Có lỗi
/// ❌ TC-V11: Password không có chữ thường → Có lỗi
/// ❌ TC-V12: Password không có chữ số → Có lỗi
/// ❌ TC-V13: ConfirmPassword không khớp → Có lỗi
/// ❌ TC-V14: PhoneNumber sai định dạng VN → Có lỗi
/// </summary>
public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    // ──────────────────────────────────────────────────────
    // Helper: Tạo command hợp lệ chuẩn
    // ──────────────────────────────────────────────────────
    private static RegisterCommand ValidCommand() =>
        new(
            FullName: "Nguyễn Văn An",
            Email: "nguyen.an@gmail.com",
            Password: "Abc@1234",
            ConfirmPassword: "Abc@1234",
            PhoneNumber: "0912345678"
        );

    // ══════════════════════════════════════════════════════
    // NHÓM 1: Happy Path (Dữ liệu hợp lệ)
    // ══════════════════════════════════════════════════════

    [Fact(DisplayName = "TC-V01: Dữ liệu hoàn toàn hợp lệ → Validator KHÔNG có lỗi")]
    public void Validate_WithAllValidData_ShouldNotHaveAnyErrors()
    {
        // Act
        var result = _validator.TestValidate(ValidCommand());

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "TC-V02: PhoneNumber để trống → Validator KHÔNG có lỗi (field optional)")]
    public void Validate_WithEmptyPhoneNumber_ShouldNotHaveErrors()
    {
        // Arrange
        var command = ValidCommand() with { PhoneNumber = null };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    // ══════════════════════════════════════════════════════
    // NHÓM 2: Validation FullName
    // ══════════════════════════════════════════════════════

    [Fact(DisplayName = "TC-V03: FullName để trống → Phải có lỗi validation")]
    public void Validate_WhenFullNameEmpty_ShouldHaveError()
    {
        var command = ValidCommand() with { FullName = string.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    [Theory(DisplayName = "TC-V04: FullName 1 ký tự → Phải có lỗi (min 2 ký tự)")]
    [InlineData("A")]
    [InlineData(" ")]
    public void Validate_WhenFullNameTooShort_ShouldHaveError(string fullName)
    {
        var command = ValidCommand() with { FullName = fullName };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    [Fact(DisplayName = "TC-V05: FullName > 100 ký tự → Phải có lỗi")]
    public void Validate_WhenFullNameTooLong_ShouldHaveError()
    {
        var command = ValidCommand() with { FullName = new string('A', 101) };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    // ══════════════════════════════════════════════════════
    // NHÓM 3: Validation Email
    // ══════════════════════════════════════════════════════

    [Fact(DisplayName = "TC-V06: Email để trống → Phải có lỗi validation")]
    public void Validate_WhenEmailEmpty_ShouldHaveError()
    {
        var command = ValidCommand() with { Email = string.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory(DisplayName = "TC-V07: Email sai định dạng → Phải có lỗi")]
    [InlineData("notanemail")]
    [InlineData("missing@")]
    [InlineData("@nodomain.com")]
    [InlineData("spaces in@email.com")]
    public void Validate_WhenEmailInvalidFormat_ShouldHaveError(string invalidEmail)
    {
        var command = ValidCommand() with { Email = invalidEmail };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    // ══════════════════════════════════════════════════════
    // NHÓM 4: Validation Password
    // ══════════════════════════════════════════════════════

    [Fact(DisplayName = "TC-V08: Password để trống → Phải có lỗi")]
    public void Validate_WhenPasswordEmpty_ShouldHaveError()
    {
        var command = ValidCommand() with { Password = string.Empty };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory(DisplayName = "TC-V09: Password < 8 ký tự → Phải có lỗi")]
    [InlineData("Abc1234")]   // 7 ký tự
    [InlineData("Ab1")]       // 3 ký tự
    public void Validate_WhenPasswordTooShort_ShouldHaveError(string shortPassword)
    {
        var command = ValidCommand() with { Password = shortPassword };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory(DisplayName = "TC-V10: Password không có chữ hoa → Phải có lỗi")]
    [InlineData("abc@1234")]     // all lowercase
    [InlineData("abcdef12")]     // no uppercase
    public void Validate_WhenPasswordHasNoUppercase_ShouldHaveError(string password)
    {
        var command = ValidCommand() with { Password = password };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory(DisplayName = "TC-V11: Password không có chữ thường → Phải có lỗi")]
    [InlineData("ABC@1234")]
    [InlineData("ABCDEFGH1")]
    public void Validate_WhenPasswordHasNoLowercase_ShouldHaveError(string password)
    {
        var command = ValidCommand() with { Password = password };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory(DisplayName = "TC-V12: Password không có số → Phải có lỗi")]
    [InlineData("AbcDefGh")]
    [InlineData("Abcdefgh!")]
    public void Validate_WhenPasswordHasNoDigit_ShouldHaveError(string password)
    {
        var command = ValidCommand() with { Password = password };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    // ══════════════════════════════════════════════════════
    // NHÓM 5: Validation ConfirmPassword
    // ══════════════════════════════════════════════════════

    [Fact(DisplayName = "TC-V13: ConfirmPassword không khớp Password → Phải có lỗi")]
    public void Validate_WhenConfirmPasswordDoesNotMatch_ShouldHaveError()
    {
        var command = ValidCommand() with { ConfirmPassword = "DifferentPass@123" };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
    }

    // ══════════════════════════════════════════════════════
    // NHÓM 6: Validation PhoneNumber
    // ══════════════════════════════════════════════════════

    [Theory(DisplayName = "TC-V14: PhoneNumber sai định dạng VN → Phải có lỗi")]
    [InlineData("123456789")]       // không bắt đầu bằng 0
    [InlineData("01234567890")]     // 11 số
    [InlineData("+84912345678")]    // dạng quốc tế
    [InlineData("abcdefghij")]      // không phải số
    [InlineData("0112345678")]      // đầu số không hợp lệ (01x)
    public void Validate_WhenPhoneNumberInvalidVietnameseFormat_ShouldHaveError(string phone)
    {
        var command = ValidCommand() with { PhoneNumber = phone };
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Theory(DisplayName = "TC-V14b: PhoneNumber đúng định dạng VN → Không có lỗi")]
    [InlineData("0912345678")]   // Viettel
    [InlineData("0812345678")]   // Viettel
    [InlineData("0562345678")]   // Vietnamobile
    [InlineData("0712345678")]   // Mobifone
    public void Validate_WhenPhoneNumberValidVietnamese_ShouldNotHaveError(string phone)
    {
        var command = ValidCommand() with { PhoneNumber = phone };
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }
}
