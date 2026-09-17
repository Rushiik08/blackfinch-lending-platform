namespace Blackfinch.Lending.Api.Contracts;

public sealed class SubmitApplicationResponse
{
    public Guid Id { get; init; }

    public decimal LoanAmount { get; init; }

    public decimal AssetValue { get; init; }

    public int CreditScore { get; init; }

    public decimal LtvPercent { get; init; }

    public required string Decision { get; init; }

    public string? DeclineReason { get; init; }

    public required PlatformMetricsResponse Metrics { get; init; }
}
