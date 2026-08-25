namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class SyllabusTemplateSection
{
    private SyllabusTemplateSection() { }

    public Guid Id { get; private set; }
    public Guid SyllabusTemplateVersionId { get; private set; }
    public string SectionCode { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public int SortOrder { get; private set; }
    public bool Required { get; private set; }
    public string ContentType { get; private set; } = null!;
    public bool Locked { get; private set; }
    public SyllabusTemplateVersion SyllabusTemplateVersion { get; private set; } = null!;
    public ICollection<SyllabusTemplateField> Fields { get; private set; } = new List<SyllabusTemplateField>();
}
