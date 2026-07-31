using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public sealed class ClaimAiReportConfiguration : IEntityTypeConfiguration<ClaimAiReport>
{
    public void Configure(EntityTypeBuilder<ClaimAiReport> builder)
    {
        builder.ToTable("ClaimAiReports");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.FraudAnalysis).HasMaxLength(2000);
        builder.Property(x => x.DamageAnalysis).HasMaxLength(2000);
        builder.Property(x => x.LogicAnalysis).HasMaxLength(2000);
        builder.Property(x => x.PolicyMatched).HasMaxLength(2000);
        builder.Property(x => x.FinalReportSummary).HasMaxLength(4000);
        
        // One to One
        builder.HasIndex(x => x.ClaimRequestId).IsUnique();
        builder.HasOne<ClaimRequest>().WithMany().HasForeignKey(x => x.ClaimRequestId).OnDelete(DeleteBehavior.Cascade);
    }
}
