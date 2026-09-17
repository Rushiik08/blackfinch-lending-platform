namespace Blackfinch.Lending.Domain;

/// <summary>
/// Applies the Blackfinch lending rules. Invalid input is rejected as a
/// validation error; failing a lending rule returns Declined.
/// </summary>
public sealed class LoanDecisionService
{
    public const decimal MinimumLoanAmount = 100_000m;
    public const decimal MaximumLoanAmount = 1_500_000m;
    public const decimal LargeLoanThreshold = 1_000_000m;
    public const int MinimumCreditScore = 1;
    public const int MaximumCreditScore = 999;

    public LoanDecision Decide(decimal loanAmount, decimal assetValue, int creditScore)
    {
        EnsureValidApplication(loanAmount, assetValue, creditScore);

        var ltv = CalculateLtv(loanAmount, assetValue);

        if (loanAmount < MinimumLoanAmount)
        {
            return Decline(ltv, "Loan amount is below £100,000.");
        }

        if (loanAmount > MaximumLoanAmount)
        {
            return Decline(ltv, "Loan amount is above £1.5 million.");
        }

        return loanAmount >= LargeLoanThreshold
            ? DecideLargeLoan(ltv, creditScore)
            : DecideStandardLoan(ltv, creditScore);
    }

    public static decimal CalculateLtv(decimal loanAmount, decimal assetValue)
    {
        if (assetValue <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(assetValue),
                "Asset value must be greater than zero to calculate LTV.");
        }

        return loanAmount / assetValue * 100m;
    }

    public static void EnsureValidApplication(decimal loanAmount, decimal assetValue, int creditScore)
    {
        if (loanAmount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(loanAmount), "Loan amount must be greater than zero.");
        }

        if (assetValue <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(assetValue), "Asset value must be greater than zero.");
        }

        if (creditScore is < MinimumCreditScore or > MaximumCreditScore)
        {
            throw new ArgumentOutOfRangeException(
                nameof(creditScore),
                $"Credit score must be between {MinimumCreditScore} and {MaximumCreditScore}.");
        }
    }

    private static LoanDecision DecideLargeLoan(decimal ltv, int creditScore)
    {
        if (ltv > 60m)
        {
            return Decline(ltv, "LTV must be 60% or less for loans of £1 million or more.");
        }

        if (creditScore < 950)
        {
            return Decline(ltv, "Credit score must be 950 or higher for loans of £1 million or more.");
        }

        return Approve(ltv);
    }

    /// <summary>
    /// The brief lists overlapping LTV conditions (&lt; 60%, &lt; 80%, &lt; 90%).
    /// They are applied as mutually exclusive bands so each credit threshold is reachable.
    /// </summary>
    private static LoanDecision DecideStandardLoan(decimal ltv, int creditScore)
    {
        if (ltv >= 90m)
        {
            return Decline(ltv, "LTV of 90% or higher is declined.");
        }

        if (ltv < 60m)
        {
            return creditScore >= 750
                ? Approve(ltv)
                : Decline(ltv, "Credit score must be 750 or higher when LTV is less than 60%.");
        }

        if (ltv < 80m)
        {
            return creditScore >= 800
                ? Approve(ltv)
                : Decline(ltv, "Credit score must be 800 or higher when LTV is 60% or more and less than 80%.");
        }

        return creditScore >= 900
            ? Approve(ltv)
            : Decline(ltv, "Credit score must be 900 or higher when LTV is 80% or more and less than 90%.");
    }

    private static LoanDecision Approve(decimal ltv) =>
        new(LoanDecisionStatus.Successful, ltv, null);

    private static LoanDecision Decline(decimal ltv, string reason) =>
        new(LoanDecisionStatus.Declined, ltv, reason);
}
