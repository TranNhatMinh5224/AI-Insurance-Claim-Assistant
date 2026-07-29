using FluentValidation;

namespace Backend.Application.Features.Auth.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Email không đúng định dạng");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu không được để trống");
        // NOTE: Login KHÔNG validate độ mạnh password (chỉ Register mới làm vậy)
        // Nếu validate format ở đây → account cũ có password yếu sẽ không login được
    }
}
