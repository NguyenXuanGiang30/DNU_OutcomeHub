using OutcomeHub.Domain.Entities.Document;

namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class SyllabusDocument
{
    private SyllabusDocument() { }
    public Guid SyllabusVersionId { get; private set; }
    public Guid DocumentVersionId { get; private set; }
    public string DocumentRole { get; private set; } = null!;
    public SyllabusVersion SyllabusVersion { get; private set; } = null!;
    public DocumentVersion DocumentVersion { get; private set; } = null!;
}
