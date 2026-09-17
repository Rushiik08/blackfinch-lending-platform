using System.ComponentModel.DataAnnotations;

namespace Blackfinch.Lending.Api.Data;

public sealed class LoanApplication
{
    public Guid Id { get; set; }

    public decimal LoanAmount { get; set; }

    public decimal AssetValue { get; set; }

    public int CreditScore { get; set; }

    public decimal LtvPercent { get; set; }

    public bool IsSuccessful { get; set; }

    [MaxLength(200)]
    public string? DeclineReason { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
