namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class SyllabusTemplateRubricScale
{
    private SyllabusTemplateRubricScale() { }

    public Guid Id { get; private set; }
    public Guid SyllabusTemplateVersionId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public SyllabusTemplateVersion SyllabusTemplateVersion { get; private set; } = null!;
    public ICollection<SyllabusTemplateRubricScaleLevel> Levels { get; private set; } = new List<SyllabusTemplateRubricScaleLevel>();
}
