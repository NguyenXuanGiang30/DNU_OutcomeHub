namespace OutcomeHub.Domain.Entities.Academic;

public sealed class ProgramPlo
{
    private ProgramPlo() { }

    public Guid Id { get; private set; }
    public Guid ProgramVersionId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Domain { get; private set; } = null!;
    public string? BloomLevel { get; private set; }
    public Guid? SourceTemplatePloId { get; private set; }
    public bool IsLocked { get; private set; }
    public int SortOrder { get; private set; }

    public ProgramVersion ProgramVersion { get; private set; } = null!;
    public TemplatePlo? SourceTemplatePlo { get; private set; }

    public static ProgramPlo Create(
        Guid id,
        Guid programVersionId,
        string code,
        string description,
        string domain,
        string? bloomLevel,
        Guid? sourceTemplatePloId,
        bool isLocked,
        int sortOrder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);

        return new ProgramPlo
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id,
            ProgramVersionId = programVersionId,
            Code = code.Trim().ToUpperInvariant(),
            Description = description.Trim(),
            Domain = domain.Trim().ToUpperInvariant(),
            BloomLevel = string.IsNullOrWhiteSpace(bloomLevel) ? null : bloomLevel.Trim().ToUpperInvariant(),
            SourceTemplatePloId = sourceTemplatePloId,
            IsLocked = isLocked,
            SortOrder = sortOrder,
        };
    }
}
