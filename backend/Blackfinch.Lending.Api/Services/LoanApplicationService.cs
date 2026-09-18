using Blackfinch.Lending.Api.Contracts;
using Blackfinch.Lending.Api.Data;
using Blackfinch.Lending.Domain;
using Microsoft.EntityFrameworkCore;

namespace Blackfinch.Lending.Api.Services;

public sealed class LoanApplicationService
{
    private readonly LendingDbContext _db;
    private readonly LoanDecisionService _decisionService;

    public LoanApplicationService(LendingDbContext db, LoanDecisionService decisionService)
    {
        _db = db;
        _decisionService = decisionService;
    }

    public async Task<SubmitApplicationResponse> SubmitAsync(
        SubmitApplicationRequest request,
        CancellationToken cancellationToken)
    {
        // Normalize monetary inputs to 2 decimal places before validation & decision
        var normalizedLoanAmount = decimal.Round(request.LoanAmount, 2, MidpointRounding.AwayFromZero);
        var normalizedAssetValue = decimal.Round(request.AssetValue, 2, MidpointRounding.AwayFromZero);

        var decision = _decisionService.Decide(
            normalizedLoanAmount,
            normalizedAssetValue,
            request.CreditScore);

        var application = new LoanApplication
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            LoanAmount = normalizedLoanAmount,
            AssetValue = normalizedAssetValue,
            CreditScore = request.CreditScore,
            LtvPercent = decision.LtvPercent,
            IsSuccessful = decision.IsSuccessful,
            DeclineReason = decision.DeclineReason,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.LoanApplications.Add(application);
        await _db.SaveChangesAsync(cancellationToken);

        var metrics = await GetMetricsAsync(cancellationToken);

        return new SubmitApplicationResponse
        {
            Id = application.Id,
            FullName = application.FullName,
            Email = application.Email,
            PhoneNumber = application.PhoneNumber,
            LoanAmount = application.LoanAmount,
            AssetValue = application.AssetValue,
            CreditScore = application.CreditScore,
            LtvPercent = decimal.Round(application.LtvPercent, 4, MidpointRounding.AwayFromZero),
            Decision = decision.Status.ToString(),
            DeclineReason = decision.DeclineReason,
            Metrics = metrics
        };
    }

    public async Task<PlatformMetricsResponse> GetMetricsAsync(CancellationToken cancellationToken)
    {
        var totalApplicants = await _db.LoanApplications.CountAsync(cancellationToken);
        if (totalApplicants == 0)
        {
            return new PlatformMetricsResponse
            {
                SuccessfulApplicants = 0,
                DeclinedApplicants = 0,
                TotalApplicants = 0,
                TotalValueOfLoansWritten = 0m,
                MeanAverageLtv = 0m
            };
        }

        var successful = await _db.LoanApplications.CountAsync(x => x.IsSuccessful, cancellationToken);
        var declined = totalApplicants - successful;

        var aggregates = await _db.LoanApplications.AsNoTracking()
            .Select(x => new { x.IsSuccessful, x.LoanAmount, x.LtvPercent })
            .ToListAsync(cancellationToken);

        var totalWritten = aggregates.Where(x => x.IsSuccessful).Sum(x => x.LoanAmount);
        var meanLtv = aggregates.Average(x => x.LtvPercent);

        return new PlatformMetricsResponse
        {
            SuccessfulApplicants = successful,
            DeclinedApplicants = declined,
            TotalApplicants = totalApplicants,
            TotalValueOfLoansWritten = decimal.Round(totalWritten, 2, MidpointRounding.AwayFromZero),
            MeanAverageLtv = decimal.Round(meanLtv, 4, MidpointRounding.AwayFromZero)
        };
    }

    public async Task<IReadOnlyList<ApplicationHistoryItemResponse>> GetHistoryAsync(
        CancellationToken cancellationToken)
    {
        return await _db.LoanApplications
            .AsNoTracking()
            .OrderByDescending(application => application.CreatedAtUtc)
            .Select(application => new ApplicationHistoryItemResponse
            {
                Id = application.Id,
                FullName = application.FullName,
                LoanAmount = application.LoanAmount,
                AssetValue = application.AssetValue,
                CreditScore = application.CreditScore,
                LtvPercent = application.LtvPercent,
                Decision = application.IsSuccessful ? "Successful" : "Declined",
                DeclineReason = application.DeclineReason,
                CreatedAtUtc = application.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }
}
