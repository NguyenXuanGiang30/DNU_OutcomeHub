using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutcomeHub.Api.Contracts;
using OutcomeHub.Infrastructure.Persistence;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/system")]
public sealed class SystemController : ControllerBase
{
    [HttpGet("status")]
    [ProducesResponseType<ServiceStatusResponse>(StatusCodes.Status200OK)]
    public ActionResult<ServiceStatusResponse> GetStatus()
    {
        return Ok(new ServiceStatusResponse("OutcomeHub API", "Running"));
    }

    [HttpGet("database")]
    [ProducesResponseType<DatabaseStatusResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<DatabaseStatusResponse>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<DatabaseStatusResponse>> GetDatabaseStatusAsync(
        [FromServices] OutcomeHubDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
        var response = new DatabaseStatusResponse(
            "PostgreSQL",
            canConnect ? "Healthy" : "Unhealthy");

        return canConnect
            ? Ok(response)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, response);
    }
}
