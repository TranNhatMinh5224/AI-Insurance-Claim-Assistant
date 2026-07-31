using FluentValidation;

namespace Backend.Application.Features.Documents.UploadDraft;

public sealed class UploadDocumentDraftCommandValidator : AbstractValidator<UploadDocumentDraftCommand>
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".pdf"];
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    public UploadDocumentDraftCommandValidator()
    {
        RuleFor(x => x.File)
            .NotNull().WithMessage("File không được để trống.");

        RuleFor(x => x.File)
            .Must(f => f.Length > 0).WithMessage("File không được rỗng.")
            .Must(f => f.Length <= MaxFileSizeBytes).WithMessage("File không được vượt quá 10 MB.")
            .Must(f =>
            {
                var ext = Path.GetExtension(f.FileName).ToLowerInvariant();
                return AllowedExtensions.Contains(ext);
            }).WithMessage("Chỉ chấp nhận file ảnh (.jpg, .jpeg, .png) hoặc PDF.")
            .When(x => x.File is not null);
    }
}
