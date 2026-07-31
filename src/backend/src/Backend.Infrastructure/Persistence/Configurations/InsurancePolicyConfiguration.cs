using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public sealed class InsurancePolicyConfiguration : IEntityTypeConfiguration<InsurancePolicy>
{
    public void Configure(EntityTypeBuilder<InsurancePolicy> builder)
    {
        builder.ToTable("InsurancePolicies");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EPolicyPdfUrl).HasMaxLength(500);

        // Đóng băng giá tại thời điểm ký hợp đồng
        builder.Property(x => x.PremiumAmount)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        // Relations
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Car>().WithMany().HasForeignKey(x => x.CarId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<InsurancePackage>().WithMany().HasForeignKey(x => x.PackageId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<PolicyTerm>().WithMany().HasForeignKey(x => x.PolicyTermId).OnDelete(DeleteBehavior.Restrict);
    }
}
