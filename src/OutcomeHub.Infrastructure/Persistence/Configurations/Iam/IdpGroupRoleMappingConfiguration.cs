using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Iam;

public sealed class IdpGroupRoleMappingConfiguration : IEntityTypeConfiguration<IdpGroupRoleMapping>
{
    public void Configure(EntityTypeBuilder<IdpGroupRoleMapping> builder)
    {
        builder.ToTable("idp_group_role_mapping", "iam", table =>
        {
            table.HasCheckConstraint("ck_idp_group_role_mapping_group", "external_group_id = btrim(external_group_id) AND char_length(external_group_id) > 0");
            table.HasCheckConstraint("ck_idp_group_role_mapping_version", "version_no > 0");
            table.HasCheckConstraint("ck_idp_group_role_mapping_status", "status IN ('DRAFT', 'IN_REVIEW', 'APPROVED', 'ACTIVE', 'EXPIRED', 'REJECTED')");
            table.HasCheckConstraint("ck_idp_group_role_mapping_effective_range", "effective_to IS NULL OR effective_to > effective_from");
            table.HasCheckConstraint("ck_idp_group_role_mapping_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });

        builder.HasKey(entity => entity.Id).HasName("pk_idp_group_role_mapping");
        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.IdentityProviderId).HasColumnName("identity_provider_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.ExternalGroupId).HasColumnName("external_group_id").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(entity => entity.RoleId).HasColumnName("role_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.RoleVersionId).HasColumnName("role_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.AccessScopeId).HasColumnName("access_scope_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.VersionNo).HasColumnName("version_no").HasColumnType("integer").IsRequired();
        builder.Property(entity => entity.EffectiveFrom).HasColumnName("effective_from").HasColumnType("date").IsRequired();
        builder.Property(entity => entity.EffectiveTo).HasColumnName("effective_to").HasColumnType("date");
        builder.Property(entity => entity.Status).HasColumnName("status").HasColumnType("varchar(32)").HasMaxLength(32).IsRequired();
        builder.Property(entity => entity.WorkflowInstanceId).HasColumnName("workflow_instance_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.SupersedesId).HasColumnName("supersedes_id").HasColumnType("uuid");
        builder.Property(entity => entity.Checksum).HasColumnName("checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();

        builder.HasOne(entity => entity.IdentityProvider).WithMany(entity => entity.GroupRoleMappings).HasForeignKey(entity => entity.IdentityProviderId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_idp_group_role_mapping_identity_provider");
        builder.HasOne(entity => entity.Role).WithMany().HasForeignKey(entity => entity.RoleId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_idp_group_role_mapping_role");
        builder.HasOne(entity => entity.RoleVersion).WithMany().HasForeignKey(entity => new { entity.RoleVersionId, entity.RoleId }).HasPrincipalKey(entity => new { entity.Id, entity.RoleId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_idp_group_role_mapping_role_version");
        builder.HasOne(entity => entity.AccessScope).WithMany().HasForeignKey(entity => entity.AccessScopeId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_idp_group_role_mapping_access_scope");
        builder.HasOne(entity => entity.WorkflowInstance).WithOne().HasForeignKey<IdpGroupRoleMapping>(entity => entity.WorkflowInstanceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_idp_group_role_mapping_workflow_instance");
        builder.HasOne(entity => entity.Supersedes).WithMany().HasForeignKey(entity => entity.SupersedesId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_idp_group_role_mapping_supersedes");

        builder.HasIndex(entity => new { entity.IdentityProviderId, entity.ExternalGroupId, entity.VersionNo }).IsUnique().HasDatabaseName("uq_idp_group_role_mapping_provider_group_version");
        builder.HasIndex(entity => entity.WorkflowInstanceId).IsUnique().HasDatabaseName("uq_idp_group_role_mapping_workflow_instance");
        builder.HasIndex(entity => entity.SupersedesId).IsUnique().HasDatabaseName("uq_idp_group_role_mapping_supersedes");
    }
}
