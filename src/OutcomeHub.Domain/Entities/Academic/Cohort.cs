namespace OutcomeHub.Domain.Entities.Academic;

public sealed class Cohort
{
    private Cohort() { }

    public Guid Id { get; private set; }
    public Guid ProgramId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public int AdmissionYear { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }

    public Program Program { get; private set; } = null!;

    public static Cohort Create(
        Guid id,
        Guid programId,
        string code,
        string name,
        int admissionYear,
        DateOnly startDate,
        DateOnly? endDate = null)
    {
        return new Cohort
        {
            Id = id,
            ProgramId = programId,
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            AdmissionYear = admissionYear,
            StartDate = startDate,
            EndDate = endDate,
        };
    }

    public void Update(string name, DateOnly? endDate)
    {
        Name = name.Trim();
        EndDate = endDate;
    }
}
