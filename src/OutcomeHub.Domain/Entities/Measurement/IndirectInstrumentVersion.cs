namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class IndirectInstrumentVersion
{
    private IndirectInstrumentVersion()
    {
    }

    public Guid Id { get; private set; }

    public Guid InstrumentId { get; private set; }

    public int VersionNo { get; private set; }

    public decimal ScaleMin { get; private set; }

    public decimal ScaleMax { get; private set; }

    public Guid WorkflowInstanceId { get; private set; }

    public string Checksum { get; private set; } = null!;

    public IndirectInstrument Instrument { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Workflow.WorkflowInstance WorkflowInstance { get; private set; } = null!;
    public ICollection<IndirectItem> Items { get; private set; } = new List<IndirectItem>();
}
