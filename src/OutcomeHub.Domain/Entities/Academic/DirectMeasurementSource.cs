namespace OutcomeHub.Domain.Entities.Academic;

public sealed class DirectMeasurementSource
{
    private DirectMeasurementSource() { }

    public Guid Id { get; private set; }
    public Guid DirectMeasurementPlanId { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public Guid CurriculumPathId { get; private set; }
    public Guid ProgramPiId { get; private set; }
    public Guid CoursePiMappingId { get; private set; }
    public int? PlannedTerm { get; private set; }
    public Guid OwnerOrgUnitId { get; private set; }
    public decimal SourceWeightRatio { get; private set; }
    public string SourceRole { get; private set; } = null!;
    public int SortOrder { get; private set; }

    public DirectMeasurementPlan DirectMeasurementPlan { get; private set; } = null!;
    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public CurriculumPath CurriculumPath { get; private set; } = null!;
    public ProgramPi ProgramPi { get; private set; } = null!;
    public CoursePiMapping CoursePiMapping { get; private set; } = null!;
    public OrgUnit OwnerOrgUnit { get; private set; } = null!;
}
