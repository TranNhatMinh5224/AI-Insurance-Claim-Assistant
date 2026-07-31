using FluentValidation;

namespace Backend.Application.Features.Admin.PolicyTerms.UploadPolicyTermDraft;

public sealed class UploadPolicyTermDraftCommandValidator : AbstractValidator<UploadPolicyTermDraftCommand>
{
    public UploadPolicyTermDraftCommandValidator()
    {
        RuleFor(x => x.PackageId)
            .NotEmpty().WithMessage("PackageId không được để trống.");

        RuleFor(x => x.Version)
            .NotEmpty().WithMessage("Phiên bản không được để trống.")
            .MaximumLength(50).WithMessage("Phiên bản tối đa 50 ký tự. Ví dụ: 2024-v1");

        RuleFor(x => x.PdfFile)
            .NotNull().WithMessage("File PDF không được để trống.")
            .Must(f => f.Length > 0).WithMessage("File PDF không được rỗng.")
            .Must(f => f.Length <= 20 * 1024 * 1024).WithMessage("File PDF không được vượt quá 20 MB.")
            .Must(f => Path.GetExtension(f.FileName).ToLowerInvariant() == ".pdf")
            .WithMessage("Chỉ chấp nhận file PDF.")
            .When(x => x.PdfFile is not null);
    }
}
