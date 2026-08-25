namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class CourseObjectiveClo
{
    private CourseObjectiveClo() { }
    public Guid CourseObjectiveId { get; private set; }
    public Guid CloId { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public CourseObjective CourseObjective { get; private set; } = null!;
    public Clo Clo { get; private set; } = null!;
}
