using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class SnapshotResourceConfiguration : IEntityTypeConfiguration<SnapshotResource>
{
    public void Configure(EntityTypeBuilder<SnapshotResource> builder)
    {
        builder.ToTable("snapshot_resource", "measurement");

        builder.HasKey(entity => new { entity.InputSnapshotId, entity.ResourceType, entity.ResourceId, entity.VersionId })
            .HasName("pk_snapshot_resource");

        builder.Property(entity => entity.InputSnapshotId)
            .HasColumnName("input_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.ResourceType)
            .HasColumnName("resource_type")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.ResourceId)
            .HasColumnName("resource_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.VersionId)
            .HasColumnName("version_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.Checksum)
            .HasColumnName("checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.CanonicalPayload)
            .HasColumnName("canonical_payload")
            .HasColumnType("jsonb")
            .IsRequired(true);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.ToTable(table => table.HasCheckConstraint("ck_snapshot_resource_checksum", "checksum ~ '^[0-9a-f]{64}$'"));
        builder.HasOne(entity => entity.InputSnapshot).WithMany().HasForeignKey(entity => entity.InputSnapshotId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_resource_input_snapshot");
    }
}
