namespace Blackfinch.Lending.Api.Contracts;

public sealed class SubmitApplicationRequest
{
    public decimal LoanAmount { get; set; }

    public decimal AssetValue { get; set; }

    public int CreditScore { get; set; }
}
