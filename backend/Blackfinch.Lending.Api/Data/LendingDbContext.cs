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
        application.Property(x => x.FullName).HasMaxLength(100).IsRequired();
        application.Property(x => x.Email).HasMaxLength(256).IsRequired();
        application.Property(x => x.PhoneNumber).HasMaxLength(30).IsRequired();
        application.Property(x => x.LoanAmount).HasPrecision(18, 2);
        application.Property(x => x.AssetValue).HasPrecision(18, 2);
        application.Property(x => x.LtvPercent).HasPrecision(18, 6);
    }

    public void EnsureSchemaUpdated()
    {
        Database.EnsureCreated();

        using var connection = Database.GetDbConnection();
        connection.Open();

        using var checkCommand = connection.CreateCommand();
        checkCommand.CommandText = "PRAGMA table_info(LoanApplications);";

        var existingColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        using (var reader = checkCommand.ExecuteReader())
        {
            while (reader.Read())
            {
                existingColumns.Add(reader.GetString(1)); // column name is at index 1
            }
        }

        if (existingColumns.Count == 0)
        {
            return;
        }

        if (!existingColumns.Contains("FullName"))
        {
            using var alterCommand = connection.CreateCommand();
            alterCommand.CommandText = "ALTER TABLE LoanApplications ADD COLUMN FullName TEXT NOT NULL DEFAULT '';";
            alterCommand.ExecuteNonQuery();
        }

        if (!existingColumns.Contains("Email"))
        {
            using var alterCommand = connection.CreateCommand();
            alterCommand.CommandText = "ALTER TABLE LoanApplications ADD COLUMN Email TEXT NOT NULL DEFAULT '';";
            alterCommand.ExecuteNonQuery();
        }

        if (!existingColumns.Contains("PhoneNumber"))
        {
            using var alterCommand = connection.CreateCommand();
            alterCommand.CommandText = "ALTER TABLE LoanApplications ADD COLUMN PhoneNumber TEXT NOT NULL DEFAULT '';";
            alterCommand.ExecuteNonQuery();
        }
    }
}
