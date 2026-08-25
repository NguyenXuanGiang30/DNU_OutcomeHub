using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Iam;

public sealed class ServiceAccountConfiguration : IEntityTypeConfiguration<ServiceAccount>
{
    public void Configure(EntityTypeBuilder<ServiceAccount> builder)
    {
        builder.ToTable("service_account", "iam", table =>
        {
            table.HasCheckConstraint("ck_service_account_client_id", "client_id = btrim(client_id) AND char_length(client_id) > 0");
            table.HasCheckConstraint("ck_service_account_purpose", "char_length(btrim(purpose)) > 0");
            table.HasCheckConstraint("ck_service_account_technical_contact", "technical_contact = btrim(technical_contact) AND char_length(technical_contact) > 0");
        });

        builder.HasKey(entity => entity.PrincipalId).HasName("pk_service_account");
        builder.Property(entity => entity.PrincipalId).HasColumnName("principal_id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.ClientId).HasColumnName("client_id").HasColumnType("varchar(128)").HasMaxLength(128).IsRequired();
        builder.Property(entity => entity.OwnerOrgUnitId).HasColumnName("owner_org_unit_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.Purpose).HasColumnName("purpose").HasColumnType("text").IsRequired();
        builder.Property(entity => entity.ExpiresAt).HasColumnName("expires_at").HasColumnType("timestamptz");
        builder.Property(entity => entity.TechnicalContact).HasColumnName("technical_contact").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();

        builder.HasOne(entity => entity.Principal).WithOne().HasForeignKey<ServiceAccount>(entity => entity.PrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_service_account_principal");
        builder.HasOne(entity => entity.OwnerOrgUnit).WithMany().HasForeignKey(entity => entity.OwnerOrgUnitId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_service_account_owner_org_unit");
        builder.HasIndex(entity => entity.ClientId).IsUnique().HasDatabaseName("uq_service_account_client_id");
        builder.HasIndex(entity => entity.OwnerOrgUnitId).HasDatabaseName("ix_service_account_owner_org_unit");
    }
}
