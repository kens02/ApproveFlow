using ApproveFlow.Application.Abstractions;
using ApproveFlow.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApproveFlow.Api.Controllers;

[ApiController]
[Route("api/balances")]
[Authorize]
public sealed class BalancesController : ControllerBase
{
    private readonly ILeaveBalanceService _service;

    public BalancesController(ILeaveBalanceService service)
    {
        _service = service;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<LeaveBalanceSummary>> Get(string userId, CancellationToken cancellationToken)
        => Ok(await _service.GetAsync(userId, cancellationToken));
}
