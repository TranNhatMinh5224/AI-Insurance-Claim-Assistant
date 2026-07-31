namespace Backend.Application.Features.Admin.PolicyTerms.ConfirmPolicyTerm;

public sealed record ConfirmPolicyTermResponse(
    Guid PolicyTermId,
    Guid PackageId,
    string Version,
    string PdfUrl,
    string EmbeddingStatus
);
