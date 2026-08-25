namespace OutcomeHub.Application.Common.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "You do not have permission to perform this action or access this resource.")
        : base(message)
    {
    }
}

public class BusinessInvariantException : Exception
{
    public BusinessInvariantException(string message) : base(message)
    {
    }

    public BusinessInvariantException(string ruleCode, string message)
        : base($"[{ruleCode}] {message}")
    {
        RuleCode = ruleCode;
    }

    public string? RuleCode { get; }
}
