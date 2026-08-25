namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class RubricLevel
{
    private RubricLevel() { }
    public Guid Id { get; private set; }
    public Guid RubricCriterionId { get; private set; }
    public string LevelCode { get; private set; } = null!;
    public int LevelOrder { get; private set; }
    public string Label { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal ScoreFrom { get; private set; }
    public decimal ScoreTo { get; private set; }
    public decimal? NumericValue { get; private set; }
    public RubricCriterion RubricCriterion { get; private set; } = null!;

    public static RubricLevel Create(
        Guid id,
        Guid rubricCriterionId,
        string levelCode,
        int levelOrder,
        string label,
        string? description,
        decimal scoreFrom,
        decimal scoreTo,
        decimal? numericValue)
    {
        return new RubricLevel
        {
            Id = id,
            RubricCriterionId = rubricCriterionId,
            LevelCode = levelCode.Trim().ToUpperInvariant(),
            LevelOrder = levelOrder,
            Label = label.Trim(),
            Description = description?.Trim(),
            ScoreFrom = scoreFrom,
            ScoreTo = scoreTo,
            NumericValue = numericValue,
        };
    }
}
