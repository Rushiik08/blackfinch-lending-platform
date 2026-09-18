using Blackfinch.Lending.Api.Contracts;
using Blackfinch.Lending.Api.Services;
using Blackfinch.Lending.Api.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Blackfinch.Lending.Api.Controllers;

[ApiController]
[Route("api/applications")]
public sealed class ApplicationsController : ControllerBase
{
    private readonly LoanApplicationService _applications;

    public ApplicationsController(LoanApplicationService applications)
    {
        _applications = applications;
    }

    [HttpPost]
    public async Task<ActionResult<SubmitApplicationResponse>> Submit(
        [FromBody] SubmitApplicationRequest request,
        CancellationToken cancellationToken)
    {
        var errors = ApplicationRequestValidator.Validate(request);
        if (errors.Count > 0)
        {
            return BadRequest(new { errors });
        }

        var response = await _applications.SubmitAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("metrics")]
    public async Task<ActionResult<PlatformMetricsResponse>> GetMetrics(CancellationToken cancellationToken)
    {
        var metrics = await _applications.GetMetricsAsync(cancellationToken);
        return Ok(metrics);
    }

    [HttpGet("history")]
    public async Task<ActionResult<IReadOnlyList<ApplicationHistoryItemResponse>>> GetHistory(
        CancellationToken cancellationToken)
    {
        var history = await _applications.GetHistoryAsync(cancellationToken);
        return Ok(history);
    }
}
