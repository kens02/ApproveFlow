using ApproveFlow.Application.Abstractions;
using ApproveFlow.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApproveFlow.Api.Controllers;

[ApiController]
[Route("api/routes")]
[Authorize(Roles = "Administrator")]
public sealed class RoutesController : ControllerBase
{
    private readonly IApprovalRouteService _service;

    public RoutesController(IApprovalRouteService service)
    {
        _service = service;
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] SetRouteInput input, CancellationToken cancellationToken)
    {
        await _service.SetAsync(input, cancellationToken);
        return NoContent();
    }
}
