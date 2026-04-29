using ApproveFlow.Application.Abstractions;
using ApproveFlow.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApproveFlow.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<DashboardSummary>> Get(
        string userId,
        [FromQuery] int year,
        [FromQuery] bool fiscalYear,
        CancellationToken cancellationToken)
        => Ok(await _service.BuildAsync(userId, year, fiscalYear, cancellationToken));
}
