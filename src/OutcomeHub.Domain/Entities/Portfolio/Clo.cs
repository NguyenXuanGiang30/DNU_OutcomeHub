namespace OutcomeHub.Domain.Entities.Portfolio;

public sealed class Clo
{
    private Clo() { }
    public Guid Id { get; private set; }
    public Guid SyllabusVersionId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Domain { get; private set; } = null!;
    public string BloomLevel { get; private set; } = null!;
    public bool IsCore { get; private set; }
    public int SortOrder { get; private set; }
    public SyllabusVersion SyllabusVersion { get; private set; } = null!;

    public static Clo Create(
        Guid id,
        Guid syllabusVersionId,
        string code,
        string description,
        string domain,
        string bloomLevel,
        bool isCore,
        int sortOrder)
    {
        return new Clo
        {
            Id = id,
            SyllabusVersionId = syllabusVersionId,
            Code = code.Trim().ToUpperInvariant(),
            Description = description.Trim(),
            Domain = domain.Trim().ToUpperInvariant(),
            BloomLevel = bloomLevel.Trim().ToUpperInvariant(),
            IsCore = isCore,
            SortOrder = sortOrder,
        };
    }

    public void Update(string description, string domain, string bloomLevel, bool isCore, int sortOrder)
    {
        Description = description.Trim();
        Domain = domain.Trim().ToUpperInvariant();
        BloomLevel = bloomLevel.Trim().ToUpperInvariant();
        IsCore = isCore;
        SortOrder = sortOrder;
    }
}
