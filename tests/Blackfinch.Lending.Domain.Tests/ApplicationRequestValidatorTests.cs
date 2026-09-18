using Blackfinch.Lending.Api.Contracts;
using Blackfinch.Lending.Api.Validation;
using FluentAssertions;
using Xunit;

namespace Blackfinch.Lending.Domain.Tests;

public class ApplicationRequestValidatorTests
{
    private static SubmitApplicationRequest CreateValidRequest() => new()
    {
        FullName = "Rahul Patil",
        Email = "rahul@example.com",
        PhoneNumber = "9876543210",
        LoanAmount = 500_000m,
        AssetValue = 750_000m,
        CreditScore = 750
    };

    [Fact]
    public void Valid_request_passes_validation()
    {
        var request = CreateValidRequest();
        var errors = ApplicationRequestValidator.Validate(request);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Null_request_returns_error()
    {
        var errors = ApplicationRequestValidator.Validate(null);

        errors.Should().ContainSingle().Which.Should().Contain("request body is required");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Missing_or_whitespace_full_name_returns_error(string? name)
    {
        var request = CreateValidRequest();
        request.FullName = name!;

        var errors = ApplicationRequestValidator.Validate(request);

        errors.Should().Contain("Full name is required.");
    }

    [Fact]
    public void Full_name_exceeding_max_length_returns_error()
    {
        var request = CreateValidRequest();
        request.FullName = new string('A', 101);

        var errors = ApplicationRequestValidator.Validate(request);

        errors.Should().Contain("Full name cannot exceed 100 characters.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Missing_or_whitespace_email_returns_error(string? email)
    {
        var request = CreateValidRequest();
        request.Email = email!;

        var errors = ApplicationRequestValidator.Validate(request);

        errors.Should().Contain("Email is required.");
    }

    [Theory]
    [InlineData("rahul@")]
    [InlineData("rahul")]
    [InlineData("@example.com")]
    [InlineData("rahul @example.com")]
    [InlineData("rahul@example")]
    public void Invalid_email_format_returns_error(string email)
    {
        var request = CreateValidRequest();
        request.Email = email;

        var errors = ApplicationRequestValidator.Validate(request);

        errors.Should().Contain("Please enter a valid email address.");
    }

    [Theory]
    [InlineData("rahul@example.com")]
    [InlineData("user.name+tag@domain.co.uk")]
    [InlineData("test_123@sub.domain.org")]
    public void Valid_email_formats_pass_validation(string email)
    {
        var request = CreateValidRequest();
        request.Email = email;

        var errors = ApplicationRequestValidator.Validate(request);

        errors.Should().BeEmpty();
    }

    [Fact]
    public void Email_exceeding_max_length_returns_error()
    {
        var request = CreateValidRequest();
        request.Email = $"{new string('a', 250)}@test.com";

        var errors = ApplicationRequestValidator.Validate(request);

        errors.Should().Contain("Email cannot exceed 254 characters.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Missing_or_whitespace_phone_returns_error(string? phone)
    {
        var request = CreateValidRequest();
        request.PhoneNumber = phone!;

        var errors = ApplicationRequestValidator.Validate(request);

        errors.Should().Contain("Phone number is required.");
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("abcdefghij")]
    [InlineData("0000000000")]
    [InlineData("+12345678901234567890")]
    public void Invalid_phone_formats_return_error(string phone)
    {
        var request = CreateValidRequest();
        request.PhoneNumber = phone;

        var errors = ApplicationRequestValidator.Validate(request);

        errors.Should().Contain("Please enter a valid phone number.");
    }

    [Theory]
    [InlineData("9876543210")]
    [InlineData("+919876543210")]
    [InlineData("+91 98765 43210")]
    [InlineData("09876543210")]
    [InlineData("+44 7911 123456")]
    [InlineData("+1-800-555-0199")]
    public void Valid_phone_formats_pass_validation(string phone)
    {
        var request = CreateValidRequest();
        request.PhoneNumber = phone;

        var errors = ApplicationRequestValidator.Validate(request);

        errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Non_positive_loan_amount_returns_error(decimal amount)
    {
        var request = CreateValidRequest();
        request.LoanAmount = amount;

        var errors = ApplicationRequestValidator.Validate(request);

        errors.Should().Contain("Loan amount must be greater than zero.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-500)]
    public void Non_positive_asset_value_returns_error(decimal asset)
    {
        var request = CreateValidRequest();
        request.AssetValue = asset;

        var errors = ApplicationRequestValidator.Validate(request);

        errors.Should().Contain("Asset value must be greater than zero so LTV can be calculated.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1000)]
    public void Credit_score_outside_boundary_returns_error(int score)
    {
        var request = CreateValidRequest();
        request.CreditScore = score;

        var errors = ApplicationRequestValidator.Validate(request);

        errors.Should().ContainMatch("Credit score must be between 1 and 999*");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(750)]
    [InlineData(999)]
    public void Valid_credit_score_boundaries_pass_validation(int score)
    {
        var request = CreateValidRequest();
        request.CreditScore = score;

        var errors = ApplicationRequestValidator.Validate(request);

        errors.Should().BeEmpty();
    }
}
