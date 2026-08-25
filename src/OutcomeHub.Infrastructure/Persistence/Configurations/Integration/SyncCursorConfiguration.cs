using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OutcomeHub.Domain.Entities.Integration;

namespace OutcomeHub.Infrastructure.Persistence.Configurations.Integration;

public sealed class SyncCursorConfiguration : IEntityTypeConfiguration<SyncCursor>
{
    public void Configure(EntityTypeBuilder<SyncCursor> builder)
    {
        builder.ToTable("sync_cursor", "integration", table => table.HasCheckConstraint("ck_sync_cursor_resource_type", "resource_type = btrim(resource_type) AND char_length(resource_type) > 0"));
        builder.HasKey(x => new { x.SourceSystemId, x.ResourceType }).HasName("pk_sync_cursor");
        builder.Property(x => x.SourceSystemId).HasColumnName("source_system_id").HasColumnType("uuid").IsRequired();
        builder.Property(x => x.ResourceType).HasColumnName("resource_type").HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        builder.Property(x => x.CursorValueCiphertext).HasColumnName("cursor_value_ciphertext").HasColumnType("bytea").IsRequired();
        builder.Property(x => x.LastSourceUpdatedAt).HasColumnName("last_source_updated_at").HasColumnType("timestamptz");
        builder.Property(x => x.LastSuccessfulJobId).HasColumnName("last_successful_job_id").HasColumnType("uuid");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz").IsRequired();
        builder.HasOne(x => x.SourceSystem).WithMany().HasForeignKey(x => x.SourceSystemId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_sync_cursor_source_system");
        builder.HasOne(x => x.LastSuccessfulJob).WithMany().HasForeignKey(x => x.LastSuccessfulJobId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_sync_cursor_last_job");
    }
}
