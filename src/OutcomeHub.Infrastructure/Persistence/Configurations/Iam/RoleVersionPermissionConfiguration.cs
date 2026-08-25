using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Iam;

public sealed class RoleVersionPermissionConfiguration : IEntityTypeConfiguration<RoleVersionPermission>
{
    public void Configure(EntityTypeBuilder<RoleVersionPermission> builder)
    {
        builder.ToTable("role_version_permission", "iam");

        builder.HasKey(entity => new { entity.RoleVersionId, entity.PermissionId }).HasName("pk_role_version_permission");
        builder.Property(entity => entity.RoleVersionId).HasColumnName("role_version_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.PermissionId).HasColumnName("permission_id").HasColumnType("uuid").IsRequired();
        builder.Property(entity => entity.GrantedAt).HasColumnName("granted_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(entity => entity.GrantedBy).HasColumnName("granted_by").HasColumnType("uuid").IsRequired();

        builder.HasOne(entity => entity.RoleVersion).WithMany(entity => entity.Permissions).HasForeignKey(entity => entity.RoleVersionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_role_version_permission_role_version");
        builder.HasOne(entity => entity.Permission).WithMany().HasForeignKey(entity => entity.PermissionId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_role_version_permission_permission");
        builder.HasOne(entity => entity.GrantedByPrincipal).WithMany().HasForeignKey(entity => entity.GrantedBy).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_role_version_permission_granted_by");
        builder.HasIndex(entity => entity.PermissionId).HasDatabaseName("ix_role_version_permission_permission");
    }
}
