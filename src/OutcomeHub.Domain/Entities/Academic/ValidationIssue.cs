namespace OutcomeHub.Domain.Entities.Academic;

public sealed class ValidationIssue
{
    private ValidationIssue() { }

    public Guid Id { get; private set; }
    public Guid ValidationRunId { get; private set; }
    public string RuleCode { get; private set; } = null!;
    public string Severity { get; private set; } = null!;
    public string EntityType { get; private set; } = null!;
    public Guid? EntityId { get; private set; }
    public string? FieldPath { get; private set; }
    public string Message { get; private set; } = null!;
    public string? Details { get; private set; }

    public ValidationRun ValidationRun { get; private set; } = null!;
}
