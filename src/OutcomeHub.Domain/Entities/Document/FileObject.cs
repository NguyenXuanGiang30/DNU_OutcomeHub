using OutcomeHub.Domain.Entities.Governance;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Document;

public sealed class FileObject
{
    private FileObject() { }
    public Guid Id { get; private set; }
    public Guid GovernedResourceId { get; private set; }
    public string StorageProvider { get; private set; } = null!;
    public string Bucket { get; private set; } = null!;
    public string ObjectKey { get; private set; } = null!;
    public string StorageVersion { get; private set; } = null!;
    public string OriginalFilename { get; private set; } = null!;
    public string DeclaredMediaType { get; private set; } = null!;
    public string? DetectedMediaType { get; private set; }
    public long SizeBytes { get; private set; }
    public string Sha256 { get; private set; } = null!;
    public string Classification { get; private set; } = null!;
    public string MalwareScanStatus { get; private set; } = null!;
    public string? MalwareScanEngine { get; private set; }
    public string? MalwareScanVersion { get; private set; }
    public DateTimeOffset? MalwareScanAt { get; private set; }
    public string? EncryptionKeyReference { get; private set; }
    public DateTimeOffset? PurgedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public GovernedResource GovernedResource { get; private set; } = null!;
    public Principal Creator { get; private set; } = null!;
}
