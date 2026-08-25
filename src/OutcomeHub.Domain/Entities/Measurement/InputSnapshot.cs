namespace OutcomeHub.Domain.Entities.Measurement;

public sealed class InputSnapshot
{
    private InputSnapshot()
    {
    }

    public Guid Id { get; private set; }

    public Guid GovernedResourceId { get; private set; }

    public Guid MeasurementPeriodId { get; private set; }

    public Guid OrgUnitId { get; private set; }

    public int SnapshotNo { get; private set; }

    public Guid PolicyVersionId { get; private set; }

    public Guid ProgramPolicyBindingId { get; private set; }

    public Guid InstitutionTemplateVersionId { get; private set; }

    public Guid ProgramVersionId { get; private set; }

    public short AcademicYearStart { get; private set; }

    public string Status { get; private set; } = null!;

    public string SchemaVersion { get; private set; } = null!;

    public string HashAlgorithm { get; private set; } = null!;

    public string? ManifestChecksum { get; private set; }

    public long PopulationCount { get; private set; }

    public long ScoreCount { get; private set; }

    public Guid? ParentSnapshotId { get; private set; }

    public Guid CreatedBy { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public Guid? SealedBy { get; private set; }

    public DateTimeOffset? SealedAt { get; private set; }

    public OutcomeHub.Domain.Entities.Governance.GovernedResource GovernedResource { get; private set; } = null!;
    public MeasurementPeriod MeasurementPeriod { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.OrgUnit OrgUnit { get; private set; } = null!;
    public CalculationPolicyVersion PolicyVersion { get; private set; } = null!;
    public ProgramPolicyBinding ProgramPolicyBinding { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.InstitutionTemplateVersion InstitutionTemplateVersion { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Academic.ProgramVersion ProgramVersion { get; private set; } = null!;
    public InputSnapshot? ParentSnapshot { get; private set; }
    public OutcomeHub.Domain.Entities.Iam.Principal Creator { get; private set; } = null!;
    public OutcomeHub.Domain.Entities.Iam.Principal? Sealer { get; private set; }

    public static InputSnapshot CreateBuilding(
        Guid id,
        Guid governedResourceId,
        Guid measurementPeriodId,
        Guid orgUnitId,
        int snapshotNo,
        Guid policyVersionId,
        Guid programPolicyBindingId,
        Guid institutionTemplateVersionId,
        Guid programVersionId,
        short academicYearStart,
        string schemaVersion,
        string hashAlgorithm,
        long populationCount,
        long scoreCount,
        Guid createdBy,
        DateTimeOffset createdAt)
    {
        return new InputSnapshot
        {
            Id = id,
            GovernedResourceId = governedResourceId,
            MeasurementPeriodId = measurementPeriodId,
            OrgUnitId = orgUnitId,
            SnapshotNo = snapshotNo,
            PolicyVersionId = policyVersionId,
            ProgramPolicyBindingId = programPolicyBindingId,
            InstitutionTemplateVersionId = institutionTemplateVersionId,
            ProgramVersionId = programVersionId,
            AcademicYearStart = academicYearStart,
            Status = "BUILDING",
            SchemaVersion = schemaVersion,
            HashAlgorithm = hashAlgorithm,
            ManifestChecksum = null,
            PopulationCount = populationCount,
            ScoreCount = scoreCount,
            CreatedBy = createdBy,
            CreatedAt = createdAt,
            SealedBy = null,
            SealedAt = null
        };
    }

    public void Seal(string manifestChecksum, Guid sealedBy, DateTimeOffset sealedAt)
    {
        ManifestChecksum = manifestChecksum;
        SealedBy = sealedBy;
        SealedAt = sealedAt;
        Status = "SEALED";
    }
}
