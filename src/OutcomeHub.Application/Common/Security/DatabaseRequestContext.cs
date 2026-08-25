namespace OutcomeHub.Application.Common.Security;

public sealed record DatabaseRequestContext
{
    public const int PurposeMaxLength = 128;

    public DatabaseRequestContext(
        Guid principalId,
        Guid requestId,
        string purpose,
        Guid? jobId = null)
    {
        if (principalId == Guid.Empty)
        {
            throw new ArgumentException("Principal ID must be provided.", nameof(principalId));
        }

        if (requestId == Guid.Empty)
        {
            throw new ArgumentException("Request ID must be provided.", nameof(requestId));
        }

        if (jobId == Guid.Empty)
        {
            throw new ArgumentException("Job ID cannot be empty when provided.", nameof(jobId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(purpose);

        var normalizedPurpose = purpose.Trim();

        if (normalizedPurpose.Length > PurposeMaxLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(purpose),
                purpose,
                $"Purpose cannot exceed {PurposeMaxLength} characters.");
        }

        PrincipalId = principalId;
        RequestId = requestId;
        Purpose = normalizedPurpose;
        JobId = jobId;
    }

    public Guid PrincipalId { get; }

    public Guid RequestId { get; }

    public string Purpose { get; }

    public Guid? JobId { get; }
}
