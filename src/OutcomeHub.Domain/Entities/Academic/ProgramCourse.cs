namespace OutcomeHub.Domain.Entities.Academic;

public sealed class ProgramCourse
{
    private ProgramCourse() { }

    public Guid Id { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public Guid CourseVersionId { get; private set; }
    public Guid CurriculumBlockId { get; private set; }
    public string CatalogRole { get; private set; } = null!;
    public decimal? CreditOverride { get; private set; }
    public bool IsLocked { get; private set; }
    public string Status { get; private set; } = null!;

    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public CourseVersion CourseVersion { get; private set; } = null!;
    public CurriculumBlock CurriculumBlock { get; private set; } = null!;

    public static ProgramCourse Create(
        Guid id,
        Guid programVersionId,
        Guid courseVersionId,
        Guid curriculumBlockId,
        string catalogRole,
        decimal? creditOverride,
        bool isLocked,
        string status = "DRAFT")
    {
        return new ProgramCourse
        {
            Id = id,
            ProgramVersionId = programVersionId,
            CourseVersionId = courseVersionId,
            CurriculumBlockId = curriculumBlockId,
            CatalogRole = catalogRole.Trim().ToUpperInvariant(),
            CreditOverride = creditOverride,
            IsLocked = isLocked,
            Status = status.Trim().ToUpperInvariant(),
        };
    }
}
