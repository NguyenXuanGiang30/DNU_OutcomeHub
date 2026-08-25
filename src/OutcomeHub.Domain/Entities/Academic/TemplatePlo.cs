namespace OutcomeHub.Domain.Entities.Academic;

public sealed class TemplatePlo
{
    private TemplatePlo() { }

    public Guid Id { get; private set; }
    public Guid InstitutionTemplateVersionId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Domain { get; private set; } = null!;
    public string? BloomLevel { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsLocked { get; private set; }

    public InstitutionTemplateVersion InstitutionTemplateVersion { get; private set; } = null!;
}
