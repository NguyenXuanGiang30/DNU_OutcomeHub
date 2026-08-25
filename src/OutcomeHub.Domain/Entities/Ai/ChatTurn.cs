namespace OutcomeHub.Domain.Entities.Ai;

public sealed class ChatTurn
{
    private ChatTurn()
    {
    }

    public Guid Id { get; private set; }

    public Guid ChatSessionId { get; private set; }

    public int TurnNo { get; private set; }

    public byte[] UserMessageCiphertext { get; private set; } = null!;

    public Guid AiJobId { get; private set; }

    public Guid? AssistantArtifactId { get; private set; }

    public DateTimeOffset DataAsOf { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public ChatSession ChatSession { get; private set; } = null!;

    public AiJob AiJob { get; private set; } = null!;

    public AiArtifact? AssistantArtifact { get; private set; }
}
