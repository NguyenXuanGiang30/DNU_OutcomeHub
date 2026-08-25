using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OutcomeHub.Domain.Entities.Iam;
using OutcomeHub.Domain.Enums.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Iam;

public sealed class PrincipalConfiguration : IEntityTypeConfiguration<Principal>
{
    private static readonly ValueConverter<PrincipalType, string> PrincipalTypeConverter = new(
        principalType => ConvertPrincipalTypeToDatabase(principalType),
        databaseValue => ConvertPrincipalTypeFromDatabase(databaseValue));

    private static readonly ValueConverter<PrincipalStatus, string> PrincipalStatusConverter = new(
        status => ConvertPrincipalStatusToDatabase(status),
        databaseValue => ConvertPrincipalStatusFromDatabase(databaseValue));

    public void Configure(EntityTypeBuilder<Principal> builder)
    {
        builder.ToTable(
            "principal",
            "iam",
            tableBuilder =>
            {
                tableBuilder.HasCheckConstraint(
                    "ck_principal_principal_type",
                    "principal_type IN ('USER', 'SERVICE_ACCOUNT', 'SYSTEM')");
                tableBuilder.HasCheckConstraint(
                    "ck_principal_status",
                    "status IN ('ACTIVE', 'LOCKED', 'DISABLED', 'EXPIRED')");
                tableBuilder.HasCheckConstraint(
                    "ck_principal_display_name",
                    "display_name = btrim(display_name) AND char_length(display_name) > 0");
            });

        builder.HasKey(principal => principal.Id)
            .HasName("pk_principal");

        builder.Property(principal => principal.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .ValueGeneratedNever();

        builder.Property(principal => principal.PrincipalType)
            .HasColumnName("principal_type")
            .HasColumnType($"varchar({Principal.PrincipalTypeMaxLength})")
            .HasMaxLength(Principal.PrincipalTypeMaxLength)
            .HasConversion(PrincipalTypeConverter)
            .IsRequired();

        builder.Property(principal => principal.Status)
            .HasColumnName("status")
            .HasColumnType($"varchar({Principal.StatusMaxLength})")
            .HasMaxLength(Principal.StatusMaxLength)
            .HasConversion(PrincipalStatusConverter)
            .IsRequired();

        builder.Property(principal => principal.DisplayName)
            .HasColumnName("display_name")
            .HasColumnType($"varchar({Principal.DisplayNameMaxLength})")
            .HasMaxLength(Principal.DisplayNameMaxLength)
            .IsRequired();

        builder.Property(principal => principal.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired();
    }

    private static string ConvertPrincipalTypeToDatabase(PrincipalType principalType)
    {
        return principalType switch
        {
            PrincipalType.User => "USER",
            PrincipalType.ServiceAccount => "SERVICE_ACCOUNT",
            PrincipalType.System => "SYSTEM",
            _ => throw new ArgumentOutOfRangeException(
                nameof(principalType),
                principalType,
                "Principal type is not supported.")
        };
    }

    private static PrincipalType ConvertPrincipalTypeFromDatabase(string databaseValue)
    {
        return databaseValue switch
        {
            "USER" => PrincipalType.User,
            "SERVICE_ACCOUNT" => PrincipalType.ServiceAccount,
            "SYSTEM" => PrincipalType.System,
            _ => throw new InvalidOperationException(
                $"Unsupported principal type value '{databaseValue}'.")
        };
    }

    private static string ConvertPrincipalStatusToDatabase(PrincipalStatus status)
    {
        return status switch
        {
            PrincipalStatus.Active => "ACTIVE",
            PrincipalStatus.Locked => "LOCKED",
            PrincipalStatus.Disabled => "DISABLED",
            PrincipalStatus.Expired => "EXPIRED",
            _ => throw new ArgumentOutOfRangeException(
                nameof(status),
                status,
                "Principal status is not supported.")
        };
    }

    private static PrincipalStatus ConvertPrincipalStatusFromDatabase(string databaseValue)
    {
        return databaseValue switch
        {
            "ACTIVE" => PrincipalStatus.Active,
            "LOCKED" => PrincipalStatus.Locked,
            "DISABLED" => PrincipalStatus.Disabled,
            "EXPIRED" => PrincipalStatus.Expired,
            _ => throw new InvalidOperationException(
                $"Unsupported principal status value '{databaseValue}'.")
        };
    }
}
