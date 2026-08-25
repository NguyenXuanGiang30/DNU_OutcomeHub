using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class SyllabusTraceability
{
    private SyllabusTraceability() { }
    public Guid Id { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public Guid ProgramCourseId { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public Guid CloId { get; private set; }
    public Guid? CoursePiMappingId { get; private set; }
    public Guid RubricCriterionId { get; private set; }
    public string DataRole { get; private set; } = null!;
    public string? EvidenceRequirement { get; private set; }
    public decimal? AllocationRatio { get; private set; }
    public Guid? ExceptionDecisionId { get; private set; }
    public string? Rationale { get; private set; }
    public SyllabusVersion SyllabusVersion { get; private set; } = null!;
    public ProgramCourse ProgramCourse { get; private set; } = null!;
    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public Clo Clo { get; private set; } = null!;
    public CoursePiMapping? CoursePiMapping { get; private set; }
    public RubricCriterion RubricCriterion { get; private set; } = null!;
    public DecisionRecord? ExceptionDecision { get; private set; }
    public DirectPiCriterionWeight? DirectPiCriterionWeight { get; private set; }
}
