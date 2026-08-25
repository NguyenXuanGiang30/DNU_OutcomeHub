namespace OutcomeHub.Domain.Entities.Academic;

public sealed class ProgramPi
{
    private ProgramPi() { }

    public Guid Id { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public Guid ProgramPloId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Guid? SourceTemplatePiId { get; private set; }
    public bool IsLocked { get; private set; }
    public bool IsCore { get; private set; }
    public decimal? WeightRatio { get; private set; }
    public int SortOrder { get; private set; }

    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public ProgramPlo ProgramPlo { get; private set; } = null!;
    public TemplatePi? SourceTemplatePi { get; private set; }

    public static ProgramPi Create(
        Guid id,
        Guid programVersionId,
        Guid programPloId,
        string code,
        string description,
        Guid? sourceTemplatePiId,
        bool isLocked,
        bool isCore,
        decimal? weightRatio,
        int sortOrder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        return new ProgramPi
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            ProgramVersionId = programVersionId,
            ProgramPloId = programPloId,
            Code = code.Trim().ToUpperInvariant(),
            Description = description.Trim(),
            SourceTemplatePiId = sourceTemplatePiId,
            IsLocked = isLocked,
            IsCore = isCore,
            WeightRatio = weightRatio,
            SortOrder = sortOrder,
        };
    }
}
