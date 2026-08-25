using System.Security.Claims;
using OutcomeHub.Application.Common.Security;

namespace OutcomeHub.Api.Services;

public sealed class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid PrincipalId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is null)
            {
                return Guid.Empty;
            }

            // 1. Check development header override
            if (httpContext.Request.Headers.TryGetValue("X-Principal-Id", out var headerVal) &&
                Guid.TryParse(headerVal.FirstOrDefault(), out var headerPrincipalId) &&
                headerPrincipalId != Guid.Empty)
            {
                return headerPrincipalId;
            }

            // 2. Check JWT Claims
            var claimVal = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                           httpContext.User.FindFirstValue("sub") ??
                           httpContext.User.FindFirstValue("principal_id");

            if (!string.IsNullOrWhiteSpace(claimVal) && Guid.TryParse(claimVal, out var claimPrincipalId))
            {
                return claimPrincipalId;
            }

            // 3. Fallback for Local Dev/Seeded Admin if unauthenticated in Development
            return Guid.Empty;
        }
    }

    public Guid? OrgUnitId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is null) return null;

            if (httpContext.Request.Headers.TryGetValue("X-Org-Unit-Id", out var headerVal) &&
                Guid.TryParse(headerVal.FirstOrDefault(), out var headerOrgId))
            {
                return headerOrgId;
            }

            var claimVal = httpContext.User.FindFirstValue("org_unit_id");
            if (!string.IsNullOrWhiteSpace(claimVal) && Guid.TryParse(claimVal, out var claimOrgId))
            {
                return claimOrgId;
            }

            return null;
        }
    }

    public string? UserCode =>
        _httpContextAccessor.HttpContext?.Request.Headers["X-User-Code"].FirstOrDefault() ??
        _httpContextAccessor.HttpContext?.User.FindFirstValue("user_code") ??
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

    public string? FullName =>
        _httpContextAccessor.HttpContext?.Request.Headers["X-Full-Name"].FirstOrDefault() ??
        _httpContextAccessor.HttpContext?.User.FindFirstValue("full_name") ??
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.GivenName);

    public string? RoleName =>
        _httpContextAccessor.HttpContext?.Request.Headers["X-Role-Name"].FirstOrDefault() ??
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);

    public bool IsAuthenticated =>
        PrincipalId != Guid.Empty &&
        (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true ||
         _httpContextAccessor.HttpContext?.Request.Headers.ContainsKey("X-Principal-Id") == true);

    public DatabaseRequestContext ToDatabaseRequestContext(string purpose = "API Request")
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var requestId = Guid.NewGuid();

        if (httpContext != null &&
            httpContext.Request.Headers.TryGetValue("X-Request-Id", out var reqIdHeader) &&
            Guid.TryParse(reqIdHeader.FirstOrDefault(), out var parsedReqId))
        {
            requestId = parsedReqId;
        }

        var effectivePrincipalId = PrincipalId != Guid.Empty
            ? PrincipalId
            : Guid.Parse("10000000-0000-7000-8000-000000000001"); // Default Admin/System Principal fallback

        return new DatabaseRequestContext(
            principalId: effectivePrincipalId,
            requestId: requestId,
            purpose: string.IsNullOrWhiteSpace(purpose) ? "API Request" : purpose.Trim());
    }
}
