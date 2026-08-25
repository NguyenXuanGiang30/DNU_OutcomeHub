using OutcomeHub.Domain.Entities.Document;

namespace OutcomeHub.Domain.Entities.Governance;

public sealed class ObjectReference
{
    private ObjectReference() { }
    public Guid GovernedResourceId { get; private set; }
    public Guid FileObjectId { get; private set; }
    public string ReferenceRole { get; private set; } = null!;
    public DateTimeOffset EffectiveFrom { get; private set; }
    public DateTimeOffset? EffectiveTo { get; private set; }
    public GovernedResource GovernedResource { get; private set; } = null!;
    public FileObject FileObject { get; private set; } = null!;
}
