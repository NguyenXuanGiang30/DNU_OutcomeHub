namespace OutcomeHub.Application.Common.Security;

public interface ICurrentUserContext
{
    Guid PrincipalId { get; }
    Guid? OrgUnitId { get; }
    string? UserCode { get; }
    string? FullName { get; }
    string? RoleName { get; }
    bool IsAuthenticated { get; }

    DatabaseRequestContext ToDatabaseRequestContext(string purpose = "API Request");
}
