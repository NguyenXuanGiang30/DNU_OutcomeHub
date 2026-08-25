namespace OutcomeHub.Domain.Entities.Academic;

public sealed class CourseOfferingInstructor
{
    private CourseOfferingInstructor() { }

    public Guid Id { get; private set; }
    public Guid CourseOfferingId { get; private set; }
    public Guid StaffId { get; private set; }
    public string AssignmentRole { get; private set; } = null!;
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public bool IsPrimary { get; private set; }

    public CourseOffering CourseOffering { get; private set; } = null!;
    public Staff Staff { get; private set; } = null!;

    public static CourseOfferingInstructor Create(
        Guid id,
        Guid courseOfferingId,
        Guid staffId,
        string assignmentRole,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo = null,
        bool isPrimary = false)
    {
        return new CourseOfferingInstructor
        {
            Id = id,
            CourseOfferingId = courseOfferingId,
            StaffId = staffId,
            AssignmentRole = assignmentRole.Trim().ToUpperInvariant(),
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            IsPrimary = isPrimary,
        };
    }

    public void Update(string assignmentRole, DateOnly? effectiveTo, bool isPrimary)
    {
        AssignmentRole = assignmentRole.Trim().ToUpperInvariant();
        EffectiveTo = effectiveTo;
        IsPrimary = isPrimary;
    }
}
