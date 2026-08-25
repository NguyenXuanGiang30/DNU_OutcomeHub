using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Domain.Entities.Governance;

public sealed class PrivacyRequest
{
    private PrivacyRequest() { }
    public Guid Id { get; private set; }
    public Guid SubjectPersonId { get; private set; }
    public string RequestType { get; private set; } = null!;
    public string LegalBasis { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public DateTimeOffset RequestedAt { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public Guid? DispositionCaseId { get; private set; }
    public Person SubjectPerson { get; private set; } = null!;
    public Principal? Approver { get; private set; }
    public DispositionCase? DispositionCase { get; private set; }
}
