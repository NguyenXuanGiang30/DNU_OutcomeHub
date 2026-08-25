using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Iam;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permission", "iam", table =>
        {
            table.HasCheckConstraint("ck_permission_resource_type", "resource_type = btrim(resource_type) AND char_length(resource_type) > 0");
            table.HasCheckConstraint("ck_permission_action", "action = btrim(action) AND char_length(action) > 0");
            table.HasCheckConstraint("ck_permission_field_scope", "field_scope = btrim(field_scope) AND char_length(field_scope) > 0");
        });

        builder.HasKey(entity => entity.Id).HasName("pk_permission");
        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.ResourceType).HasColumnName("resource_type").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.Action).HasColumnName("action").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.FieldScope).HasColumnName("field_scope").HasColumnType("varchar(128)").HasMaxLength(128).IsRequired();
        builder.Property(entity => entity.Description).HasColumnName("description").HasColumnType("text");

        builder.HasIndex(entity => new { entity.ResourceType, entity.Action, entity.FieldScope }).IsUnique().HasDatabaseName("uq_permission_resource_action_field_scope");
    }
}
