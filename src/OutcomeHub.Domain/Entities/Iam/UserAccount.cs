using OutcomeHub.Domain.Entities.Academic;

namespace OutcomeHub.Domain.Entities.Iam;

public sealed class UserAccount
{
    private UserAccount()
    {
    }

    public Guid PrincipalId { get; private set; }
    public Guid? PersonId { get; private set; }
    public string? Username { get; private set; }
    public byte[]? EmailCiphertext { get; private set; }
    public string? EmailLookupHash { get; private set; }
    public DateTimeOffset? LastLoginAt { get; private set; }

    public Principal Principal { get; private set; } = null!;
    public Person? Person { get; private set; }
}
