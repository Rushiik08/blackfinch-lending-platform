namespace Blackfinch.Lending.Api.Contracts;

public sealed class ApplicationHistoryItemResponse
{
    public Guid Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public decimal LoanAmount { get; init; }

    public decimal AssetValue { get; init; }

    public int CreditScore { get; init; }

    public decimal LtvPercent { get; init; }

    public required string Decision { get; init; }

    public string? DeclineReason { get; init; }

    public DateTime CreatedAtUtc { get; init; }
}
