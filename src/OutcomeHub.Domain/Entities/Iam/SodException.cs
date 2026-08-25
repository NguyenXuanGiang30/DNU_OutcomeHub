using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Domain.Entities.Iam;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "SodException represents an approved separation-of-duties exception, not a CLR exception type.")]
public sealed class SodException
{
    private SodException()
    {
    }

    public Guid Id { get; private set; }
    public Guid RuleId { get; private set; }
    public Guid PrincipalId { get; private set; }
    public Guid AccessScopeId { get; private set; }
    public string Reason { get; private set; } = null!;
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly EffectiveTo { get; private set; }
    public Guid DecisionId { get; private set; }
    public Guid ApprovedBy { get; private set; }

    public SodRule Rule { get; private set; } = null!;
    public Principal Principal { get; private set; } = null!;
    public AccessScope AccessScope { get; private set; } = null!;
    public DecisionRecord Decision { get; private set; } = null!;
    public Principal ApprovedByPrincipal { get; private set; } = null!;
}
