namespace Blackfinch.Lending.Api.Contracts;

public sealed class PlatformMetricsResponse
{
    public int SuccessfulApplicants { get; init; }

    public int DeclinedApplicants { get; init; }

    public int TotalApplicants { get; init; }

    public decimal TotalValueOfLoansWritten { get; init; }

    public decimal MeanAverageLtv { get; init; }
}
