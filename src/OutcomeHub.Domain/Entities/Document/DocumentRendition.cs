namespace OutcomeHub.Domain.Entities.Document;

public sealed class DocumentRendition
{
    private DocumentRendition() { }
    public Guid Id { get; private set; }
    public Guid DocumentVersionId { get; private set; }
    public string RenditionType { get; private set; } = null!;
    public Guid FileObjectId { get; private set; }
    public string RendererName { get; private set; } = null!;
    public string RendererVersion { get; private set; } = null!;
    public string? TemplateChecksum { get; private set; }
    public string Checksum { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DocumentVersion DocumentVersion { get; private set; } = null!;
    public FileObject FileObject { get; private set; } = null!;
}
