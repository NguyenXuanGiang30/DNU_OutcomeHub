using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Ai;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Ai;

public sealed class AiSourceScopeConfiguration : IEntityTypeConfiguration<AiSourceScope>
{
    public void Configure(EntityTypeBuilder<AiSourceScope> builder)
    {
        builder.ToTable("ai_source_scope", "ai");

        builder.HasKey(entity => new { entity.AiSourceSnapshotId, entity.ResourceSecurityScopeId })
            .HasName("pk_ai_source_scope");

        builder.Property(entity => entity.AiSourceSnapshotId)
            .HasColumnName("ai_source_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ResourceSecurityScopeId)
            .HasColumnName("resource_security_scope_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ScopeChecksum)
            .HasColumnName("scope_checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.HasIndex(entity => entity.ResourceSecurityScopeId)
            .HasDatabaseName("ix_ai_source_scope_security_scope");

        builder.HasOne(entity => entity.AiSourceSnapshot)
            .WithMany()
            .HasForeignKey(entity => entity.AiSourceSnapshotId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_source_scope_snapshot");

        builder.HasOne(entity => entity.ResourceSecurityScope)
            .WithMany()
            .HasForeignKey(entity => entity.ResourceSecurityScopeId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_ai_source_scope_security_scope");

        builder.ToTable(tableBuilder => tableBuilder.HasCheckConstraint(
            "ck_ai_source_scope_checksum",
            "scope_checksum ~ '^[0-9a-f]{64}$'"));
    }
}
