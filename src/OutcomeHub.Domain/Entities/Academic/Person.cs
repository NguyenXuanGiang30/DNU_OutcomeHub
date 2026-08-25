using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Domain.Entities.Academic;

public sealed class Person
{
    private Person() { }

    public Guid Id { get; private set; }
    public Guid? SourceSystemId { get; private set; }
    public string? SourcePersonId { get; private set; }
    public string FullName { get; private set; } = null!;
    public byte[]? ContactCiphertext { get; private set; }
    public string? ContactLookupHash { get; private set; }
    public string Status { get; private set; } = null!;
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }

    public SourceSystem? SourceSystem { get; private set; }

    public static Person Create(
        Guid id,
        string fullName,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo = null,
        string status = "ACTIVE",
        Guid? sourceSystemId = null,
        string? sourcePersonId = null,
        byte[]? contactCiphertext = null,
        string? contactLookupHash = null)
    {
        return new Person
        {
            Id = id,
            FullName = fullName.Trim(),
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            Status = status.Trim().ToUpperInvariant(),
            SourceSystemId = sourceSystemId,
            SourcePersonId = string.IsNullOrWhiteSpace(sourcePersonId) ? null : sourcePersonId.Trim(),
            ContactCiphertext = contactCiphertext,
            ContactLookupHash = string.IsNullOrWhiteSpace(contactLookupHash) ? null : contactLookupHash.Trim().ToLowerInvariant(),
        };
    }

    public void Update(string fullName, string status, DateOnly? effectiveTo)
    {
        FullName = fullName.Trim();
        Status = status.Trim().ToUpperInvariant();
        EffectiveTo = effectiveTo;
    }
}
