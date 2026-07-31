namespace Backend.Application.Features.Admin.PolicyTerms.UploadPolicyTermDraft;

public sealed record UploadPolicyTermDraftResponse(
    string DraftFileName,  // Dùng để confirm ở bước 2
    string PreviewUrl,     // URL PDF trong draft/ để Admin xem trước
    Guid PackageId,
    string Version
);
