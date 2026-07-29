using FluentValidation;

namespace Backend.Application.Features.Auth.RefreshToken;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access Token không được để trống");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh Token không được để trống");
    }
}
