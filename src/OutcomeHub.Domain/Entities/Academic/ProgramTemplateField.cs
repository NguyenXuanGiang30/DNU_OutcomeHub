namespace OutcomeHub.Domain.Entities.Academic;

public sealed class ProgramTemplateField
{
    private ProgramTemplateField() { }

    public Guid Id { get; private set; }
    public Guid ProgramTemplateSectionId { get; private set; }
    public string FieldCode { get; private set; } = null!;
    public string Label { get; private set; } = null!;
    public string DataType { get; private set; } = null!;
    public bool Required { get; private set; }
    public string LockMode { get; private set; } = null!;
    public string? DefaultValue { get; private set; }
    public string? ValidationSchema { get; private set; }
    public int SortOrder { get; private set; }

    public ProgramTemplateSection ProgramTemplateSection { get; private set; } = null!;
}
