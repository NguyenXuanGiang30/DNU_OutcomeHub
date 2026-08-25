namespace OutcomeHub.Domain.Entities.Academic;

public sealed class StudentPath
{
    private StudentPath() { }

    public Guid Id { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid ProgramId { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public Guid CurriculumPathId { get; private set; }
    public string PathStatus { get; private set; } = null!;
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public Guid? DecisionId { get; private set; }
    public bool IsPrimary { get; private set; }

    public Student Student { get; private set; } = null!;
    public Program Program { get; private set; } = null!;
    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public CurriculumPath CurriculumPath { get; private set; } = null!;
    public DecisionRecord? Decision { get; private set; }

    public static StudentPath Create(
        Guid id,
        Guid studentId,
        Guid programId,
        Guid programVersionId,
        Guid curriculumPathId,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo = null,
        string pathStatus = "ACTIVE",
        Guid? decisionId = null,
        bool isPrimary = true)
    {
        return new StudentPath
        {
            Id = id,
            StudentId = studentId,
            ProgramId = programId,
            ProgramVersionId = programVersionId,
            CurriculumPathId = curriculumPathId,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            PathStatus = pathStatus.Trim().ToUpperInvariant(),
            DecisionId = decisionId,
            IsPrimary = isPrimary,
        };
    }

    public void Update(string pathStatus, DateOnly? effectiveTo, bool isPrimary)
    {
        PathStatus = pathStatus.Trim().ToUpperInvariant();
        EffectiveTo = effectiveTo;
        IsPrimary = isPrimary;
    }
}
