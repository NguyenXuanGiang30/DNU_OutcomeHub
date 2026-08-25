namespace OutcomeHub.Domain.Entities.Academic;

public sealed class TemplatePi
{
    private TemplatePi() { }

    public Guid Id { get; private set; }
    public Guid InstitutionTemplateVersionId { get; private set; }
    public Guid TemplatePloId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public int SortOrder { get; private set; }
    public bool IsLocked { get; private set; }
    public bool IsCore { get; private set; }

    public InstitutionTemplateVersion InstitutionTemplateVersion { get; private set; } = null!;
    public TemplatePlo TemplatePlo { get; private set; } = null!;
}
