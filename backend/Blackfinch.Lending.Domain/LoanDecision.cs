namespace Blackfinch.Lending.Domain;

public sealed record LoanDecision(
    LoanDecisionStatus Status,
    decimal LtvPercent,
    string? DeclineReason)
{
    public bool IsSuccessful => Status == LoanDecisionStatus.Successful;
}
