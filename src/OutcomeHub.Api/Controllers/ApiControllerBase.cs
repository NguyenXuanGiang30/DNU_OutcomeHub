using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.Common.Security;

namespace OutcomeHub.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    private ICurrentUserContext? _currentUserContext;

    protected ICurrentUserContext CurrentUser =>
        _currentUserContext ??= HttpContext.RequestServices.GetRequiredService<ICurrentUserContext>();

    protected DatabaseRequestContext GetDatabaseRequestContext(string purpose = "API Request") =>
        CurrentUser.ToDatabaseRequestContext(purpose);

    protected ActionResult<ApiResponse<T>> OkResponse<T>(T data, string? message = null)
    {
        return Ok(ApiResponse.Ok(data, message));
    }

    protected ActionResult<ApiResponse<PagedResult<T>>> PagedResponse<T>(
        PagedResult<T> pagedResult,
        string? message = null)
    {
        return Ok(ApiResponse.Ok(pagedResult, message));
    }

    protected ActionResult<ApiResponse<T>> CreatedResponse<T>(
        string actionName,
        object routeValues,
        T data,
        string? message = null)
    {
        return CreatedAtAction(actionName, routeValues, ApiResponse.Ok(data, message));
    }
}
