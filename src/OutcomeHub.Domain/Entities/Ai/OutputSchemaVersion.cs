using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Ai;

public sealed class OutputSchemaVersion
{
    private OutputSchemaVersion()
    {
    }

    public Guid Id { get; private set; }

    public string Code { get; private set; } = null!;

    public int VersionNo { get; private set; }

    public string JsonSchema { get; private set; } = null!;

    public string Checksum { get; private set; } = null!;

    public string Status { get; private set; } = null!;

    public Guid? ApprovedBy { get; private set; }

    public DateTimeOffset? ApprovedAt { get; private set; }

    public Guid? ActivationDecisionId { get; private set; }

    public Principal? Approver { get; private set; }

    public ActivationDecision? ActivationDecision { get; private set; }
}
