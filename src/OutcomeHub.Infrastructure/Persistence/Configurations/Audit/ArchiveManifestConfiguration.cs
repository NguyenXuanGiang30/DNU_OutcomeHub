using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Audit;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Audit;

public sealed class ArchiveManifestConfiguration : IEntityTypeConfiguration<ArchiveManifest>
{
    public void Configure(EntityTypeBuilder<ArchiveManifest> builder)
    {
        // first_event_id/last_event_id cannot be relational FKs without occurred_at, which is part
        // of the partitioned audit_event PK; archive verification validates both IDs and hash range.
        builder.ToTable("archive_manifest", "audit", table =>
        {
            table.HasCheckConstraint("ck_archive_manifest_period", "period_to > period_from");
            table.HasCheckConstraint("ck_archive_manifest_event_count", "event_count > 0");
            table.HasCheckConstraint("ck_archive_manifest_root_hash", "root_hash ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_archive_manifest_object_checksum", "object_checksum ~ '^[0-9a-f]{64}$'");
            table.HasCheckConstraint("ck_archive_manifest_verified_at", "verified_at IS NULL OR verified_at >= archived_at");
        });
        builder.HasKey(x => x.Id).HasName("pk_archive_manifest");
        builder.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid").ValueGeneratedNever();
        builder.Property(x => x.GovernedResourceId).HasColumnName("governed_resource_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.PeriodFrom).HasColumnName("period_from").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.PeriodTo).HasColumnName("period_to").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.FirstEventId).HasColumnName("first_event_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.LastEventId).HasColumnName("last_event_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.EventCount).HasColumnName("event_count").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.RootHash).HasColumnName("root_hash").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(x => x.Signature).HasColumnName("signature").HasColumnType("bytea").IsRequired();
        builder.Property(x => x.ObjectUri).HasColumnName("object_uri").HasColumnType("varchar(2048)").HasMaxLength(2048).IsRequired();
        builder.Property(x => x.ObjectChecksum).HasColumnName("object_checksum").HasColumnType("char(64)").HasMaxLength(64).IsFixedLength().IsRequired();
        builder.Property(x => x.ArchivedAt).HasColumnName("archived_at").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.VerifiedAt).HasColumnName("verified_at").HasColumnType("timestamptz");
        builder.HasOne(x => x.GovernedResource).WithMany().HasForeignKey(x => x.GovernedResourceId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_archive_manifest_governed_resource");
        builder.HasIndex(x => new { x.GovernedResourceId, x.PeriodFrom, x.PeriodTo }).IsUnique().HasDatabaseName("uq_archive_manifest_resource_period");
    }
}
