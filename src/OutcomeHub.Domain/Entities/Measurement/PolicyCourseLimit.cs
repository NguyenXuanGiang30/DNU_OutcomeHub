namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class PolicyCourseLimit
{
    private PolicyCourseLimit()
    {
    }

    public Guid PolicyVersionId { get; private set; }

    public string CourseType { get; private set; } = null!;

    public int? MaxMCount { get; private set; }

    public int? MaxDirectPiCount { get; private set; }

    public bool ExceptionRequired { get; private set; }

    public CalculationPolicyVersion PolicyVersion { get; private set; } = null!;
}
