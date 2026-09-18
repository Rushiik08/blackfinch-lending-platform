namespace Blackfinch.Lending.Api.Contracts;

public sealed class SubmitApplicationResponse
{
    public Guid Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string PhoneNumber { get; init; } = string.Empty;

    public decimal LoanAmount { get; init; }

    public decimal AssetValue { get; init; }

    public int CreditScore { get; init; }

    public decimal LtvPercent { get; init; }

    public required string Decision { get; init; }

    public string? DeclineReason { get; init; }

    public required PlatformMetricsResponse Metrics { get; init; }
}
