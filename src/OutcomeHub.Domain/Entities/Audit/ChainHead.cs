namespace OutcomeHub.Domain.Entities.Audit;

public sealed class ChainHead
{
    private ChainHead() { }

    public DateOnly PartitionStart { get; private set; }
    public Guid ChainId { get; private set; }
    public long LastSequence { get; private set; }
    public string LastHash { get; private set; } = null!;
    public long RowVersion { get; private set; }
}
