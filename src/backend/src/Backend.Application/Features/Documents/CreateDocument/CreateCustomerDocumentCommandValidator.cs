using Backend.Domain.Enums;
using FluentValidation;

namespace Backend.Application.Features.Documents.CreateDocument;

public sealed class CreateCustomerDocumentCommandValidator : AbstractValidator<CreateCustomerDocumentCommand>
{
    public CreateCustomerDocumentCommandValidator()
    {
        RuleFor(x => x.DraftFileName)
            .NotEmpty().WithMessage("Tên file nháp không được để trống.")
            .Must(f => !f.Contains("..") && !f.Contains("/") && !f.Contains("\\"))
            .WithMessage("Tên file không hợp lệ.");

        RuleFor(x => x.DocumentType)
            .IsInEnum().WithMessage("Loại giấy tờ không hợp lệ.");
    }
}
