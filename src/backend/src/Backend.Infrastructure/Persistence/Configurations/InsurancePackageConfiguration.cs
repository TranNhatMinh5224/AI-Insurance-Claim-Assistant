using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Infrastructure.Persistence.Configurations;

public sealed class InsurancePackageConfiguration : IEntityTypeConfiguration<InsurancePackage>
{
    public void Configure(EntityTypeBuilder<InsurancePackage> builder)
    {
        builder.ToTable("InsurancePackages");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.CoverageDescription).HasMaxLength(2000);

        // decimal ánh xạ thành numeric(18,2) trong PostgreSQL
        builder.Property(x => x.BasePrice)
            .IsRequired()
            .HasColumnType("numeric(18,2)");
    }
}
