namespace OutcomeHub.Domain.Entities.Academic;

public sealed class ProgramTemplateSection
{
    private ProgramTemplateSection() { }

    public Guid Id { get; private set; }
    public Guid InstitutionTemplateVersionId { get; private set; }
    public string SectionCode { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public int SortOrder { get; private set; }
    public bool Required { get; private set; }
    public string LockMode { get; private set; } = null!;

    public InstitutionTemplateVersion InstitutionTemplateVersion { get; private set; } = null!;
}
