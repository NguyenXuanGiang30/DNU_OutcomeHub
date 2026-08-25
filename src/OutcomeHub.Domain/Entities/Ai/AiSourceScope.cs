using OutcomeHub.Domain.Entities.Governance;

namespace OutcomeHub.Domain.Entities.Ai;

public sealed class AiSourceScope
{
    private AiSourceScope()
    {
    }

    public Guid AiSourceSnapshotId { get; private set; }

    public Guid ResourceSecurityScopeId { get; private set; }

    public string ScopeChecksum { get; private set; } = null!;

    public AiSourceSnapshot AiSourceSnapshot { get; private set; } = null!;

    public ResourceSecurityScope ResourceSecurityScope { get; private set; } = null!;
}
