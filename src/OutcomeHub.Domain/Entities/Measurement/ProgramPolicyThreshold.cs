namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class ProgramPolicyThreshold
{
    private ProgramPolicyThreshold()
    {
    }

    public Guid Id { get; private set; }

    public Guid ProgramPolicyBindingId { get; private set; }

    public string OutcomeLevel { get; private set; } = null!;

    public Guid? SyllabusVersionId { get; private set; }

    public Guid? CloId { get; private set; }

    public Guid? ProgramPiId { get; private set; }

    public Guid? ProgramPloId { get; private set; }

    public decimal ThetaInd { get; private set; }

    public decimal ThetaCoh { get; private set; }

    public decimal? NearThreshold { get; private set; }

    public int MinSampleSize { get; private set; }

    public ProgramPolicyBinding ProgramPolicyBinding { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Portfolio.SyllabusVersion? SyllabusVersion { get; private set; }
    public OutcomeHub.Domain.Entities.Portfolio.Clo? Clo { get; private set; }
    public OutcomeHub.Domain.Entities.Academic.ProgramPi? ProgramPi { get; private set; }
    public OutcomeHub.Domain.Entities.Academic.ProgramPlo? ProgramPlo { get; private set; }
}
