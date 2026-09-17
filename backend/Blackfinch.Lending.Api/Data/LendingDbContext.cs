using Microsoft.EntityFrameworkCore;

namespace Blackfinch.Lending.Api.Data;

public sealed class LendingDbContext : DbContext
{
    public LendingDbContext(DbContextOptions<LendingDbContext> options)
        : base(options)
    {
    }

    public DbSet<LoanApplication> LoanApplications => Set<LoanApplication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var application = modelBuilder.Entity<LoanApplication>();
        application.HasKey(x => x.Id);
        application.Property(x => x.LoanAmount).HasPrecision(18, 2);
        application.Property(x => x.AssetValue).HasPrecision(18, 2);
        application.Property(x => x.LtvPercent).HasPrecision(18, 6);
    }
}
