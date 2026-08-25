using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Measurement;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Measurement;

public sealed class SnapshotManifestChunkConfiguration : IEntityTypeConfiguration<SnapshotManifestChunk>
{
    public void Configure(EntityTypeBuilder<SnapshotManifestChunk> builder)
    {
        builder.ToTable("snapshot_manifest_chunk", "measurement");

        builder.HasKey(entity => new { entity.InputSnapshotId, entity.EntityType, entity.ChunkNo })
            .HasName("pk_snapshot_manifest_chunk");

        builder.Property(entity => entity.InputSnapshotId)
            .HasColumnName("input_snapshot_id")
            .HasColumnType("uuid")
            .IsRequired(true);

        builder.Property(entity => entity.EntityType)
            .HasColumnName("entity_type")
            .HasColumnType("varchar(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.ChunkNo)
            .HasColumnName("chunk_no")
            .HasColumnType("integer")
            .IsRequired(true);

        builder.Property(entity => entity.RowCount)
            .HasColumnName("row_count")
            .HasColumnType("bigint")
            .IsRequired(true);

        builder.Property(entity => entity.FirstKey)
            .HasColumnName("first_key")
            .HasColumnType("varchar(255)")
            .HasMaxLength(255)
            .IsRequired(true);

        builder.Property(entity => entity.LastKey)
            .HasColumnName("last_key")
            .HasColumnType("varchar(255)")
            .HasMaxLength(255)
            .IsRequired(true);

        builder.Property(entity => entity.Checksum)
            .HasColumnName("checksum")
            .HasColumnType("char(64)")
            .HasMaxLength(64)
            .IsRequired(true);

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamptz")
            .IsRequired(true);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("ck_snapshot_manifest_chunk_no", "chunk_no >= 0");
            table.HasCheckConstraint("ck_snapshot_manifest_chunk_count", "row_count >= 0");
            table.HasCheckConstraint("ck_snapshot_manifest_chunk_checksum", "checksum ~ '^[0-9a-f]{64}$'");
        });
        builder.HasOne(entity => entity.InputSnapshot).WithMany().HasForeignKey(entity => entity.InputSnapshotId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_snapshot_manifest_chunk_input_snapshot");
    }
}
