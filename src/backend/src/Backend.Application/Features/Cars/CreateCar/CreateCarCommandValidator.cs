using FluentValidation;

namespace Backend.Application.Features.Cars.CreateCar;

public sealed class CreateCarCommandValidator : AbstractValidator<CreateCarCommand>
{
    public CreateCarCommandValidator()
    {
        RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("Biển số xe không được để trống.")
            .MaximumLength(20).WithMessage("Biển số không được vượt quá 20 ký tự.")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("Biển số chỉ chứa chữ cái hoa, số và dấu gạch ngang.");

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Hãng xe không được để trống.")
            .MaximumLength(100);

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Dòng xe không được để trống.")
            .MaximumLength(100);

        RuleFor(x => x.ManufacturingYear)
            .InclusiveBetween(1980, DateTime.UtcNow.Year)
            .WithMessage($"Năm sản xuất phải từ 1980 đến {DateTime.UtcNow.Year}.");
    }
}
