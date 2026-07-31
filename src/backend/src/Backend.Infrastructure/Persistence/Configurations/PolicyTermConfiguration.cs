using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public sealed class PolicyTermConfiguration : IEntityTypeConfiguration<PolicyTerm>
{
    public void Configure(EntityTypeBuilder<PolicyTerm> builder)
    {
        builder.ToTable("PolicyTerms");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Version).IsRequired().HasMaxLength(50);
        builder.Property(x => x.PdfUrl).IsRequired().HasMaxLength(500);
        builder.Property(x => x.QdrantCollectionName).HasMaxLength(100);
        
        // FK to InsurancePackage
        builder.HasOne<InsurancePackage>()
               .WithMany()
               .HasForeignKey(x => x.PackageId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
