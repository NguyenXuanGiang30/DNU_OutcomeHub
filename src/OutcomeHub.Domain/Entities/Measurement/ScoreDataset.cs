using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Governance;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class ScoreDataset
{
    private ScoreDataset() { }

    public Guid Id { get; private set; }
    public Guid GovernedResourceId { get; private set; }
    public Guid SourceSystemId { get; private set; }
    public short AcademicYearStart { get; private set; }
    public Guid CourseOfferingId { get; private set; }
    public string Classification { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    public GovernedResource GovernedResource { get; private set; } = null!;
    public SourceSystem SourceSystem { get; private set; } = null!;
    public CourseOffering CourseOffering { get; private set; } = null!;
    public ICollection<ScoreIdentity> ScoreIdentities { get; private set; } = new List<ScoreIdentity>();

    public static ScoreDataset Create(
        Guid id,
        Guid governedResourceId,
        Guid sourceSystemId,
        short academicYearStart,
        Guid courseOfferingId,
        string classification = "CONFIDENTIAL",
        DateTimeOffset? createdAt = null)
    {
        return new ScoreDataset
        {
            Id = id,
            GovernedResourceId = governedResourceId,
            SourceSystemId = sourceSystemId,
            AcademicYearStart = academicYearStart,
            CourseOfferingId = courseOfferingId,
            Classification = classification.Trim().ToUpperInvariant(),
            CreatedAt = createdAt ?? DateTimeOffset.UtcNow,
        };
    }
}
