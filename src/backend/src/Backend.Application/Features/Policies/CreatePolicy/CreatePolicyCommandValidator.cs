using FluentValidation;

namespace Backend.Application.Features.Policies.CreatePolicy;

public sealed class CreatePolicyCommandValidator : AbstractValidator<CreatePolicyCommand>
{
    public CreatePolicyCommandValidator()
    {
        RuleFor(x => x.CarId)
            .NotEmpty().WithMessage("CarId không được để trống.");

        RuleFor(x => x.PackageId)
            .NotEmpty().WithMessage("PackageId không được để trống.");
    }
}
