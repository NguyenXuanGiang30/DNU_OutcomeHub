namespace OutcomeHub.Domain.Entities.Result;

public sealed class PublicationAudience
{
    private PublicationAudience()
    {
    }

    public Guid PublicationId { get; private set; }

    public Guid AccessScopeId { get; private set; }

    public string AudienceRole { get; private set; } = null!;

    public bool AllowStudentDetail { get; private set; }

    public Publication Publication { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Iam.AccessScope AccessScope { get; private set; } = null!;
}
