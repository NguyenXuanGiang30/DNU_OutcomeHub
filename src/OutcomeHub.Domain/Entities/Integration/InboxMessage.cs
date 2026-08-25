namespace OutcomeHub.Domain.Entities.Integration;

public sealed class InboxMessage
{
    private InboxMessage() { }

    public Guid Id { get; private set; }
    public Guid SourceSystemId { get; private set; }
    public string MessageId { get; private set; } = null!;
    public string MessageType { get; private set; } = null!;
    public int EventSchemaVersion { get; private set; }
    public string Payload { get; private set; } = null!;
    public string PayloadChecksum { get; private set; } = null!;
    public string Classification { get; private set; } = null!;
    public int SignatureKeyVersion { get; private set; }
    public bool SignatureValid { get; private set; }
    public string Nonce { get; private set; } = null!;
    public DateTimeOffset SourceTimestamp { get; private set; }
    public DateTimeOffset ReceivedAt { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }
    public string Status { get; private set; } = null!;
    public int AttemptCount { get; private set; }
    public Guid? LockedBy { get; private set; }
    public DateTimeOffset? LockedUntil { get; private set; }
    public string? ErrorCode { get; private set; }

    public SourceSystem SourceSystem { get; private set; } = null!;
}
