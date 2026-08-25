namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class IndirectInstrument
{
    private IndirectInstrument()
    {
    }

    public Guid Id { get; private set; }

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public Guid OwnerOrgUnitId { get; private set; }

    public OutcomeHub.Domain.Entities.Academic.OrgUnit OwnerOrgUnit { get; private set; } = null!;
    public ICollection<IndirectInstrumentVersion> Versions { get; private set; } = new List<IndirectInstrumentVersion>();
}
