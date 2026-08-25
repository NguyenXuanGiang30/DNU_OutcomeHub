namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class SyllabusTemplateRubricScaleLevel
{
    private SyllabusTemplateRubricScaleLevel() { }

    public Guid Id { get; private set; }
    public Guid RubricScaleId { get; private set; }
    public string LevelCode { get; private set; } = null!;
    public string Label { get; private set; } = null!;
    public int LevelOrder { get; private set; }
    public decimal ScoreFrom { get; private set; }
    public decimal ScoreTo { get; private set; }
    public decimal? NumericValue { get; private set; }
    public SyllabusTemplateRubricScale RubricScale { get; private set; } = null!;
}
