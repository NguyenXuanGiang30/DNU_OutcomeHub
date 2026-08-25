using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class SourceRecordMapConfiguration : IEntityTypeConfiguration<SourceRecordMap>
{
    public void Configure(EntityTypeBuilder<SourceRecordMap> builder)
    {
        builder.ToTable("source_record_map", "integration", table => table.HasCheckConstraint("ck_source_record_map_checksum", "last_payload_checksum ~ '^[0-9a-f]{64}$'"));
        builder.HasKey(x => new { x.SourceSystemId, x.EntityType, x.SourceRecordId }).HasName("pk_source_record_map");
        builder.Property(x => x.SourceSystemId).HasColumnName("source_system_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.EntityType).HasColumnName("entity_type").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.SourceRecordId).HasColumnName("source_record_id").HasColumnType("varchar(255)").HasMaxLength(255).IsRequired();
        builder.Property(x => x.TargetId).HasColumnName("target_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.SourceUpdatedAt).HasColumnName("source_updated_at").HasColumnType("timestamptz");
        builder.Property(x => x.LastPayloadChecksum).HasColumnName("last_payload_checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnType("varchar(20)").HasMaxLength(20).IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz").IsRequired();
        builder.HasOne(x => x.SourceSystem).WithMany().HasForeignKey(x => x.SourceSystemId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_source_record_map_source_system");
        builder.HasIndex(x => new { x.EntityType, x.TargetId }).HasDatabaseName("ix_source_record_map_entity_target");
    }
}
