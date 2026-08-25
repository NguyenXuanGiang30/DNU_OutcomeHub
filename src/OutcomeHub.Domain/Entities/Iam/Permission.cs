namespace OutcomeHub.Domain.Entities.Iam;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "Permission is the established IAM domain term and database entity name.")]
public sealed class Permission
{
    private Permission()
    {
    }

    public Guid Id { get; private set; }
    public string ResourceType { get; private set; } = null!;
    public string Action { get; private set; } = null!;
    public string FieldScope { get; private set; } = null!;
    public string? Description { get; private set; }
}
