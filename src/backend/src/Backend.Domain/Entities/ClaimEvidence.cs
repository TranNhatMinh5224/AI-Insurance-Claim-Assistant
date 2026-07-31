using Backend.Domain.Enums;
using System.Text.Json;

namespace Backend.Domain.Entities;

public sealed class ClaimEvidence
{
    private ClaimEvidence() { }

    public Guid Id { get; private set; }
    public Guid ClaimRequestId { get; private set; }
    public EvidenceType EvidenceType { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public string ImageHash { get; private set; } = string.Empty;
    public JsonDocument? ExtractedData { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static ClaimEvidence Create(Guid claimId, EvidenceType type, string imageUrl, string imageHash)
    {
        return new ClaimEvidence
        {
            Id = Guid.NewGuid(),
            ClaimRequestId = claimId,
            EvidenceType = type,
            ImageUrl = imageUrl,
            ImageHash = imageHash,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateExtractedData(JsonDocument data)
    {
        ExtractedData = data;
    }
}
