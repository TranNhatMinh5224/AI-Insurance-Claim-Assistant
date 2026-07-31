using Backend.Domain.Enums;
using System.Text.Json;

namespace Backend.Domain.Entities;

public sealed class CustomerDocument
{
    private CustomerDocument() { }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DocumentType DocumentType { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public string ImageHash { get; private set; } = string.Empty;
    public JsonDocument? MetadataJson { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static CustomerDocument Create(Guid userId, DocumentType type, string imageUrl, string imageHash)
    {
        return new CustomerDocument
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DocumentType = type,
            ImageUrl = imageUrl,
            ImageHash = imageHash,
            VerificationStatus = VerificationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateMetadata(JsonDocument metadata, VerificationStatus status)
    {
        MetadataJson = metadata;
        VerificationStatus = status;
    }
}
