namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class LearningMaterial
{
    private LearningMaterial() { }
    public Guid Id { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public string MaterialType { get; private set; } = null!;
    public string Citation { get; private set; } = null!;
    public string? Url { get; private set; }
    public bool Required { get; private set; }
    public int SortOrder { get; private set; }
    public SyllabusVersion SyllabusVersion { get; private set; } = null!;
}
