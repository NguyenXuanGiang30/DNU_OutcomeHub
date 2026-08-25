namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class MeasurementThresholdOverride
{
    private MeasurementThresholdOverride()
    {
    }

    public Guid Id { get; private set; }

    public Guid MeasurementPeriodId { get; private set; }

    public Guid ProgramVersionId { get; private set; }

    public string OutcomeLevel { get; private set; } = null!;

    public Guid? CourseOfferingId { get; private set; }

    public Guid? SyllabusVersionId { get; private set; }

    public Guid? CloId { get; private set; }

    public Guid? ProgramPiId { get; private set; }

    public Guid? ProgramPloId { get; private set; }

    public decimal ThetaInd { get; private set; }

    public decimal ThetaCoh { get; private set; }

    public decimal? NearThreshold { get; private set; }

    public int? MinSampleSize { get; private set; }

    public string Reason { get; private set; } = null!;

    public Guid WorkflowInstanceId { get; private set; }

    public MeasurementPeriod MeasurementPeriod { get; private set; } = null!;
    public MeasurementPeriodOffering? PeriodOffering { get; private set; }
    public OutcomeHub.Domain.Entities.Academic.CourseOffering? CourseOffering { get; private set; }
    public OutcomeHub.Domain.Entities.Portfolio.SyllabusVersion? SyllabusVersion { get; private set; }
    public OutcomeHub.Domain.Entities.Portfolio.Clo? Clo { get; private set; }
    public OutcomeHub.Domain.Entities.Academic.ProgramPi? ProgramPi { get; private set; }
    public OutcomeHub.Domain.Entities.Academic.ProgramPlo? ProgramPlo { get; private set; }
    public OutcomeHub.Domain.Entities.Workflow.WorkflowInstance WorkflowInstance { get; private set; } = null!;
}
