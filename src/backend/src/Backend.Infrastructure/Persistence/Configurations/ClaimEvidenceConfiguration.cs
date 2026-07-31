using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public sealed class ClaimEvidenceConfiguration : IEntityTypeConfiguration<ClaimEvidence>
{
    public void Configure(EntityTypeBuilder<ClaimEvidence> builder)
    {
        builder.ToTable("ClaimEvidences");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.ImageUrl).IsRequired().HasMaxLength(500);
        builder.Property(x => x.ImageHash).HasMaxLength(256);
        builder.Property(x => x.ExtractedData).HasColumnType("jsonb");
        
        builder.HasIndex(x => x.ImageHash); // Index for fast cross-claim fraud check
        
        // Relations
        builder.HasOne<ClaimRequest>().WithMany().HasForeignKey(x => x.ClaimRequestId).OnDelete(DeleteBehavior.Cascade);
    }
}
