using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Integration;

public sealed class QuarantineCorrection
{
    private QuarantineCorrection() { }

    public Guid Id { get; private set; }
    public Guid QuarantineRecordId { get; private set; }
    public int RevisionNo { get; private set; }
    public string NormalizedPayload { get; private set; } = null!;
    public string Reason { get; private set; } = null!;
    public Guid CorrectedBy { get; private set; }
    public DateTimeOffset CorrectedAt { get; private set; }
    public string Checksum { get; private set; } = null!;

    public QuarantineRecord QuarantineRecord { get; private set; } = null!;
    public Principal CorrectedByPrincipal { get; private set; } = null!;
}
