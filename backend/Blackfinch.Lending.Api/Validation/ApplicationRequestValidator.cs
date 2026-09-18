using System.Text.RegularExpressions;
using Blackfinch.Lending.Api.Contracts;
using Blackfinch.Lending.Domain;

namespace Blackfinch.Lending.Api.Validation;

public static partial class ApplicationRequestValidator
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[a-zA-Z0-9-]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PhoneAllowedCharsRegex = new(
        @"^(\+)?[0-9\s\-()]+$",
        RegexOptions.Compiled);

    public const int MaxFullNameLength = 100;
    public const int MaxEmailLength = 254;

    public static IReadOnlyList<string> Validate(SubmitApplicationRequest? request)
    {
        var errors = new List<string>();

        if (request is null)
        {
            errors.Add("A request body is required.");
            return errors;
        }

        // 1. Full Name Validation
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            errors.Add("Full name is required.");
        }
        else if (request.FullName.Trim().Length > MaxFullNameLength)
        {
            errors.Add($"Full name cannot exceed {MaxFullNameLength} characters.");
        }

        // 2. Email Validation
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors.Add("Email is required.");
        }
        else
        {
            var trimmedEmail = request.Email.Trim();
            if (trimmedEmail.Length > MaxEmailLength)
            {
                errors.Add($"Email cannot exceed {MaxEmailLength} characters.");
            }
            else if (!EmailRegex.IsMatch(trimmedEmail))
            {
                errors.Add("Please enter a valid email address.");
            }
        }

        // 3. Phone Number Validation
        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            errors.Add("Phone number is required.");
        }
        else
        {
            var trimmedPhone = request.PhoneNumber.Trim();
            var digitsOnly = Regex.Replace(trimmedPhone, @"\D", "");

            if (!PhoneAllowedCharsRegex.IsMatch(trimmedPhone) ||
                digitsOnly.Length < 7 ||
                digitsOnly.Length > 15 ||
                digitsOnly.All(c => c == '0'))
            {
                errors.Add("Please enter a valid phone number.");
            }
        }

        // 4. Financial & Score Validations
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
