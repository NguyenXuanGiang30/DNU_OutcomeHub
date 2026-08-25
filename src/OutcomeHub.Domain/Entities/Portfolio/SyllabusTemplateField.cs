namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class SyllabusTemplateField
{
    private SyllabusTemplateField() { }

    public Guid Id { get; private set; }
    public Guid SyllabusTemplateSectionId { get; private set; }
    public Guid SyllabusTemplateVersionId { get; private set; }
    public string FieldCode { get; private set; } = null!;
    public string Label { get; private set; } = null!;
    public string DataType { get; private set; } = null!;
    public bool Required { get; private set; }
    public string LockMode { get; private set; } = null!;
    public string? DefaultValue { get; private set; }
    public string? ValidationSchema { get; private set; }
    public int SortOrder { get; private set; }
    public SyllabusTemplateSection SyllabusTemplateSection { get; private set; } = null!;
    public SyllabusTemplateVersion SyllabusTemplateVersion { get; private set; } = null!;
}
