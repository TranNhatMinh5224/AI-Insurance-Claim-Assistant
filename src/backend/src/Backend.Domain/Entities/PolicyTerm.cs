using Backend.Domain.Enums;

namespace Backend.Domain.Entities;

public sealed class PolicyTerm
{
    private PolicyTerm() { }

    public Guid Id { get; private set; }
    public Guid PackageId { get; private set; }
    public string Version { get; private set; } = string.Empty;
    public string PdfUrl { get; private set; } = string.Empty;
    public string? ExtractedText { get; private set; }
    public EmbeddingStatus EmbeddingStatus { get; private set; }
    public string QdrantCollectionName { get; private set; } = string.Empty;
    public bool IsCurrent { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static PolicyTerm Create(Guid packageId, string version, string pdfUrl)
    {
        return new PolicyTerm
        {
            Id = Guid.NewGuid(),
            PackageId = packageId,
            Version = version,
            PdfUrl = pdfUrl,
            EmbeddingStatus = EmbeddingStatus.PendingReview,
            IsCurrent = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateExtractedText(string text)
    {
        ExtractedText = text;
    }

    public void ConfirmAndEmbed(string collectionName)
    {
        EmbeddingStatus = EmbeddingStatus.ApprovedAndEmbedded;
        QdrantCollectionName = collectionName;
    }

    public void MarkAsOutdated()
    {
        IsCurrent = false;
    }
}
