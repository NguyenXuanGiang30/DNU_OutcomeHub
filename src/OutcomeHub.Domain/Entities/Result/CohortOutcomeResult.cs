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

    public static CohortOutcomeResult Create(
        short academicYearStart,
        Guid id,
        Guid batchId,
        Guid orgUnitId,
        Guid programId,
        Guid programVersionId,
        Guid measurementPeriodId,
        Guid cohortId,
        Guid curriculumPathId,
        string outcomeLevel,
        Guid? cloId,
        Guid? programPiId,
        Guid? programPloId,
        string method,
        long populationCount,
        long denominatorCount,
        long attainedCount,
        long notAttainedObservedCount,
        long notAttainedCount,
        decimal? attainmentRate,
        decimal thetaCoh,
        string outcomeStatus,
        long missingInDenominatorCount = 0,
        long missingExcludedCount = 0,
        long policyExcludedCount = 0,
        bool privacySuppressed = false)
    {
        return new CohortOutcomeResult
        {
            AcademicYearStart = academicYearStart,
            Id = id,
            BatchId = batchId,
            OrgUnitId = orgUnitId,
            ProgramId = programId,
            ProgramVersionId = programVersionId,
            MeasurementPeriodId = measurementPeriodId,
            CohortId = cohortId,
            CurriculumPathId = curriculumPathId,
            OutcomeLevel = outcomeLevel,
            CloId = cloId,
            ProgramPiId = programPiId,
            ProgramPloId = programPloId,
            Method = method,
            PopulationCount = populationCount,
            DenominatorCount = denominatorCount,
            AttainedCount = attainedCount,
            NotAttainedObservedCount = notAttainedObservedCount,
            NotAttainedCount = notAttainedCount,
            AttainmentRate = attainmentRate,
            ThetaCoh = thetaCoh,
            OutcomeStatus = outcomeStatus,
            MissingInDenominatorCount = missingInDenominatorCount,
            MissingExcludedCount = missingExcludedCount,
            PolicyExcludedCount = policyExcludedCount,
            PrivacySuppressed = privacySuppressed
        };
    }
}
