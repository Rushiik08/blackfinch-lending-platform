using Blackfinch.Lending.Domain;
using FluentAssertions;

namespace Blackfinch.Lending.Domain.Tests;

public class LoanDecisionServiceTests
{
    private readonly LoanDecisionService _sut = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Decide_rejects_non_positive_loan_amount(decimal loanAmount)
    {
        var act = () => _sut.Decide(loanAmount, assetValue: 500_000m, creditScore: 800);

        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("loanAmount");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Decide_rejects_non_positive_asset_value(decimal assetValue)
    {
        var act = () => _sut.Decide(loanAmount: 200_000m, assetValue, creditScore: 800);

        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("assetValue");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1000)]
    [InlineData(-1)]
    public void Decide_rejects_credit_score_outside_1_to_999(int creditScore)
    {
        var act = () => _sut.Decide(loanAmount: 200_000m, assetValue: 500_000m, creditScore);

        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("creditScore");
    }

    [Fact]
    public void CalculateLtv_is_loan_over_asset_times_100()
    {
        LoanDecisionService.CalculateLtv(300_000m, 500_000m).Should().Be(60m);
    }

    [Fact]
    public void Loan_below_100000_is_declined()
    {
        var result = _sut.Decide(99_999.99m, 1_000_000m, 999);

        result.Status.Should().Be(LoanDecisionStatus.Declined);
        result.DeclineReason.Should().Contain("£100,000");
    }

    [Fact]
    public void Loan_at_100000_passes_the_minimum_amount_gate()
    {
        var result = _sut.Decide(100_000m, 250_000m, 750);

        result.Status.Should().Be(LoanDecisionStatus.Successful);
        result.LtvPercent.Should().Be(40m);
    }

    [Fact]
    public void Loan_above_1_5_million_is_declined()
    {
        var result = _sut.Decide(1_500_000.01m, 10_000_000m, 999);

        result.Status.Should().Be(LoanDecisionStatus.Declined);
        result.DeclineReason.Should().Contain("£1.5 million");
    }

    [Fact]
    public void Loan_at_1_5_million_uses_the_large_loan_rules()
    {
        var result = _sut.Decide(1_500_000m, 2_500_000m, 950);

        result.Status.Should().Be(LoanDecisionStatus.Successful);
        result.LtvPercent.Should().Be(60m);
    }

    [Fact]
    public void Loan_at_1_million_uses_the_large_loan_rules()
    {
        var result = _sut.Decide(1_000_000m, 2_000_000m, 950);

        result.Status.Should().Be(LoanDecisionStatus.Successful);
        result.LtvPercent.Should().Be(50m);
    }

    [Fact]
    public void Loan_just_below_1_million_uses_the_standard_rules()
    {
        var result = _sut.Decide(999_999.99m, 2_000_000m, 750);

        result.Status.Should().Be(LoanDecisionStatus.Successful);
    }

    [Fact]
    public void Large_loan_with_ltv_exactly_60_and_credit_950_is_successful()
    {
        _sut.Decide(1_200_000m, 2_000_000m, 950).Status.Should().Be(LoanDecisionStatus.Successful);
    }

    [Fact]
    public void Large_loan_with_ltv_above_60_is_declined()
    {
        var result = _sut.Decide(1_200_000.01m, 2_000_000m, 999);

        result.Status.Should().Be(LoanDecisionStatus.Declined);
        result.DeclineReason.Should().Contain("60%");
    }

    [Fact]
    public void Large_loan_with_credit_949_is_declined()
    {
        var result = _sut.Decide(1_000_000m, 2_000_000m, 949);

        result.Status.Should().Be(LoanDecisionStatus.Declined);
        result.DeclineReason.Should().Contain("950");
    }

    [Fact]
    public void Standard_loan_ltv_below_60_requires_credit_750()
    {
        _sut.Decide(200_000m, 400_000m, 750).Status.Should().Be(LoanDecisionStatus.Successful);
        _sut.Decide(200_000m, 400_000m, 749).Status.Should().Be(LoanDecisionStatus.Declined);
    }

    [Fact]
    public void Standard_loan_ltv_exactly_60_requires_credit_800_not_750()
    {
        _sut.Decide(300_000m, 500_000m, 800).Status.Should().Be(LoanDecisionStatus.Successful);
        _sut.Decide(300_000m, 500_000m, 799).Status.Should().Be(LoanDecisionStatus.Declined);
        _sut.Decide(300_000m, 500_000m, 750).Status.Should().Be(LoanDecisionStatus.Declined);
    }

    [Fact]
    public void Standard_loan_ltv_just_below_80_requires_credit_800()
    {
        _sut.Decide(399_999.99m, 500_000m, 800).Status.Should().Be(LoanDecisionStatus.Successful);
        _sut.Decide(399_999.99m, 500_000m, 799).Status.Should().Be(LoanDecisionStatus.Declined);
    }

    [Fact]
    public void Standard_loan_ltv_exactly_80_requires_credit_900()
    {
        _sut.Decide(400_000m, 500_000m, 900).Status.Should().Be(LoanDecisionStatus.Successful);
        _sut.Decide(400_000m, 500_000m, 899).Status.Should().Be(LoanDecisionStatus.Declined);
        _sut.Decide(400_000m, 500_000m, 800).Status.Should().Be(LoanDecisionStatus.Declined);
    }

    [Fact]
    public void Standard_loan_ltv_just_below_90_requires_credit_900()
    {
        _sut.Decide(449_999.99m, 500_000m, 900).Status.Should().Be(LoanDecisionStatus.Successful);
        _sut.Decide(449_999.99m, 500_000m, 899).Status.Should().Be(LoanDecisionStatus.Declined);
    }

    [Fact]
    public void Standard_loan_ltv_of_90_or_higher_is_declined()
    {
        _sut.Decide(450_000m, 500_000m, 999).Status.Should().Be(LoanDecisionStatus.Declined);
        _sut.Decide(500_000m, 500_000m, 999).Status.Should().Be(LoanDecisionStatus.Declined);
    }

    [Fact]
    public void Credit_score_boundaries_1_and_999_are_accepted_as_input()
    {
        _sut.Decide(200_000m, 400_000m, 1).Status.Should().Be(LoanDecisionStatus.Declined);
        _sut.Decide(200_000m, 400_000m, 999).Status.Should().Be(LoanDecisionStatus.Successful);
    }

    [Fact]
    public void Normalized_loan_amount_at_100000_boundary_evaluates_consistently()
    {
        var rawAmount = 99_999.996m;
        var normalizedAmount = decimal.Round(rawAmount, 2, MidpointRounding.AwayFromZero);
        normalizedAmount.Should().Be(100_000.00m);

        var result = _sut.Decide(normalizedAmount, 250_000m, 750);
        result.Status.Should().Be(LoanDecisionStatus.Successful);
        result.LtvPercent.Should().Be(40m);
    }

    [Fact]
    public void Normalized_loan_amount_below_100000_boundary_is_declined()
    {
        var rawAmount = 99_999.994m;
        var normalizedAmount = decimal.Round(rawAmount, 2, MidpointRounding.AwayFromZero);
        normalizedAmount.Should().Be(99_999.99m);

        var result = _sut.Decide(normalizedAmount, 250_000m, 750);
        result.Status.Should().Be(LoanDecisionStatus.Declined);
        result.DeclineReason.Should().Contain("£100,000");
    }
}
