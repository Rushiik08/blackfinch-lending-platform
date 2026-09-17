using Blackfinch.Lending.Api.Contracts;
using Blackfinch.Lending.Domain;

namespace Blackfinch.Lending.Api.Validation;

public static class ApplicationRequestValidator
{
    public static IReadOnlyList<string> Validate(SubmitApplicationRequest? request)
    {
        var errors = new List<string>();

        if (request is null)
        {
            errors.Add("A request body is required.");
            return errors;
        }

        if (request.LoanAmount <= 0)
        {
            errors.Add("Loan amount must be greater than zero.");
        }

        if (request.AssetValue <= 0)
        {
            errors.Add("Asset value must be greater than zero so LTV can be calculated.");
        }

        if (request.CreditScore is < LoanDecisionService.MinimumCreditScore
            or > LoanDecisionService.MaximumCreditScore)
        {
            errors.Add(
                $"Credit score must be between {LoanDecisionService.MinimumCreditScore} and {LoanDecisionService.MaximumCreditScore}.");
        }

        return errors;
    }
}
