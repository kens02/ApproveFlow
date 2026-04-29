using ApproveFlow.Application.Abstractions;
using ApproveFlow.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApproveFlow.Api.Controllers;

[ApiController]
[Route("api/leave-requests")]
[Authorize]
public sealed class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestService _service;

    public LeaveRequestsController(ILeaveRequestService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<LeaveRequestSummary>> Create([FromBody] CreateLeaveRequestInput input, CancellationToken cancellationToken)
        => Ok(await _service.CreateAsync(input, cancellationToken));

    [HttpPost("{id:guid}/approve")]
    public async Task<ActionResult<LeaveRequestSummary>> Approve(Guid id, [FromBody] ActionInput input, CancellationToken cancellationToken)
        => Ok(await _service.ApproveAsync(input with { RequestId = id }, cancellationToken));

    [HttpPost("{id:guid}/reject")]
    public async Task<ActionResult<LeaveRequestSummary>> Reject(Guid id, [FromBody] ActionInput input, CancellationToken cancellationToken)
        => Ok(await _service.RejectAsync(input with { RequestId = id }, cancellationToken));

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<LeaveRequestSummary>> Cancel(Guid id, [FromBody] ActionInput input, CancellationToken cancellationToken)
        => Ok(await _service.CancelAsync(input with { RequestId = id }, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LeaveRequestSummary>> Get(Guid id, CancellationToken cancellationToken)
    {
        var item = await _service.GetAsync(id, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet("applicant/{applicantId}")]
    public async Task<ActionResult<IReadOnlyList<LeaveRequestSummary>>> GetForApplicant(string applicantId, CancellationToken cancellationToken)
        => Ok(await _service.GetForApplicantAsync(applicantId, cancellationToken));

    [HttpGet("approver/{approverId}")]
    public async Task<ActionResult<IReadOnlyList<LeaveRequestSummary>>> GetForApprover(string approverId, CancellationToken cancellationToken)
        => Ok(await _service.GetForApproverAsync(approverId, cancellationToken));

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<IReadOnlyList<LeaveRequestSummary>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _service.GetAllAsync(cancellationToken));
}
