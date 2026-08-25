namespace OutcomeHub.Domain.Entities.Governance;

public sealed class DispositionItem
{
    private DispositionItem() { }
    public Guid Id { get; private set; }
    public Guid DispositionCaseId { get; private set; }
    public Guid GovernedResourceId { get; private set; }
    public Guid RetentionBindingId { get; private set; }
    public string PlannedAction { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public bool ObjectDeleted { get; private set; }
    public bool DatabaseAnonymized { get; private set; }
    public string? Error { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public DispositionCase DispositionCase { get; private set; } = null!;
    public GovernedResource GovernedResource { get; private set; } = null!;
    public RetentionBinding RetentionBinding { get; private set; } = null!;
}
