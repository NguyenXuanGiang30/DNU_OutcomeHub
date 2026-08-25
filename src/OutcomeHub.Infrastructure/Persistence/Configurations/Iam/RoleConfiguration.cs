using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Iam;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Iam;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("role", "iam", table =>
        {
            table.HasCheckConstraint("ck_role_code", "code = btrim(code) AND char_length(code) > 0");
            table.HasCheckConstraint("ck_role_name", "name = btrim(name) AND char_length(name) > 0");
            table.HasCheckConstraint("ck_role_status", "status IN ('ACTIVE', 'DISABLED')");
        });

        builder.HasKey(entity => entity.Id).HasName("pk_role");
        builder.Property(entity => entity.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(entity => entity.Code).HasColumnName("code").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.Name).HasColumnName("name").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(entity => entity.IsSystem).HasColumnName("is_system").HasColumnType("boolean").IsRequired();
        builder.Property(entity => entity.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(entity => entity.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz").IsRequired();

        builder.HasIndex(entity => entity.Code).IsUnique().HasDatabaseName("uq_role_code");
    }
}
