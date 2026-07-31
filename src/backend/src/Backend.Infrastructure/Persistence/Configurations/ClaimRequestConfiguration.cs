using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public sealed class ClaimRequestConfiguration : IEntityTypeConfiguration<ClaimRequest>
{
    public void Configure(EntityTypeBuilder<ClaimRequest> builder)
    {
        builder.ToTable("ClaimRequests");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.IncidentDescription).IsRequired().HasMaxLength(4000);
        builder.Property(x => x.StaffNote).HasMaxLength(2000);
        
        // Relations
        builder.HasOne<InsurancePolicy>().WithMany().HasForeignKey(x => x.InsurancePolicyId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.AssignedStaffId).OnDelete(DeleteBehavior.SetNull);
    }
}
