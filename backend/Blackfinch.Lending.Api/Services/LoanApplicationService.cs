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
        var decision = _decisionService.Decide(
            request.LoanAmount,
            request.AssetValue,
            request.CreditScore);

        var application = new LoanApplication
        {
            Id = Guid.NewGuid(),
            LoanAmount = decimal.Round(request.LoanAmount, 2, MidpointRounding.AwayFromZero),
            AssetValue = decimal.Round(request.AssetValue, 2, MidpointRounding.AwayFromZero),
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
        var applications = await _db.LoanApplications.AsNoTracking().ToListAsync(cancellationToken);

        var successful = applications.Count(x => x.IsSuccessful);
        var declined = applications.Count(x => !x.IsSuccessful);
        var totalWritten = applications.Where(x => x.IsSuccessful).Sum(x => x.LoanAmount);
        var meanLtv = applications.Count == 0
            ? 0m
            : applications.Average(x => x.LtvPercent);

        return new PlatformMetricsResponse
        {
            SuccessfulApplicants = successful,
            DeclinedApplicants = declined,
            TotalApplicants = applications.Count,
            TotalValueOfLoansWritten = decimal.Round(totalWritten, 2, MidpointRounding.AwayFromZero),
            MeanAverageLtv = decimal.Round(meanLtv, 4, MidpointRounding.AwayFromZero)
        };
    }
}
