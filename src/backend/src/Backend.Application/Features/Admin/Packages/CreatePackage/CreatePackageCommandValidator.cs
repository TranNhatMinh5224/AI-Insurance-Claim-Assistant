using FluentValidation;

namespace Backend.Application.Features.Admin.Packages.CreatePackage;

public sealed class CreatePackageCommandValidator : AbstractValidator<CreatePackageCommand>
{
    public CreatePackageCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên gói bảo hiểm không được để trống.")
            .MinimumLength(3).WithMessage("Tên gói phải có ít nhất 3 ký tự.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Mô tả không được để trống.")
            .MaximumLength(2000);

        RuleFor(x => x.BasePrice)
            .GreaterThan(0).WithMessage("Phí bảo hiểm phải lớn hơn 0.")
            .LessThanOrEqualTo(1_000_000_000).WithMessage("Phí bảo hiểm không hợp lệ (tối đa 1 tỷ VND).");

        RuleFor(x => x.CoverageDescription)
            .NotEmpty().WithMessage("Mô tả quyền lợi bảo hiểm không được để trống.")
            .MaximumLength(2000);
    }
}
