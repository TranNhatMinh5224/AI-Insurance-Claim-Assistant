using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<CustomerDocument> CustomerDocuments { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<InsurancePackage> InsurancePackages { get; set; }
    public DbSet<PolicyTerm> PolicyTerms { get; set; }
    public DbSet<InsurancePolicy> InsurancePolicies { get; set; }
    public DbSet<ClaimRequest> ClaimRequests { get; set; }
    public DbSet<ClaimEvidence> ClaimEvidences { get; set; }
    public DbSet<ClaimAiReport> ClaimAiReports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
