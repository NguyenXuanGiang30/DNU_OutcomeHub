using OutcomeHub.Domain.Entities.Governance;

namespace OutcomeHub.Domain.Entities.Audit;

public sealed class ArchiveManifest
{
    private ArchiveManifest() { }

    public Guid Id { get; private set; }
    public Guid GovernedResourceId { get; private set; }
    public DateTimeOffset PeriodFrom { get; private set; }
    public DateTimeOffset PeriodTo { get; private set; }
    public Guid FirstEventId { get; private set; }
    public Guid LastEventId { get; private set; }
    public long EventCount { get; private set; }
    public string RootHash { get; private set; } = null!;
    public byte[] Signature { get; private set; } = null!;
    public string ObjectUri { get; private set; } = null!;
    public string ObjectChecksum { get; private set; } = null!;
    public DateTimeOffset ArchivedAt { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }

    public GovernedResource GovernedResource { get; private set; } = null!;
}
