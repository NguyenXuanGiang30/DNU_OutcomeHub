namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class IndirectItem
{
    private IndirectItem()
    {
    }

    public Guid Id { get; private set; }

    public Guid InstrumentVersionId { get; private set; }

    public Guid ProgramVersionId { get; private set; }

    public string Code { get; private set; } = null!;

    public string Prompt { get; private set; } = null!;

    public Guid? ProgramPiId { get; private set; }

    public Guid? ProgramPloId { get; private set; }

    public decimal WeightRatio { get; private set; }

    public IndirectInstrumentVersion InstrumentVersion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramVersion ProgramVersion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramPi? ProgramPi { get; private set; }
    public OutcomeHub.Domain.Entities.Academic.ProgramPlo? ProgramPlo { get; private set; }
}
