namespace OutcomeHub.Domain.Entities.Quality;

public sealed class ImprovementEvidence
{
    private ImprovementEvidence()
    {
    }

    public Guid Id { get; private set; }

    public Guid ImprovementPlanId { get; private set; }

    public Guid? ImprovementActionId { get; private set; }

    public Guid EvidenceVersionId { get; private set; }

    public string LinkRole { get; private set; } = null!;

    public Guid? VerifiedBy { get; private set; }

    public DateTimeOffset? VerifiedAt { get; private set; }

    public ImprovementPlan ImprovementPlan { get; private set; } = null!;
    public ImprovementAction? ImprovementAction { get; private set; }
    public OutcomeHub.Domain.Entities.Document.EvidenceVersion EvidenceVersion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Iam.Principal? VerifiedByPrincipal { get; private set; }

    /// <summary>
    /// Creates unverified evidence attached to a plan (and optionally to an action).
    /// </summary>
    public static ImprovementEvidence Create(
        Guid id,
        Guid improvementPlanId,
        Guid? improvementActionId,
        Guid evidenceVersionId,
        string linkRole)
    {
        return new ImprovementEvidence
        {
            Id = id,
            ImprovementPlanId = improvementPlanId,
            ImprovementActionId = improvementActionId,
            EvidenceVersionId = evidenceVersionId,
            LinkRole = linkRole,
            VerifiedBy = null,
            VerifiedAt = null
        };
    }

    /// <summary>
    /// Verifies the evidence by the authorized principal.
    /// </summary>
    public void Verify(Guid verifiedBy, DateTimeOffset verifiedAt)
    {
        VerifiedBy = verifiedBy;
        VerifiedAt = verifiedAt;
    }
}
