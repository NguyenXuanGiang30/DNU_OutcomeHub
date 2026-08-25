namespace OutcomeHub.Domain.Entities.Result;

public sealed class CohortOutcomeResult
{
    private CohortOutcomeResult()
    {
    }

    public short AcademicYearStart { get; private set; }

    public Guid Id { get; private set; }

    public Guid BatchId { get; private set; }

    public Guid OrgUnitId { get; private set; }

    public Guid ProgramId { get; private set; }

    public Guid ProgramVersionId { get; private set; }

    public Guid MeasurementPeriodId { get; private set; }

    public Guid CohortId { get; private set; }

    public Guid CurriculumPathId { get; private set; }

    public string OutcomeLevel { get; private set; } = null!;

    public Guid? CloId { get; private set; }

    public Guid? ProgramPiId { get; private set; }

    public Guid? ProgramPloId { get; private set; }

    public string Method { get; private set; } = null!;

    public long PopulationCount { get; private set; }

    public long DenominatorCount { get; private set; }

    public long AttainedCount { get; private set; }

    public long NotAttainedObservedCount { get; private set; }

    public long MissingInDenominatorCount { get; private set; }

    public long NotAttainedCount { get; private set; }

    public long MissingExcludedCount { get; private set; }

    public long PolicyExcludedCount { get; private set; }

    public decimal? AttainmentRate { get; private set; }

    public decimal ThetaCoh { get; private set; }

    public string OutcomeStatus { get; private set; } = null!;

    public bool PrivacySuppressed { get; private set; }

    public ResultBatch Batch { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.OrgUnit OrgUnit { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Program Program { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramVersion ProgramVersion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Measurement.MeasurementPeriod MeasurementPeriod { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.Cohort Cohort { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CurriculumPath CurriculumPath { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Portfolio.Clo? Clo { get; private set; }
    public OutcomeHub.Domain.Entities.Academic.ProgramPi? ProgramPi { get; private set; }
    public OutcomeHub.Domain.Entities.Academic.ProgramPlo? ProgramPlo { get; private set; }
}
