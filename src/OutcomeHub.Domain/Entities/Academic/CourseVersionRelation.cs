namespace OutcomeHub.Domain.Entities.Academic;

public sealed class CourseVersionRelation
{
    private CourseVersionRelation() { }

    public Guid Id { get; private set; }
    public Guid FromCourseVersionId { get; private set; }
    public Guid ToCourseVersionId { get; private set; }
    public Guid? ProgramVersionId { get; private set; }
    public string RelationType { get; private set; } = null!;
    public Guid DecisionId { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string Status { get; private set; } = null!;
    public string? Rationale { get; private set; }

    public CourseVersion FromCourseVersion { get; private set; } = null!;
    public CourseVersion ToCourseVersion { get; private set; } = null!;
    public ProgramVersion? ProgramVersion { get; private set; }
    public DecisionRecord Decision { get; private set; } = null!;
}
