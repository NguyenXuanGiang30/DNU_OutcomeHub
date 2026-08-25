namespace OutcomeHub.Domain.Entities.Academic;

public sealed class Student
{
    private Student() { }

    public Guid PersonId { get; private set; }
    public string StudentCode { get; private set; } = null!;
    public Guid AdmissionCohortId { get; private set; }
    public string CurrentStatus { get; private set; } = null!;

    public Person Person { get; private set; } = null!;
    public Cohort AdmissionCohort { get; private set; } = null!;

    public static Student Create(
        Guid personId,
        string studentCode,
        Guid admissionCohortId,
        string currentStatus = "ACTIVE")
    {
        return new Student
        {
            PersonId = personId,
            StudentCode = studentCode.Trim().ToUpperInvariant(),
            AdmissionCohortId = admissionCohortId,
            CurrentStatus = currentStatus.Trim().ToUpperInvariant(),
        };
    }

    public void Update(string currentStatus)
    {
        CurrentStatus = currentStatus.Trim().ToUpperInvariant();
    }
}
