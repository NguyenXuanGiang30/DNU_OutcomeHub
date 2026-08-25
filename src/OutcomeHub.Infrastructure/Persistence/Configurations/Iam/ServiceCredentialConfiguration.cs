using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Iam;

public sealed class ServiceCredentialConfiguration : IEntityTypeConfiguration<ServiceCredential>
{
    public void Configure(EntityTypeBuilder<ServiceCredential> builder)
    {
        builder.ToTable("service_credential", "iam", table =>
        {
            table.HasCheckConstraint("ck_service_credential_type", "credential_type IN ('CLIENT_SECRET', 'API_KEY', 'MTLS', 'JWK')");
            table.HasCheckConstraint("ck_service_credential_effective_range", "effective_to IS NULL OR effective_to > effective_from");
            table.HasCheckConstraint("ck_service_credential_revocation", "(revoked_at IS NULL AND revoked_by IS NULL AND revoke_reason IS NULL) OR (revoked_at IS NOT NULL AND revoked_by IS NOT NULL AND char_length(btrim(revoke_reason)) > 0)");
            table.HasCheckConstraint(
                "ck_service_credential_material",
                "(credential_type = 'CLIENT_SECRET' AND num_nonnulls(secret_hash, secret_reference) = 1 AND certificate_thumbprint IS NULL AND public_jwk IS NULL) OR " +
                "(credential_type = 'API_KEY' AND key_prefix IS NOT NULL AND secret_hash IS NOT NULL AND secret_reference IS NULL AND certificate_thumbprint IS NULL AND public_jwk IS NULL) OR " +
                "(credential_type = 'MTLS' AND certificate_thumbprint IS NOT NULL AND secret_hash IS NULL AND secret_reference IS NULL AND public_jwk IS NULL) OR " +
                "(credential_type = 'JWK' AND public_jwk IS NOT NULL AND secret_hash IS NULL AND secret_reference IS NULL AND certificate_thumbprint IS NULL)");
        });

        builder.HasKey(entity => entity.Id).HasName("pk_service_credential");
        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.ServicePrincipalId).HasColumnName("service_principal_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.CredentialType).HasColumnName("credential_type").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.KeyPrefix).HasColumnName("key_prefix").HasColumnType("varchar(32)").HasMaxLength(32);
        builder.Property(entity => entity.SecretHash).HasColumnName("secret_hash").HasColumnType("varchar(255)").HasMaxLength(255);
        builder.Property(entity => entity.SecretReference).HasColumnName("secret_reference").HasColumnType("varchar(512)").HasMaxLength(512);
        builder.Property(entity => entity.CertificateThumbprint).HasColumnName("certificate_thumbprint").HasColumnType("varchar(128)").HasMaxLength(128);
        builder.Property(entity => entity.PublicJwk).HasColumnName("public_jwk").HasColumnType("jsonb");
        builder.Property(entity => entity.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(entity => entity.EffectiveTo).HasColumnName("effective_to").HasColumnType("date");
        builder.Property(entity => entity.RevokedAt).HasColumnName("revoked_at").HasColumnType("timestamptz");
        builder.Property(entity => entity.RevokedBy).HasColumnName("revoked_by").HasColumnType("uuid");
        builder.Property(entity => entity.RevokeReason).HasColumnName("revoke_reason").HasColumnType("text");
        builder.Property(entity => entity.LastUsedAt).HasColumnName("last_used_at").HasColumnType("timestamptz");

        builder.HasOne(entity => entity.ServiceAccount).WithMany(entity => entity.Credentials).HasForeignKey(entity => entity.ServicePrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_service_credential_service_account");
        builder.HasOne(entity => entity.RevokedByPrincipal).WithMany().HasForeignKey(entity => entity.RevokedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_service_credential_revoked_by");
        builder.HasIndex(entity => new { entity.ServicePrincipalId, entity.EffectiveFrom }).HasDatabaseName("ix_service_credential_service_effective_from");
        builder.HasIndex(entity => entity.KeyPrefix).HasDatabaseName("ix_service_credential_key_prefix");
        builder.HasIndex(entity => entity.CertificateThumbprint).HasDatabaseName("ix_service_credential_certificate_thumbprint");
    }
}
