using FluentValidation;

namespace Backend.Application.Features.Auth.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ tên không được để trống")
            .MinimumLength(2).WithMessage("Họ tên phải có ít nhất 2 ký tự")
            .MaximumLength(100).WithMessage("Họ tên không được vượt quá 100 ký tự");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Email không đúng định dạng")
            .MaximumLength(255).WithMessage("Email không được vượt quá 255 ký tự");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu không được để trống")
            .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự")
            .Matches("[A-Z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ hoa")
            .Matches("[a-z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ thường")
            .Matches("[0-9]").WithMessage("Mật khẩu phải có ít nhất 1 chữ số");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Xác nhận mật khẩu không được để trống")
            .Equal(x => x.Password).WithMessage("Xác nhận mật khẩu không khớp");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^(0[3|5|7|8|9])+([0-9]{8})$")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Số điện thoại không đúng định dạng Việt Nam (VD: 0912345678)");
    }
}
