using OutcomeHub.Domain.Enums.Iam;

namespace OutcomeHub.Domain.Entities.Iam;

public sealed class Principal
{
    public const int PrincipalTypeMaxLength = 20;
    public const int StatusMaxLength = 20;
    public const int DisplayNameMaxLength = 255;

    private Principal()
    {
    }

    private Principal(
        Guid id,
        PrincipalType principalType,
        PrincipalStatus status,
        string displayName,
        DateTimeOffset createdAt)
    {
        Id = id;
        PrincipalType = principalType;
        Status = status;
        DisplayName = displayName;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public PrincipalType PrincipalType { get; private set; }

    public PrincipalStatus Status { get; private set; }

    public string DisplayName { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    public static Principal Create(
        PrincipalType principalType,
        PrincipalStatus status,
        string displayName,
        DateTimeOffset createdAt)
    {
        if (!Enum.IsDefined(principalType))
        {
            throw new ArgumentOutOfRangeException(
                nameof(principalType),
                principalType,
                "Principal type is not supported.");
        }

        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(
                nameof(status),
                status,
                "Principal status is not supported.");
        }

        if (createdAt == default)
        {
            throw new ArgumentOutOfRangeException(
                nameof(createdAt),
                createdAt,
                "Created time must be provided.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        var normalizedDisplayName = displayName.Trim();

        if (normalizedDisplayName.Length > DisplayNameMaxLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(displayName),
                displayName,
                $"Display name cannot exceed {DisplayNameMaxLength} characters.");
        }

        return new Principal(
            Guid.CreateVersion7(),
            principalType,
            status,
            normalizedDisplayName,
            createdAt.ToUniversalTime());
    }
}
