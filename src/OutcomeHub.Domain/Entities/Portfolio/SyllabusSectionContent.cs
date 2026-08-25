using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class SyllabusSectionContent
{
    private SyllabusSectionContent() { }

    public Guid Id { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public Guid SyllabusTemplateVersionId { get; private set; }
    public Guid TemplateFieldId { get; private set; }
    public string? ContentText { get; private set; }
    public string? ContentJsonb { get; private set; }
    public string SourceKind { get; private set; } = null!;
    public bool IsInherited { get; private set; }
    public Guid LastEditedBy { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid UpdatedBy { get; private set; }
    public long RowVersion { get; private set; }
    public SyllabusVersion SyllabusVersion { get; private set; } = null!;
    public SyllabusTemplateVersion SyllabusTemplateVersion { get; private set; } = null!;
    public SyllabusTemplateField TemplateField { get; private set; } = null!;
    public Principal LastEditor { get; private set; } = null!;
}
