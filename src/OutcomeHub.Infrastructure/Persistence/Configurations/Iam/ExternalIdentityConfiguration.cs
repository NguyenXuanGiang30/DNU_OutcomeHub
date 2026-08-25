using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Iam;

public sealed class ExternalIdentityConfiguration : IEntityTypeConfiguration<ExternalIdentity>
{
    public void Configure(EntityTypeBuilder<ExternalIdentity> builder)
    {
        builder.ToTable("external_identity", "iam", table =>
        {
            table.HasCheckConstraint("ck_external_identity_subject", "subject = btrim(subject) AND char_length(subject) > 0");
            table.HasCheckConstraint("ck_external_identity_claims_hash", "claims_hash ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_external_identity_seen_range", "last_seen_at >= first_seen_at");
        });

        builder.HasKey(entity => entity.Id).HasName("pk_external_identity");
        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.UserPrincipalId).HasColumnName("user_principal_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.IdentityProviderId).HasColumnName("identity_provider_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.Subject).HasColumnName("subject").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(entity => entity.ClaimsSnapshot).HasColumnName("claims_snapshot").HasColumnType("jsonb");
        builder.Property(entity => entity.ClaimsHash).HasColumnName("claims_hash").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(entity => entity.FirstSeenAt).HasColumnName("first_seen_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(entity => entity.LastSeenAt).HasColumnName("last_seen_at").HasColumnType("timestamptz").IsRequired();

        builder.HasOne(entity => entity.UserAccount).WithMany().HasForeignKey(entity => entity.UserPrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_external_identity_user_account");
        builder.HasOne(entity => entity.IdentityProvider).WithMany(entity => entity.ExternalIdentities).HasForeignKey(entity => entity.IdentityProviderId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_external_identity_identity_provider");
        builder.HasIndex(entity => new { entity.IdentityProviderId, entity.Subject }).IsUnique().HasDatabaseName("uq_external_identity_provider_subject");
        builder.HasIndex(entity => entity.UserPrincipalId).HasDatabaseName("ix_external_identity_user_principal");
    }
}
