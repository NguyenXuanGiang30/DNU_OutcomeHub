namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class SnapshotOffering
{
    private SnapshotOffering()
    {
    }

    public Guid InputSnapshotId { get; private set; }

    public Guid CourseOfferingId { get; private set; }

    public Guid ProgramCourseId { get; private set; }

    public Guid CourseVersionId { get; private set; }

    public Guid SyllabusVersionId { get; private set; }

    public Guid? CurriculumPathId { get; private set; }

    public string SourceRole { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    public InputSnapshot InputSnapshot { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CourseOffering CourseOffering { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramCourse ProgramCourse { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CourseVersion CourseVersion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Portfolio.SyllabusVersion SyllabusVersion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.CurriculumPath? CurriculumPath { get; private set; }
}
