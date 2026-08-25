using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Academic;

public sealed class ValidationRun
{
    private ValidationRun() { }

    public Guid Id { get; private set; }
    public string AggregateType { get; private set; } = null!;
    public Guid AggregateId { get; private set; }
    public string RulesetVersion { get; private set; } = null!;
    public string ContentHash { get; private set; } = null!;
    public bool Passed { get; private set; }
    public DateTimeOffset RunAt { get; private set; }
    public Guid RequestedBy { get; private set; }

    public Principal RequestedByPrincipal { get; private set; } = null!;
}
