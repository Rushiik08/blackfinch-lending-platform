using System.ComponentModel.DataAnnotations;

namespace Blackfinch.Lending.Api.Data;

public sealed class LoanApplication
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    public decimal LoanAmount { get; set; }

    public decimal AssetValue { get; set; }

    public int CreditScore { get; set; }

    public decimal LtvPercent { get; set; }

    public bool IsSuccessful { get; set; }

    [MaxLength(200)]
    public string? DeclineReason { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
