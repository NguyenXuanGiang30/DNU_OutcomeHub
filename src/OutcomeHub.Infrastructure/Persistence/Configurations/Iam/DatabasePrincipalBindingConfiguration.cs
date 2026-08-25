using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Iam;

public sealed class DatabasePrincipalBindingConfiguration : IEntityTypeConfiguration<DatabasePrincipalBinding>
{
    public void Configure(EntityTypeBuilder<DatabasePrincipalBinding> builder)
    {
        builder.ToTable("database_principal_binding", "iam", table =>
        {
            table.HasCheckConstraint("ck_database_principal_binding_role_name", "database_role_name = btrim(database_role_name) AND char_length(database_role_name) > 0");
            table.HasCheckConstraint("ck_database_principal_binding_status", "status IN ('ACTIVE', 'EXPIRED', 'REVOKED')");
            table.HasCheckConstraint("ck_database_principal_binding_effective_range", "effective_to IS NULL OR effective_to > effective_from");
            table.HasCheckConstraint("ck_database_principal_binding_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });

        builder.HasKey(entity => new { entity.DatabaseRoleName, entity.EffectiveFrom }).HasName("pk_database_principal_binding");
        builder.Property(entity => entity.DatabaseRoleName).HasColumnName("database_role_name").HasColumnType("varchar(63)").HasMaxLength(63).IsRequired();
        builder.Property(entity => entity.ServicePrincipalId).HasColumnName("service_principal_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.AccessScopeId).HasColumnName("access_scope_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(entity => entity.EffectiveTo).HasColumnName("effective_to").HasColumnType("date");
        builder.Property(entity => entity.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(entity => entity.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();

        builder.HasOne(entity => entity.ServiceAccount).WithMany().HasForeignKey(entity => entity.ServicePrincipalId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_database_principal_binding_service_account");
        builder.HasOne(entity => entity.AccessScope).WithMany().HasForeignKey(entity => entity.AccessScopeId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_database_principal_binding_access_scope");
        builder.HasIndex(entity => new { entity.DatabaseRoleName, entity.Status, entity.EffectiveFrom }).HasDatabaseName("ix_database_principal_binding_role_status_effective");
        builder.HasAnnotation("OutcomeHub:ExclusionConstraint:ex_database_principal_binding_active_range", "database_role_name WITH =, daterange(effective_from, effective_to, '[)') WITH && WHERE (status = 'ACTIVE')");
    }
}
