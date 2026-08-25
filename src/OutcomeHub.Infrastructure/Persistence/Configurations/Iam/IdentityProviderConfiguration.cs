using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Iam;

public sealed class IdentityProviderConfiguration : IEntityTypeConfiguration<IdentityProvider>
{
    public void Configure(EntityTypeBuilder<IdentityProvider> builder)
    {
        builder.ToTable("identity_provider", "iam", table =>
        {
            table.HasCheckConstraint("ck_identity_provider_code", "code = btrim(code) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_identity_provider_protocol", "protocol IN ('OIDC', 'SAML')");
            table.HasCheckConstraint("ck_identity_provider_status", "status IN ('ACTIVE', 'DISABLED')");
            table.HasCheckConstraint("ck_identity_provider_mapping_version", "claims_mapping_version > 0");
            table.HasCheckConstraint("ck_identity_provider_effective_range", "effective_to IS NULL OR effective_to > effective_from");
        });

        builder.HasKey(entity => entity.Id).HasName("pk_identity_provider");
        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.Protocol).HasColumnName("protocol").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(entity => entity.IssuerOrEntityId).HasColumnName("issuer_or_entity_id").HasColumnType("varchar(512)").HasMaxLength(512).IsRequired();
        builder.Property(entity => entity.ClientId).HasColumnName("client_id").HasColumnType("varchar(255)").HasMaxLength(255);
        builder.Property(entity => entity.MetadataUrl).HasColumnName("metadata_url").HasColumnType("varchar(2048)").HasMaxLength(2048);
        builder.Property(entity => entity.ClaimsMapping).HasColumnName("claims_mapping").HasColumnType("jsonb").IsRequired();
        builder.Property(entity => entity.ClaimsMappingVersion).HasColumnName("claims_mapping_version").HasColumnType("integer").IsRequired();
        builder.Property(entity => entity.SecretReference).HasColumnName("secret_reference").HasColumnType("varchar(512)").HasMaxLength(512);
        builder.Property(entity => entity.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(entity => entity.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(entity => entity.EffectiveTo).HasColumnName("effective_to").HasColumnType("date");

        builder.HasIndex(entity => entity.Code).IsUnique().HasDatabaseName("uq_identity_provider_code");
        builder.HasIndex(entity => new { entity.Protocol, entity.IssuerOrEntityId }).IsUnique().HasDatabaseName("uq_identity_provider_protocol_issuer");
    }
}
